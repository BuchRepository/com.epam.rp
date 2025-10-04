using com.epam.rp.core;
using com.epam.rp.core.Configuration;
using com.epam.rp.ui.Business.Enums;
using com.epam.rp.ui.Core;
using TestDataLoader = com.epam.rp.ui.Core.TestDataLoader;

[assembly: Parallelize(Workers = 5, Scope = ExecutionScope.MethodLevel)]

namespace com.epam.rp.ui.Tests;

[TestClass]
public class FiltersTests : TestBase
{
    private readonly string _login;
    private readonly string _password;
    
    public FiltersTests()
    {
        _login = ConfigManager.Login ?? throw new InvalidOperationException("LOGIN not found in config");
        _password = ConfigManager.Password ?? throw new InvalidOperationException("PASSWORD not found in config");
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
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        
        LoggerService.Info("Start test");
        LoggerService.Info("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        LoggerService.Info("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        LoggerService.Info("Click on 'Add' button");
        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);

        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, true), $"Filter '{filterName}' should be present after adding.");

        FiltersPage!.DeleteFilter(filterName);
        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, false), $"Filter '{filterName}' should be deleted.");
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
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
       
        LoggerService.Info("Start test");
        LoggerService.Info("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        LoggerService.Info("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        LoggerService.Info("Click on 'Add' button");
        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);
       
        FiltersPage.DeleteFilter(filterName);
        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, false),
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
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        
        LoggerService.Info("Start test");
        LoggerService.Info("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);

        LoggerService.Info("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        LoggerService.Info("Click on 'Add' button");
        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);
        
        Assert.IsTrue(
            launchesPage.IsFilterVisible(filterName, shouldBeVisible: true),
            $"Created filter '{filterName}' should be presented on Launches page."
        );

        FiltersPage!.DisableDisplayOnLaunches(filterName);
        FiltersPage!.WaitForState(filterName, FiltersState.Off);
        
        launchesPage.RefreshPage();

        Assert.IsTrue(
            launchesPage.IsFilterVisible(filterName, shouldBeVisible: false),
            $"Toggled filter '{filterName}' should disappear on Launches page."
        );
        
        FiltersPage.DeleteFilter(filterName);
        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, false),
            $"Filter '{filterName}' should be deleted.");
    }
    
    public static IEnumerable<object[]> EditFilterData()
    {
        var testData = TestDataLoader.LoadTestData<dynamic>("TestData.json", "EditFilter");
        foreach (var item in testData)
            yield return new object[] { item };
    }

    [TestMethod]
    [DynamicData(nameof(EditFilterData), DynamicDataSourceType.Method)]
    public void UserCanEditFilter(dynamic data)
    {
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        string newFilterName = $"Updated_{filterName}";
        
        LoggerService.Info("Start test");
        LoggerService.Info("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        LoggerService.Info("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        
        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);

        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, true), $"Filter '{filterName}' should be present.");

        FiltersPage!.EditFilter(filterName, newFilterName);
        
        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(newFilterName, true), $"Edited filter '{newFilterName}' should be present.");
        Assert.IsFalse(FiltersPage!.WaitForFilterVisibility(filterName, true), $"Old filter '{filterName}' should no longer exist.");

        FiltersPage.DeleteFilter(newFilterName);
        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(newFilterName, false), $"Filter '{newFilterName}' should be deleted.");
    }

    public static IEnumerable<object[]> CopyFilterData()
    {
        var testData = TestDataLoader.LoadTestData<dynamic>("TestData.json", "CopyFilter");
        foreach (var item in testData)
            yield return new object[] { item };
    }

    [TestMethod]
    [DynamicData(nameof(CopyFilterData), DynamicDataSourceType.Method)]
    public void UserCanCopyFilter(dynamic data)
    {
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        string copiedFilterName = $"Copy {filterName}";

        LoggerService.Info("Start test");
        LoggerService.Info("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        LoggerService.Info("Open 'Filters' page");
        FiltersPage!.OpenFiltersPage();
        
        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);

        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(filterName, true), $"Filter '{filterName}' should be present.");

        launchesPage.ClickFilterByName(filterName);
        launchesPage.CopyFilter(filterName);

        Assert.IsTrue(FiltersPage!.WaitForFilterVisibility(copiedFilterName, true), $"Copied filter '{copiedFilterName}' should be present.");

        FiltersPage.DeleteFilter(filterName);
        FiltersPage.DeleteFilter(copiedFilterName);
    }
}