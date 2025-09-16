using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Serilog;

namespace Core;

public abstract class BasePage
{
    protected readonly IWebDriver _driver;
    protected readonly WebDriverWait _wait;

    protected BasePage(IWebDriver driver, int defaultTimeout = 10)
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
            
                Log.Information($"Successful click on element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                Log.Warning($"Attempt {attempts} failed for click on {locator}: {ex.Message}");
                if (attempts == 3)
                {
                    try
                    {
                        var element = _driver.FindElement(locator);
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
                        Log.Information($"JS click succeeded on element: {locator}");
                        return;
                    }
                    catch (Exception jsex)
                    {
                        //TakeScreenshot($"ClickError_{DateTime.Now:yyyyMMdd_HHmmss}.png");
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

                Log.Information($"Successfully typed '{text}' into element: {locator}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                Log.Warning($"Attempt {attempts} failed for typing into {locator}: {ex.Message}");
                if (attempts == 3)
                {
                    //TakeScreenshot($"TypeError_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    throw;
                }
            }
        }
    }
    
    public void TakeScreenshot(string fileName)
    {
        try
        {
            var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
            screenshot.SaveAsFile(fileName);
            Log.Information($"Screenshot saved: {fileName}");
        }
        catch (Exception e)
        {
            Log.Error(e, "Screenshot did not capture");
        }
    }
}