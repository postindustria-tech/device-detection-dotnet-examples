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
using System;
using System.Net;
using System.Net.Http;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    public class GettingStartedTestBase : WebServerTestBase
    {
        /// <summary>
        /// Test that the running <see cref="WebServerTestBase.ServerTask"/> 
        /// is able to run and returns code 200 upon https request. The
        /// <see cref="WebServerTestBase.StartServer(Action)"/> must have been
        /// called in the test initialization method.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [DynamicData(nameof(AllUrlsData),typeof(WebServerTestBase))]
        public void VerifyExample_Returns_Status_Code_200(string url)
        {
            // Setup
            using (var http = new HttpClient())
            using (var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
            })
            { 
                request.Headers.Add("User-Agent", "abc");

                // Act
                var response = http.Send(request);

                // Assert
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
