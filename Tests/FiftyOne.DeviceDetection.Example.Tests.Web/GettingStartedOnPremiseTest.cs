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

using FiftyOne.DeviceDetection.Examples;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V113;
using OpenQA.Selenium.Support.UI;
using Stubble.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;


namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    [TestClass]
    public class GettingStartedOnPremiseTest : WebExampleTestBase
    {
        private const string STATIC_HTML_ENDPOINT = "/static.html";
        private const string TEST_PAGE_ENDPOINT = "/testpage.html";
        private WebDriver _driver;

        [TestInitialize]
        public void Init()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AcceptInsecureCertificates = true;
            // run in headless mode.
            chromeOptions.AddArgument("--headless=new");
            try
            {
                _driver = new ChromeDriver(chromeOptions);
            }
            catch (WebDriverException)
            {
                Assert.Inconclusive("Could not create a ChromeDriver, check " +
                    "that the Chromium driver is installed");
            }

        }

        /// <summary>
        /// Test that the 'GettingStarted-Web' example is able to run and 
        /// returns code 200 upon https request.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task VerifyExample_OnPremise_Returns_Status_Code_200()
        {
            //Arrange 

            // We are using the same port that the example is listening on.
            int port = Constants.LOCALHOST_HTTP_PORTS[0];
            var stopToken = new CancellationTokenSource();
            
            // Running the main method of the example to ensure we are testing
            // the same code and that the same configuraton is used by both tests and examples
            var serverTask = Task.Run(() => 
                Examples.OnPremise.GettingStartedWeb.Program.Main(
                    new string[] { }), 
                    stopToken.Token);

            using (var http = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(String.Format("http://localhost:{0}",port))
                };
                request.Headers.Add("User-Agent", "abc");

                // Act
                var response = await http.SendAsync(request);

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

                if (request != null) { request.Dispose(); }
                if (response != null) { response.Dispose(); }
            }
            stopToken.Cancel(false);

        }

        /// <summary>
        /// Ports on which the example is listening are defined in <see cref="Constants"/>
        /// </summary>
        public static IEnumerable<object[]> UrlsData
        {
            get
            {
                return new[]
                {
                    new object[] { $"https://localhost:{Constants.LOCALHOST_HTTPS_PORTS[0]}" },
                    new object[] { $"https://localhost:{Constants.LOCALHOST_HTTPS_PORTS[1]}" }
                };

            }
        }

        /// <summary>
        /// Verifies that the 51d cookie is populated.
        /// </summary>
        /// <param name="url"></param>
        [DataTestMethod]
        [DynamicData(nameof(UrlsData))]
        public void VerifyExample_GetHighEntropyValues_Populates_51D_Cookie(string url)
        {
            // Arrange
            var stopToken = new CancellationTokenSource();
            var serverTask = Task.Run(() =>
                Examples.OnPremise.GettingStartedWeb.Program.Main(
                    new string[] { }),
                    stopToken.Token);

            using (_driver)
            {
                // Enable DevTools
                var devTools = _driver as IDevTools;
                var session = devTools.GetDevToolsSession();

                var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V113.DevToolsSessionDomains>();
                domains.Network.Enable(new OpenQA.Selenium.DevTools.V113.Network.EnableCommandSettings());


                // Act
                _driver.Navigate().GoToUrl(url + STATIC_HTML_ENDPOINT);

                // Wait for the page to load
                Thread.Sleep(TimeSpan.FromSeconds(3));

                var cookies = domains.Network.GetAllCookies().Result;
                var fod_cookie = cookies.Cookies.Where(c => c.Name == "51D_GetHighEntropyValues").Single();
                var bytes = new Span<byte>(new byte[1024]);

                // Assert
                Assert.IsNotNull(fod_cookie);
                Assert.IsTrue(Convert.TryFromBase64String(fod_cookie.Value, bytes, out int _));

                // Quit the driver
                _driver.Quit();
            }
            stopToken.Cancel(false);

        }

        [TestMethod]
        public void VerifyExample_GetHighEntropyValues_Contains_CORS_Response_Header()
        {
            // Arrange
            var stopToken = new CancellationTokenSource();
            var serverTask = Task.Run(() =>
                Examples.OnPremise.GettingStartedWeb.Program.Main(
                    new string[] { }),
                    stopToken.Token);

            using (_driver)
            {
                // Enable DevTools
                var devTools = _driver as IDevTools;
                var session = devTools.GetDevToolsSession();

                var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V113.DevToolsSessionDomains>();
                domains.Network.Enable(new OpenQA.Selenium.DevTools.V113.Network.EnableCommandSettings());

                Dictionary<string, string> headerValuePairs = new();

                // Get Response Headers
                domains.Network.ResponseReceived += (sender, e) =>
                {
                    var headers = e.Response.Headers;
                    var responseUrl = e.Response.Url;
                    if (responseUrl.Equals($"{UrlsData.ElementAt(0).Single()}/51dpipeline/json"))
                    {
                        foreach (var header in headers)
                        {
                            headerValuePairs.Add(header.Key.ToLower(), header.Value);
                        }
                    }
                };

                // Act
                // Do a cross origin request
                var url = UrlsData.ElementAt(1).Single().ToString();
                _driver.Navigate().GoToUrl(url + STATIC_HTML_ENDPOINT);

                // Wait for the page to load
                Thread.Sleep(TimeSpan.FromSeconds(3));

                // Assert
                // Verify that the response contains the header
                Assert.IsTrue(headerValuePairs.ContainsKey("access-control-allow-origin"));

                // Quit the driver
                _driver.Quit();
            }
            stopToken.Cancel(false);
        }
        [DataTestMethod]
        [DynamicData(nameof(UrlsData))]
        public void VerifyExample_GetHighEntropyValues_Fod_Completes(string url)
        {
            // Arrange
            var stopToken = new CancellationTokenSource();
            var serverTask = Task.Run(() =>
                Examples.OnPremise.GettingStartedWeb.Program.Main(
                    new string[] { }),
                    stopToken.Token);
            using (_driver)
            {
                // Enable DevTools
                var devTools = _driver as IDevTools;
                var session = devTools.GetDevToolsSession();
                IJavaScriptExecutor js = _driver;

                var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V113.DevToolsSessionDomains>();
                domains.Network.Enable(new OpenQA.Selenium.DevTools.V113.Network.EnableCommandSettings());

                _driver.Navigate().GoToUrl(url + TEST_PAGE_ENDPOINT);

                // This throws an exception if the timeout period elapses,
                // which will cause the test to fail.
                var complete = new WebDriverWait(_driver, TimeSpan.FromSeconds(20)).Until(
                     driver =>
                         js.ExecuteScript("return test").Equals("complete") &&
                         js.ExecuteScript("return deviceid").Equals("loading") == false);


                // Quit the _driver
                _driver.Quit();
            }
            stopToken.Cancel(false);

        }
       
    }

}
