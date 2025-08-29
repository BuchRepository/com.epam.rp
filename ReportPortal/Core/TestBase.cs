using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;

namespace ReportPortal.Core;

public class TestBase
{
    protected IWebDriver? Driver;
    
    protected static ExtentReports extent;
    protected ExtentTest test;
    
    private static string reportPath = Path.Combine(AppContext.BaseDirectory, "ExtentReports.html");
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var htmlReporter = new ExtentHtmlReporter(reportPath);
        htmlReporter.Config.DocumentTitle = "Test Report";
        htmlReporter.Config.ReportName = "UI Test Report";
        htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;
        extent = new ExtentReports();
        extent.AttachReporter(htmlReporter);
    }
        
    [SetUp]
    public void SetUp()
    {
        test = extent.CreateTest(TestContext.CurrentContext.Test.Name);
        
        string browser = TestContext.Parameters.Get("browser", "chrome");
        Driver = DriverFactory.CreateDriver(browser);
        if (Driver == null)
        {
            throw new InvalidOperationException("Driver initialization failed.");
        }
        Driver.Navigate().GoToUrl("https://rp.epam.com");
    }

    [TearDown]
    public void TearDown()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = TestContext.CurrentContext.Result.StackTrace;

        switch (status)
        {
            case NUnit.Framework.Interfaces.TestStatus.Failed:
                test.Fail("Test Failed").Fail(stacktrace);
                break;
            case NUnit.Framework.Interfaces.TestStatus.Passed:
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
    }
}