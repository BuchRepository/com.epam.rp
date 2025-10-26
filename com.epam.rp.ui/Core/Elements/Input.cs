using com.epam.rp.core;
using OpenQA.Selenium;

namespace com.epam.rp.ui.Core.Elements;

public class Input : BaseElement
{
    public Input(IWebDriver driver, By locator, string name) 
        : base(driver, locator, name) { }

    public void Type(string text)
    {
        var element = FluentWait();
        element.Clear();
        element.SendKeys(text);
        LoggerService.Info($"Typed '{text}' into input: {Name}");
    }
}