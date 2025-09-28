using com.epam.rp.core;
using com.epam.rp.ui.Business.Enums;
using com.epam.rp.ui.Core;
using OpenQA.Selenium;

namespace com.epam.rp.ui.Business.Pages;

public class FiltersPage : BasePage
{
    public FiltersPage(IWebDriver driver) : base(driver) { }

    private readonly By _filtersMenuItem = By.XPath("//a[contains(@href,'/filters')]");
    private readonly By _addFilterButton   = By.XPath("//span[text()='Add Filter']");
    private readonly By _confirmDeleteFilterButton = By.XPath("//button[text()='Delete']");
    
    private By FilterByName(string name) => By.XPath($"//span[text()='{name}']");
    private By DeleteButtonByName(string name) => By.XPath($"//span[text()='{name}']/following::div[contains(@class, 'deleteFilterButton')][1]");
    private By ToggleByName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'inputSwitcher')][1]");
    private By StateByName(string name, FiltersState state) => 
        By.XPath($"//span[text()='{name}']/following::span[text()='{state.ToString().ToUpper()}'][1]");
    
    public void OpenFiltersPage()
    {
        Click(_filtersMenuItem);
    } 
    
    public LaunchesPage ClickAddFilter()
    {
        Click(_addFilterButton);
        return new LaunchesPage(Driver);
    }
    
    public void DeleteFilter(string filterName)
    {
        Click(DeleteButtonByName(filterName));
        Click(_confirmDeleteFilterButton);
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

    public void ToggleDisplayOnLaunches(string filterName)
    {
        Click(ToggleByName(filterName));
    }

    public void WaitForState(string filterName, FiltersState state)
    {
        FindVisible(StateByName(filterName, state));
    }
}