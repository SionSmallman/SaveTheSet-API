using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using sts_net.Tests.Selenium.PageObjects;

namespace sts_net.Tests.Selenium.Tests.Chrome
{
    internal class BaseTests
    {
        private IWebDriver driver;
        private string homeUrl;
        private HomePage homePage;
        private PlaylistPage playlistPage;

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

        }

        [Test]
        public void RecentlySavedCardsAreDisplayed()
        {
            WebDriverWait waitForCardLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var recentlySavedCards = waitForCardLoad.Until(d => d.FindElement(By.CssSelector("#mask")).Displayed);

            Assert.That(recentlySavedCards, Is.True);
        }

        [Test]
        public void SearchForSetlistWithValidUrlShouldSucceed()
        {
            homePage.SearchForSetlist("https://www.setlist.fm/setlist/oliver-francis/2022/trees-dallas-tx-73bff249.html");

            //Wait for setlist to load on page
            WebDriverWait waitForSetlistLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            bool setlistIsDisplayed = waitForSetlistLoad.Until(d => d.FindElement(By.CssSelector("#songlist-container")).Displayed);
            var songs = driver.FindElements(By.CssSelector("#songlist-song"));
            Assert.That(setlistIsDisplayed, Is.True);
            Assert.That(songs.Count, Is.EqualTo(4));

        }

        [Test]
        public void LoginFromHeaderShouldSucceed()
        {

            homePage.LoginFromHeader("savetheset.dummy@gmail.com", "SaveTheSet1");
            var profileData = homePage.GetSpotifyProfileDetails();
            var profileName = homePage.GetDisplayName();
            Assert.That(profileData != null, Is.True);
            Assert.That(profileName, Is.EqualTo("savetheset"));
        }

        [Test]
        public void LoginFromSetlistPageShouldSucceed()
        {
            homePage.SearchForSetlist("https://www.setlist.fm/setlist/oliver-francis/2022/trees-dallas-tx-73bff249.html");

            //Wait for setlist to load on page
            WebDriverWait waitForSetlistLoad = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            bool setlistIsDisplayed = waitForSetlistLoad.Until(d => d.FindElement(By.CssSelector("#songlist-container")).Displayed);

            playlistPage.LoginOnPlaylistPage("savetheset.dummy@gmail.com", "SaveTheSet1");

            bool profileNameIsDisplayed = driver.FindElement(By.CssSelector("#spotify-profile-displayname")).Displayed;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var profileData = js.ExecuteScript("return window.localStorage.getItem('spProfile');");

            Assert.That(profileNameIsDisplayed, Is.True);
            Assert.That(profileData != null, Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }

    }
}
