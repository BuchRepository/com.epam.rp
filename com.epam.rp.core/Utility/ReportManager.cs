using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace com.epam.rp.core.Utility
{
    public static class ReportManager
    {
        private static ExtentReports? _uiExtent;
        private static ExtentReports? _apiExtent;

        public static ExtentReports GetExtent(bool isUi = true)
        {
            if (isUi)
            {
                if (_uiExtent == null)
                    _uiExtent = CreateExtent("ExtentReports_UI.html");
                return _uiExtent;
            }
            else
            {
                if (_apiExtent == null)
                    _apiExtent = CreateExtent("ExtentReports_API.html");
                return _apiExtent;
            }
        }

        private static ExtentReports CreateExtent(string fileName)
        {
            string reportPath = Path.Combine(AppContext.BaseDirectory, fileName);
            var htmlReporter = new ExtentHtmlReporter(reportPath);
            htmlReporter.Config.DocumentTitle = fileName;
            htmlReporter.Config.ReportName = fileName.Replace(".html", "");
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard;

            var extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
            return extent;
        }

        public static void FlushReports()
        {
            _uiExtent?.Flush();
            _apiExtent?.Flush();
        }
    }
}
