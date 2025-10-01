using com.epam.rp.core;
using com.epam.rp.ui.Core;
using OpenQA.Selenium;

namespace com.epam.rp.ui.Business.Pages;

public class LaunchesPage : BasePage
{
    public LaunchesPage(IWebDriver driver) : base(driver) { }

    private readonly By _saveButton = By.XPath("//span[text()='Save']"); 
    private readonly By _addFilterButton = By.XPath("//button[contains(text(), 'Add')]"); 
    private readonly By _filtersMenuItem = By.XPath("//a[contains(@href,'/filters')]");
    private readonly By _filterNameInput = By.XPath("//input[@placeholder='Enter filter name']");
    private readonly By _moreOptionsButton = By.XPath("//div[text()='More']");
    private readonly By _enterQuantityInput = By.XPath("//input[@placeholder='Enter quantity']");
    
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
        Click(_filtersMenuItem);
    }

    private void SelectParameter(string parameter,  string quantity)
    {
        Click(_moreOptionsButton);
        Click(By.XPath($"//span[text()='{parameter}']"));
        Type(_enterQuantityInput, quantity);
    }

    private void EnterFilterName(string filterName) => Type(_filterNameInput, filterName);
    private void SaveFilter() => Click(_saveButton);
    private void ConfirmAddFilter() => Click(_addFilterButton);

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