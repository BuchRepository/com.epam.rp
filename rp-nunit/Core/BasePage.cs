using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Core;

public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly WebDriverWait _wait;

    protected BasePage(IWebDriver driver, int defaultTimeout = 20)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultTimeout));
    }

    protected IWebElement Find(By locator) => 
        _wait.Until(ExpectedConditions.ElementExists(locator));
        
    protected IWebElement FindVisible(By locator) => 
        _wait.Until(ExpectedConditions.ElementIsVisible(locator));
        
    protected IWebElement FindClickable(By locator) => 
        _wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        
    public void Click(By locator)
    {
        FindClickable(locator).Click();
    }

    public void Type(By locator, string text)
    {
        FindVisible(locator).SendKeys(text);
    }
}