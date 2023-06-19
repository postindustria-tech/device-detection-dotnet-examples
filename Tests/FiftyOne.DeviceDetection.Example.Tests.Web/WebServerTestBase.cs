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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Example.Tests.Web
{
    public class WebServerTestBase
    {
        /// <summary>
        /// Protocol, domain, and port base URLs on which the example is 
        /// listening for SSL connections. <see cref="Constants"/>
        /// </summary>
        public static IEnumerable<object[]> HttpsUrlsData =>
            Constants.LOCALHOST_HTTPS_PORTS.Select(i =>
                new object[] { $"https://localhost:{i}" }).ToArray();

        /// <summary>
        /// Protocol, domain, and port base URLs on which the example is 
        /// listening. <see cref="Constants"/>
        /// </summary>
        public static IEnumerable<object[]> AllUrlsData =>
            Constants.LOCALHOST_HTTP_PORTS.Select(i =>
                new object[] { $"http://localhost:{i}" }).Concat(
                HttpsUrlsData).ToArray();

        /// <summary>
        /// The task for the running web server under test.
        /// See <see cref="StartServer(Action{string[]})"/>.
        /// </summary>
        protected Task ServerTask { get; private set; }

        /// <summary>
        /// Stop token used to stop the web server when the test completes.
        /// </summary>
        protected readonly CancellationTokenSource StopSource =
            new CancellationTokenSource();

        /// <summary>
        /// Cleans up after the test.
        /// </summary>
        [TestCleanup]
        public void TestCleanupServer()
        {
            StopSource.Cancel(true);
            if (ServerTask != null)
            {
                ServerTask.Wait();
                Assert.IsNull(ServerTask.Exception);
                Assert.IsTrue(ServerTask.IsCompletedSuccessfully);
            }
        }

        /// <summary>
        /// Starts the web server storing the task in <see cref="ServerTask"/>.
        /// The action passed should ensure that <see cref="StopSource"/> token
        /// is passed so that the web server under test can cooperate with the
        /// test framework to shutdown the web server elegantly and avoid the
        /// ports remaining in use and thus preventing another task from 
        /// running.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        protected void StartServer(Action action)
        {
            ServerTask = Task.Run(action, StopSource.Token);
        }
    }
}
