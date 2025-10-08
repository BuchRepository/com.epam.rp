using com.epam.rp.ui.Core.Elements;
using OpenQA.Selenium;
namespace com.epam.rp.ui.Business.Pages;

public class LoginPage : BasePage
{
    private readonly Input _username;
    private readonly Input _password;
    private readonly Button _loginButton;
    
    public LoginPage(IWebDriver driver) : base(driver)
    {
        _username = new Input(driver, By.XPath("//input[@name='login']"), "Username field");
        _password = new Input(driver, By.XPath("//input[@name='password']"), "Password field");
        _loginButton = new Button(driver, By.XPath("//button[text()='Login']"), "Login button");
    }
    
    public void Login(string user, string pass)
    {
        _username.Type(user);
        _password.Type(pass);
        _loginButton.ClickButton();
    }
}