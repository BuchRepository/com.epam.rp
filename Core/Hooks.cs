using Core;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace com.epam.rp.Core;


[Binding]
public sealed class Hooks
{
    private readonly ScenarioContext _context;
    public static string Login { get; private set; }
    public static string Password { get; private set; }
    
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
    public void InitDriverAndPages()
    {
        IWebDriver driver = DriverFactory.CreateDriver("chrome");

        _context["driver"] = driver;

        _context["loginPage"] = new Business.Pages.LoginPage(driver);
        _context["filtersPage"] = new Business.Pages.FiltersPage(driver);
    }

    [AfterScenario]
    public void CleanUp()
    {
        if (_context.TryGetValue("driver", out IWebDriver driver))
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}