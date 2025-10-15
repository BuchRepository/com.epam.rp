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
    private IWebDriver? _driver;
    private static ExtentReports? _extent;
    private ExtentTest? _test;
       
    protected LoginPage? LoginPage;
    protected FiltersPage? FiltersPage;
    
    private static SlackNotifier? _slackNotifier;
       
    private TestContext TestContext { get; set; } = null!;

    [AssemblyInitialize]
    public static async Task AssemblyInit(TestContext context)    
    {
        _extent = ReportManager.GetExtent(isUI: true);
        
        LoggerService.InitLogger();
        
        _slackNotifier = new SlackNotifier();
        await _slackNotifier.SendMessage($"UI Test Assembly STARTED at {DateTime.Now}");
    }
    
    [TestInitialize]
    public void SetUp()
    {
        _test = _extent!.CreateTest(TestContext.TestName);
        
        string browser = TestContext.Properties.Contains("browser")
            ? TestContext.Properties["browser"]?.ToString() ?? "chrome"
            : "chrome";

        _driver = DriverFactory.CreateDriver(browser, uniqueProfile: true);
        if (_driver == null)
            throw new InvalidOperationException("Driver initialization failed.");

        _driver.Navigate().GoToUrl("https://rp.epam.com");

        LoginPage = new LoginPage(_driver);
        FiltersPage = new FiltersPage(_driver);
    }

    [TestCleanup]
    public void CleanUp()   
    {
        var outcome = TestContext.CurrentTestOutcome;

        if (outcome == UnitTestOutcome.Failed && _driver is not null)
        {
            try
            {
                string? screenshotPath = ScreenshotHelper.TakeScreenshot(
                    _driver, 
                    Log.Logger, 
                    TestContext.TestName
                );

                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    _test!.Fail("Test Failed").AddScreenCaptureFromPath(screenshotPath);
                    LoggerService.Info($"Screenshot saved: {screenshotPath}");
                }
                else
                {
                    _test!.Fail("Test Failed - no screenshot available");
                }
            }
            catch (Exception e)
            {
                LoggerService.Error("Failed to take screenshot on test failure", e);
                _test!.Fail("Test Failed - screenshot error: " + e.Message);
            }
        }
        
        else if (outcome == UnitTestOutcome.Passed)
        {
            _test!.Pass("Test Passed");
        }
        else
        {
            _test!.Skip("Test Skipped");
        }   

        try
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
        catch (Exception ex)
        {
            LoggerService.Error("Error while disposing driver", ex);
        }
        _driver = null;
        
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