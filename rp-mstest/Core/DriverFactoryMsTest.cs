using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;

namespace com.epam.rp_mstest.Core;

public static class DriverFactoryMsTest
{
    public static IWebDriver CreateDriver(string browser = "chrome", bool uniqueProfile = false)
    {
        IWebDriver driver;

        switch (browser.ToLower())
        {
            case "firefox":
                driver = new FirefoxDriver();
                break;
            case "safari":
                driver = new SafariDriver();
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
                    string profilePath = Path.Combine(Path.GetTempPath(), $"chrome_profile_{Guid.NewGuid():N}");
                    Directory.CreateDirectory(profilePath);
                    options.AddArgument($"--user-data-dir={profilePath}");
                    options.AddArgument("--disable-extensions");
                }
                
                var service = ChromeDriverService.CreateDefaultService();
                service.Port = 0;
                driver = new ChromeDriver(service, options);
                break;
            
            default:
                throw new NotSupportedException($"Browser '{browser}' is not supported.");
        }
        
        return driver;
    }
}