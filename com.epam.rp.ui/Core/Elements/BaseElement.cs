using com.epam.rp.core;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace com.epam.rp.ui.Core.Elements;

using OpenQA.Selenium;

public class BaseElement
{
    protected readonly IWebDriver Driver;
    protected readonly By Locator;
    protected readonly string Name;
    protected readonly WebDriverWait Wait;

    public BaseElement(IWebDriver driver, By locator, string name, int timeout = 20)
    {
        Driver = driver;
        Locator = locator;
        Name = name;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
    }

    protected IWebElement Element
    {
        get
        {
            try
            {
                return Wait.Until(drv =>
                {
                    var element = drv.FindElement(Locator);
                    return element.Displayed ? element : null;
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                LoggerService.Error($"Timeout waiting for element '{Name}' by locator: {Locator}. {ex.Message}");
                throw;
            }
        }
    }
    
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
            var element = drv.FindElement(Locator);
            return (element.Displayed && element.Enabled) ? element : null;
        });
    }
    
    public virtual void ClickButton()
    {
        int attempts = 0;
        while (attempts < 3)
        {
            try
            {
                FluentWait().Click();
                LoggerService.Info($"Clicked on element: {Name}");
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                LoggerService.Warn($"Standard click failed for {Name}: {ex.Message}, trying JS click");
                Thread.Sleep(300);
            }
        }
        JsClick();
    }
    
    #region JS Executor Methods
    
    private IJavaScriptExecutor Js => (IJavaScriptExecutor)Driver;
    
    public void ScrollToElement()
    {
        Js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", Element);
        LoggerService.Info($"Scrolled to element: {Name}");
    }

    public bool IsScrolledIntoView()
    {
        object? result = Js.ExecuteScript(@"
        const rect = arguments[0].getBoundingClientRect();
        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth)
        );
    ", Element);

        bool isInView = result is bool b && b;

        LoggerService.Info($"Element '{Name}' is in view: {isInView}");
        return isInView;
    }

    public void JsClick()
    {
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", Element);
        LoggerService.Info($"Clicked with JS on: {Name}");
    }
    
    #endregion
    
    #region Actions Methods

    public void DragAndDropTo(BaseElement target)
    {
        var actions = new Actions(Driver);
        actions.DragAndDrop(Element, target.Element).Perform();
        LoggerService.Info($"Dragged '{Name}' to '{target.Name}'");
    }

    public void ResizeByOffset(int offsetX, int offsetY)
    {
        var actions = new Actions(Driver);
        actions.ClickAndHold(Element)
            .MoveByOffset(offsetX, offsetY)
            .Release()
            .Perform();
        LoggerService.Info($"Resized element '{Name}' by X:{offsetX}, Y:{offsetY}");
    }

    #endregion
}
