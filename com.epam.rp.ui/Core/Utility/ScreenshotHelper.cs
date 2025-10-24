using OpenQA.Selenium;

namespace com.epam.rp.ui.Core.Utility;

public static class ScreenshotHelper
{
    public static string? TakeScreenshot(IWebDriver driver, Serilog.ILogger logger, string? scenarioName = null)
    {
        try
        {
            string screenshotsDir = Path.Combine(AppContext.BaseDirectory, "screenshots");

            Directory.CreateDirectory(screenshotsDir);

            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
            string fileName = $"{scenarioName ?? "screenshot"}_{timestamp}_{Guid.NewGuid():N}.png";
            string fullPath = Path.Combine(screenshotsDir, fileName);

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