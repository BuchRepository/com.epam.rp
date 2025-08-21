using Microsoft.Extensions.Configuration;
using ReportPortal.Business.Models;
using ReportPortal.Business.Pages;
using ReportPortal.Core;

namespace ReportPortal.Tests;

[TestFixture]
public class FilterTests :TestBase
{
    private readonly string LOGIN;
    private readonly string PASSWORD;

    public FilterTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        LOGIN = configuration["Credentials:Login"];
        PASSWORD = configuration["Credentials:Password"];
    }

    [Test]
    public void Filter_ShouldBeDisplayedInList()
    {
        if (Driver == null)
        {
            throw new NullReferenceException("Driver is null! Ensure it is properly initialized.");
        }
            
        var loginPage = new LoginPage(Driver);
        loginPage.Login(LOGIN, PASSWORD);

        var filtersPage = new FiltersPage(Driver);
        filtersPage.OpenFiltersSection();

        var filter = new FilterModel("DEMO_FILTER", "sergii_buchkivskyi");

        Assert.That(filtersPage.IsFilterPresent(filter.FilterName), Is.True,
            "Filter should exists in the filters list.");
    }
}