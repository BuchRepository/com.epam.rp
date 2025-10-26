using System.Text;
using com.epam.rp.core.Configuration;

namespace com.epam.rp.core.Utility;

public class SlackNotifier
{
    private static readonly HttpClient Client = new HttpClient();
    private readonly string _slackWebhookUrl = ConfigManager.SlackWebhookUrl ?? throw new InvalidOperationException("SlackWebhookUrl not found in config");
    

    public async Task SendMessage(string message)
    {
        var payload = new { text = message };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await Client.PostAsync(_slackWebhookUrl, content);
    }
}
