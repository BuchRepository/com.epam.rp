using Business.Enums;
using Business.Pages;
using Microsoft.Extensions.Configuration;
using com.epam.rp_nunit.Core;
using Core;
using OpenQA.Selenium.Chrome;
using Serilog;

namespace Tests;

[TestFixture]
public class FilterTests : TestBase
{
    private readonly string _login;
    private readonly string _password;
    
    public FilterTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        _login = configuration["LOGIN"];
        _password = configuration["PASSWORD"];
    }
    
    [Test]
    public void UserCanAddFilter()
    {
        string filterName = $"Automation bugs_{Guid.NewGuid():N}";
        string parameter = "Automation Bug";
        string quantity = "1";
        bool expectedResult = true;
        
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
    
    [Test]
    public void UserCanRemoveFilter()
    {
        string filterName = $"Product bugs_{Guid.NewGuid():N}";
        string parameter = "Product Bug";
        string quantity = "1";
        bool expectedResult = false;
       
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

    [Test]
    public void UserCanToggleFilterDisplay()
    {
        string filterName = $"System issues_{Guid.NewGuid():N}";
        string parameter = "System Issue";
        string quantity = "1";
        
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