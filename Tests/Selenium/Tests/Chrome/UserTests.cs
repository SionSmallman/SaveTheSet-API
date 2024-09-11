using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using sts_net.Tests.Selenium.PageObjects;
using Microsoft.IdentityModel.Tokens;

namespace sts_net.Tests.Selenium.Tests.Chrome
{
    internal class UserTests
    {
        private IWebDriver driver;
        private string homeUrl;
        private HomePage homePage;
        private PlaylistPage playlistPage;
        private SuccessPage successPage;

        [SetUp]
        public void Setup()
        {
            // LOCAL DEV
            //var options = new ChromeOptions
            //{
            //    AcceptInsecureCertificates = true,
            //};
            //driver = new ChromeDriver(options);
            //homeUrl = "http://localhost:5173/";

            // PROD
            driver = new ChromeDriver();
            homeUrl = "https://sts.sionsmallman.com/";

            driver.Url = homeUrl;
            driver.Manage().Window.Maximize();
            homePage = new HomePage(driver);
            playlistPage = new PlaylistPage(driver);
            successPage = new SuccessPage(driver);

            homePage.LoginFromHeader("savetheset.dummy@gmail.com", "SaveTheSet1");
            homePage.SearchForSetlist("https://www.setlist.fm/setlist/oliver-francis/2022/trees-dallas-tx-73bff249.html");

            //Wait for setlist to load on page
            WebDriverWait waitForSetlistLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            bool setlistIsDisplayed = waitForSetlistLoad.Until(d => d.FindElement(By.CssSelector("#songlist-container")).Displayed);
        }

        [Test]
        public void PlaylistForm_DefaultValuesAreSet()
        {
            IWebElement playlistFormNameInput = driver.FindElement(By.CssSelector("#form-playlist-name"));
            string defaultNameValue = playlistFormNameInput.GetAttribute("value");
            Assert.That(defaultNameValue, Is.EqualTo("Oliver Francis - Dallas, 19-11-2022"));

            IWebElement playlistFormDescInput = driver.FindElement(By.CssSelector("#form-playlist-description"));
            string defaultDescValue = playlistFormDescInput.GetAttribute("value");
            Assert.That(defaultDescValue, Is.EqualTo("Setlist from Oliver Francis, playing at Trees, Dallas on 19-11-2022"));
        }

        [Test]
        public void PlaylistFormShowsErrorIfNoName()
        {
            IWebElement playlistNameFormInput = driver.FindElement(By.CssSelector("#form-playlist-name"));
            playlistNameFormInput.SendKeys(Keys.Control + "a");
            playlistNameFormInput.SendKeys(Keys.Delete);

            // Check that error is displayed under name input when the field is empty
            bool nameInputErrorIsDisplayed = driver.FindElement(By.CssSelector("#playlist-name-error")).Displayed;
            Assert.That(nameInputErrorIsDisplayed, Is.True);

            // Error is shown under submit button if submission attempted with no name
            IWebElement formSubmitButton = driver.FindElement(By.CssSelector("#form-submit"));
            formSubmitButton.Click();
            IWebElement formSubmissionError = driver.FindElement(By.CssSelector("#form-error"));
            Assert.That(formSubmissionError.Displayed, Is.True);
        }

        [Test]
        public void PlaylistSongsCanBeSelectedAndDeselected()
        {
            var songs = driver.FindElements(By.CssSelector("#songlist-song"));
            IWebElement firstSong = songs[0];

            //Setlist used has 4 songs
            firstSong.Click();
            WebDriverWait waitForAnimation = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            bool songIsDeselected = waitForAnimation.Until(d=>firstSong.GetCssValue("opacity") == "0.4");
            Assert.That(firstSong.GetCssValue("opacity"), Is.EqualTo("0.4"));

            firstSong.Click();
            bool songIsSelected = waitForAnimation.Until(d => firstSong.GetCssValue("opacity") == "1");
            Assert.That(firstSong.GetCssValue("opacity"), Is.EqualTo("1"));
        }

        [Test]
        public void PlaylistsCanBeSaved()
        {
            playlistPage.SavePlaylist();
            WebDriverWait waitForPlaylistSave = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            string successLogoSelector = "#root > div > div > div > main > div > div > div > svg";
            bool successPageIsDisplayed = waitForPlaylistSave.Until(d => d.FindElement(By.CssSelector(successLogoSelector)).Displayed);
            string spotifyPlaylistLink = successPage.GetCreatedPlaylistLink();
            string spotifyPlaylistId = spotifyPlaylistLink.Split("/").Last();

            Assert.That(successPageIsDisplayed, Is.True);
            Assert.That(spotifyPlaylistLink.IsNullOrEmpty(), Is.False);
            Assert.That(spotifyPlaylistId.IsNullOrEmpty(), Is.False);
        }

        [Test]
        public void OpenSpotifyButtonWorks()
        {
            playlistPage.SavePlaylist();
            WebDriverWait waitForPlaylistSave = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            IWebElement openSpotifyButton = waitForPlaylistSave.Until(d => d.FindElement(By.CssSelector("#success-open-spotify")));
            openSpotifyButton.Click();
        }

        [Test]
        public void UserIsLoggedOutWhenDeletingProfile()
        {
            playlistPage.NavigateToSettings();

            WebDriverWait waitForSettingsLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            IWebElement dataDeletionSetting = waitForSettingsLoad.Until(d => d.FindElement(By.XPath("/html/body/div[1]/div/div/div/main/div[2]/div/div/button")));
            dataDeletionSetting.Click();

            WebDriverWait waitForModalLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            IWebElement dataDeletionModalConfirm = waitForModalLoad.Until(d => d.FindElement(By.CssSelector("#data-delete-modal-confirm")));
            dataDeletionModalConfirm.Click();
            dataDeletionModalConfirm.Click();


            // Check user has been logged out and taken back to home page
            WebDriverWait waitForHomePageLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            bool homePageIsLoaded = waitForHomePageLoad.Until(d => d.FindElement(By.CssSelector("#setlist-form-url")).Displayed);

            var profileData = homePage.GetSpotifyProfileDetails();
            var profileName = driver.FindElements(By.CssSelector("#spotify-profile-displayname"));

            Assert.That(driver.Url, Is.EqualTo(homeUrl));
            Assert.That(profileData == null, Is.True);
            Assert.That(profileName.Count, Is.EqualTo(0));
        }

        [Test]
        public void UserCanLogout()
        {
            playlistPage.Logout();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var profileData = js.ExecuteScript("return window.localStorage.getItem('spProfile');");
            var profileName = driver.FindElements(By.CssSelector("#spotify-profile-displayname"));

            Assert.That(profileData == null, Is.True);
            Assert.That(profileName.Count, Is.EqualTo(0));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }
    }
}
