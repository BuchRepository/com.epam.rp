using AventStack.ExtentReports;
using com.epam.rp.core;
using com.epam.rp.core.Utility;
using com.epam.rp.ui.Business.Pages;
using com.epam.rp.ui.Core.Utility;
using OpenQA.Selenium;
using Serilog;

namespace com.epam.rp.ui.Core;


[TestClass]
public class TestBase
{
    private IWebDriver? Driver;
    private static ExtentReports? Extent;
    private ExtentTest? Test;
    private static SlackNotifier? _slackNotifier;
    
    protected LoginPage? LoginPage;
    protected FiltersPage? FiltersPage;
       
    public TestContext TestContext { get; set; } = null!;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)    
    {
        Extent = ReportManager.GetExtent(isUi: true);
        LoggerService.InitLogger();
        
        _slackNotifier = new SlackNotifier();
        Task.Run(() => _slackNotifier.SendMessage($"UI tests started at {DateTime.Now}"));
    }
    
    [TestInitialize]
    public void SetUp()
    {
        Test = Extent!.CreateTest(TestContext.TestName);
        string browser = TestContext.Properties.Contains("browser")
            ? TestContext.Properties["browser"]?.ToString() ?? "chrome"
            : "chrome";

        Driver = DriverFactory.CreateDriver(browser);
        if (Driver == null)
            throw new InvalidOperationException("Driver initialization failed.");

        int attempts = 0;
        while (attempts < 3)
        {
            try
            {
                Driver.Navigate().GoToUrl("https://rp.epam.com");
                break;
            }
            catch (WebDriverException)
            {
                attempts++;
                Thread.Sleep(2000);
            }
        }
        
        ClearBrowserData(Driver);

        Driver.Navigate().Refresh();

        LoginPage = new LoginPage(Driver);
        FiltersPage = new FiltersPage(Driver);
    }
    
    private void ClearBrowserData(IWebDriver driver)
    {
        try
        {
            driver.Manage().Cookies.DeleteAllCookies();
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "window.localStorage.clear(); window.sessionStorage.clear();");
            LoggerService.Info("Browser data cleared (cookies, localStorage, sessionStorage).");
        }
        catch (Exception ex)
        {
            LoggerService.Error("Failed to clear browser data", ex);
        }
    }

    [TestCleanup]
    public void CleanUp()
    {
        var outcome = TestContext.CurrentTestOutcome;

        try
        {
            if (outcome == UnitTestOutcome.Failed && Driver is not null)
            {
                string? screenshotPath = ScreenshotHelper.TakeScreenshot(Driver, Log.Logger, TestContext.TestName);
                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    Test!.Fail("Test Failed").AddScreenCaptureFromPath(screenshotPath);
                    LoggerService.Info($"Screenshot saved: {screenshotPath}");
                }
                else
                {
                    Test!.Fail("Test Failed - no screenshot available");
                }
            }
            else if (outcome == UnitTestOutcome.Passed)
            {
                Test!.Pass("Test Passed");
            }
            else
            {
                Test!.Skip("Test Skipped");
            }
        }
        catch (Exception e)
        {
            LoggerService.Error("Error during test cleanup", e);
        }
        finally
        {
            try
            {
                Driver?.Quit();
                Driver?.Dispose();
            }
            catch (Exception ex)
            {
                LoggerService.Error("Error while disposing driver", ex);
            }
            Driver = null;
        }
    }
    
    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        ReportManager.FlushReports();
        Task.Run(() => _slackNotifier?.SendMessage($"UI tests finished at {DateTime.Now}"));
    }
}