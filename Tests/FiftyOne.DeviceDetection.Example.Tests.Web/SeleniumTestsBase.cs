using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using WebDriverManager;
using FiftyOne.DeviceDetection.Examples;
using System.Collections.Generic;
using OpenQA.Selenium.Edge;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    public class SeleniumTestsBase
    {
        protected IWebDriver Driver { get; set; }


        protected void InitializeChromeDriver()
        {
            // If the driver and chrome versions are different it may cause unexpected behaviour. 
            // See: https://sites.google.com/chromium.org/driver/downloads and https://github.com/rosolko/WebDriverManager.Net
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
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
                Assert.Inconclusive("Could not create a ChromeDriver, check " +
                    "that the Chromium driver is installed");
            }
        }

        protected void InitializeEdgeDriver()
        {
            new DriverManager().SetUpDriver(new EdgeConfig(), VersionResolveStrategy.MatchingBrowser);
            var edgeOptions = new EdgeOptions();
            edgeOptions.AcceptInsecureCertificates = true;
            edgeOptions.AddArgument("--headless=new");
            try
            {
                Driver = new EdgeDriver(edgeOptions);
            }
            catch (WebDriverException)
            {
                Assert.Inconclusive("Could not create a ChromeDriver, check " +
                    "that the Chromium driver is installed");
            }
        }
    }
}
