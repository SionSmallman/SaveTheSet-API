using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sts_net.Tests.Selenium.PageObjects
{
    internal abstract class Page
    {

        private readonly IWebDriver _driver;
        private By loginButtonBy = By.CssSelector("#login-button");
        private By spotifyUsernameBy = By.CssSelector("#login-username");
        private By spotifyPasswordBy = By.CssSelector("#login-password");
        private By headerDisplayNameBy = By.CssSelector("#spotify-profile-displayname");
        private By userDropdownBy = By.CssSelector("#user-dropdown");
        private By userSettingsBy = By.CssSelector("#user-settings");
        private By logoutButtonBy = By.CssSelector("#logout");

       public Page(IWebDriver driver)
        {
            _driver = driver;
        }

        public virtual void LoginFromHeader(string username, string password)
        {
            IWebElement headerSignInButton = _driver.FindElement(loginButtonBy);
            headerSignInButton.Click();

            //Swap to Spotify window and input test credentials
            WebDriverWait waitForNewPageLoad = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            waitForNewPageLoad.Until(d => d.WindowHandles.Count > 1);
            IList<string> windowHandles = new List<string>(_driver.WindowHandles);
            _driver.SwitchTo().Window(windowHandles[1]);

            IWebElement spotifyUsernameInput = _driver.FindElement(spotifyUsernameBy);
            spotifyUsernameInput.SendKeys("savetheset.dummy@gmail.com");
            IWebElement spotifyPasswordInput = _driver.FindElement(spotifyPasswordBy);
            spotifyPasswordInput.SendKeys("SaveTheSet1");
            IWebElement spotifyLoginButton = _driver.FindElement(loginButtonBy);
            spotifyLoginButton.Click();

            // Back to main window, check credentials in local storage and header is updated
            _driver.SwitchTo().Window(windowHandles[0]);
            WebDriverWait waitForProfileLoad = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            bool profileNameIsDisplayed = waitForProfileLoad.Until(d => d.FindElement(headerDisplayNameBy).Displayed);
        }

        public virtual string GetDisplayName()
        {
            return _driver.FindElement(headerDisplayNameBy).Text;
        }

        public virtual object GetSpotifyProfileDetails()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            var profileData = js.ExecuteScript("return window.localStorage.getItem('spProfile');");
            return profileData;
        }

        public virtual void NavigateToSettings()
        {
            IWebElement userDropdownMenu = _driver.FindElement(userDropdownBy);
            userDropdownMenu.Click();

            IWebElement userSettings = _driver.FindElement(userSettingsBy);
            userSettings.Click();
        }

        public virtual void Logout()
        {
            IWebElement userDropdownMenu = _driver.FindElement(userDropdownBy);
            userDropdownMenu.Click();

            IWebElement logout = _driver.FindElement(logoutButtonBy);
            logout.Click();
        }
    }
}
