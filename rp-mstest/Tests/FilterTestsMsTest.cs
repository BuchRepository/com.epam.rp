using com.epam.rp_mstest.Businnes.Enums;
using com.epam.rp_mstest.Businnes.Pages;
using com.epam.rp_mstest.Core;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using Serilog;
[assembly: Parallelize(Workers = 5, Scope = ExecutionScope.MethodLevel)]

namespace com.epam.rp_mstest.Tests;

[TestClass]
public class FilterTestsMsTest : TestBaseMsTest
{
    private readonly string _login;
    private readonly string _password;
    
    public FilterTestsMsTest()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

        _login = configuration["LOGIN"];
        _password = configuration["PASSWORD"];
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public static IEnumerable<object[]> AddFilterData()
    {
        var testData = TestDataLoader.LoadTestData<dynamic>("TestData.json", "AddFilter");
        foreach (var item in testData)
        {
            yield return new object[] { item }; 
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(AddFilterData), DynamicDataSourceType.Method)]
    public void UserCanAddFilter(dynamic data)
    {
        using var driver = new ChromeDriver();
        var loginPage = new LoginPageMsTest(driver);
        var filtersPage = new FiltersPageMsTest(driver);
        
        driver.Navigate().GoToUrl("https://rp.epam.com");
        
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        bool expectedResult = data.expectedResult;
        
        Log.Information("Start test");
        Log.Information("Login to ReportPortal cabinet");
        loginPage!.Login(_login, _password);
        Log.Information("Open 'Filters' page");
        filtersPage!.OpenFiltersPage();
        Log.Information("Click on 'Add' button");
        var launchesPage = filtersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);

        Assert.IsTrue(filtersPage!.WaitForFilterVisibility(filterName, expectedResult), $"Filter '{filterName}' should be present after adding.");

        filtersPage!.DeleteFilter(filterName);
        Assert.IsTrue(filtersPage!.WaitForFilterVisibility(filterName, false), $"Filter '{filterName}' should be deleted.");
    }
    
    public static IEnumerable<object[]> RemoveFilterData()
    {
        var testData = TestDataLoader.LoadTestData<dynamic>("TestData.json", "RemoveFilter");
        foreach (var item in testData)
        {
            yield return new object[] { item };
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(RemoveFilterData), DynamicDataSourceType.Method)]
    public void UserCanRemoveFilter(dynamic data)
    {
        using var driver = new ChromeDriver();
        var loginPage = new LoginPageMsTest(driver);
        var filtersPage = new FiltersPageMsTest(driver);
        
        driver.Navigate().GoToUrl("https://rp.epam.com");
        
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        bool expectedResult = data.expectedResult;
       
        Log.Information("Start test");
        Log.Information("Login to ReportPortal cabinet");
        loginPage!.Login(_login, _password);
        Log.Information("Open 'Filters' page");
        filtersPage!.OpenFiltersPage();
        Log.Information("Click on 'Add' button");
        var launchesPage = filtersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);
       
        filtersPage.DeleteFilter(filterName);
        Assert.IsTrue(filtersPage!.WaitForFilterVisibility(filterName, expectedResult),
            $"Filter '{filterName}' should be deleted.");
    }

    public static IEnumerable<object[]> ToggleDisplayData()
    {
        var testData = TestDataLoader.LoadTestData<dynamic>("TestData.json", "ToggleDisplay");
        foreach (var item in testData)
        {
            yield return new object[] { item };
        }
    }

    [TestMethod]
    [DynamicData(nameof(ToggleDisplayData), DynamicDataSourceType.Method)]
    public void UserCanToggleFilterDisplay(dynamic data)
    {
        using var driver = new ChromeDriver();
        var loginPage = new LoginPageMsTest(driver);
        var filtersPage = new FiltersPageMsTest(driver);
        
        driver.Navigate().GoToUrl("https://rp.epam.com");
        
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        
        Log.Information("Start test");
        Log.Information("Login to ReportPortal cabinet");
        loginPage!.Login(_login, _password);

        Log.Information("Open 'Filters' page");
        filtersPage!.OpenFiltersPage();
        Log.Information("Click on 'Add' button");
        var launchesPage = filtersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);
        
        Assert.IsTrue(
            launchesPage.IsFilterVisible(filterName, shouldBeVisible: true),
            $"Created filter '{filterName}' should be presented on Launches page."
        );

        filtersPage!.ToggleDisplayOnLaunches(filterName);
        filtersPage!.WaitForState(filterName, FiltersState.Off);
        
        launchesPage.RefreshPage();

        Assert.IsTrue(
            launchesPage.IsFilterVisible(filterName, shouldBeVisible: false),
            $"Toggled filter '{filterName}' should disappear on Launches page."
        );
        
        filtersPage.DeleteFilter(filterName);
        Assert.IsTrue(filtersPage!.WaitForFilterVisibility(filterName, false),
            $"Filter '{filterName}' should be deleted.");
    }
}