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
                var options = new ChromeOptions();
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--remote-allow-origins=*");
                options.AddArgument("--disable-software-rasterizer");
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--disable-gpu");
                
                driver = new ChromeDriver(options);
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