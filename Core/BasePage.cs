using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Serilog;
using TechTalk.SpecFlow;

namespace com.epam.rp.Core;

public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly WebDriverWait _wait;
    protected readonly ILogger _logger;

    protected BasePage(IWebDriver driver,  int defaultTimeout = 5)
    {
        _driver = driver;
        _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultTimeout));
        
        if (ScenarioContext.Current.TryGetValue("logger", out ILogger logger))
        {
            _logger = logger;
        }
        else
        {
            _logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }
    }

    protected IWebElement Find(By locator) => 
        _wait.Until(ExpectedConditions.ElementExists(locator));
        
    protected IWebElement FindVisible(By locator) => 
        _wait.Until(ExpectedConditions.ElementIsVisible(locator));
        
    protected IWebElement FindClickable(By locator) => 
        _wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        
    public void Click(By locator)
    {
        int attempts = 0;
        while (attempts < 3)
        {
            try
            {
                _wait.Until(driver =>
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
            
                _logger.Information($"Clicked on element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                _logger.Warning($"Attempt {attempts} failed for click on {locator}: {ex.Message}");
                if (attempts == 3)
                {
                    try
                    {
                        var element = _driver.FindElement(locator);
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
                        _logger.Information($"JS click succeeded on element: {locator}");
                        return;
                            
                    }
                    catch (Exception jsex)
                    {
                        _logger.Error(jsex, $"JS click failed on element: {locator}");
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
                _wait.Until(_ =>
                {
                    try
                    {
                        var el = _driver.FindElement(locator);
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

                _logger.Information($"Successfully typed '{text}' into element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                _logger.Warning($"Attempt {attempts} failed for typing into {locator}: {ex.Message}");
                if (attempts == 3)
                {
                    throw;
                }
            }
        }
    }
}