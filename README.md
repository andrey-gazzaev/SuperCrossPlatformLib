# SuperCrossPlatformLib

This is a sample project that demonstrates how to use .NET 9.0 WASM  like JS libraries.

## Install

You need to install the .NET 9.0 SDK
and run next install `wasm-tools`


## How to run

```bash
  dotnet publish SuperCrossPlatformLib -c Release
```

After that, copy all files from `SuperCrossPlatformLib\wwwroot` to the `SuperCrossPlatformLib\bin\Release\net9.0\browser-wasm\AppBundle` folder. 

Now you can run the `index.html` file