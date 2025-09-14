using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Business.Pages;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Serilog;
using NUnitAssert = NUnit.Framework.Assert;
using NUnitTestContext = NUnit.Framework.TestContext;


namespace Core;

public class TestBase
{
    protected IWebDriver? Driver;
    protected static ExtentReports extent;
    protected ExtentTest test;
    
    protected LoginPage? LoginPage;
    protected FiltersPage? FiltersPage;
    
    private static readonly object _extentLock = new object();
    private string _logFile = string.Empty;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        lock (_extentLock)
        {
            if (extent == null)
            {
                string reportPath = Path.Combine(AppContext.BaseDirectory, $"ExtentReports_{Guid.NewGuid():N}.html");

                var htmlReporter = new ExtentHtmlReporter(reportPath);
                htmlReporter.Config.DocumentTitle = "Test Report";
                htmlReporter.Config.ReportName = "UI Test Report";
                htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

                extent = new ExtentReports();
                extent.AttachReporter(htmlReporter);
            }
        }
    }
        
    [SetUp]
    public void SetUp()
    {
        Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "logs"));
        _logFile = Path.Combine(AppContext.BaseDirectory, "logs", $"logfile_{Guid.NewGuid():N}.log");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(_logFile)
            .CreateLogger();
        
        string browser = NUnitTestContext.Parameters.Get("browser", "chrome");
        Driver = DriverFactory.CreateDriver(browser, uniqueProfile: true);
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
            string screenshotName = $"TestFail_{Guid.NewGuid():N}.png";
            try
            {
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                screenshot.SaveAsFile(screenshotName);
                Log.Information($"Screenshot saved: {screenshotName}");
                
                lock (_extentLock)
                {
                    test.AddScreenCaptureFromPath(screenshotName);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to take screenshot on test failure");
            }
        }

        lock (_extentLock)
        {
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
        }

        if (Driver != null)
        {
            Driver.Quit();
            Driver.Dispose();
            Driver = null;
        }

        lock (_extentLock)
        {
            extent.Flush();
        }
    }
}