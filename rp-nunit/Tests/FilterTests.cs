using Microsoft.Extensions.Configuration;
using Business.Models;
using Core;
using Serilog;

namespace Tests;

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

    [Test]
    public void DisplayOnLaunchesShouldToggleAndReturn()
    {
        Log.Information("Start test");
        LoginPage!.Login(LOGIN, PASSWORD);
        FiltersPage!.OpenFiltersSection();

        // 1. Verify that Demo filter is presented
        var filter = new FilterModel("DEMO_FILTER");
        bool isFilterPresented = FiltersPage.IsFilterPresent(filter.FilterName);
        Assert.That(isFilterPresented, Is.True, "Demo filter should exists in the filters list.");

        Log.Information($"Demo filter is presented in the filters list: {isFilterPresented}");

        // 2. Get the current state
        bool initialState = FiltersPage.IsDisplayOnLaunchesOn();
        Log.Information($"Initial DisplayOnLaunches state: {(initialState ? "ON" : "OFF")}");

        // 3. Toggle to the opposite state
        FiltersPage!.ToggleDisplayOnLaunches();
        FiltersPage!.WaitForState(!initialState);

        // 4. Verify that the state changed to the opposite
        bool toggledState = FiltersPage!.IsDisplayOnLaunchesOn();
        Assert.That(toggledState, Is.EqualTo(!initialState),
            "State should be toggled to the opposite.");

        Log.Information($"State after toggle: {(toggledState ? "ON" : "OFF")}");

        // 5. Toggle back to the initial state
        FiltersPage!.ToggleDisplayOnLaunches();
        FiltersPage!.WaitForState(initialState);

        // 6. Verify that the state was reverted back
        bool finalState = FiltersPage!.IsDisplayOnLaunchesOn();
        Assert.That(finalState, Is.EqualTo(initialState),
            "State should be reverted back to initial.");

        Log.Information($"Final state: {(finalState ? "ON" : "OFF")}");
    }
}