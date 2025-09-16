using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;

namespace Core;

public static class DriverFactory
{
    public static string? LastProfilePath { get; private set; }
    
    public static IWebDriver CreateDriver(string browser = "chrome", bool uniqueProfile = false)
    {
        IWebDriver driver;
        string? profilePath = null;

        switch (browser.ToLower())
        {
            case "firefox":
                var ffOptions = new FirefoxOptions();
                ffOptions.AddArgument("--headless");
                ffOptions.AddArgument("--no-sandbox");
                ffOptions.AddArgument("--disable-dev-shm-usage");
                ffOptions.AddArgument("--width=1920");
                ffOptions.AddArgument("--height=1080");
                driver = new FirefoxDriver(ffOptions);
                break;
            case "chrome":
                var options = new ChromeOptions();
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--remote-allow-origins=*");
                options.AddArgument("--disable-software-rasterizer");
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--no-first-run");
                options.AddArgument("--disable-extensions");
                
                if (uniqueProfile)
                {
                    profilePath = Path.Combine(Path.GetTempPath(), $"chrome_profile_{Guid.NewGuid():N}");
                    Directory.CreateDirectory(profilePath);
                    options.AddArgument($"--user-data-dir={profilePath}");
                }
                
                var service = CreateDriverServiceWithRetry();
                service.HideCommandPromptWindow = true;
                driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(60));
                break;
            
            default:
                throw new NotSupportedException($"Browser '{browser}' is not supported.");
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