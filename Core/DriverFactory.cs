using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace com.epam.rp.Core;

public static class DriverFactory
{
    public static IWebDriver CreateDriver(string browser = "chrome")
    {
        IWebDriver driver;

        switch (browser.ToLower())
        {
            case "firefox":
                var ffOptions = new FirefoxOptions();
                ffOptions.AddArgument("--headless");
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
                options.AddArgument("--window-size=1920,1080");
                options.AddArgument("--disable-extensions");
                
                driver = new ChromeDriver(options);
                break;
            
            default:
                throw new NotSupportedException($"Browser '{browser}' is not supported.");
        }

        return driver;
    }
}