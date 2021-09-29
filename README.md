## NetProxy
 
[![CircleCI](https://circleci.com/gh/exeal-es/NetProxy/tree/main.svg?style=svg&circle-token=9434f71d7bf6f2a7d8d87516ce6c8ba3de6a7859)](https://circleci.com/gh/exeal-es/NetProxy/tree/main)
[![CodeFactor](https://www.codefactor.io/repository/github/exeal-es/netproxy/badge?s=9593bc70cc1c793dc13bc3e695721acbe99068e2)](https://www.codefactor.io/repository/github/exeal-es/netproxy)

NetCore proxy middleware to support [Strangler fig pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/strangler-fig)

## :pencil: Usage

First, you must add the reverse proxy middleware in your StartUp.cs.

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseReverseProxyMiddleware();
    ...
}
```

And also you need to add destination proxy url in your proxy api appsettings.json like this:

```
"ReverseProxy": {
    "DestinationURL": "http://localhost:5001"
  }
```

## :pick: Built Using

- [netcore](https://dotnet.microsoft.com/download)

## :balance_scale: License

MIT
