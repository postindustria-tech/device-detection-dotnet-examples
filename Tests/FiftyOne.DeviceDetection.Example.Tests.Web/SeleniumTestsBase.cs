/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2022 51 Degrees Mobile Experts Limited, Davidson House,
 * Forbury Square, Reading, Berkshire, United Kingdom RG1 3EU.
 *
 * This Original Work is licensed under the European Union Public Licence
 * (EUPL) v.1.2 and is subject to its terms as set out below.
 *
 * If a copy of the EUPL was not distributed with this file, You can obtain
 * one at https://opensource.org/licenses/EUPL-1.2.
 *
 * The 'Compatible Licences' set out in the Appendix to the EUPL (as may be
 * amended by the European Commission) shall be deemed incompatible for
 * the purposes of the Work and the provisions of the compatibility
 * clause in Article 5 of the EUPL shall not apply.
 * 
 * If using the Work as, or as part of, a network application, by 
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading, 
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Edge;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using DevToolsSessionDomains = OpenQA.Selenium.DevTools.DevToolsSessionDomains;
using OpenQA.Selenium.Firefox;
using System.Threading.Tasks;

// Used to map new version features.
using Enhanced = OpenQA.Selenium.DevTools.V114;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    public class SeleniumTestsBase : WebServerTestBase
    {
        /// <summary>
        /// Number of seconds to wait for a response that might satisfy the 
        /// test.
        /// </summary>
        protected static readonly TimeSpan TEST_TIMEOUT = 
            TimeSpan.FromSeconds(20);

        /// <summary>
        /// The driver being used for the active test. See 
        /// <see cref="InitializeChromeDriver"/>,
        /// <see cref="InitializeEdgeDriver"/>,
        /// <see cref="InitializeFirefoxDriver"/>.
        /// </summary>
        protected IWebDriver Driver { get; private set; }

        /// <summary>
        /// Network adapter if supported by the driver.
        /// </summary>
        protected Enhanced.Network.NetworkAdapter Network { get; private set; }

        /// <summary>
        /// Used to create new network adapters.
        /// </summary>
        private static readonly Enhanced.Network.EnableCommandSettings 
            NetworkSettings = 
            new Enhanced.Network.EnableCommandSettings();

        /// <summary>
        /// Cleans up after the test.
        /// </summary>
        [TestCleanup]
        public void CleanupDriver()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver.Dispose();
            }
        }

        /// <summary>
        /// Sets the <see cref="Driver"/> property for Chrome tests. If the 
        /// initilaization fails the test is flagged as inconclusive.
        /// </summary>
        protected void InitializeChromeDriver()
        {
            // If the driver and chrome versions are different it may cause
            // unexpected behaviour. 
            // See: https://sites.google.com/chromium.org/driver/downloads and
            // https://github.com/rosolko/WebDriverManager.Net
            var setupResult =  new DriverManager().SetUpDriver(
                new ChromeConfig(),
                VersionResolveStrategy.Latest);
            Console.WriteLine("Driver: " + setupResult);
            var chromeOptions = new ChromeOptions();
            chromeOptions.AcceptInsecureCertificates = true;
            chromeOptions.AddArgument("--headless=new");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            try
            {
                Driver = new ChromeDriver(chromeOptions);
            }
            catch (WebDriverException)
            {
                Assert.Inconclusive(
                    "Could not create a ChromeDriver, check " +
                    "that the Chromium driver is installed");
            }
            Network = GetNetwork(Driver).Result;
        }

        /// <summary>
        /// Sets the <see cref="Driver"/> property for Edge tests. If the 
        /// initilaization fails the test is flagged as inconclusive.
        /// </summary>
        protected void InitializeEdgeDriver()
        {
            var setupResult = new DriverManager().SetUpDriver(
                new EdgeConfig(), 
                VersionResolveStrategy.Latest);
            Console.WriteLine("Driver: " + setupResult);
            var firefoxOptions = new EdgeOptions();
            firefoxOptions.AcceptInsecureCertificates = true;
            firefoxOptions.AddArgument("--headless=new");
            try
            {
                Driver = new EdgeDriver(firefoxOptions);
            }
            catch (WebDriverException)
            {
                Assert.Inconclusive(
                    "Could not create a FirefoxDriver, check " +
                    "that the Edge driver is installed");
            }
            Network = GetNetwork(Driver).Result;
        }

        /// <summary>
        /// Sets the <see cref="Driver"/> property for Firefox tests. If the 
        /// initilaization fails the test is flagged as inconclusive.
        /// </summary>
        protected void InitializeFirefoxDriver()
        {
            var setupResult = new DriverManager().SetUpDriver(
                new FirefoxConfig(),
                VersionResolveStrategy.Latest);
            Console.WriteLine("Driver: " + setupResult);
            var firefoxOptions = new FirefoxOptions();
            firefoxOptions.AcceptInsecureCertificates = true;
            firefoxOptions.AddArgument("--headless");
            firefoxOptions.EnableDevToolsProtocol = true;
            try
            {
                Driver = new FirefoxDriver(firefoxOptions);
            }
            catch (WebDriverException)
            {
                Assert.Inconclusive(
                    "Could not create a EdgeDriver, check " +
                    "that the Edge driver is installed");
            }
            Network = GetNetwork(Driver).Result;
        }

        private static async Task<Enhanced.Network.NetworkAdapter> GetNetwork(
            IWebDriver driver)
        {
            var domains = (driver as IDevTools).GetDevToolsSession()
                .GetVersionSpecificDomains<DevToolsSessionDomains>();

            // If the dev tools support session network inspection then
            // initialize the network interface and add a reference to the
            // adapter.
            var modern = domains as Enhanced.DevToolsSessionDomains;
            if (modern != null)
            {
                await modern.Network.Enable(NetworkSettings);
                return modern.Network;
            }
            return null;
        }
    }
}
