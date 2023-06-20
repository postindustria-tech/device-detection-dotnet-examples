using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiftyOne.DeviceDetection.Examples.Cloud.GettingStartedWeb.ClientOnly.Pages
{
    public class Model : PageModel
    {
        /// <summary>
        /// Resource key provided by the environment variable with the key 
        /// <see cref="ExampleUtils.CLOUD_RESOURCE_KEY_ENV_VAR"/>.
        /// </summary>
        public string? ResourceKey { get; private set; }

        /// <summary>
        /// Cloud end point provided by the environment variable with the key 
        /// <see cref="ExampleUtils.CLOUD_END_POINT_ENV_VAR"/>. Will default
        /// to 
        /// </summary>
        public string? CloudEndPoint { get; private set; }

        /// <summary>
        /// The default cloud end point if the environment variable is not
        /// configured.
        /// </summary>
        private const string DEFAULT_END_POINT = "https://cloud.51degrees.com";

        public Model()
        {
            ResourceKey = Environment.GetEnvironmentVariable(
                ExampleUtils.CLOUD_RESOURCE_KEY_ENV_VAR);
            var cloudEndPoint = Environment.GetEnvironmentVariable(
                ExampleUtils.CLOUD_END_POINT_ENV_VAR);
            CloudEndPoint = cloudEndPoint == null ?
                DEFAULT_END_POINT :
                cloudEndPoint;
        }

        public void OnGet()
        {
        }
    }
}
