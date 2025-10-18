using com.epam.rp.core;
using com.epam.rp.ui.Business.Enums;
using com.epam.rp.ui.Core.Elements;
using OpenQA.Selenium;

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
        if (GetFilterDisplayStatus(filterName) == false)
        {
            FindClickable(ToggleByName(filterName)).Click();
        }
    }

    public void DisableDisplayOnLaunches(string filterName)
    {
        if (GetFilterDisplayStatus(filterName))
        {
            FindClickable(ToggleByName(filterName)).Click();
        }
    }

    public void WaitForState(string filterName, FiltersState state)
    {
        FindVisible(StateByName(filterName, state));
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
            Driver.Navigate().Refresh();
            LoggerService.Info($"Refreshing page to check if filter '{filterName}' is visible");
            Thread.Sleep(2000);
            
            var elements = Driver.FindElements(FilterByName(filterName));

            if (!elements.Any())
            {
                LoggerService.Warn($"No elements found for filter '{filterName}'");
                return false;
            }

            foreach (var el in elements)
            {
                LoggerService.Info($"Found element text: '{el.Text}'");
            }
            
            FindVisible(FilterByName(filterName));
            return true;
        }
        catch (WebDriverTimeoutException)
        {
            LoggerService.Warn($"Filter '{filterName}' is not visible.");
            return false;
        }
        catch (NoSuchElementException)
        {
            LoggerService.Warn($"Filter '{filterName}' does not exist.");
            return false;
        }
    }
}