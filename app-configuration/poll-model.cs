// https://www.mshowto.org/yazilim-gelistiriciler-icin-azure-azure-app-configuration-bolum-2.html

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = new ConfigurationBuilder();
IConfigurationRefresher configurationRefresher = null;

builder.AddAzureAppConfiguration(options =>
{
    options.Connect("Endpoint=https://mshowto-appconfig.azconfig.io;Id=***;Secret=***")
           .ConfigureRefresh(refresh => refresh.Register("mshowtoconsoleapp:settings:key1"));

    configurationRefresher = options.GetRefresher();
});

var config = builder.Build();

Task.Run(async () =>
{
    while (true)
    {
        Console.WriteLine($"Key1: {config["mshowtoconsoleapp:settings:key1"]}");

        await configurationRefresher.TryRefreshAsync();
        await Task.Delay(5000);
    }
}).Wait();