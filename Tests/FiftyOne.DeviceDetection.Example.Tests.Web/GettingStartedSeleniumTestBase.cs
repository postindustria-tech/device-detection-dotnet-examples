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
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    public class GettingStartedSeleniumTestBase : SeleniumTestsBase
    {
        private const string STATIC_HTML_ENDPOINT = "/static.html";
        private const string TEST_PAGE_ENDPOINT = "/testpage.html";

        public GettingStartedSeleniumTestBase(
            Func<CancellationToken, Task> startServer) : base(startServer)
        {
        }

        /// <summary>
        /// Verifies that the 51d cookie is populated.
        /// </summary>
        /// <param name="url"></param>
        [DataTestMethod]
        [DynamicData(nameof(Parameters.HttpsUrlsData), typeof(Parameters))]
        public void VerifyExample_GetHighEntropyValues_Populates_51D_Cookie(string url)
        {
            if (Network == null)
            {
                Assert.Inconclusive(
                    "Test session does not support cookie verification");
            }

            // Act
            Driver.Navigate().GoToUrl(url + STATIC_HTML_ENDPOINT);

            // Wait for the page to load
            new WebDriverWait(Driver, TEST_TIMEOUT).Until(driver => true);

            var cookies = Network.GetAllCookies().Result;
            var fod_cookie = cookies.Cookies.Where(c => 
                c.Name == "51D_GetHighEntropyValues").Single();
            var bytes = new Span<byte>(new byte[1024]);

            // Assert
            Assert.IsNotNull(fod_cookie);
            Assert.IsTrue(Convert.TryFromBase64String(
                fod_cookie.Value, 
                bytes, 
                out int _));
        }

        [DataTestMethod]
        [DynamicData(nameof(Parameters.HttpsUrlsData), typeof(Parameters))]
        public void VerifyExample_GetHighEntropyValues_Contains_CORS_Response_Header(string url)
        {
            const string KEY = "access-control-allow-origin";

            if (Network == null)
            {
                Assert.Inconclusive(
                    "Test session does not support CORS verification");
            }

            Dictionary<string, string> headerValuePairs = new();

            // Get Response Headers if the URL relates to '/51dpipeline/json'.
            Network.ResponseReceived += (sender, e) =>
            {
                var headers = e.Response.Headers;
                var responseUrl = e.Response.Url;
                if (responseUrl.Contains("/51dpipeline/json"))
                {
                    foreach (var header in headers)
                    {
                        headerValuePairs.Add(header.Key.ToLower(), header.Value);
                    }
                }
            };

            // Act
            // Do a cross origin request
            Driver.Navigate().GoToUrl(url + STATIC_HTML_ENDPOINT);

            // Wait for the page to load
            new WebDriverWait(Driver, TEST_TIMEOUT).Until(driver =>
            {
                return headerValuePairs.ContainsKey(KEY);
            });

            // Assert
            // Verify that the response contains the header
            Assert.IsTrue(headerValuePairs.ContainsKey(KEY));
            Assert.IsTrue(headerValuePairs[KEY].Equals(url));
        }

        [DataTestMethod]
        [DynamicData(nameof(Parameters.HttpsUrlsData), typeof(Parameters))]
        public void VerifyExample_GetHighEntropyValues_Fod_Completes(string url)
        {
            // Act
            Driver.Navigate().GoToUrl(url + TEST_PAGE_ENDPOINT);

            // This throws an exception if the timeout period elapses,
            // which will cause the test to fail.
            var result = new WebDriverWait(Driver, TEST_TIMEOUT).Until(
                driver =>
                {
                    // Gets the value of the global JavaScript variable test
                    // from the TEST_PAGE_ENDPOINT HTML page. Checks this value
                    // is 'complete' to indiciate that the complete event
                    // fired.
                    var js = (IJavaScriptExecutor)driver;
                    var test = js.ExecuteScript("return test;");
                    return test.Equals("complete");
                });

            // Assert
            Assert.IsTrue(result);
        }
    }
}
