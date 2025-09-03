using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using com.epam.rp_mstest.Businnes.Pages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace com.epam.rp_mstest.Core
{
    
    [TestClass]
    public class TestBase
    {
        protected IWebDriver? Driver;
        protected LoginPageMsTest? LoginPage;
        protected FiltersPageMsTest? FiltersPage;
        protected static ExtentReports? extent;
        protected ExtentTest? test;
        
        [ClassInitialize]
        public static void ClassSetup(TestContext context)
        {
            string reportPath = Path.Combine(AppContext.BaseDirectory, "ExtentReports_MSTest.html");
            var htmlReporter = new ExtentHtmlReporter(reportPath)
            {
                Config =
                {
                    DocumentTitle = "Test Report",
                    ReportName = "UI Test Report",
                    Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Standard
                }
            };

            extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
        }

        [TestInitialize]
        public void SetupTest()
        {
            test = extent?.CreateTest(TestContext.TestName);
            
            Driver = DriverFactoryMsTest.CreateDriver("chrome");
            if (Driver == null)
            {
                throw new InvalidOperationException("Driver initialization failed.");
            }
            
            Driver.Navigate().GoToUrl("https://rp.epam.com");
            
            LoginPage = new LoginPageMsTest(Driver);
            FiltersPage = new FiltersPageMsTest(Driver);
        }

        [TestCleanup]
        public void TeardownTest()
        {
            var outcome = TestContext.CurrentTestOutcome;
            switch (outcome)
            {
                case UnitTestOutcome.Failed:
                    test?.Fail("Test Failed");
                    break;
                case UnitTestOutcome.Passed:
                    test?.Pass("Test Passed");
                    break;
                default:
                    test?.Skip("Test Skipped");
                    break;
            }

            Driver?.Quit();
            Driver?.Dispose();
            Driver = null;

            extent?.Flush();
        }

        public TestContext TestContext { get; set; }
    }
}
