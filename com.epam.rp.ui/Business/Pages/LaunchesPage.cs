using com.epam.rp.core;
using com.epam.rp.ui.Core;
using com.epam.rp.ui.Core.Elements;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace com.epam.rp.ui.Business.Pages;

public class LaunchesPage : BasePage
{
    private readonly Button _saveButton;
    private readonly Button _addFilterButton;
    private readonly Input _filterNameInput;
    private readonly Input _enterQuantityInput;
    private readonly By _filtersMenuInput;
    private readonly By _moreOptionsButton;

    public LaunchesPage(IWebDriver driver) : base(driver)
    {
        _saveButton = new Button(driver, By.XPath("//span[text()='Save']"), "Save button");
        _addFilterButton = new Button(driver, By.XPath("//button[contains(text(), 'Add')]"), "Add filter button");
        _filterNameInput = new Input(driver, By.XPath("//input[@placeholder='Enter filter name']"), "Filter name input");
        _enterQuantityInput = new Input(driver, By.XPath("//input[@placeholder='Enter quantity']"), "Enter quantity input");
        _filtersMenuInput= By.XPath("//a[contains(@href,'/filters')]");
        _moreOptionsButton = By.XPath("//div[text()='More']");
    }

    //private readonly By _saveButton = By.XPath("//span[text()='Save']");
    /*private readonly By _addFilterButton = By.XPath("//button[contains(text(), 'Add')]"); 
    private readonly By _filtersMenuItem = By.XPath("//a[contains(@href,'/filters')]");
    private readonly By _filterNameInput = By.XPath("//input[@placeholder='Enter filter name']");
    private readonly By _moreOptionsButton = By.XPath("//div[text()='More']");
    private readonly By _enterQuantityInput = By.XPath("//input[@placeholder='Enter quantity']");*/
    
    public void AddFilter(string filterName, string parameter, string quantity)
    {
        LoggerService.Info("Select specific parameter and quantity");
        SelectParameter(parameter, quantity);
        LoggerService.Info("Click save button");
        SaveFilter();
        LoggerService.Info("Enter filter name");
        EnterFilterName(filterName);
        LoggerService.Info("Confirm filter adding");
        ConfirmAddFilter();
        LoggerService.Info("Back to 'Filters' page");
        Click(_filtersMenuInput);
    }

    private void SelectParameter(string parameter,  string quantity)
    {
        Click(_moreOptionsButton);
        Click(By.XPath($"//span[text()='{parameter}']"));
        _enterQuantityInput.Type(quantity);
    }

    private void EnterFilterName(string filterName) => _filterNameInput.Type(filterName);
    private void SaveFilter() => _saveButton.ClickButton();
    private void ConfirmAddFilter() => _addFilterButton.ClickButton();

    public bool IsFilterVisible(string filterName, bool shouldBeVisible = true)
    {
        var locator = By.XPath($"//span[contains(@class,'filter') and normalize-space(text())='{filterName}']");
    
        try
        {
            return Wait.Until(drv =>
            {
                try
                {
                    var elems = drv.FindElements(locator);
                    if (elems.Count == 0) 
                    {
                        LoggerService.Info($"Filter '{filterName}' not found in DOM.");
                        return !shouldBeVisible; 
                    }
                    
                    bool isDisplayed = elems.Any(e => e.Displayed && e.Enabled && e.Size.Height > 0 && e.Size.Width > 0);
                    
                    if (isDisplayed)
                        LoggerService.Info($"Filter '{filterName}' is currently visible.");
                    else
                        LoggerService.Info($"Filter '{filterName}' is currently hidden.");

                    return shouldBeVisible ? isDisplayed : !isDisplayed;
                }
                catch (StaleElementReferenceException)
                {
                    LoggerService.Warn($"StaleElementReferenceException encountered for filter '{filterName}', retrying...");
                    return !shouldBeVisible;
                }
            });
        }
        catch (WebDriverTimeoutException)
        {
            var state = shouldBeVisible ? "visible" : "hidden";
            LoggerService.Error($"Timeout waiting for filter '{filterName}' to be {state}.");
            return false;
        }
    }

    public void RefreshPage()
    {
        Driver.Navigate().Refresh();
    }
}