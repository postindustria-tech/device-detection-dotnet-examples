## Examples
The examples provided demonstrate how to use the 51Degrees device detection in both cloud-based and on-premise environments.
They showcase different aspects of device detection, including retrieving device details based on User-Agent and User-Agent Client Hints HTTP header values, accessing meta-data, performing TAC and native model lookups, viewing match metrics, offline processing, and performance optimization. 
These examples serve as practical guides for developers to understand and implement device detection capabilities within their applications.

## Cloud resource keys

A resource key configured with the properties needed
to run most of the examples can be obtained [here](https://configure.51degrees.com/jqz435Nc). 
To use the resource key in the example it can be supplied as an environment variable called "SUPER_RESOURCE_KEY".

Some cloud examples require an enhanced resource key containing a license key. And some
on-premise examples require you to provide a license key. You can find out about 
resource keys and license keys at our [pricing page](https://51degrees.com/pricing). 

## Running examples with changes to Pipeline packages

A common use case is to make a change to the Pipeline logic in
device-detection-dotnet and then use these examples to observe the results of the
change.

By default, the examples are configured to use the packages from nuget feed.
In order to produce and use local packages instead:

- Clone and make your changes to device-detection-dotnet
- Create and install the packages to the local nuget feed
  `dotnet pack [Project] -o "[PackagesFolder]" /p:PackageVersion=0.0.0 -c [Configuration] /p:Platform=[Architecture]
  dotnet nuget push "[PackagesFolder]/*.nupkg" -s LocalFeed`
- Update the version of the device-detection-dotnet in the examples project:
   `dotnet add package "FiftyOne.DeviceDetection" --version 0.0.0 --source LocalFeed`

The same principle can be applied to incorporate changes in pipeline-dotnet if needed.


The tables below describe the examples available in this repository.

### Cloud

| Example                                | Description |
|----------------------------------------|-------------|
| GettingStarted-Console                 | How to use the 51Degrees Cloud service to determine details about a device based on its User-Agent and User-Agent Client Hints HTTP header values. |
| GettingStarted-Web                     | How to use the 51Degrees Cloud service to determine details about a device as part of a simple ASP.NET website. |
| Metadata                               | How to access the meta-data that relates to things like the properties populated device detection |
| TacLookup                              | How to get device details from a TAC (Type Allocation Code) using the 51Degrees cloud service. |
| NativeModelLookup                      | How to get device details from a native model name using the 51Degrees cloud service. |
| GetAllProperties                       | How to iterate through all properties available in the cloud response. |
| ClientHints NetCore 3.1                | Legacy example. Retained for the associated automated tests. See GettingStarted-Web instead. |
| ClientHints Not Integrated NetCore 3.1 | Legacy example. Our ASP.NET integration will automatically set the `Accept-CH` header that is used to request User-Agent Client Hints headers.This example demonstrates how to do this without using the integration component. |

### On-Premise

| Example                                | Description |
|----------------------------------------|-------------|
| GettingStarted-Console                 | How to use the 51Degrees on-premise device detection API to determine details about a device based on its User-Agent and User-Agent Client Hints HTTP header values. |
| GettingStarted-Web                     | How to use the 51Degrees Cloud service to determine details about a device as part of a simple ASP.NET website. |
| Metadata                               | How to access the meta-data that relates to things like the properties populated device detection |
| offline_processing                     | Example showing how to ingest a file containing data from web requests and perform detection against the entries. |
| Performance                            | How to configure the various performance options and run a simple performance test. |
| UpdateOnStartup                        | How to configure the Pipeline to automatically update the device detection data file on startup. |
| UpdatePollingInterval                  | Ho to configure and verify the various automatic data file update settings. |
| ClientHints NetCore 3.1                | Legacy example. Retained for the associated automated tests. See GettingStarted-Web instead. |
| ClientHints Not Integrated NetCore 3.1 | Legacy example. Our ASP.NET integration will automatically set the `Accept-CH` header that is used to request User-Agent Client Hints headers. This example demonstrates how to do this without using the integration component. |
