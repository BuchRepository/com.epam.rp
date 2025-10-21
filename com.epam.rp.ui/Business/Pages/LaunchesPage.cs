using com.epam.rp.core;
using com.epam.rp.ui.Core.Elements;
using OpenQA.Selenium;

namespace com.epam.rp.ui.Business.Pages;

public class LaunchesPage : BasePage
{
    private readonly Button _saveButton;
    private readonly Button _addFilterButton;
    private readonly Button _cloneButton;
    private readonly Input _filterNameInput;
    private readonly Input _enterQuantityInput;
    private readonly By _filtersMenuItem;
    private readonly By _moreOptions;
    private readonly By _launchesMenuItem;

    public LaunchesPage(IWebDriver driver) : base(driver)
    {
        _saveButton = new Button(driver, By.XPath("//span[text()='Save']"), "Save button");
        _addFilterButton = new Button(driver, By.XPath("//button[contains(text(), 'Add')]"), "Add filter button");
        _cloneButton = new Button(driver, By.XPath("//button[@title='Clone']"), "Clone button");
        _filterNameInput = new Input(driver, By.XPath("//input[@placeholder='Enter filter name']"), "Filter name input");
        _enterQuantityInput = new Input(driver, By.XPath("//input[@placeholder='Enter quantity']"), "Enter quantity input");
        _filtersMenuItem= By.XPath("//a[contains(@href,'/filters')]");
        _moreOptions = By.XPath("//div[text()='More']");
        _launchesMenuItem = By.XPath("//a[contains(@href,'launches') and contains(@class,'sidebarButton')]");
    }
    
    private By FilterByName(string name) => By.XPath($"//span[text()='{name}']");
    
    public void OpenLaunchesPage()
    {
        Click(_launchesMenuItem);
    } 
    
    public void OpenFiltersPage()
    {
        Click(_filtersMenuItem);
    } 
    
    public void AddFilter(string filterName, string parameter, string quantity)
    {
        LoggerService.Info($"Current URL: {Driver.Url}");
        LoggerService.Info("Select specific parameter and quantity");
        SelectParameter(parameter, quantity);
        SaveFilter();
        EnterFilterName(filterName);
        ConfirmAddFilter();
        LoggerService.Info("Back to 'Filters' page");
        Click(_filtersMenuItem);
    }
    
    private void SelectParameter(string parameter, string quantity)
    {
        Click(_moreOptions);

        var parameterCheckbox = new Checkbox(
            Driver,
            By.XPath($"//span[text()='{parameter}']"),
            $"Parameter '{parameter}'"
        );

        parameterCheckbox.Check();
        _enterQuantityInput.Type(quantity);
    }

    private void EnterFilterName(string filterName) => _filterNameInput.Type(filterName);
    private void SaveFilter() => _saveButton.ClickButton();
    private void ConfirmAddFilter() => _addFilterButton.ClickButton();
    
    public void ClickFilterByName(string filterName)
    {
        Click(FilterByName(filterName));
    }

    public void CopyFilter()
    {
        _cloneButton.ClickButton();
        SaveFilter();
        ConfirmAddFilter();
        LoggerService.Info("Back to 'Filters' page");
        Click(_filtersMenuItem);
    }
}