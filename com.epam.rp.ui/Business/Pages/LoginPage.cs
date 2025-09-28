using com.epam.rp.ui.Core;
using OpenQA.Selenium;
namespace com.epam.rp.ui.Business.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IWebDriver driver) : base(driver) { }	

    private readonly By _loginInput = By.XPath("//*[@name='login']");
    private readonly By _passwordInput = By.XPath("//*[@name='password']");
    private readonly By _submitButton = By.XPath("//*[@type='submit']");

    public void Login(string login, string password)
    {
        Type(_loginInput, login);
        Type(_passwordInput, password);
        Click(_submitButton);
    }
}