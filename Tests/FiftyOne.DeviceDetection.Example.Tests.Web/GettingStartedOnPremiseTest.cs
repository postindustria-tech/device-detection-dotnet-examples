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
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    [TestClass]
    public class GettingStartedOnPremiseTest : WebExampleTestBase
    {
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
                    RequestUri = new Uri(String.Format("http://localhost:{0}", port))
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
    }

}
