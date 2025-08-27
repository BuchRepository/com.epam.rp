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
        
        SelfLog.Enable(Console.Out);
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
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
            
            Console.WriteLine($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");
            Console.WriteLine($"Expected path for logfile: {Path.Combine(AppContext.BaseDirectory, "logs/logfile.log")}");
            
            var testFilePath = Path.Combine(AppContext.BaseDirectory, "logs/test-logfile.log");
            Directory.CreateDirectory(Path.GetDirectoryName(testFilePath));
            File.WriteAllText(testFilePath, "This is a test log message.");
            Console.WriteLine($"Test log file path: {testFilePath}");
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