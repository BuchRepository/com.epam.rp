using com.epam.rp.core;
using com.epam.rp.ui.Business.Enums;
using com.epam.rp.ui.Core.Elements;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace com.epam.rp.ui.Business.Pages;

public class FiltersPage : BasePage
{
    private readonly Button _filtersMenuItem;
    private readonly Button _launchesMenuItem;
    private readonly Button _addFilterButton;
    private readonly Button _updateButton;
    private readonly Button _confirmDeleteFilterButton;
    private readonly Input _filterNameInput;
    
    public FiltersPage(IWebDriver driver) : base(driver)
    {
        _filtersMenuItem = new Button(driver, By.XPath("//a[contains(@href,'/filters')]"), "Filters menu item");
        _launchesMenuItem = new Button(driver, By.XPath("//a[contains(@href,'launches') and contains(@class,'sidebarButton')]"), "Launches menu item");
        _addFilterButton = new Button(driver, By.XPath("//span[text()='Add Filter']"), "Add Filter button");
        _updateButton = new Button(driver, By.XPath("//button[text()='Update']"), "Update filter button");
        _confirmDeleteFilterButton = new Button(driver, By.XPath("//button[text()='Delete']"), "Confirm Delete Filter button");
        _filterNameInput = new Input(driver, By.XPath("//input[@placeholder='Enter filter name']"), "Filter name input");
    }
    
    private By EditButtonByName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'filterName__pencil')][1]");
    private By DeleteButtonByName(string name) => By.XPath($"//span[text()='{name}']/following::div[contains(@class, 'deleteFilterButton')][1]");
    private By ToggleByName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'inputSwitcher')][1]");
    private By FilterToggleStateLocator(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'displayFilter')][1]");
    
    public void OpenFiltersPage()
    {
        _filtersMenuItem.ClickButton();
    }
    
    public void OpenLaunchesPage()
    {
        _launchesMenuItem.ClickButton();
    } 
    
    public LaunchesPage ClickAddFilter()
    {
        LoggerService.Info($"Current URL: {Driver.Url}");
        _addFilterButton.ClickButton();
        return new LaunchesPage(Driver);
    }
    
    public void DeleteFilter(string filterName)
    {
        Click(DeleteButtonByName(filterName));
        _confirmDeleteFilterButton.ClickButton();
    }
    
    public string GetFilterToggleState(string filterName)
    {
        var stateOfFilterToggle= Find(FilterToggleStateLocator(filterName));
        return stateOfFilterToggle.Text.Trim().ToUpperInvariant();
    }

    public void SetStateOfFilter(string filterName, FiltersState targetState)
    {
        try
        {
            var currentState = GetFilterToggleState(filterName).ToUpperInvariant();
            var desiredState = targetState.ToString().ToUpper();

            LoggerService.Info($"Current state of '{filterName}' is '{currentState}', target state is '{desiredState}'");

            if (currentState != desiredState)
            {
                LoggerService.Info($"Changing Display on Launches for '{filterName}' from '{currentState}' to '{desiredState}'");
                Click(ToggleByName(filterName));

                var newState = GetFilterToggleState(filterName);
                LoggerService.Info($"State of element after click for '{filterName}' is '{newState}'");
            }
            else
            {
                LoggerService.Info($"Display on Launches for '{filterName}' is already '{desiredState}', no action taken");
            }
        }
        catch (NoSuchElementException)
        {
            LoggerService.Warn($"Could not find state element for filter '{filterName}'");
        }
        catch (Exception ex)
        {
            LoggerService.Error($"Error setting Display on Launches for '{filterName}' to '{targetState.ToString().ToUpper()}': {ex.Message}");
        }
    }
    
    public bool IsFilterVisible(string filterName)
    {
        try
        {
            LoggerService.Info($"Checking visibility of filter '{filterName}' on Filters page.");

            var filterLocator = By.XPath($"//span[text()='{filterName}']");
            var element = Wait.Until(ExpectedConditions.ElementExists(filterLocator));

            bool visible = element.Displayed;
            LoggerService.Info($"Filter '{filterName}' is {(visible ? "visible" : "not visible")} on Filters page.");
            return visible;
        }
        catch (WebDriverTimeoutException)
        {
            LoggerService.Warn($"Filter '{filterName}' not visible on Filters page within timeout.");
            return false;
        }
        catch (NoSuchElementException)
        {
            LoggerService.Warn($"Filter '{filterName}' not found on Filters page.");
            return false;
        }
    }

    
    public void EditFilter(string oldName, string newName)
    {
        Click(EditButtonByName(oldName));
        LoggerService.Info("Enter new filter name");
        _filterNameInput.Type(newName);
        _updateButton.ClickButton();
    }
}