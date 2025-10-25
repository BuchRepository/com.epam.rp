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
        var useSauceLabs = Environment.GetEnvironmentVariable("USE_SAUCELABS")?.ToLower() == "true";
        
        Console.WriteLine($"[DEBUG] RUN_REMOTE={Environment.GetEnvironmentVariable("RUN_REMOTE")}");
        Console.WriteLine($"[DEBUG] USE_SAUCELABS={Environment.GetEnvironmentVariable("USE_SAUCELABS")}");
        
        if (useSauceLabs)
            return CreateSauceLabsDriver(browser);
        
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
            "firefox" => new FirefoxOptions { AcceptInsecureCertificates = true },
            "chrome" => new ChromeOptions { AcceptInsecureCertificates = true },
            _ => throw new NotSupportedException($"Browser '{browser}' is not supported.")
        };

        return new RemoteWebDriver(new Uri(gridUrl), options.ToCapabilities(), TimeSpan.FromSeconds(180));
    }
    
    private static IWebDriver CreateSauceLabsDriver(string browser)
    {
        var username = Environment.GetEnvironmentVariable("SAUCE_USERNAME");
        var accessKey = Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY");
        var buildTag = Environment.GetEnvironmentVariable("BUILD_TAG") ?? $"jenkins-build-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var testName = Environment.GetEnvironmentVariable("TEST_NAME") ?? "ReportPortal UI Tests";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(accessKey))
        {
            Console.WriteLine("[ERROR] Missing Sauce Labs credentials! Falling back to local driver.");
            return CreateLocalDriver(browser);
        }

        var sauceUrl = "https://ondemand.eu-central-1.saucelabs.com:443/wd/hub";

        Console.WriteLine($"[INFO] Creating RemoteWebDriver for Sauce Labs...");
        Console.WriteLine($"[INFO] Browser: {browser}");
        Console.WriteLine($"[INFO] Platform: Windows 11");

        var sauceOptions = new Dictionary<string, object>
        {
            ["username"] = username,
            ["accessKey"] = accessKey,
            ["build"] = buildTag,
            ["name"] = testName,
            ["screenResolution"] = "1920x1080",
            ["seleniumVersion"] = "4.23.0"
        };

        DriverOptions options = browser.ToLower() switch
        {
            "chrome" => new ChromeOptions
            {
                PlatformName = "Windows 11",
                BrowserVersion = "latest"
            },
            "firefox" => new FirefoxOptions
            {
                PlatformName = "Windows 11",
                BrowserVersion = "latest"
            },
            _ => throw new NotSupportedException($"Browser '{browser}' is not supported on Sauce Labs.")
        };

        options.AddAdditionalOption("sauce:options", sauceOptions);

        try
        {
            var driver = new RemoteWebDriver(new Uri(sauceUrl), options.ToCapabilities(), TimeSpan.FromSeconds(180));
            Console.WriteLine("[INFO] RemoteWebDriver successfully created on Sauce Labs.");
            return driver;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to create Sauce Labs driver: {ex.Message}");
            Console.WriteLine("[WARN] Falling back to local driver...");
            return CreateLocalDriver(browser);
        }
    }

    /*
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
    */
}