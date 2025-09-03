using com.epam.rp_mstest.Core;
using OpenQA.Selenium;

namespace com.epam.rp_mstest.Businnes.Pages;

public class LoginPageMsTest : BasePageMsTest
{
    public LoginPageMsTest(IWebDriver driver) : base(driver) { }	

    private readonly By LoginInput = By.XPath("//*[@name='login']");
    private readonly By PasswordInput = By.XPath("//*[@name='password']");
    private readonly By SubmitButton = By.XPath("//*[@type='submit']");

    public void Login(string login, string password)
    {
        Type(LoginInput, login);
        Type(PasswordInput, password);
        Click(SubmitButton);
    }
}