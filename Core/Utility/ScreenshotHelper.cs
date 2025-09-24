using System;
using System.IO;
using OpenQA.Selenium;

namespace com.epam.rp.Core.Utility;


public static class ScreenshotHelper
{
    public static string? TakeScreenshot(IWebDriver driver, Serilog.ILogger logger, string? scenarioName = null)
    {
        try
        {
            string screenshotsDir = Path.Combine(AppContext.BaseDirectory, "screenshots" , Guid.NewGuid().ToString());
            Directory.CreateDirectory(screenshotsDir);

            string fileName = $"{scenarioName ?? "screenshot"}_{Guid.NewGuid():N}.png";
            string fullPath = Path.Combine(screenshotsDir, fileName);

            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(fullPath);

            logger.Information($"Screenshot saved: {fullPath}");
            return fullPath;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to take screenshot");
            return null;
        }
    }
}
