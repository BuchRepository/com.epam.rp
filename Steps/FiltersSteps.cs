using com.epam.rp.Business.Enums;
using com.epam.rp.Business.Pages;
using com.epam.rp.Core;
using Serilog;
using TechTalk.SpecFlow;

namespace com.epam.rp.Steps;

[Binding]
public class FilterSteps
{
    private readonly ScenarioContext _context;

    public FilterSteps(ScenarioContext context)
    {
        _context = context;
    }
    
    private string GetFilterName()
    {
        if (!_context.TryGetValue("filterName", out string? filterName) || string.IsNullOrEmpty(filterName))
        {
            throw new InvalidOperationException("Filter name is not set in ScenarioContext.");
        }
        return filterName;
    }

    [Given(@"I am logged in as a valid user")]
    public void GivenIAmLoggedInAsValidUser()
    {
        var loginPage = _context.Get<LoginPage>("loginPage");
        Log.Information($"Login to ReportPortal cabinet with user {Hooks.Login}");
        if (Hooks.Login == null || Hooks.Password == null)
        {
            throw new InvalidOperationException("Login or Password not configured. Check appsettings.json or BeforeTestRun.");
        }
        loginPage.Login(Hooks.Login, Hooks.Password);
    }

    [Given(@"I am on the Filters page")]
    public void GivenIAmOnFiltersPage()
    {
        var filtersPage = _context.Get<FiltersPage>("filtersPage");
        Log.Information("Navigate to 'Filters' page");
        filtersPage.OpenFiltersPage();
    }

    [When(@"I create a filter with name ""(.*)"" and parameter ""(.*)"" and quantity ""(.*)""")]
    public void WhenICreateFilter(string filterName, string parameter, string quantity)
    {
        var filtersPage = _context.Get<FiltersPage>("filtersPage");
        var launchesPage = _context.Get<LaunchesPage>("launchesPage");
        
        Log.Information("Add new filter");
        string uniqueName = $"{filterName}_{Guid.NewGuid():N}";
        _context["filterName"] = uniqueName;

        launchesPage = filtersPage.ClickAddFilter();
        launchesPage.AddFilter(uniqueName, parameter, quantity);
        
        _context["launchesPage"] = launchesPage;
    }

    [Then(@"the filter should be visible on the Filters page")]
    public void ThenFilterShouldBeVisible()
    {
        var filtersPage = _context.Get<FiltersPage>("filtersPage");
        var filterName = GetFilterName();
        
        Assert.IsTrue(filtersPage.WaitForFilterVisibility(filterName, true));
    }

    [When(@"I delete the filter")]
    [Then(@"I delete the filter")]
    public void ThenIDeleteFilter()
    {
        var filtersPage = _context.Get<FiltersPage>("filtersPage");
        var filterName = GetFilterName();
        
        filtersPage.DeleteFilter(filterName);
    }

    [Then(@"the filter should not be visible on the Filters page")]
    public void ThenFilterShouldNotBeVisible()
    {
        var filtersPage = _context.Get<FiltersPage>("filtersPage");
        var filterName = GetFilterName();
        
        Assert.IsTrue(filtersPage.WaitForFilterVisibility(filterName, false));
    }

    [When(@"I toggle display")]
    public void WhenIToggleDisplay()
    {
        var filtersPage = _context.Get<FiltersPage>("filtersPage");
        var filterName = GetFilterName();
        
        filtersPage.ToggleDisplayOnLaunches(filterName);
    }
    
    [When(@"I wait ""(.*)"" state")]
    public void WhenIWaitState(string state)
    {
        var filtersPage = _context.Get<FiltersPage>("filtersPage");
        var filterName = GetFilterName();
        
        var stateValue = state == "ON" ? FiltersState.On.ToString().ToUpper() : FiltersState.Off.ToString().ToUpper();   
        filtersPage.WaitForState(filterName, stateValue);
    }
    
    [Then(@"The filter should be visible on the Launches page")]
    public void ThenTheFilterShouldBeVisibleOnLaunchesPage()
    {
        var launchesPage = _context.Get<LaunchesPage>("launchesPage");
        var filterName = GetFilterName();

        launchesPage.RefreshPage();
        Assert.IsTrue(launchesPage.IsFilterVisible(filterName, true), 
            $"Created filter should be presented on Launches page.");
    }

    [Then(@"The filter should not be visible on the Launches page")]
    public void ThenTheFilterShouldNotBeVisibleOnLaunchesPage()
    {
        var launchesPage = _context.Get<LaunchesPage>("launchesPage");
        var filterName = GetFilterName();
        
        launchesPage.RefreshPage();
        Assert.IsTrue(launchesPage.IsFilterVisible(filterName, false), 
            $"Created filter should not be presented on Launches page.");
    }
}