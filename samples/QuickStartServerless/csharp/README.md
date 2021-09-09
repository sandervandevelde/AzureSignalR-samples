# SignalR Service Serverless Quick Start (C#)

In this sample, we demonstrate how to broadcast messages with SignalR Service and Azure Function in serverless.

## Prerequisites

* Visual Studio or Visual Studio Code
* [Azure Function Core Tools](https://review.docs.microsoft.com/azure/azure-functions/functions-run-local?tabs=windows%2Ccsharp%2Cbash&branch=pr-en-us-162554#v2)
* [.NET](https://dotnet.microsoft.com/download)

## SignalR

We use SignalR Services to connect the HTML page logic to both the maps and the Azure function

*Note:* The SignalR Services resource must have the 'serverless' service mode!

Pricing is seen here: https://azure.microsoft.com/en-us/pricing/details/signalr-service/

This project is based on this example: https://github.com/aspnet/AzureSignalR-samples/tree/main/samples/QuickStartServerless/csharp

That original project was designed to be executed on a desktop machine.

To deploy this to Azure Functions in the cloud, just add the two application settings in your Azure Function App:

- AzureSignalRConnectionString : The connectionstring of your SignalR service
- ttn-ih-test-weu-ih_events_IOTHUB : This is the eventhub-compatible endpoint of your IoT Hub

Notice the use of an IoT Hub consumer group named 'afa'.


## AzureMap

An AzureMap resource is created to represent the location in the map.

The subscriptionKey eg. 'TTwUawjUKHW7cfBOeuldtsfHZNnQ7BW11AEZR4nEVo4' is 'hardcoded' in the HTML page. This code is simplified for demonstration purposes.

Check the [documentation](https://docs.microsoft.com/en-us/azure/azure-maps/azure-maps-authentication) on how to restrict access.

*Note*: The key shown in the sample code will be made obsolete once the code is made publicly available.

## Source

This sample is freely based on this [quick start](https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-quickstart-azure-functions-csharp).
