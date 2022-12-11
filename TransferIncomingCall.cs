// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using Azure.Communication.CallAutomation;
using Azure.Communication;

namespace ACSBuzzer
{
    public class TransferIncomingCall
    {
        private readonly ILogger _logger;

        public TransferIncomingCall(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TransferIncomingCall>();
        }

        [Function("TransferIncomingCall")]
        public void Run([EventGridTrigger] MyEvent input)
        {
            _logger.LogInformation(input.Data.ToString());

            var dataStr = input.Data.ToString();

            if (dataStr == null || dataStr.Length == 0)
            { 
                return;
            }

            var inputObject = JsonNode.Parse(dataStr).AsObject();
            var incomingCallContext = (string)inputObject["incomingCallContext"];

            var client = new CallAutomationClient(Environment.GetEnvironmentVariable("ACSConnectionString"));
            client.RedirectCall(incomingCallContext, new PhoneNumberIdentifier(Environment.GetEnvironmentVariable("MyPhoneNumber")));
            _logger.LogInformation("Redirect successfull");
        }
    }

    public class MyEvent
    {
        public string Id { get; set; }

        public string Topic { get; set; }

        public string Subject { get; set; }

        public string EventType { get; set; }

        public DateTime EventTime { get; set; }

        public object Data { get; set; }
    }
}
