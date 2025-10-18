using OpenQA.Selenium;

namespace com.epam.rp.ui.Core.Utility;

public static class ScreenshotHelper
{
    public static string? TakeScreenshot(IWebDriver driver, Serilog.ILogger logger, string? scenarioName = null)
    {
        try
        {
            string screenshotsDir = Path.Combine(AppContext.BaseDirectory, "screenshots");

            if (!Directory.Exists(screenshotsDir))
            {
                Directory.CreateDirectory(screenshotsDir);
            }
            else
            {
                foreach (var dir in Directory.GetDirectories(screenshotsDir))
                    Directory.Delete(dir, true);
                foreach (var file in Directory.GetFiles(screenshotsDir))
                    File.Delete(file);
            }
            
            string runId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID") ?? "local";
            string browser = Environment.GetEnvironmentVariable("BROWSER") ?? "default-1";
            string runDir = Path.Combine(screenshotsDir, runId, browser);
            Directory.CreateDirectory(runDir);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
            string fileName = $"{scenarioName ?? "screenshot"}_{timestamp}_{Guid.NewGuid():N}.png";
            string fullPath = Path.Combine(runDir, fileName);

            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(fullPath);

            logger.Information($"Screenshot saved: {fullPath}, URL: {driver.Url}, Scenario: {scenarioName}");

            return fullPath;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to take screenshot");
            return null;
        }
    }
}