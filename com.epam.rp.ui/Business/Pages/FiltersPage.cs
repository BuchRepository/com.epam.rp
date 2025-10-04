using com.epam.rp.core;
using com.epam.rp.ui.Business.Enums;
using com.epam.rp.ui.Core.Elements;
using OpenQA.Selenium;

namespace com.epam.rp.ui.Business.Pages;

public class FiltersPage : BasePage
{
    private readonly Button _filtersMenuItem;
    private readonly Button _addFilterButton;
    private readonly Button _confirmDeleteFilterButton;
    
    public FiltersPage(IWebDriver driver) : base(driver)
    {
        _filtersMenuItem = new Button(driver, By.XPath("//a[contains(@href,'/filters')]"), "Filters menu item");
        _addFilterButton = new Button(driver, By.XPath("//span[text()='Add Filter']"), "Add Filter button");
        _confirmDeleteFilterButton = new Button(driver, By.XPath("//button[text()='Delete']"), "Confirm Delete Filter button");
    }
    
    private By FilterByName(string name) => By.XPath($"//span[text()='{name}']");
    private By DeleteButtonByName(string name) => By.XPath($"//span[text()='{name}']/following::div[contains(@class, 'deleteFilterButton')][1]");
    private By ToggleByName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'inputSwitcher')][1]");
    private By StateByName(string name, FiltersState state) => 
        By.XPath($"//span[text()='{name}']/following::span[text()='{state.ToString().ToUpper()}'][1]");
    
    public void OpenFiltersPage()
    {
        _filtersMenuItem.ClickButton();
    } 
    
    public LaunchesPage ClickAddFilter()
    {
        _addFilterButton.ClickButton();
        return new LaunchesPage(Driver);
    }
    
    public void DeleteFilter(string filterName)
    {
        Click(DeleteButtonByName(filterName));
        _confirmDeleteFilterButton.ClickButton();
    }
    
    public bool WaitForFilterVisibility(string filterName, bool shouldExist = true)
    {
        try
        {
            return Wait.Until(driver =>
            {
                var elements = driver.FindElements(FilterByName(filterName));
                var isVisible = elements.Any(e => e.Displayed);
                return isVisible == shouldExist;
            });
        }
        catch (WebDriverTimeoutException)
        {
            LoggerService.Warn($"Filter '{filterName}' did not reach state {shouldExist}");
            return false;
        }
    }

    /*public void ToggleDisplayOnLaunches(string filterName)
    {
        Click(ToggleByName(filterName));
    }*/
    
    private Checkbox GetDisplayOnLaunchesCheckbox(string filterName)
    {
        return new Checkbox(Driver, ToggleByName(filterName), $"Display on Launches for '{filterName}'");
    }

    public void EnableDisplayOnLaunches(string filterName)
    {
        var checkbox = GetDisplayOnLaunchesCheckbox(filterName);
        checkbox.Check();
    }

    public void DisableDisplayOnLaunches(string filterName)
    {
        var checkbox = GetDisplayOnLaunchesCheckbox(filterName);
        checkbox.Uncheck();
    }

    public void WaitForState(string filterName, FiltersState state)
    {
        FindVisible(StateByName(filterName, state));
    }
}