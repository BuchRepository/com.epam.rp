using System.Linq;
using com.epam.rp.Core;
using Serilog;
using TechTalk.SpecFlow;

namespace com.epam.rp.Business.Pages;
    
using OpenQA.Selenium;

public class LaunchesPage : BasePage
{
    
    public LaunchesPage(IWebDriver driver, ScenarioContext context) : base(driver, context) { }

    private readonly By _saveButton = By.XPath("//span[text()='Save']"); 
    private readonly By _addFilterButton = By.XPath("//button[contains(text(), 'Add')]"); 
    private readonly By _filtersMenuItem = By.XPath("//a[contains(@href,'/filters')]");
    private readonly By _filterNameInput = By.XPath("//input[@placeholder='Enter filter name']");
    private readonly By _moreOptionsButton = By.XPath("//div[text()='More']");
    private readonly By _enterQuantityInput = By.XPath("//input[@placeholder='Enter quantity']");


    public void AddFilter(string filterName, string parameter, string quantity)
    {
        Log.Information("Select specific parameter and quantity");
        SelectParameter(parameter, quantity);
        Log.Information("Click save button");
        SaveFilter();
        Log.Information("Enter filter name");
        EnterFilterName(filterName);
        Log.Information("Confirm filter adding");
        ConfirmAddFilter();
        Log.Information("Back to 'Filters' page");
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
                        Log.Information("Filter '{FilterName}' not found in DOM.", filterName);
                        return !shouldBeVisible; 
                    }
                    
                    bool isDisplayed = elems.Any(e => e.Displayed && e.Enabled && e.Size.Height > 0 && e.Size.Width > 0);


                    if (isDisplayed)
                        Log.Information("Filter '{FilterName}' is currently visible.", filterName);
                    else
                        Log.Information("Filter '{FilterName}' is currently hidden.", filterName);

                    return shouldBeVisible ? isDisplayed : !isDisplayed;
                }
                catch (StaleElementReferenceException)
                {
                    Log.Warning("StaleElementReferenceException encountered for filter '{FilterName}', retrying...", filterName);
                    return !shouldBeVisible;
                }
            });
        }
        catch (WebDriverTimeoutException)
        {
            Log.Error("Timeout waiting for filter '{FilterName}' to be {ExpectedState}.", filterName, 
                shouldBeVisible ? "visible" : "hidden");
            return false;
        }
    }

    public void RefreshPage()
    {
        Driver.Navigate().Refresh();
    }
}