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
       
    protected LoginPage? LoginPage;
    protected FiltersPage? FiltersPage;
    
    private static SlackNotifier? _slackNotifier;
       
    public TestContext TestContext { get; set; } = null!;

    [AssemblyInitialize]
    public static async Task AssemblyInit(TestContext context)    
    {
        Extent = ReportManager.GetExtent(isUi: true);
        
        LoggerService.InitLogger();
        
        _slackNotifier = new SlackNotifier();
        await _slackNotifier.SendMessage($"UI Test Assembly STARTED at {DateTime.Now}");
    }
    
    [TestInitialize]
    public void SetUp()
    {
        Test = Extent!.CreateTest(TestContext.TestName);
        
        string browser = TestContext.Properties.Contains("browser")
            ? TestContext.Properties["browser"]?.ToString() ?? "chrome"
            : "chrome";

        Driver = DriverFactory.CreateDriver(browser, uniqueProfile: false);
        if (Driver == null)
            throw new InvalidOperationException("Driver initialization failed.");

        Driver.Navigate().GoToUrl("https://rp.epam.com");
        
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

        if (outcome == UnitTestOutcome.Failed && Driver is not null)
        {
            try
            {
                string? screenshotPath = ScreenshotHelper.TakeScreenshot(
                    Driver, 
                    Log.Logger, 
                    TestContext.TestName
                );

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
            catch (Exception e)
            {
                LoggerService.Error("Failed to take screenshot on test failure", e);
                Test!.Fail("Test Failed - screenshot error: " + e.Message);
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
        
        if (!string.IsNullOrEmpty(DriverFactory.LastProfilePath) &&
            Directory.Exists(DriverFactory.LastProfilePath))
        {
            try
            {
                Directory.Delete(DriverFactory.LastProfilePath, true);
            }
            catch (Exception ex)
            {
                LoggerService.Error("Error while deleting profile directory", ex);
            }
        }
    }
    
    [AssemblyCleanup]
    public static async Task AssemblyCleanup()
    {
        ReportManager.FlushReports();
        
        if (_slackNotifier != null)
        {
            await _slackNotifier.SendMessage($"UI Test Assembly FINISHED at {DateTime.Now}");
        }
    }
}