using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sts_net.Tests.Selenium.PageObjects
{
    internal class SuccessPage : Page
    {

        protected IWebDriver _driver;
        private By homeButton;
        private By openSpotifyButtonBy = By.CssSelector("#success-open-spotify");
        private By createdPlaylistLinkInputBy = By.CssSelector("#success-playlist-link");
        private By copyCreatedPlaylistLinkBy = By.CssSelector("#success-copy-playlist-link");


        public SuccessPage(IWebDriver driver) : base(driver)
        {
            _driver = driver;
        }

        public string GetCreatedPlaylistLink()
        {
            return _driver.FindElement(createdPlaylistLinkInputBy).GetAttribute("value");
        }
        public IWebElement GetCreatedPlaylistElement()
        {
            return _driver.FindElement(createdPlaylistLinkInputBy);
        }
    }
}
