﻿## Exeal.StranglerProxy

[![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://GitHub.com/Naereen/StrapDown.js/graphs/commit-activity)
[![NuGet version](https://img.shields.io/nuget/v/Exeal.StranglerProxy.svg)](https://www.nuget.org/packages/Exeal.StranglerProxy)
[![CircleCI](https://circleci.com/gh/exeal-es/StranglerProxy/tree/main.svg?style=svg&circle-token=9434f71d7bf6f2a7d8d87516ce6c8ba3de6a7859)](https://circleci.com/gh/exeal-es/StranglerProxy/tree/main)
[![CodeFactor](https://www.codefactor.io/repository/github/exeal-es/stranglerproxy/badge?s=e7bc88343e337a93bb31f0823cf4c3721de6ae6b)](https://www.codefactor.io/repository/github/exeal-es/stranglerproxy)

Net Core proxy middleware to support [Strangler fig pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/strangler-fig)

## :zap: How does it works?
![strangler-fig-proxy](https://user-images.githubusercontent.com/7398909/135844331-887d3fc1-cd15-44f0-9b26-8fa666a01192.jpg)

## :pencil: Usage

First, you must add the strangler proxy middleware in your `Startup.cs`.

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseStranglerProxyMiddleware();
    ...
}
```

And also you need to add destination proxy url in your proxy api `appsettings.json` like this:

```
"StranglerProxy": {
  "DestinationURL": "http://localhost:5001",
  "ExcludedResources": [ ]
}
```
You can also filter out certain resources so the proxy would not try to match those and always go to the destination URL

```
"StranglerProxy": {
  "DestinationURL": "http://localhost:5001",
  "ExcludedResources": [ "test13", "Test14/Hi" ]
}
```

## :pick: Built Using

- [netcore](https://dotnet.microsoft.com/download)

## :balance_scale: License

[MIT](https://github.com/exeal-es/StranglerProxy/blob/main/LICENSE)

## Maintainers

* [Exeal Solutions S.L.](https://www.exeal.com)

See also the list of [contributors](https://github.com/exeal-es/StranglerProxy/contributors) who participated in this project.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/exeal-es/StranglerProxy/tags).
