/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
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

using FiftyOne.Pipeline.Core.FlowElements;
using FiftyOne.Pipeline.Engines;
using FiftyOne.Pipeline.Engines.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// @example OnPremise/GettingStarted-Console/Program.cs
/// 
/// @include{doc} example-getting-started-onpremise.txt
/// 
/// This example is available in full on [GitHub](https://github.com/51Degrees/device-detection-dotnet-examples/blob/master/Examples/OnPremise/GettingStarted-Console/Program.cs). 
/// 
/// @include{doc} example-require-datafile.txt
/// 
/// Required NuGet Dependencies:
/// - FiftyOne.DeviceDetection
/// </summary>
namespace FiftyOne.DeviceDetection.Examples.OnPremise.GettingStartedConsole
{
    public class Program
    {
        public class Example : ExampleBase
        {
            public void Run(string dataFile, ILoggerFactory loggerFactory, TextWriter output)
            {
                // In this example, we use the DeviceDetectionPipelineBuilder and configure it
                // in code. For more information about builders in general see the documentation at
                // https://51degrees.com/documentation/_concepts__configuration__builders__index.html

                // Note that we wrap the creation of a pipeline in a using to control its life cycle
                using (var pipeline = new DeviceDetectionPipelineBuilder(loggerFactory)
                    .UseOnPremise(dataFile, null, false)
                    // We use the low memory profile as its performance is sufficient for this
                    // example. See the documentation for more detail on this and other
                    // configuration options:
                    // https://51degrees.com/documentation/_device_detection__features__performance_options.html
                    // https://51degrees.com/documentation/_features__automatic_datafile_updates.html
                    // https://51degrees.com/documentation/_features__usage_sharing.html
                    .SetPerformanceProfile(PerformanceProfiles.LowMemory)
                    // inhibit sharing usage for this example, usually this should be set to "true"
                    .SetShareUsage(false)
                    // inhibit auto-update of the data file for this test
                    .SetAutoUpdate(false)
                    .SetDataUpdateOnStartUp(false)
                    .SetDataFileSystemWatcher(false)
                    .Build())
                {
                    var deviceIDs = new List<string>(ExampleUtils.EvidenceValues.Count);

                    // carry out some sample detections
                    // and collect device IDs
                    foreach (var values in ExampleUtils.EvidenceValues)
                    {
                        AnalyseEvidence(values, pipeline, output, out var nextDeviceID);
                        deviceIDs.Add(nextDeviceID);
                    }

                    // carry out detections using pre-acquired deviceIDs
                    foreach (var nextDeviceID in deviceIDs.Where(d => d is not null))
                    {
                        var evidence = new Dictionary<string, object>()
                        {
                            { "query.51D_deviceId", nextDeviceID },
                        };
                        AnalyseEvidence(evidence, pipeline, output, out _);
                    }

                    ExampleUtils.CheckDataFile(pipeline, loggerFactory.CreateLogger<Program>());
                }
            }

            private void AnalyseEvidence(
                Dictionary<string, object> evidence, 
                IPipeline pipeline, 
                TextWriter output,
                out string deviceID)
            {
                // FlowData is a data structure that is used to convey information required for
                // detection and the results of the detection through the pipeline.
                // Information required for detection is called "evidence" and usually consists
                // of a number of HTTP Header field values, in this case represented by a
                // Dictionary<string, object> of header name/value entries.
                //
                // FlowData is wrapped in a using block in order to ensure that the unmanaged
                // resources allocated by the native device detection library are freed
                using (var data = pipeline.CreateFlowData())
                {
                    StringBuilder message = new StringBuilder();

                    // list the evidence
                    message.AppendLine("Input values:");
                    foreach (var entry in evidence)
                    {
                        message.AppendLine($"\t{entry.Key}: {entry.Value}");
                    }
                    output.WriteLine(message.ToString());

                    // Add the evidence values to the flow data
                    data.AddEvidence(evidence);

                    // Process the flow data.
                    data.Process();

                    message = new StringBuilder();
                    message.AppendLine("Results:");
                    // Now that it's been processed, the flow data will have been populated with
                    // the result. In this case, we want information about the device, which we
                    // can get by asking for a result matching the `IDeviceData` interface.
                    var device = data.Get<IDeviceData>();

                    // Display the results of the detection, which are called device properties.
                    // See the property dictionary at
                    // https://51degrees.com/developers/property-dictionary
                    // for details of all available properties.
                    OutputValue("Mobile Device", device.IsMobile, message);
                    OutputValue("Platform Name", device.PlatformName, message);
                    OutputValue("Platform Version", device.PlatformVersion, message);
                    OutputValue("Browser Name", device.BrowserName, message);
                    OutputValue("Browser Version", device.BrowserVersion, message);
                    OutputValue("DeviceId", device.DeviceId, message);
                    message.AppendLine("Matched");
                    foreach(var entry in device.UserAgents.Value)
                    {
                        message.AppendLine($"\t{entry}");
                    }
                    output.WriteLine(message.ToString());

                    // 'Return' the Device ID string to the caller
                    deviceID = device.DeviceId.HasValue ? device.DeviceId.Value : null;
                }
            }

            private void OutputValue(string name, 
                IAspectPropertyValue value,
                StringBuilder message)
            {
                // Individual result values have a wrapper called `AspectPropertyValue`.
                // This functions similarly to a null-able type.
                // If the value has not been set then trying to access the `Value` property will
                // throw an exception. `AspectPropertyValue` also includes the `NoValueMessage`
                // property, which describes why the value has not been set.
                message.AppendLine(value.HasValue ?
                    $"\t{name}: " + value.Value :
                    $"\t{name}: " + value.NoValueMessage);
            }
        }

        static void Main(string[] args)
        {
            // Use the supplied path for the data file or find the lite file that is included
            // in the repository.
            var dataFile = args.Length > 0 ? args[0] :
                // In this example, by default, the 51degrees "Lite" file needs to be somewhere in the
                // project space, or you may specify another file as a command line parameter.
                //
                // Note that the Lite data file is only used for illustration, and has limited accuracy
                // and capabilities. Find out about the Enterprise data file on our pricing page:
                // https://51degrees.com/pricing
                ExampleUtils.FindFile(Constants.LITE_HASH_DATA_FILE_NAME);

            // Configure a logger to output to the console.
            var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
            var logger = loggerFactory.CreateLogger<Program>();

            if (dataFile != null)
            {
                new Example().Run(dataFile, loggerFactory, Console.Out);
            } 
            else
            {
                logger.LogError("Failed to find a device detection data file. Make sure the " +
                    "device-detection-data submodule has been updated by running " +
                    "`git submodule update --recursive`.");
            }

            // Dispose the logger to ensure any messages get flushed
            loggerFactory.Dispose();
        }
    }
}