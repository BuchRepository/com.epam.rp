using Business.Enums;
using Business.Pages;
using com.epam.rp.Core;
using Serilog;
using TechTalk.SpecFlow;

namespace com.epam.rp.Steps;

[Binding]
public class FilterSteps
{
    private readonly ScenarioContext _context;
    private LoginPage _loginPage;
    private FiltersPage _filtersPage;
    private LaunchesPage _launchesPage;

    public FilterSteps(ScenarioContext context)
    {
        _context = context;
        
        _loginPage = _context.Get<LoginPage>("loginPage");
        _filtersPage = _context.Get<FiltersPage>("filtersPage");
        _launchesPage = _context.Get<LaunchesPage>("launchesPage");
    }

    [Given(@"I am logged in as a valid user")]
    public void GivenIAmLoggedInAsValidUser()
    {
        Log.Information($"Login to ReportPortal cabinet with user {Hooks.Login}");
        _loginPage.Login(Hooks.Login, Hooks.Password);
    }

    [Given(@"I am on the Filters page")]
    public void GivenIAmOnFiltersPage()
    {
        Log.Information("Navigate to 'Filters' page");
        _filtersPage.OpenFiltersPage();
    }

    [When(@"I create a filter with name ""(.*)"" and parameter ""(.*)"" and quantity ""(.*)""")]
    public void WhenICreateFilter(string filterName, string parameter, string quantity)
    {
        Log.Information("Add new filter");
        string uniqueName = $"{filterName}_{Guid.NewGuid():N}";
        _context["filterName"] = uniqueName;

        _launchesPage = _filtersPage.ClickAddFilter();
        _launchesPage.AddFilter(uniqueName, parameter, quantity);
    }

    [Then(@"the filter should be visible on the Filters page")]
    public void ThenFilterShouldBeVisible()
    {
        Assert.IsTrue(_filtersPage.WaitForFilterVisibility(_context["filterName"].ToString(), true));
    }

    [When(@"I delete the filter")]
    [Then(@"I delete the filter")]
    public void ThenIDeleteFilter()
    {
        _filtersPage.DeleteFilter(_context["filterName"].ToString());
    }

    [Then(@"the filter should not be visible on the Filters page")]
    public void ThenFilterShouldNotBeVisible()
    {
        Assert.IsTrue(_filtersPage.WaitForFilterVisibility(_context["filterName"].ToString(), false));
    }

    [When(@"I toggle display")]
    public void WhenIToggleDisplay()
    {
        _filtersPage.ToggleDisplayOnLaunches(_context["filterName"].ToString());
    }
    
    [When(@"I wait ""(.*)"" state")]
    public void WhenIWaitState(string state)
    {
        var stateValue = state == "ON" ? FiltersState.ON.ToString() : FiltersState.OFF.ToString();   
        _filtersPage.WaitForState(_context["filterName"].ToString(), stateValue);
    }
    
    [Then(@"The filter should be visible on the Launches page")]
    public void ThenTheFilterShouldBeVisibleOnLaunchesPage()
    {
        _launchesPage.RefreshPage();
        Assert.IsTrue(_launchesPage.IsFilterVisible(_context["filterName"].ToString(), true), 
            $"Created filter should be presented on Launches page.");
    }

    [Then(@"The filter should not be visible on the Launches page")]
    public void ThenTheFilterShouldNotBeVisibleOnLaunchesPage()
    {
        _launchesPage.RefreshPage();
        Assert.IsTrue(_launchesPage.IsFilterVisible(_context["filterName"].ToString(), false), 
            $"Created filter should not be presented on Launches page.");
    }
}