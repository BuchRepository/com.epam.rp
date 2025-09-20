using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Business.Pages;
using com.epam.rp.Core.Utility;
using Core;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Serilog;
using TechTalk.SpecFlow;

namespace com.epam.rp.Core;


[Binding]
public sealed class Hooks
{
    private readonly ScenarioContext _context;
    public static string? Login { get; private set; }
    public static string? Password { get; private set; }
    private const string BaseUrl = "https://rp.epam.com";
    
    public Hooks(ScenarioContext context)
    {
        _context = context;
    }
    
    [BeforeTestRun]
    public static void LoadConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Login = configuration["LOGIN"];
        Password = configuration["PASSWORD"];
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
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
        string logsDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logsDir);
        string logFile = Path.Combine(logsDir, $"logfile_{Guid.NewGuid():N}.log");

        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logFile)
            .CreateLogger();

        _context["logger"] = logger;

        IWebDriver driver = DriverFactory.CreateDriver("chrome");
        driver.Navigate().GoToUrl(BaseUrl); 
        _context["driver"] = driver;

        _context["loginPage"] = new LoginPage(driver);
        _context["filtersPage"] = new FiltersPage(driver);
        _context["launchesPage"] = new LaunchesPage(driver);

        string reportPath = Path.Combine(AppContext.BaseDirectory, $"ExtentReport_{Guid.NewGuid():N}.html");
        var htmlReporter = new ExtentHtmlReporter(reportPath);
        htmlReporter.Config.DocumentTitle = "Test Report";
        htmlReporter.Config.ReportName = "UI Test Report";
        htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

        var extent = new ExtentReports();
        extent.AttachReporter(htmlReporter);
        var test = extent.CreateTest(_context.ScenarioInfo.Title);

        _context["extent"] = extent;
        _context["test"] = test;
    }

    [AfterScenario]
    public void CleanUp()
    {
        var driver = _context.Get<IWebDriver>("driver");
        var logger = _context.Get<Serilog.ILogger>("logger");
        var extent = _context.Get<ExtentReports>("extent");
        var test = _context.Get<ExtentTest>("test");

        if (_context.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
        {
            string screenshotPath = ScreenshotHelper.TakeScreenshot(driver, logger, _context.ScenarioInfo.Title);
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
            extent.Flush();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error while flushing ExtentReports");
        }
        finally
        {
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
}