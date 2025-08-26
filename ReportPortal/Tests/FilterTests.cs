using Microsoft.Extensions.Configuration;
using ReportPortal.Business.Models;
using ReportPortal.Business.Pages;
using ReportPortal.Core;
using Serilog;
using Serilog.Debugging;

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

        LOGIN = configuration["LOGIN"];
        PASSWORD = configuration["PASSWORD"];
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        SelfLog.Enable(Console.Out);
        Console.WriteLine($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");
        Console.WriteLine($"Expected path for logfile: {Path.Combine(AppContext.BaseDirectory, "logs/logfile.log")}");
    }

    [Test]
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
            Log.Debug("Debug information here");
            
            loginPage.Login(LOGIN, PASSWORD);
            
            try
            {
                string testLogFilePath = Path.Combine(AppContext.BaseDirectory, "logs/test-logfile.log");
                File.WriteAllText(testLogFilePath, "Test log file creation works.");
                Console.WriteLine($"Test logfile created: {testLogFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create test logfile: {ex.Message}");
            }
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

        Assert.That(filtersPage.IsFilterPresent(filter.FilterName), Is.True,
            "Filter should exists in the filters list.");
    }
}