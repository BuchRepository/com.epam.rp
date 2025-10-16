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
    public static string? ApiToken => Config["API_TOKEN"] ?? throw new InvalidOperationException("ApiToken not set");
    public static string? SlackWebhookUrl => Config["SLACK_WEBHOOK_URL"] ?? throw new InvalidOperationException("Slack Webhook Url not set");
}