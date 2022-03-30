// https://www.mshowto.org/yazilim-gelistiriciler-icin-azure-azure-app-configuration-bolum-3.html

using Azure.Messaging.EventGrid;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Extensions;

var builder = new ConfigurationBuilder();
IConfigurationRefresher configurationRefresher = null;

builder.AddAzureAppConfiguration(options =>
{
    options.Connect("Endpoint=https://mshowto-appconfig.azconfig.io;Id=***;Secret=***")
           .ConfigureRefresh(refresh => refresh.Register("mshowtoconsoleapp:settings:key1"));

    configurationRefresher = options.GetRefresher();
});

var config = builder.Build();

var subscriptionClient = new SubscriptionClient(connectionString:"Endpoint=sb://mshowto-sbns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=***", 
                                                topicPath:"appconfig-topic", 
                                                subscriptionName:"appconfig-sub1");

subscriptionClient.RegisterMessageHandler(
    handler: (message, cancellationToken) =>
    {
        var eventGridEvent = EventGridEvent.Parse(BinaryData.FromBytes(message.Body));
        eventGridEvent.TryCreatePushNotification(out PushNotification pushNotification);
        configurationRefresher.ProcessPushNotification(pushNotification);

        return Task.CompletedTask;
    },
    exceptionReceivedHandler: (exceptionArgs) =>
    {
        Console.WriteLine($"{exceptionArgs.Exception}");
        return Task.CompletedTask;
    });

Task.Run(async () =>
{
    while (true)
    {
        Console.WriteLine($"Key1: {config["mshowtoconsoleapp:settings:key1"]}");

        await configurationRefresher.TryRefreshAsync();
        await Task.Delay(5000);
    }
}).Wait();