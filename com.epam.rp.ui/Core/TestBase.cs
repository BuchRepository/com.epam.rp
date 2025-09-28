using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using com.epam.rp.core;
using com.epam.rp.ui.Business.Pages;
using com.epam.rp.ui.Core.Utility;
using OpenQA.Selenium;
using Serilog;

namespace com.epam.rp.ui.Core;


[TestClass]
public class TestBase
{
    protected IWebDriver? Driver;
    protected static ExtentReports? Extent;
    protected ExtentTest? Test;
       
    protected LoginPage? LoginPage;
    protected FiltersPage? FiltersPage;
       
    public TestContext TestContext { get; set; } = null!;

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        string reportPath = Path.Combine(AppContext.BaseDirectory, "ExtentReports.html");
        Console.WriteLine($"[LOG] ExtentReports path: {reportPath}");
        
        var htmlReporter = new ExtentHtmlReporter(reportPath);
        htmlReporter.Config.DocumentTitle = "Test Report";
        htmlReporter.Config.ReportName = "UI Test Report";
        htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;
        Extent = new ExtentReports();
        Extent.AttachReporter(htmlReporter);
        
        LoggerService.InitLogger();
    }
    
    [TestInitialize]
    public void SetUp()
    {
        //test = extent.CreateTest(TestContext.TestName);
        
        string browser = TestContext.Properties.Contains("browser")
            ? TestContext.Properties["browser"]?.ToString() ?? "chrome"
            : "chrome";

        Driver = DriverFactory.CreateDriver(browser, uniqueProfile: true);
        if (Driver == null)
            throw new InvalidOperationException("Driver initialization failed.");

        Driver.Navigate().GoToUrl("https://rp.epam.com");

        LoginPage = new LoginPage(Driver);
        FiltersPage = new FiltersPage(Driver);
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
                    LoggerService.Info($"Screenshot saved: {screenshotPath}");
                }
            }
            catch (Exception e)
            {
                LoggerService.Error("Failed to take screenshot on test failure", e);
            }
            
            //test.Fail("Test Failed");
        }
        
        /*else if (outcome == UnitTestOutcome.Passed)
        {
            test.Pass("Test Passed");
        }
        else
        {
            test.Skip("Test Skipped");
        }   */

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

        //extent.Flush();
    }
}