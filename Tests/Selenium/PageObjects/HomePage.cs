using OpenQA.Selenium;

namespace sts_net.Tests.Selenium.PageObjects
{
    internal class HomePage : Page
    {
        private readonly IWebDriver _driver;

        private By setlistInputBy = By.CssSelector("#setlist-form-url");
        private By setlistSearchButton = By.CssSelector("#setlist-form-search");


        public HomePage(IWebDriver driver) : base(driver)
        {
            _driver = driver;
        }

        public void SearchForSetlist(string setlistFmUrl)
        {
            IWebElement setlistInput = _driver.FindElement(setlistInputBy);
            setlistInput.SendKeys(setlistFmUrl);
            IWebElement searchButton = _driver.FindElement(setlistSearchButton);
            searchButton.Click();
        }
    }
}
