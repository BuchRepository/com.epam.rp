using Allure.NUnit;
using OpenQA.Selenium;

namespace ReportPortal.Core;

public class TestBase
{
    protected IWebDriver? Driver;
        
    [SetUp]
    public void SetUp()
    {
        string browser = TestContext.Parameters.Get("browser", "chrome");
        Driver = DriverFactory.CreateDriver(browser);
        if (Driver == null)
        {
            throw new InvalidOperationException("Driver initialization failed.");
        }
        Driver.Navigate().GoToUrl("https://rp.epam.com");
    }

    [TearDown]
    public void TearDown()
    {
        if (Driver != null)
        {
            Driver.Quit();
            Driver.Dispose();
        }
        Driver = null;
    }
}