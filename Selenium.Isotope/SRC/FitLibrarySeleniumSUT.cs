using System;
using System.Threading;
using fit;
using fitlibrary;
using Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using System.Reflection;

namespace Selenium.Isotope
{
    public class FitLibrarySeleniumSUT : DoFixture
    {
        #region Private Variables
        private static ISelenium _seleniumInstance;
        const string cMaxTimeOut = "30000"; // Default timeout for a single action is 30 seconds
        ISelenium SystemUnderTest { get { return _seleniumInstance; } }
        #endregion
        #region Private Methods
        /// <summary>
        /// Helper method. Prepares new SeleniumWait object to be used by WaitFor... functions
        /// </summary>
        private SeleniumWait<ISelenium> prepareNewWait(String timeout = cMaxTimeOut)
        {
            ISelenium selenium = SystemUnderTest;
            SeleniumWait<ISelenium> wait = new SeleniumWait<ISelenium>(selenium);
            wait.Timeout = TimeSpan.FromMilliseconds(Convert.ToDouble(timeout));
            return wait;
        }
        /// <summary>
        /// Creates a new instance of the WebDrive usign string type name.
        /// </summary>
        private IWebDriver NewWebDriver(String typeName)
        {
            TypeName driverType = new TypeName(typeName);
            IWebDriver driver = (IWebDriver)driverType.CreateInstance();
            return driver;
        }
        /// <summary>
        /// Sets selenium instance to become new SystemUnderTest.
        /// Starts new selenium using start() method.
        /// </summary>
        private void UseNewSelenium(ISelenium selenium)
        {
            _seleniumInstance = selenium;
            _seleniumInstance.Start();
            this.SetSystemUnderTest(_seleniumInstance);
        }
        #endregion
        #region Public Methods
        #region New Selenium Instances
        /// <summary>
        /// Create an instance of DefaultSelenium class (AKA Selenium 1, AKA RC, AKA CORE) and use it as SystemUnderTests.
        /// Requires selenium standalone server to be running.
        /// </summary>
        /// <param name="serverHost">Selenium server HOST</param>
        /// <param name="serverPort">Selenium server port</param>
        /// <param name="browserString">Browser to be used i.e. *firefox for FF</param>
        /// <param name="browserURL">Base URL for all the tests</param>
        public ISelenium NewDefaultSelenium(String serverHost, int serverPort, String browserString, String browserURL)
        {
            ISelenium selenium = new DefaultSelenium(serverHost, serverPort, browserString, browserURL);
            UseNewSelenium(selenium);
            return selenium;
        }
        /// <summary>
        /// Create an instance of WebDriverBackedSelenium class and use it as a SystemUnderTests.
        /// Does not require Selenium standalone server but may require driver-specific configuration.
        /// </summary>
        /// <param name="typeName">type of a new WebDrive instance: FirefoxDriver, ChromeDriver, InternetExplorerDriver</param>
        /// <param name="browserURL">Base URL for all the tests</param>
        public ISelenium NewWebDriverBackedSelenium(String typeName, String browserURL)
        {
            IWebDriver driver = NewWebDriver(typeName);
            ISelenium selenium = new WebDriverBackedSelenium(driver, browserURL);
            UseNewSelenium(selenium);
            return selenium;
        }
        /// <summary>
        /// Get an instance of the remote webdriver, create new webdriverbackedselenium instance and use it as a SystemUnderTests.
        /// Requires at least two selenium servers running:
        /// One Selenium HUB
        /// At least one Selenium Node
        /// <summary>
        /// <param name="hubURI">a URI to the selenium hub. f.e. http://127.0.0.1:4444/wd/hub </param>
        /// <param name="capabilitiesMethod">FireFox, InternetExplorer, PhantomJS, HtmlUnit, HtmlUnitWithJavaScript, IPhone, IPad, Chrome, Android, Opera, Safari</param>
        public ISelenium NewRemoteWebDriverBackedSelenium(String hubURI, String capabilitiesMethod)
        {
            DesiredCapabilities capabilities = new DesiredCapabilities();
            Type type = typeof(DesiredCapabilities);
            MethodInfo standardCapabilities = type.GetMethod(capabilitiesMethod, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
            capabilities = (DesiredCapabilities)standardCapabilities.Invoke(capabilities, null);
            IWebDriver driver = new RemoteWebDriver(new Uri(hubURI), capabilities);
            ISelenium selenium = new WebDriverBackedSelenium(driver, hubURI);
            UseNewSelenium(selenium);
            return selenium;
        }
        #endregion
        #region other public methods (pause, echo, store, etc.)
        public void pause(int milliseconds)
        {
            //  throws InterruptedException 
            Thread.Sleep(milliseconds);
        }
        /// <summary>
        /// Simple Echo Function. Returns input string.
        /// </summary>
        /// <param name="input">a string</param>
        /// <returns>Input string as is</returns>
        public string echo(string input)
        {
            return input;
        }
        /// <summary>
        /// wraps echo to store strings into fitnesse symbols
        /// </summary>
        /// <param name="input">a string</param>
        /// <returns>Input String as is. Use symbol recall characters to store output into a symbol.</returns>
        public string store(string input)
        {
            return echo(input);
        }
        #endregion
        #region WaitFor... methods
        /// <summary>
        /// Waits for alert identified by cLocator to appear until cTimeout exceeds
        /// </summary>
        /// <param name="cLocator">Alert Locator (not in use)</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForAlert(string cLocator, string cTimeOut = cMaxTimeOut)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeOut);
            bResult = wait.Until(sel => (sel.IsAlertPresent() == true));
            return bResult;
        }
        /// <summary>
        /// Overload of WaitForAlert. Uses default timeout. 
        /// </summary>
        /// <param name="cLocator">Alert Locator (not in use)</param>
        /// <returns>True if event happened within expeted timeout.</returns>        
        public bool WaitForAlert(string cLocator)
        {
            bool bResult = WaitForAlert(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for Alert to become Not Present  
        /// </summary>
        /// <param name="cLocator">Alert Locator</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForNotAlert(string cLocator, string cTimeOut = cMaxTimeOut)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeOut);
            bResult = wait.Until(sel => (sel.IsAlertPresent() ? sel.GetAlert() != null : sel.IsAlertPresent() == false));
            return bResult;
        }
        /// <summary>
        /// Overload of WaitForNotAlert. Uses default timeout. 
        /// </summary>
        /// <param name="cLocator">Alert Locator</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForNotAlert(string cLocator)
        {
            bool bResult = WaitForNotAlert(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for the text to BE present on the page. 
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForText(string cLocator, string cTimeOut = cMaxTimeOut)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeOut);
            bResult = wait.Until(sel => (sel.IsTextPresent(cLocator) == true));
            return bResult;
        }
        /// <summary>
        /// Overload of WaitForText. Uses default timeout. 
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForText(string cLocator)
        {
            bool bResult = WaitForText(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for the text NOT to be present on the page. 
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForNotText(string cLocator, string cTimeOut = cMaxTimeOut)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeOut);
            bResult = wait.Until(sel => (sel.IsTextPresent(cLocator) == false));
            return bResult;
        }
        /// <summary>
        /// Overload of WaitForNotText. Uses default timeout. 
        /// </summary>
        /// <param name="cLocator"></param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForNotText(string cLocator)
        {
            bool bResult = WaitForNotText(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for the invisible element to become visible for the maximum of xxx milliseconds. 
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForVisible(string cLocator, string cTimeOut)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeOut);
            bResult = wait.Until(sel => (sel.IsVisible(cLocator) == true));
            return bResult;
        }
        /// <summary>
        /// Overload Of WaitForVisible. Uses default Timeout.
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForVisible(string cLocator)
        {
            bool bResult = WaitForVisible(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for the visible element to become invisible. 
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForNotVisible(string cLocator, string cTimeOut)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeOut);
            bResult = wait.Until(sel => (sel.IsVisible(cLocator) == false));
            return bResult;
        }
        /// <summary>
        /// Overload Of WaitForNotVisible. Uses default Timeout.
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public bool WaitForNotVisible(string cLocator)
        {
            bool bResult = WaitForNotVisible(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for an element specified as locator until timeout exceeds
        /// </summary>
        /// <param name="cLocator">Selenium element Locator</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public Boolean WaitForElementPresent(String cLocator, String cTimeout)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeout);
            bResult = wait.Until(sel => (sel.IsElementPresent(cLocator) == true));
            return bResult;
        }
        /// <summary>
        /// Overload of WaitForElementPresent. Uses default timeout. 
        /// </summary>
        /// <param name="cLocator">Selenium Element Locator</param>
        /// <returns>True if event happened within expeted timeout.</returns>        
        public bool WaitForElementPresent(string cLocator)
        {
            bool bResult = WaitForElementPresent(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for an element specified as locator to be removed from the page until timeout exceeds
        /// </summary>
        /// <param name="cLocator">Selenium element Locator</param>
        /// <param name="cTimeOut">Milliseconds i.e. 3000 for 3 seconds</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public Boolean WaitForElementNotPresent(String cLocator, String cTimeout)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeout);
            bResult = wait.Until(sel => (sel.IsElementPresent(cLocator) == false));
            return bResult;
        }
        /// <summary>
        /// Overload of WaitForElementNotPresent. Uses default timeout. 
        /// </summary>
        /// <param name="cLocator">Selenium Element Locator</param>
        /// <returns>True if event happened within expeted timeout.</returns>        
        public bool WaitForElementNotPresent(string cLocator)
        {
            bool bResult = WaitForElementNotPresent(cLocator, cMaxTimeOut);
            return bResult;
        }
        /// <summary>
        /// Waits for an total count of elements identified by xPath to become NOT equal to cpecified value until timeout exceeds
        /// </summary>
        /// <param name="cLocator">Selenium element locator</param>
        /// <param name="value">Expected count value</param>
        /// <returns>True if event happened within expeted timeout.</returns>
        public Boolean WaitForXpathCount(String cLocator, String value, String cTimeout)
        {
            bool bResult = false;
            SeleniumWait<ISelenium> wait = prepareNewWait(cTimeout);
            bResult = wait.Until(sel => (sel.GetXpathCount(cLocator).Equals(value)));
            return bResult;
        }
        /// <summary>
        /// Overload of WaitForXpathCount. Uses default timeout. 
        /// </summary>
        /// <param name="cLocator">attributeLocator - an element locator followed by an @ sign and then the name of the attribute, e.g. "foo@bar"</param>
        /// <param name="value">Expected value</param>
        /// <returns>True if event happened within expeted timeout.</returns>        
        public bool WaitForXpathCount(string cLocator, String value)
        {
            bool bResult = WaitForXpathCount(cLocator, value, cMaxTimeOut);
            return bResult;
        }
        #endregion
        #endregion
    }
}