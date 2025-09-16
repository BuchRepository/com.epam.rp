using Business.Enums;
using Business.Pages;
using Microsoft.Extensions.Configuration;
using com.epam.rp_nunit.Core;
using Core;
using OpenQA.Selenium.Chrome;
using Serilog;

namespace Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class FilterTests : TestBase
{
    private readonly string _login;
    private readonly string _password;
    
    public FilterTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

        _login = configuration["LOGIN"];
        _password = configuration["PASSWORD"];
    }

    public static IEnumerable<dynamic> AddFilterData()
    {
        return TestDataLoader.LoadTestData<dynamic>("TestData.json", "AddFilter");
    }
    
    [Test, TestCaseSource(nameof(AddFilterData))]
    public void UserCanAddFilter(dynamic data)
    {
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        bool expectedResult = data.expectedResult;
        
        Log.Information("Start test");
        Log.Information("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        Log.Information("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        Log.Information("Click on 'Add' button");
        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);

        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, expectedResult), $"Filter '{filterName}' should be present after adding.");

        FiltersPage!.DeleteFilter(filterName);
        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, false), $"Filter '{filterName}' should be deleted.");
    }
    
    public static IEnumerable<dynamic> RemoveFilterData()
    {
        return TestDataLoader.LoadTestData<dynamic>("TestData.json", "RemoveFilter");
    }
    
    [Test, TestCaseSource(nameof(RemoveFilterData))]
    public void UserCanRemoveFilter(dynamic data)
    {
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        bool expectedResult = data.expectedResult;
       
        Log.Information("Start test");
        Log.Information("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        Log.Information("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        Log.Information("Click on 'Add' button");
        var launchesPage = FiltersPage.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);
       
        FiltersPage.DeleteFilter(filterName);
        Assert.IsTrue(FiltersPage.WaitForFilterVisibility(filterName, expectedResult),
            $"Filter '{filterName}' should be deleted.");
    }

    public static IEnumerable<dynamic> ToggleDisplayData()
    {
        return TestDataLoader.LoadTestData<dynamic>("TestData.json", "ToggleDisplay");
    }

    [Test, TestCaseSource(nameof( ToggleDisplayData))]
    public void UserCanToggleFilterDisplay(dynamic data)
    {
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        
        Log.Information("Start test");
        Log.Information("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);

        Log.Information("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        Log.Information("Click on 'Add' button");
        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);
        
        Assert.That(
            launchesPage.IsFilterVisible(filterName, shouldBeVisible: true),
            Is.True,
            $"Created filter '{filterName}' should be presented on Launches page."
        );

        FiltersPage!.ToggleDisplayOnLaunches(filterName);
        FiltersPage!.WaitForState(filterName, FiltersState.Off);
        
        launchesPage.RefreshPage();

        Assert.That(
            launchesPage.IsFilterVisible(filterName, shouldBeVisible: false),
            Is.True,
            $"Created filter '{filterName}' should not be presented on Launches page."
        );
        
        FiltersPage.DeleteFilter(filterName);
        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, false),
            $"Filter '{filterName}' should be deleted.");
    }
}