using System;
using System.Collections.Generic;
using System.Text;

namespace FiftyOne.DeviceDetection.Examples
{
    public class ExampleOptions
    {
        /// <summary>
        /// Path to the data file to use in the example. Null by default.
        /// </summary>
        public string DataFilePath { get; internal set; }

        /// <summary>
        /// Path to the evidence YAML file to use in the example. Null by default.
        /// </summary>
        public string EvidenceFile { get; internal set; }

        /// <summary>
        /// Path to a JSON file to output to if applicable in the example. Null by default.
        /// </summary>
        public string JsonOutput { get; internal set; }
    }
}
