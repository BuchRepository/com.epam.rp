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
    /*protected IWebDriver? Driver;
    
    protected LoginPage? LoginPage;
    protected FiltersPage? FiltersPage;*/
    //protected IWebDriver? Driver;
    
    /*protected static ExtentReports extent;
    protected ExtentTest test;*/
    
    /*[OneTimeSetUp]
    public void OneTimeSetup()
    {
        string reportPath = Path.Combine(AppContext.BaseDirectory, "ExtentReports.html");
        NUnitTestContext.Progress.WriteLine($"[LOG] ExtentReports path: {reportPath}");
        
        var htmlReporter = new ExtentHtmlReporter(reportPath);
        htmlReporter.Config.DocumentTitle = "Test Report";
        htmlReporter.Config.ReportName = "UI Test Report";
        htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;
        extent = new ExtentReports();
        extent.AttachReporter(htmlReporter);
    }*/
        
    /*[SetUp]
    public void SetUp()
    {
        test = extent.CreateTest(NUnitTestContext.CurrentContext.Test.Name);
        
        string browser = NUnitTestContext.Parameters.Get("browser", "chrome");
        Driver = DriverFactory.CreateDriver(browser);
        if (Driver == null)
        {
            throw new InvalidOperationException("Driver initialization failed.");
        }
        Driver.Navigate().GoToUrl("https://rp.epam.com");
        
        LoginPage = new LoginPage(Driver);
        FiltersPage = new FiltersPage(Driver);
    }*/

    [TearDown]
    public void TearDown()
    {
        /*var status = NUnitTestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = NUnitTestContext.CurrentContext.Result.StackTrace;

        if (status == TestStatus.Failed)
        {
            string screenshotName = $"TestFail_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            try
            {
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
                screenshot.SaveAsFile(screenshotName);
                Log.Information($"Screenshot saved: {screenshotName}");
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to take screenshot on test failure");
            }
        }*/
        
        /*switch (status)
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
        }*/
        
        /*if (Driver != null)
        {
            Driver.Quit();
            Driver.Dispose();
        }
        Driver = null;
        
        extent.Flush();*/
        
        /*try
        {
            extent?.Flush();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ExtentReports flush failed: {ex.Message}");
        }*/
    }
}