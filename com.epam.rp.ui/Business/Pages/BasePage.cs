using com.epam.rp.core;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Serilog;

namespace com.epam.rp.ui.Business.Pages;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver, int defaultTimeout = 10)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultTimeout));
    }

    protected IWebElement Find(By locator) => 
        Wait.Until(ExpectedConditions.ElementExists(locator));
    
    protected IWebElement FindVisible(By locator) => 
        Wait.Until(driver => 
        {
            var el = driver.FindElement(locator);
            return el.Displayed ? el : null;
        });
        
    protected IWebElement FindClickable(By locator) => 
        Wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        
    public void Click(By locator)
    {
        int attempts = 0;
        while (attempts < 3)
        {
            try
            {
                Wait.Until(driver =>
                {
                    try
                    {
                        var el = driver.FindElement(locator);
                        if (el.Displayed && el.Enabled)
                        {
                            try
                            {
                                el.Click();
                                return true;
                            }
                            catch (ElementClickInterceptedException)
                            {
                                Thread.Sleep(200);
                                return false;
                            }
                        }
                        return false;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                });
            
                LoggerService.Info($"Successful click on element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                LoggerService.Warn($"Attempt {attempts} failed for click on {locator}: {ex.Message}");
                if (attempts == 3)
                {
                    try
                    {
                        var element = Driver.FindElement(locator);
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
                        LoggerService.Info($"JS click succeeded on element: {locator}");
                        return;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "JS click failed");
                    }
                }
            }
        }
    }

    public void Type(By locator, string text)
    {
        int attempts = 0;
        while (attempts < 3)
        {
            try
            {
                Wait.Until(_ =>
                {
                    try
                    {
                        var el = Driver.FindElement(locator);
                        if (el.Displayed && el.Enabled)
                        {
                            el.Clear();
                            el.SendKeys(text);
                            return true;
                        }
                        return false;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                });

                LoggerService.Info($"Successfully typed '{text}' into element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                LoggerService.Warn($"Attempt {attempts} failed for typing into {locator}: {ex.Message}");
            }
        }
    }
    
    public bool IsFilterVisible(string filterName)
{
    try
    {
        Thread.Sleep(1500);
        string currentUrl = Driver.Url;
        LoggerService.Info($"Checking visibility of filter '{filterName}' on {currentUrl}");

        if (currentUrl.Contains("/filters"))
        {
            var filterLocator = By.XPath($"//span[text()='{filterName}']");
            var el = Find(filterLocator);

            if (el.Displayed)
            {
                LoggerService.Info($"Filter '{filterName}' is visible on Filters page.");
                return true;
            }
            LoggerService.Warn($"Filter '{filterName}' exists but is hidden on Filters page.");
            return false;
        }

        else if (currentUrl.Contains("/launches"))
        {
            var filterLocator = By.XPath($"//span[contains(text(),'{filterName}')]");
            var elements = Driver.FindElements(filterLocator);

            bool visible = elements.Any(e => e.Displayed);
            LoggerService.Info(visible
                ? $"Filter '{filterName}' is visible on Launches page."
                : $"Filter '{filterName}' is not visible on Launches page.");
            return visible;
        }

        LoggerService.Warn($"Unknown page context while checking filter '{filterName}'. URL: {currentUrl}");
        return false;
    }
    catch (NoSuchElementException)
    {
        LoggerService.Warn($"Filter '{filterName}' not found in DOM.");
        return false;
    }
    catch (WebDriverTimeoutException)
    {
        LoggerService.Warn($"Timeout while searching for filter '{filterName}'.");
        return false;
    }
    catch (Exception ex)
    {
        LoggerService.Error($"Unexpected error checking filter '{filterName}': {ex.Message}");
        return false;
    }
}

    
    public void RefreshPage()
    {
        Driver.Navigate().Refresh();
    }
}