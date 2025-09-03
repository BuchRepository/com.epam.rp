using Microsoft.Extensions.Configuration;
using ReportPortal.Business.Models;
using ReportPortal.Business.Pages;
using ReportPortal.Core;
using Serilog;

namespace ReportPortal.Tests;

[TestFixture]
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

    [Test(Description = "Verify that a specific filter is displayed in the filters list.")]
    public void Filter_ShouldBeDisplayedInList()
    {
        if (Driver == null)
        {
            throw new NullReferenceException("Driver is null! Ensure it is properly initialized.");
        }
            
        var loginPage = new LoginPage(Driver);
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
        
        var filtersPage = new FiltersPage(Driver);
        filtersPage.OpenFiltersSection();

        var filter = new FilterModel("DEMO_FILTER", "sergii_buchkivskyi");

        NUnit.Framework.Assert.That(filtersPage.IsFilterPresent(filter.FilterName), Is.True,
            "Filter should exists in the filters list.");
    }
}