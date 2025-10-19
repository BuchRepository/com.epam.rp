using com.epam.rp.core;
using com.epam.rp.ui.Business.Enums;
using com.epam.rp.ui.Core.Elements;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace com.epam.rp.ui.Business.Pages;

public class FiltersPage : BasePage
{
    private readonly Button _filtersMenuItem;
    private readonly Button _addFilterButton;
    private readonly Button _updateButton;
    private readonly Button _confirmDeleteFilterButton;
    private readonly Input _filterNameInput;
    
    public FiltersPage(IWebDriver driver) : base(driver)
    {
        _filtersMenuItem = new Button(driver, By.XPath("//a[contains(@href,'/filters')]"), "Filters menu item");
        _addFilterButton = new Button(driver, By.XPath("//span[text()='Add Filter']"), "Add Filter button");
        _updateButton = new Button(driver, By.XPath("//button[text()='Update']"), "Update filter button");
        _confirmDeleteFilterButton = new Button(driver, By.XPath("//button[text()='Delete']"), "Confirm Delete Filter button");
        _filterNameInput = new Input(driver, By.XPath("//input[@placeholder='Enter filter name']"), "Filter name input");
    }
    
    private By FilterByName(string name) => By.XPath($"//span[contains(@class,'filterName__name') and text()='{name}']");
    private By EditButtonByName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'filterName__pencil')][1]");
    private By DeleteButtonByName(string name) => By.XPath($"//span[text()='{name}']/following::div[contains(@class, 'deleteFilterButton')][1]");
    private By ToggleByName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'inputSwitcher')][1]");
    private By StateByName(string name, FiltersState state) => 
        By.XPath($"//span[text()='{name}']/following::span[text()='{state.ToString().ToUpper()}'][1]");
    private By ToggleStateByFilterName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'displayFilter')][1]");
    
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
    
    /*
    public bool WaitForFilterVisibility(string filterName, bool shouldExist = true)
    {
        try
        {
            return Wait.Until(driver =>
            {
                var elements = driver.FindElements(FilterByName(filterName));
                if (shouldExist)
                {
                    return elements.Any(e => e.Displayed);
                }
                else
                {
                    return elements.Count == 0;
                }
            });
        }
        catch (WebDriverTimeoutException)
        {
            LoggerService.Warn($"Filter '{filterName}' did not reach state {shouldExist}. Retrying once...");
            Thread.Sleep(2000);

            var elements = Driver.FindElements(FilterByName(filterName));
            var isVisible = elements.Any(e => e.Displayed);
            return isVisible == shouldExist;
        }
    }*/
    
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
    
    /*
    private Checkbox GetDisplayOnLaunchesCheckbox(string filterName)
    {
        return new Checkbox(Driver, ToggleByName(filterName), $"Display on Launches for '{filterName}'");
    } */
    
    public bool GetFilterDisplayStatus(string filterName)
    {
        var statusLocator = ToggleByName(filterName);

        try
        {
            var statusElement = FindVisible(statusLocator);
            var statusText = statusElement.Text.Trim().ToUpperInvariant();
            LoggerService.Info($"Display status for '{filterName}' is '{statusText}'");

            return statusText == "ON";
        }
        catch (WebDriverTimeoutException)
        {
            LoggerService.Warn($"Timeout: could not find display status element for '{filterName}'");
            return false;
        }
    }


    public void EnableDisplayOnLaunches(string filterName)
    {
        if (Find(StateByName(filterName, FiltersState.Off)).Text.Trim().ToUpperInvariant() == "OFF")
        {
            Click(ToggleByName(filterName));
        }
    }
    
    public string StateTextOfFilterToggle(string filterName)
    {
        var stateOfFilterToggle= Find(ToggleStateByFilterName(filterName));
        return stateOfFilterToggle.Text.Trim().ToUpperInvariant();
    }

    public void DisableDisplayOnLaunches(string filterName)
    {
        try
        {
            var stateOfFilterToggle= Find(ToggleStateByFilterName(filterName));
            var currentState = stateOfFilterToggle.Text.Trim().ToUpperInvariant();

            LoggerService.Info($"Current Display on Launches state for '{filterName}' is '{currentState}'");

            if (currentState == "ON")
            {
                LoggerService.Info($"Disabling Display on Launches for '{filterName}'");
                Click(ToggleByName(filterName));
                LoggerService.Info($"State of element after click for '{filterName}' is '{StateTextOfFilterToggle(filterName)}'");
            }
            else
            {
                LoggerService.Info($"Display on Launches already OFF for '{filterName}', no action taken");
            }
        }
        catch (NoSuchElementException)
        {
            LoggerService.Warn($"Could not find state element for filter '{filterName}'");
        }
        catch (Exception ex)
        {
            LoggerService.Error($"Error disabling Display on Launches for '{filterName}': {ex.Message}");
        }
    }


    public LaunchesPage WaitForState(string filterName, FiltersState state)
    {
        FindVisible(StateByName(filterName, state));
        return new LaunchesPage(Driver);
    }
    
    public void EditFilter(string oldName, string newName)
    {
        Click(EditButtonByName(oldName));
        LoggerService.Info("Enter new filter name");
        _filterNameInput.Type(newName);
        _updateButton.ClickButton();
    }
    
    public bool IsFilterVisible(string filterName)
    {
        try
        {
            var el = Find(FilterByName(filterName));

            if (el.Displayed)
            {
                LoggerService.Info($"Filter '{filterName}' is visible.");
                return true;
            }

            LoggerService.Warn($"Filter '{filterName}' exists but is not visible.");
            return false;
        }
        catch (NoSuchElementException)
        {
            LoggerService.Warn($"Filter '{filterName}' not found in DOM.");
            return false;
        }
        catch (WebDriverTimeoutException)
        {
            LoggerService.Warn($"Timeout while searching for filter '{filterName}'.");
            return false;
        }
        catch (Exception ex)
        {
            LoggerService.Error($"Unexpected error checking filter '{filterName}': {ex.Message}");
            return false;
        }
    }
    
    public void GoToFilters()
    {
        Driver.Navigate().GoToUrl("https://rp.epam.com/ui/#sergii_buchkivskyi_personal/filters");
        Wait.Until(driver => driver.Url.Contains("/filters"));
    }
}