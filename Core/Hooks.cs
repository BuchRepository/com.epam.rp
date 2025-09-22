using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using com.epam.rp.Business.Pages;
using com.epam.rp.Core.Utility;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Serilog;
using TechTalk.SpecFlow;

namespace com.epam.rp.Core;


[Binding]
public sealed class Hooks
{
    private readonly ScenarioContext _context;
    private static ExtentReports? _extentReports;
    private static ILogger? _rootLogger;
    public static string? Login { get; private set; }
    public static string? Password { get; private set; }
    private const string BaseUrl = "https://rp.epam.com";
    
    public Hooks(ScenarioContext scenarioContext)
    {
        _context = scenarioContext;
    }
    
    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Login = configuration["LOGIN"];
        Password = configuration["PASSWORD"];
        
        _rootLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log_root.txt")
            .CreateLogger();
        
        string reportPath = Path.Combine(AppContext.BaseDirectory, "ExtentReport.html");
        var htmlReporter = new ExtentHtmlReporter(reportPath);
        htmlReporter.Config.DocumentTitle = "Test Report";
        htmlReporter.Config.ReportName = "UI Test Report";
        htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

        _extentReports = new ExtentReports();
        _extentReports.AttachReporter(htmlReporter);
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        _extentReports?.Flush();
        _rootLogger?.Information("All tests finished. Reports flushed.");
        Log.CloseAndFlush();
    }

    [BeforeFeature]
    public static void BeforeFeature()
    {
    }

    [AfterFeature]
    public static void AfterFeature()
    {
    }
    
    [BeforeScenario]
    public void BeforeScenario()
    {
        string logFile = Path.Combine(AppContext.BaseDirectory, "logs", $"log_{Guid.NewGuid():N}.txt");
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFile)
            .CreateLogger();
        _context["logger"] = logger;

        IWebDriver driver = DriverFactory.CreateDriver();
        driver.Navigate().GoToUrl(BaseUrl); 
        _context["driver"] = driver;

        _context["loginPage"] = new LoginPage(driver);
        _context["filtersPage"] = new FiltersPage(driver);
        _context["launchesPage"] = new LaunchesPage(driver);

        if (_extentReports == null) 
            throw new InvalidOperationException("_extentReports is not initialized");

        var test = _extentReports.CreateTest(_context.ScenarioInfo.Title);

        _context["extent"] = _extentReports;
        _context["test"] = test;
    }

    [AfterScenario]
    public void CleanUp()
    {
        var driver = _context.Get<IWebDriver>("driver");
        var logger = _context.Get<ILogger>("logger");
        var test = _context.Get<ExtentTest>("test");

        if (_context.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
        {
            string? screenshotPath = ScreenshotHelper.TakeScreenshot(driver, logger, _context.ScenarioInfo.Title);
            if (screenshotPath != null)
            {
                test.AddScreenCaptureFromPath(screenshotPath);
            }

            test.Fail("Scenario failed");
        }
        else if (_context.ScenarioExecutionStatus == ScenarioExecutionStatus.OK)
        {
            test.Pass("Scenario passed");
        }
        else
        {
            test.Skip("Scenario skipped");
        }
        
        try
        {
            driver.Quit();
            driver.Dispose();
            logger.Information("Driver successfully closed");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to quit or dispose WebDriver");
        }
    }
}