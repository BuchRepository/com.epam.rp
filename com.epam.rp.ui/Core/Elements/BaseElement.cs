using com.epam.rp.core;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace com.epam.rp.ui.Core.Elements;

using OpenQA.Selenium;

public class BaseElement
{
    protected readonly IWebDriver Driver;
    protected readonly By Locator;
    protected readonly string Name;
    protected readonly WebDriverWait Wait;

    public BaseElement(IWebDriver driver, By locator, string name, int timeout = 10)
    {
        Driver = driver;
        Locator = locator;
        Name = name;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
    }

    protected IWebElement Element => Driver.FindElement(Locator);
    
    protected IWebElement FluentWait(int timeoutInSeconds = 10, int pollingIntervalMs = 500)
    {
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(timeoutInSeconds),
            PollingInterval = TimeSpan.FromMilliseconds(pollingIntervalMs)
        };

        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));

        return fluentWait.Until(drv =>
        {
            return (Element.Displayed && Element.Enabled) ? Element : null;
        });
    }
    
    public virtual void ClickButton()
    {
        try
        {
            FluentWait().Click();
            LoggerService.Info($"Clicked on element: {Name}");
        }
        catch (Exception ex)
        {
            LoggerService.Warn($"Standard click failed for {Name}: {ex.Message}, trying JS click");
            JsClick();
        }
    }

    public void JsClick()
    {
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", Element);
        LoggerService.Info($"Clicked with JS on: {Name}");
    }
}
