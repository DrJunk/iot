using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;

namespace IoT_Cloud
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=NumberAdder.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=ueimZaZ9d107AaCoBZ3d1vd+VM0d0vye93mjl70PWUM=";
        static string iotHubD2cEndpoint = "messages/events";

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);


            IRMessage newMessage = new IRMessage(new List<double>(new double[] { 1, 2, 1, 2}), true);
            string encoded = newMessage.Encode();
            Console.WriteLine("encoded: " + encoded);

            Console.WriteLine("Write a message!");
            string message = Console.ReadLine();
            SendCloudToDeviceMessageAsync(message).Wait();

            /////////////// Now read

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                var receiver = eventHubClient.GetDefaultConsumerGroup().
                    CreateReceiver(partition, DateTime.Now);
                ReceiveMessagesFromDeviceAsync(receiver);
            }
            Console.ReadLine();
        }

        async static Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received: '{0}'", data);
            }
        }

        private async static Task SendCloudToDeviceMessageAsync(string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await serviceClient.SendAsync("ourDeviceID", commandMessage);
        }
    }
}
