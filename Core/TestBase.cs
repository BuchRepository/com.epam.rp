using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Business.Pages;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Serilog;
using NUnitAssert = NUnit.Framework.Assert;
using NUnitTestContext = NUnit.Framework.TestContext;


namespace Core;

public abstract class TestBase
{
    protected ExtentReports extent;
    protected ExtentTest test;
    
    private string _logFile = string.Empty;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        string reportPath = Path.Combine(AppContext.BaseDirectory, $"ExtentReports_{Guid.NewGuid():N}.html");

        var htmlReporter = new ExtentHtmlReporter(reportPath);
        htmlReporter.Config.DocumentTitle = "Test Report";
        htmlReporter.Config.ReportName = "UI Test Report";
        htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

        extent = new ExtentReports();
        extent.AttachReporter(htmlReporter);
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        extent.Flush();
    }
    
    protected void InitLogging(string testName)
    {
        string logFile = Path.Combine(AppContext.BaseDirectory, "logs", $"logfile_{Guid.NewGuid():N}.log");
        Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "logs"));

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFile)
            .CreateLogger();

        test = extent.CreateTest(testName);
    }
        
    /*[SetUp]
    public void SetUp()
    {
        Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "logs"));
        _logFile = Path.Combine(AppContext.BaseDirectory, "logs", $"logfile_{Guid.NewGuid():N}.log");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(_logFile)
            .CreateLogger();
        
        string browser = NUnitTestContext.Parameters.Get("browser", "chrome");
        Driver = DriverFactory.CreateDriver(browser);
        Driver.Navigate().GoToUrl("https://rp.epam.com");
        
        LoginPage = new LoginPage(Driver);
        FiltersPage = new FiltersPage(Driver);
        
        test = extent.CreateTest(NUnitTestContext.CurrentContext.Test.Name);
    }

    [TearDown]
    public void TearDown()
    {
        var status = NUnitTestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = NUnitTestContext.CurrentContext.Result.StackTrace;

        if (status == TestStatus.Failed)
        {
            string screenshotName = $"TestFail_{status}_{Guid.NewGuid():N}.png";
            try
            {
                var screenshotsDir = Path.Combine(AppContext.BaseDirectory, "screenshots");
                Directory.CreateDirectory(screenshotsDir);

                var fullPath = Path.Combine(screenshotsDir, screenshotName);
                
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                screenshot.SaveAsFile(fullPath);
                
                Log.Information($"Screenshot saved: {fullPath}");
                
                test.AddScreenCaptureFromPath(screenshotName);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to take screenshot on test failure");
            }
        }
        
        switch (status)
        {
            case TestStatus.Failed:
                test.Fail("Test Failed").Fail(stacktrace);
                break;
            case TestStatus.Passed:
                test.Pass("Test Passed");
                break;
            default:
                test.Skip("Test Skipped");
                break;
        }

        if (Driver != null)
        {
            Driver.Quit();
            Driver.Dispose();
        }
        Driver = null;

        extent.Flush();
    }*/
}