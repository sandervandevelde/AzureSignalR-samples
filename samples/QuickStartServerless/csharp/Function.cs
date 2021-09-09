using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Newtonsoft.Json;

using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json.Linq;

//using System.Net.Http;

namespace CSharp
{
    /// <summary>
    /// https://azure.microsoft.com/en-us/pricing/details/signalr-service/
    ///
    /// https://github.com/aspnet/AzureSignalR-samples/tree/main/samples/QuickStartServerless/csharp
    /// </summary>

    public static class Function
    {
        [FunctionName("index")]
        public static IActionResult GetHomePage([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, ExecutionContext context)
        {
            var path = Path.Combine(context.FunctionAppDirectory, "content", "index.html");
            return new ContentResult
            {
                Content = File.ReadAllText(path),
                ContentType = "text/html",
            };
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "serverless")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("broadcast")]
        public static async Task Broadcast(
            [EventHubTrigger("dummy", ConsumerGroup = "afa", Connection = "ttn-ih-test-weu-ih_events_IOTHUB")] EventData message,
            ILogger log,
            [SignalR(HubName = "serverless")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            dynamic data = JObject.Parse(Encoding.UTF8.GetString(message.Body.Array));

            var webMessage = new WebMessage();
            webMessage.deviceId = (string)data.end_device_ids.device_id;
            webMessage.timestamp = DateTime.Parse((string)data.received_at).ToUniversalTime();

            if (data != null
                    && data.uplink_message != null
                    && data.uplink_message.decoded_payload != null
                    && data.uplink_message.decoded_payload.latitude != null
                    && data.uplink_message.decoded_payload.longitude != null
                    && data.uplink_message.decoded_payload.battery != null)
            {
                webMessage.battery = (string)data.uplink_message.decoded_payload.battery;
                webMessage.latitude = (decimal)data.uplink_message.decoded_payload.latitude;
                webMessage.longitude = (decimal)data.uplink_message.decoded_payload.longitude;

                log.LogInformation($"MESSAGE SENT: DeviceId: {webMessage.deviceId} - timestamp: {webMessage.timestamp} - lat: {webMessage.latitude} - lon: {webMessage.longitude} ");

                var json = JsonConvert.SerializeObject(webMessage);

                await Task.Delay(1);
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "gpsMessage",
                        Arguments = new[] { json }
                    });
            }
            else
            {
                //log.LogInformation($"NULL ENCOUNTERED {myIoTHubMessage}");
            }
        }

        public class WebMessage
        {
            public string deviceId { get; set; }
            public DateTime timestamp { get; set; }
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }
            public string battery { get; set; }
        }
    }
}