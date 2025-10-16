using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace com.epam.rp.ui.Core;

public static class DriverFactory
{
    public static string? LastProfilePath { get; private set; }

    public static IWebDriver CreateDriver(string browser = "chrome", bool uniqueProfile = false)
    {
        var runRemote = Environment.GetEnvironmentVariable("RUN_REMOTE")?.ToLower() == "true";
        var gridUrl = Environment.GetEnvironmentVariable("SELENIUM_GRID_URL") ?? "http://localhost:4444/wd/hub";
        if (runRemote)
        {
            return CreateRemoteDriver(browser, gridUrl);
        }
        else
        {
            return CreateLocalDriver(browser, uniqueProfile);
        }
    }
    
    private static IWebDriver CreateRemoteDriver(string browser, string gridUrl)
    {
        ICapabilities capabilities;

        switch (browser.ToLower())
        {
            case "firefox":
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AddArgument("-headless");
                capabilities = firefoxOptions.ToCapabilities();
                break;
            case "chrome":
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--headless=new");
                chromeOptions.AddArgument("--no-sandbox");
                chromeOptions.AddArgument("--disable-dev-shm-usage");
                chromeOptions.AddArgument("--disable-gpu");
                chromeOptions.AddArgument("--window-size=1920,1080");
                capabilities = chromeOptions.ToCapabilities();
                break;
            
            default:
                throw new NotSupportedException($"Browser '{browser}' is not supported.");
        }
        
        return new RemoteWebDriver(new Uri(gridUrl), capabilities, TimeSpan.FromSeconds(180));
    }
        
    private static IWebDriver CreateLocalDriver(string browser, bool uniqueProfile)
    {
        IWebDriver driver;
        string? profilePath;

        switch (browser.ToLower())
        {
            case "firefox":
                driver = new FirefoxDriver();
                break;

            case "chrome":
                var options = new ChromeOptions();
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");

                if (uniqueProfile)
                {
                    profilePath = Path.Combine(Path.GetTempPath(), $"chrome_profile_{Guid.NewGuid():N}");
                    Directory.CreateDirectory(profilePath);
                    options.AddArgument($"--user-data-dir={profilePath}");
                    options.AddArgument("--disable-extensions");
                    LastProfilePath = profilePath;
                }

                var service = CreateDriverServiceWithRetry();
                service.HideCommandPromptWindow = true;
                driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(120));
                break;

            default:
                throw new NotSupportedException($"Browser '{browser}' is not supported locally.");
        }

        return driver;
    } 
    
    private static ChromeDriverService CreateDriverServiceWithRetry(int retries = 5)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                var service = ChromeDriverService.CreateDefaultService();
                service.Port = new Random().Next(49152, 65535);
                return service;
            }
            catch
            {
                if (i == retries - 1) throw;
                Thread.Sleep(200);
            }
        }

        throw new WebDriverException("Failed to create ChromeDriverService with unique port.");
    }
}