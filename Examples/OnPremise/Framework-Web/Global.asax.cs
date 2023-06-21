using FiftyOne.DeviceDetection.Examples;
using FiftyOne.DeviceDetection.Hash.Engine.OnPremise.FlowElements;
using FiftyOne.DeviceDetection.Uach;
using FiftyOne.Pipeline.Engines.FiftyOne.FlowElements;
using FiftyOne.Pipeline.JavaScriptBuilder.FlowElement;
using FiftyOne.Pipeline.JsonBuilder.FlowElement;
using System;
using System.IO;
using System.Web;

namespace Framework_Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Modify the configuration to a full path to the data file if the standard example is being used.
            var configFile = new FileInfo(HttpContext.Current.Server.MapPath("~/App_Data/51Degrees.json"));
            var fullPath = ExampleUtils.FindFile("51Degrees-LiteV4.1.hash", configFile.Directory);
            if (String.IsNullOrEmpty(fullPath) == false)
            {
                var config = File.ReadAllText(configFile.FullName);
                File.WriteAllText(
                    configFile.FullName, 
                    config.Replace(
                        "\"DataFile\": \"51Degrees-LiteV4.1.hash\"",
                        $"\"DataFile\": \"{fullPath.Replace("\\", "\\\\")}\""));
            }

            // Make sure the assemblies that are needed by the pipeline are loaded into the app domain.
            // This is needed in order from BuildFromConfiguration to be able to find the relevant builder types when
            // using reflection.
            AppDomain.CurrentDomain.Load(typeof(UachJsConversionElement).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(DeviceDetectionHashEngine).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(JavaScriptBuilderElement).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(JsonBuilderElement).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(SequenceElementBuilder).Assembly.GetName());
        }
    }
}