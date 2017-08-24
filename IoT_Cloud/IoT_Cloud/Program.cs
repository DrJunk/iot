using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoT_Cloud
{
    class Program
    {
        static IoTHubCloud cloud;

        static void Main(string[] args)
        {
            cloud = new IoTHubCloud();
            Task t = cloud.ListenForMessagesFromDeviceAsync();

            Console.WriteLine("Welcome to Iot Azure Hub Portal Device Cloud!\n" +
                "Commands: 'transmit', 'startRecording', 'endRecording', 'exit'");

            string message = Console.ReadLine();
            while (!message.Equals("exit"))
            {
                if (message.Equals("transmit")) // this is temporary
                {
                    IRMessage newMessage = new IRMessage(new List<double>(new double[] { 1, 2, 1, 2, 3 }), true);
                    string encoded = newMessage.Encode();
                    message = "transmit:" + encoded;
                }
                Console.WriteLine("sending: " + message);
                cloud.SendCloudToDeviceMessageAsync(message).Wait();
      
                message = Console.ReadLine();
            }
        }
    }
}