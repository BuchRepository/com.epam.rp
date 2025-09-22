using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Serilog;
using TechTalk.SpecFlow;

namespace com.epam.rp.Core;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly ScenarioContext Context;
    protected readonly WebDriverWait Wait;
    protected readonly ILogger Logger;

    protected BasePage(IWebDriver driver,  ScenarioContext context, int defaultTimeout = 5)
    {
        Driver = driver;
        Context = context;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultTimeout));
        
        if (Context.TryGetValue("logger", out ILogger logger))
        {
            Logger = logger;
        }
        else
        {
            Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }
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
            
                Logger.Information($"Clicked on element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                Logger.Warning($"Attempt {attempts} failed for click on {locator}: {ex.Message}");
                if (attempts == 3)
                {
                    try
                    {
                        var element = Driver.FindElement(locator);
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
                        Logger.Information($"JS click succeeded on element: {locator}");
                        return;
                            
                    }
                    catch (Exception jsex)
                    {
                        Logger.Error(jsex, $"JS click failed on element: {locator}");
                        throw;
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

                Logger.Information($"Successfully typed '{text}' into element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                Logger.Warning($"Attempt {attempts} failed for typing into {locator}: {ex.Message}");
                if (attempts == 3)
                {
                    throw;
                }
            }
        }
    }
}