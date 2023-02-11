# MobileDeviceSharp
## A C# object-oriented wrapper around Libimobiledevice
[Libmobiledevice](https://libimobiledevice.org/) is a cool project. It is useful to interact programmatically with Apple devices exactly like [iTunes](https://www.apple.com/itunes) do. It's a C Open source library. An Open source .NET bindings library exits on NuGet, and it's called [imobiledevice-net](https://www.nuget.org/packages/imobiledevice-net), but this library is just a binding and remains a 100 % functional programming API. So it does not respect C# language standards.
The objective of this project is to provide a fully object-oriented API for C# and .NET developers to interact with Apple device. It uses [Libimobiledevice](https://libimobiledevice.org) under the hood but try to use all the C# languages features to provide an easy-to-use API.
Somme examples:

- The `LockdownSession.ParAsync` API can be used to pair a device asynchronously.
- The `PlistDictionary` implement [IDictionary<String,PlistNode>](https://docs.microsoft.com/dotnet/api/system.collections.generic.idictionary-2) interface.
- The `AFCStream` class allow interacting with Apple device files using the `Apple file conduit` protocol with the standard .NET [Stream](https://docs.microsoft.com/dotnet/api/system.io.stream) API.
…

## Main API and associated libraries
We have created some libraries, indeed we wanted to separate the project into different libraries as much as possible (in the future we could try to make other separations if possible). In this part we will describe the content of the already implemented APIs (when it's possible a link with it's main associated native [Libmobiledevice](https://libimobiledevice.org/) library is provided)
### MobileDeviceSharp library
#### MobileDeviceSharp
This namespace contains the core APIs.
- `IDevice` (provide .NET API for [libimobiledevice.h](https://docs.libimobiledevice.org/libimobiledevice/latest/libimobiledevice_8h.html))
- `DeviceWatcher`
- `LockdownSession` (Provide .NET API for [lockdown.h](https://docs.libimobiledevice.org/libimobiledevice/latest/lockdown_8h.html))

- …

#### MobileDeviceSharp.Usbmuxd
- `UsbmuxdService`

#### MobileDeviceSharp.ProperyList
- `PlistDocument`
- `PlistDictionary`
- `PlistArray`
- …
#### MobileDeviceSharp.DiagnosticsRelay
> Provide .NET API for [diagnostics_relay.h](https://docs.libimobiledevice.org/libimobiledevice/latest/diagnostics__relay_8h.html)
- `DiagnosticRelaySession`
#### MobileDeviceSharp.NotificationProxy
> Provide .NET API for [notification_proxy.h](https://docs.libimobiledevice.org/libimobiledevice/latest/notification__proxy_8h.html)
- `NotificationProxySession`
### MobileDeviceSharp.AFC library
#### MobileDeviceSharp.AFC
> Provide .NET API for [afc.h](https://docs.libimobiledevice.org/libimobiledevice/latest/afc_8h.html)
- `AFCSession`
- `AFC2Session`
- `AFCFile`
- `AFCDirectory`
- `AFCStream`
- …

We also have `MobileDeviceSharp.SourceGenerator` and `MobileDeviceSharp.CompilerServices` to make programming these libraries easier using [Source Generators](https://docs.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview).
## Code of Conduct
Please note that this project is released with a [Contributor Code of Conduct.](CODE_OF_CONDUCT.md) By participating in this project you agree to abide by its terms.

## Contributing
We welcome contributions to the MobileDeviceSharp project. Please check out the [CONTRIBUTING.md](CONTRIBUTING.md) file for more information on how to get started.

Please note that currently, this project does not have any unit tests. So, we ask that contributors take extra care to ensure the stability and reliability of the code when making changes. Thank you!

## Conclusion
We are aware that there is still a lot of work to implement all the APIs proposed in the [Libmobiledevice](https://libimobiledevice.org/) library in a modern C# compliant way, but it is a good starting point, and we hope that this project can grow over time to make it easy to develop software that connects with Apple Devices in .NET.
