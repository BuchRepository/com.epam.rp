using com.epam.rp.core;
using com.epam.rp.core.Configuration;
using com.epam.rp.core.Models;
using com.epam.rp.ui.Business.Enums;
using com.epam.rp.ui.Business.Pages;
using com.epam.rp.ui.Core;
using TestDataLoader = com.epam.rp.core.TestDataLoader;

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

    /*
    public static IEnumerable<object[]> AddFilterData()
    {
        var testData = TestDataLoader.LoadTestData<FilterTestData>("TestData.json", "AddFilter");
        foreach (var item in testData)
        {
            yield return new object[] { item }; 
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(AddFilterData), DynamicDataSourceType.Method)]
    public void UserCanAddFilter(FilterTestData data)
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
        Assert.IsTrue(FiltersPage!.IsFilterVisible(filterName), $"Filter '{filterName}' should be visible after adding.");

        FiltersPage!.DeleteFilter(filterName);
        Assert.IsFalse(FiltersPage!.IsFilterVisible(filterName), $"Filter '{filterName}' should be deleted.");
    }
    
    public static IEnumerable<object[]> RemoveFilterData()
    {
        var testData = TestDataLoader.LoadTestData<FilterTestData>("TestData.json", "RemoveFilter");
        foreach (var item in testData)
        {
            yield return new object[] { item };
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(RemoveFilterData), DynamicDataSourceType.Method)]
    public void UserCanRemoveFilter(FilterTestData data)
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
        Assert.IsFalse(FiltersPage!.IsFilterVisible(filterName), $"Filter '{filterName}' should be deleted.");
    }
    */

    public static IEnumerable<object[]> ToggleDisplayData()
    {
        var testData = TestDataLoader.LoadTestData<FilterTestData>("TestData.json", "ToggleDisplay");
        foreach (var item in testData)
        {
            yield return new object[] { item };
        }
    }
    
    [TestMethod]
    [DynamicData(nameof(ToggleDisplayData), DynamicDataSourceType.Method)]
    public void UserCanToggleFilterDisplay(FilterTestData data)
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
        
        FiltersPage!.DisableDisplayOnLaunches(filterName);
        launchesPage = FiltersPage!.WaitForState(filterName, FiltersState.Off);
        
        Assert.IsFalse(launchesPage.IsFilterVisible(filterName), $"Toggled filter '{filterName}' should not be presented on Launches page."
        );

        FiltersPage.DeleteFilter(filterName);
        Assert.IsFalse(FiltersPage!.IsFilterVisible(filterName), $"Filter '{filterName}' should be deleted.");
    }

    /*
    public static IEnumerable<object[]> EditFilterData()
    {
        var testData = TestDataLoader.LoadTestData<FilterTestData>("TestData.json", "EditFilter");
        foreach (var item in testData)
            yield return new object[] { item };
    }

    [TestMethod]
    [DynamicData(nameof(EditFilterData), DynamicDataSourceType.Method)]
    public void UserCanEditFilter(FilterTestData data)
    {
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        string newFilterName = $"Updated_{filterName}";

        LoggerService.Info("Start test");
        LoggerService.Info("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        FiltersPage!.OpenFiltersPage();

        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);

        Assert.IsTrue(FiltersPage!.IsFilterVisible(filterName), $"Filter '{filterName}' should be presented.");

        FiltersPage!.EditFilter(filterName, newFilterName);

        Assert.IsTrue(FiltersPage!.IsFilterVisible(newFilterName), $"Edited filter '{newFilterName}' should be presented.");
        Assert.IsFalse(FiltersPage!.IsFilterVisible(filterName), $"Old filter '{filterName}' should be deleted.");

        FiltersPage.DeleteFilter(newFilterName);
        Assert.IsFalse(FiltersPage!.IsFilterVisible(newFilterName), $"Filter '{newFilterName}' should be deleted.");
    }

    public static IEnumerable<object[]> CopyFilterData()
    {
        var testData = TestDataLoader.LoadTestData<FilterTestData>("TestData.json", "CopyFilter");
        foreach (var item in testData)
            yield return new object[] { item };
    }

    [TestMethod]
    [DynamicData(nameof(CopyFilterData), DynamicDataSourceType.Method)]
    public void UserCanCopyFilter(FilterTestData data)
    {
        string filterName = $"{data.filterName}_{Guid.NewGuid():N}";
        string parameter = data.parameter;
        string quantity = data.quantity;
        string copiedFilterName = $"Copy {filterName}";

        LoggerService.Info("Start test");
        LoggerService.Info("Login to ReportPortal cabinet");
        LoginPage!.Login(_login, _password);
        FiltersPage!.OpenFiltersPage();

        var launchesPage = FiltersPage!.ClickAddFilter();
        launchesPage.AddFilter(filterName, parameter, quantity);

        Assert.IsTrue(FiltersPage!.IsFilterVisible(filterName), $"Filter '{filterName}' should be present.");

        launchesPage.ClickFilterByName(filterName);
        launchesPage.CopyFilter();

        Assert.IsTrue(FiltersPage!.IsFilterVisible(copiedFilterName), $"Copied filter '{copiedFilterName}' should be present.");

        FiltersPage.DeleteFilter(filterName);
        Assert.IsFalse(FiltersPage!.IsFilterVisible(filterName), $"Filter '{filterName}' should be deleted.");
        FiltersPage.DeleteFilter(copiedFilterName);
        Assert.IsFalse(FiltersPage!.IsFilterVisible(copiedFilterName), $"Filter '{copiedFilterName}' should be deleted.");
    }
    */
}