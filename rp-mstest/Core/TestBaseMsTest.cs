using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using com.epam.rp_mstest.Businnes.Pages;
using OpenQA.Selenium;
using Serilog;

namespace com.epam.rp_mstest.Core;


[TestClass]
public class TestBaseMsTest
{
    /*protected IWebDriver? Driver;

    protected LoginPageMsTest? LoginPage;
    protected FiltersPageMsTest? FiltersPage;

    protected static ExtentReports extent;
    protected ExtentTest test;
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
        extent = new ExtentReports();
        extent.AttachReporter(htmlReporter);
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
    }
    
    [TestInitialize]
    public void SetUp()
    {
        test = extent.CreateTest(TestContext.TestName);
        
        string browser = TestContext.Properties.Contains("browser")
            ? TestContext.Properties["browser"]?.ToString() ?? "chrome"
            : "chrome";

        Driver = DriverFactoryMsTest.CreateDriver(browser);
        if (Driver == null)
            throw new InvalidOperationException("Driver initialization failed.");

        Driver.Navigate().GoToUrl("https://rp.epam.com");

        LoginPage = new LoginPageMsTest(Driver);
        FiltersPage = new FiltersPageMsTest(Driver);
    }

    [TestCleanup]
    public void CleanUp()   
    {
        var outcome = TestContext.CurrentTestOutcome;

        if (outcome == UnitTestOutcome.Failed)
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
            
            test.Fail("Test Failed");
        }
        
        else if (outcome == UnitTestOutcome.Passed)
        {
            test.Pass("Test Passed");
        }
        else
        {
            test.Skip("Test Skipped");
        }

        Driver?.Quit();
        Driver?.Dispose();
        Driver = null;

        extent.Flush();
    }*/
}