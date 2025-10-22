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
        Wait.Until(ExpectedConditions.ElementIsVisible(locator));

        
    protected IWebElement FindClickable(By locator) => 
        Wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        
    public void Click(By locator)
    {
        bool clicked = false;

        for (int attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                var element = FindClickable(locator);
                element.Click();
                LoggerService.Info($"Clicked on element: {locator} (attempt {attempt})");
                clicked = true;
                break;
            }
            catch (ElementClickInterceptedException)
            {
                LoggerService.Warn($"Click intercepted on {locator}, retrying... (attempt {attempt})");
                Thread.Sleep(200);
            }
            catch (Exception ex)
            {
                LoggerService.Warn($"Attempt {attempt} failed for click on {locator}: {ex.Message}");
            }
        }

        if (!clicked)
        {
            try
            {
                var element = Driver.FindElement(locator);
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
                LoggerService.Info($"Fallback JS click succeeded on element: {locator}");
            }
            catch (Exception e)
            {
                LoggerService.Error($"Fallback JS click failed on {locator}", e);
                throw;
            }
        }
    }

    public void Type(By locator, string text)
    {
        for (int attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                var element = FindVisible(locator);

                if (element.Enabled)
                {
                    element.Clear();
                    element.SendKeys(text);
                    LoggerService.Info($"Typed '{text}' into element: {locator}");
                    return;
                }

                LoggerService.Warn($"Element {locator} is not enabled (attempt {attempt})");
            }
            catch (StaleElementReferenceException)
            {
                LoggerService.Warn($"Stale element {locator}, retrying (attempt {attempt})...");
            }
            catch (WebDriverTimeoutException)
            {
                LoggerService.Warn($"Timeout locating element {locator} (attempt {attempt})");
            }
            catch (Exception ex)
            {
                LoggerService.Warn($"Attempt {attempt} failed to type into {locator}: {ex.Message}");
            }
            Thread.Sleep(300);
        }
        throw new WebDriverTimeoutException($"Failed to type into element {locator} after 3 attempts.");
    }
    
    public void RefreshPage()
    {
        Driver.Navigate().Refresh();
    }
}