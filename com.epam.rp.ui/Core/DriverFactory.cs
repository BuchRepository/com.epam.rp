using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace com.epam.rp.ui.Core;

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
                driver = new FirefoxDriver();
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
                
                if (uniqueProfile)
                {
                    profilePath = Path.Combine(Path.GetTempPath(), $"chrome_profile_{Guid.NewGuid():N}");
                    Directory.CreateDirectory(profilePath);
                    options.AddArgument($"--user-data-dir={profilePath}");
                    options.AddArgument("--disable-extensions");
                }
                
                var service = CreateDriverServiceWithRetry();
                service.HideCommandPromptWindow = true;
                driver = new ChromeDriver(service, options, TimeSpan.FromSeconds(120));
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
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