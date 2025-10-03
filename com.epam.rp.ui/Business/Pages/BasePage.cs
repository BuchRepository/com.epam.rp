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

    protected BasePage(IWebDriver driver, int defaultTimeout = 5)
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
}