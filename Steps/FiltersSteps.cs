using Business.Pages;
using com.epam.rp.Core;
using TechTalk.SpecFlow;

namespace Tests;

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
        _loginPage.Login(Hooks.Login, Hooks.Password);
    }

    [Given(@"I am on the Filters page")]
    public void GivenIAmOnFiltersPage()
    {
        _filtersPage.OpenFiltersPage();
    }

    [When(@"I create a filter with name ""(.*)"" and parameter ""(.*)"" and quantity ""(.*)""")]
    public void WhenICreateFilter(string filterName, string parameter, string quantity)
    {
        string uniqueName = $"{filterName}_{Guid.NewGuid():N}";
        _context["filterName"] = uniqueName;

        _launchesPage = _filtersPage.ClickAddFilter();
        _launchesPage.AddFilter(uniqueName, parameter, quantity);
    }

    [Then(@"the filter should be visible on the Filters page")]
    public void ThenFilterShouldBeVisible()
    {
        var name = _context["filterName"].ToString();
        Assert.IsTrue(_filtersPage.WaitForFilterVisibility(name, true));
    }

    [When(@"I delete the filter")]
    public void WhenIDeleteFilter()
    {
        var name = _context["filterName"].ToString();
        _filtersPage.DeleteFilter(name);
    }

    [Then(@"the filter should not be visible on the Filters page")]
    public void ThenFilterShouldNotBeVisible()
    {
        var name = _context["filterName"].ToString();
        Assert.IsTrue(_filtersPage.WaitForFilterVisibility(name, false));
    }

    [When(@"I toggle display of ""(.*)""")]
    public void WhenIToggleDisplay(string filterName)
    {
        var name = _context["filterName"].ToString();
        _filtersPage.ToggleDisplayOnLaunches(name);
    }
    
    [Then(@"the filter ""(.*)"" should be visible on Launches page")]
    public void ThenFilterShouldBeVisibleOnLaunchesPage(string filterName)
    {
        Assert.IsTrue(_launchesPage.IsFilterVisible(filterName, true));
    }

    [Then(@"the filter ""(.*)"" should not be visible on Launches page")]
    public void ThenFilterShouldNotBeVisibleOnLaunchesPage(string filterName)
    {
        Assert.IsTrue(!_launchesPage.IsFilterVisible(filterName, false));
    }

}