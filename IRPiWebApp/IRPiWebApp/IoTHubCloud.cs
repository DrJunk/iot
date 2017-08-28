using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;

namespace IRPiWebApp
{
    class IoTHubCloud
    {
        string connectionString;
        string iotHubD2cEndpoint;
        ServiceClient serviceClient;
        EventHubClient eventHubClient;

        public IoTHubCloud()
        {
            connectionString = "HostName=MainIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=f4RkS/lSdFcJcSIoRqaKKKgWnGhlYOe5GSHORqLIZxA=";
            iotHubD2cEndpoint = "messages/events";

            // for sending messages, connect to iot hub
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            // for receiving messages
            //eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

        }

        public async Task ListenForMessagesFromDeviceAsync()
        {
            string[] d2cPartitions;
            d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                var receiver = eventHubClient.GetDefaultConsumerGroup().
                    CreateReceiver(partition, DateTime.Now);
                ReceiveMessagesFromDeviceAsync(receiver);
            }
        }

        async Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                HandleData(data);
            }
        }

        public async Task SendCloudToDeviceMessageAsync(string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await serviceClient.SendAsync("MainDevice", commandMessage);
        }

        public void HandleData(string data)
        {
            if (data.StartsWith("irMSG:"))
            {
                IRMessage msg = new IRMessage(data.Substring("irMsg:".Length));
                Console.WriteLine("Recording received: '{0}'", msg.ToString());
            }
            else
            {
                Console.WriteLine("Message received: '{0}'", data);
            }
        }
    }
}
