using com.epam.rp.Core;
using OpenQA.Selenium;
using Serilog;

namespace com.epam.rp.Business.Pages;

public class FiltersPage : BasePage
{
    public FiltersPage(IWebDriver driver) : base(driver) { }

    private readonly By _filtersMenuItem = By.XPath("//a[contains(@href,'/filters')]");
    private readonly By _addFilterButton   = By.XPath("//span[text()='Add Filter']");
    private readonly By _confirmDeleteFilterButton = By.XPath("//button[text()='Delete']");
    
    private By FilterByName(string name) => By.XPath($"//span[text()='{name}']");
    private By DeleteButtonByName(string name) => By.XPath($"//span[text()='{name}']/following::div[contains(@class, 'deleteFilterButton')][1]");
    private By ToggleByName(string name) => By.XPath($"//span[text()='{name}']/following::span[contains(@class,'inputSwitcher')][1]");
    private By StateByName(string name, string state) => 
        By.XPath($"//span[text()='{name}']/following::span[text()='{state.ToUpper()}'][1]");


    public void OpenFiltersPage()
    {
        Click(_filtersMenuItem);
    } 
    
    public LaunchesPage ClickAddFilter()
    {
        Click(_addFilterButton);
        return new LaunchesPage(_driver);
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
            return _wait.Until(driver =>
            {
                var elements = driver.FindElements(FilterByName(filterName));
                var isVisible = elements.Any(e => e.Displayed);
                return isVisible == shouldExist;
            });
        }
        catch (WebDriverTimeoutException)
        {
            Log.Warning($"Filter '{filterName}' did not reach state {shouldExist}");
            return false;
        }
    }

    public void ToggleDisplayOnLaunches(string filterName)
    {
        Click(ToggleByName(filterName));
    }

    public void WaitForState(string filterName, string state)
    {
        FindVisible(StateByName(filterName, state));
    }
}