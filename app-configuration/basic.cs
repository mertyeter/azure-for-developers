// https://www.mshowto.org/yazilim-gelistiriciler-icin-azure-azure-app-configuration-bolum-1.html

using Microsoft.Extensions.Configuration;
 
var builder = new ConfigurationBuilder();
builder.AddAzureAppConfiguration("Endpoint=https://mshowto-appconfig.azconfig.io;Id=***;Secret=***");
var config = builder.Build();
 
Console.WriteLine($"Key1: {config["mshowtoconsoleapp:settings:key1"]}");