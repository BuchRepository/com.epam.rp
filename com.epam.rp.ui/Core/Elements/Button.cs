using OpenQA.Selenium;

namespace com.epam.rp.ui.Core.Elements;

public class Button : BaseElement
{
    public Button(IWebDriver driver, By locator, string name) 
        : base(driver, locator, name) { }
}