using com.epam.rp.Core;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace com.epam.rp.Business.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IWebDriver driver, ScenarioContext context) : base(driver, context) { }	

    private readonly By _loginInput = By.XPath("//*[@name='login']");
    private readonly By _passwordInput = By.XPath("//*[@name='password']");
    private readonly By _submitButton = By.XPath("//*[@type='submit']");

    public void Login(string login, string password)
    {
        Logger.Information("Current URL before login: {Url}", Driver.Url);
        Type(_loginInput, login);
        Type(_passwordInput, password);
        Click(_submitButton);
    }
}