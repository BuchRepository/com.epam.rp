using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace com.epam.rp.ui.Core;

public static class DriverFactory
{
    public static IWebDriver CreateDriver(string browser = "chrome")
    {
        var runRemote = Environment.GetEnvironmentVariable("RUN_REMOTE")?.ToLower() == "true";
        var gridUrl = Environment.GetEnvironmentVariable("SELENIUM_GRID_URL") ?? "http://localhost:4444/wd/hub";

        return runRemote
            ? CreateRemoteDriver(browser, gridUrl)
            : CreateLocalDriver(browser);
    }

    private static IWebDriver CreateLocalDriver(string browser)
    {
        switch (browser.ToLower())
        {
            case "firefox":
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.AddArguments("-headless", "--width=1920", "--height=1080");
                return new FirefoxDriver(firefoxOptions);

            case "chrome":
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("--headless=new", "--no-sandbox", "--disable-dev-shm-usage",
                    "--disable-gpu", "--window-size=1920,1080");

                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                return new ChromeDriver(service, chromeOptions, TimeSpan.FromSeconds(120));

            default:
                throw new NotSupportedException($"Browser '{browser}' is not supported locally.");
        }
    }

    private static IWebDriver CreateRemoteDriver(string browser, string gridUrl)
    {
        DriverOptions options = browser.ToLower() switch
        {
            "firefox" => new FirefoxOptions
            {
                AcceptInsecureCertificates = true
            },
            "chrome" => new ChromeOptions
            {
                AcceptInsecureCertificates = true
            },
            _ => throw new NotSupportedException($"Browser '{browser}' is not supported.")
        };

        if (browser.Equals("chrome", StringComparison.OrdinalIgnoreCase))
        {
            ((ChromeOptions)options).AddArguments("--headless=new", "--no-sandbox", "--disable-dev-shm-usage",
                "--disable-gpu", "--window-size=1920,1080");
        }
        else if (browser.Equals("firefox", StringComparison.OrdinalIgnoreCase))
        {
            ((FirefoxOptions)options).AddArguments("-headless", "--width=1920", "--height=1080");
        }

        return new RemoteWebDriver(new Uri(gridUrl), options.ToCapabilities(), TimeSpan.FromSeconds(180));
    }
}