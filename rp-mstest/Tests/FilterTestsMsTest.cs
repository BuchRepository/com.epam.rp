using com.epam.rp_mstest.Businnes.Models;
using com.epam.rp_mstest.Businnes.Pages;
using com.epam.rp_mstest.Core;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace com.epam.rp_mstest.Tests;


[TestClass]
public class FilterTests : TestBase
{
    private readonly string LOGIN;
    private readonly string PASSWORD;

    public FilterTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        LOGIN = configuration["LOGIN"];
        PASSWORD = configuration["PASSWORD"];

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    [TestMethod]
    [Description("Verify that a specific filter is displayed in the filters list.")]
    public void Filter_ShouldBeDisplayedInList()
    {
        if (Driver == null)
            throw new NullReferenceException("Driver is null! Ensure it is properly initialized.");

        var loginPage = new LoginPageMsTest(Driver);
        try
        {
            Log.Information("Start test");
            loginPage.Login(LOGIN, PASSWORD);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "The test encountered an error.");
        }
        finally
        {
            Log.CloseAndFlush();
        }

        var filtersPage = new FiltersPageMsTest(Driver);
        filtersPage.OpenFiltersSection();

        var filter = new FilterModelMsTest("DEMO_FILTER", "sergii_buchkivskyi");
        Assert.IsTrue(filtersPage.IsFilterPresent(filter.FilterName),
            "Filter should exist in the filters list.");
    }
}

