using Cloud_Client_Hints_Not_Integrated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiftyOne.DeviceDetection.Example.Tests.Web.Cloud
{
    [TestClass]
    public class ClientHintsNonIntegratedTest : ClientHintsTest<Program>
    {
        /// <summary>
        /// Test that the 'Cloud_Client_Hints_Not_Integrated_NetCore_31' 
        /// example is returning the expected Accept-CH header values 
        /// for various different scenarios.
        /// </summary>
        /// <param name="resourceKey">
        /// The resource key to use when creating the web server 
        /// running the example.
        /// </param>
        /// <param name="userAgent">
        /// The value to set the User-Agent header to when making 
        /// a request to the example web server.
        /// </param>
        /// <param name="expectedAcceptCH">
        /// A list of the values that are expected in the Accept-CH 
        /// header in the response from the example web server.
        /// </param>
        /// <returns></returns>
        [DataTestMethod]
        [DynamicData(
            nameof(Parameters.VerifyCloudExample_DATA), 
            typeof(Parameters),
            DynamicDataSourceType.Method)]
        public async Task VerifyNonIntegratedExample_Cloud(
            string resourceKey,
            string userAgent,
            List<string> expectedAcceptCH)
        {
            await VerifyExample(
                resourceKey,
                userAgent,
                expectedAcceptCH,
                "Cloud-UACH-manual");
        }
    }
}
