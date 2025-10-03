using com.epam.rp.core;
using OpenQA.Selenium;

namespace com.epam.rp.ui.Core.Elements;

public class Checkbox : BaseElement
{
    public Checkbox(IWebDriver driver, By locator, string name)
        : base(driver, locator, name)
    {
    }

    public void Check()
    {
        if (!IsChecked())
        {
            ClickButton();
            LoggerService.Info($"Checked checkbox: {Name}");
        }
        else
        {
            LoggerService.Info($"Checkbox {Name} was already checked");
        }
    }

    public void Uncheck()
    {
        if (IsChecked())
        {
            ClickButton();
            LoggerService.Info($"Unchecked checkbox: {Name}");
        }
        else
        {
            LoggerService.Info($"Checkbox {Name} was already unchecked");
        }
    }

    public bool IsChecked()
    {
        bool isChecked = Element.Selected;
        LoggerService.Info($"Checkbox {Name} is {(isChecked ? "checked" : "unchecked")}");
        return isChecked;
    }
}
