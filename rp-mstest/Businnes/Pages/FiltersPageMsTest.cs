using com.epam.rp_mstest.Core;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace com.epam.rp_mstest.Businnes.Pages;

public class FiltersPageMsTest : BasePageMsTest
{
    public FiltersPageMsTest(IWebDriver driver) : base(driver) { }

    private readonly By CreateFilterButton = By.XPath("//span[contains(text(), 'Add filter')]");
    private readonly By LaunchNameInput    = By.XPath("//input[@placeholder='Enter name']");
    private readonly By SaveButton         = By.XPath("//span[contains(text(), 'Save')]");
    private readonly By filtersMenuItem = By.XPath("//a[contains(@href,'/filters')]");
    
    public void OpenFiltersSection()
    {
        var filtersLink = _wait.Until(ExpectedConditions.ElementToBeClickable(filtersMenuItem));
        filtersLink.Click();
    }

    public bool IsFilterPresent(string filterName)
    {
        var filterElement = _wait.Until(ExpectedConditions.ElementExists(By.XPath($"//span[text()='{filterName}']")));
        return filterElement.Displayed;
    }
}