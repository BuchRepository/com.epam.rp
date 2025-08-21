using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Safari;

namespace ReportPortal.Core;

public static class DriverFactory
{
    public static IWebDriver CreateDriver(string browser = "chrome")
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
                Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.EnableVerboseLogging = true;
                Console.WriteLine("Log file path: " + service.LogPath);
                service.LogPath = "/tmp/chrome_driver_log.txt";
                service.EnableVerboseLogging = true;
                
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--remote-allow-origins=*");
                options.AddArgument("--disable-gpu");
                
                driver = new ChromeDriver(service, options);
                break;
            
            default:
                throw new NotSupportedException($"Browser '{browser}' is not supported.");
        }
        
        if (driver == null)
        {
            throw new InvalidOperationException("Driver could not be created. Ensure all configurations are correct.");
        }

        return driver;
    }
}