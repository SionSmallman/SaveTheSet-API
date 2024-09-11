using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace sts_net.Tests.Selenium.PageObjects
{
    internal class PlaylistPage : Page
    {
        private readonly IWebDriver _driver;
        private By loginBodyBy = By.XPath("//*[@id=\"login-button\"]");
        private By spotifyUsernameBy = By.CssSelector("#login-username");
        private By spotifyPasswordBy = By.CssSelector("#login-password");
        private By loginButtonBy = By.CssSelector("#login-button");
        private By headerDisplayNameBy = By.CssSelector("#spotify-profile-displayname");
        private By savePlaylistButtonBy = By.CssSelector("#form-submit");

        public PlaylistPage(IWebDriver driver) : base(driver)
        {
            _driver = driver;
        }

        public void LoginOnPlaylistPage(string spotifyUsername, string spotifyPassword)
        {
            // Get button from body and press
            IWebElement bodySignInButton = _driver.FindElement(loginBodyBy);
            bodySignInButton.Click();

            //Swap to Spotify window and input test credentials
            WebDriverWait waitForNewPageLoad = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            waitForNewPageLoad.Until(d => d.WindowHandles.Count > 1);
            IList<string> windowHandles = new List<string>(_driver.WindowHandles);

            _driver.SwitchTo().Window(windowHandles[1]);
            IWebElement spotifyUsernameInput = _driver.FindElement(spotifyUsernameBy);
            spotifyUsernameInput.SendKeys(spotifyUsername);
            IWebElement spotifyPasswordInput = _driver.FindElement(spotifyPasswordBy);
            spotifyPasswordInput.SendKeys(spotifyPassword);
            IWebElement spotifyLoginButton = _driver.FindElement(loginButtonBy);
            spotifyLoginButton.Click();

            // Back to main window, wait for profile to load in header
            _driver.SwitchTo().Window(windowHandles[0]);
            WebDriverWait waitForProfileLoad = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            bool profileNameIsDisplayed = waitForProfileLoad.Until(d => d.FindElement(headerDisplayNameBy).Displayed);
        }

        public void SavePlaylist()
        {
            IWebElement formSubmitButton = _driver.FindElement(savePlaylistButtonBy);
            formSubmitButton.Click();
        }
    }
}
