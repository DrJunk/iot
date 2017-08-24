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
        static IoTHubCloud cloud;

        static void Main(string[] args)
        {
            cloud = new IoTHubCloud();
            Console.WriteLine("Welcome to Iot Azure Hub Portal Device Cloud!\n m for message, r for record");



            string message = Console.ReadLine();
            if (message.Equals("m"))
            {
                IRMessage newMessage = new IRMessage(new List<double>(new double[] { 1, 2, 1, 2, 3}), true);
                string encoded = newMessage.Encode();
                Console.WriteLine("sending to transmit: " + encoded);
                cloud.SendCloudToDeviceMessageAsync("transmit:" + message).Wait();
            }

            /////////////// Now read
            cloud.ListenForMessagesFromDeviceAsync();
            Console.ReadLine();
        }
    }
}
