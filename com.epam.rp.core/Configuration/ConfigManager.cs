using Microsoft.Extensions.Configuration;

namespace com.epam.rp.core.Configuration;

public static class ConfigManager
{
    private static readonly IConfiguration Config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();
    
    public static string? Login => Config["LOGIN"];
    public static string? Password => Config["PASSWORD"];
    public static string ApiToken => Config["API_TOKEN"] ?? throw new InvalidOperationException("ApiToken not set");
    public static string SlackWebhookUrl => Config["SLACK_WEBHOOK_URL"] ?? throw new InvalidOperationException("Slack Webhook Url not set");
    public static string SauceUsername => Config["SAUCE_USERNAME"] 
                                          ?? throw new InvalidOperationException("SAUCE_USERNAME not set");
    public static string SauceAccessKey => Config["SAUCE_ACCESS_KEY"] 
                                           ?? throw new InvalidOperationException("SAUCE_ACCESS_KEY not set");
    public static bool UseSauceLabs => bool.TryParse(Config["USE_SAUCELABS"], out var value) && value;
    public static bool RunRemote => bool.TryParse(Config["RUN_REMOTE"], out var value) && value;
}