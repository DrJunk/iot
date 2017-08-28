using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace IoT_Cloud
{
    class Program
    {
        static IoTHubCloud cloud;
        static ServiceClient serviceClient;
        static string connectionString = "HostName=MainIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=f4RkS/lSdFcJcSIoRqaKKKgWnGhlYOe5GSHORqLIZxA=";
            
        static void Main(string[] args)
        {
            //cloud = new IoTHubCloud();
            //Task t = cloud.ListenForMessagesFromDeviceAsync();


            Console.WriteLine("Welcome to Iot Azure Hub Portal Device Cloud!\n" +
                "Commands: 'transmit', 'startRecording', 'endRecording', 'exit'");

            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            InvokeMethod().Wait();
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            /*string message = Console.ReadLine();
            while (!message.Equals("exit"))
            {
                if (message.Equals("transmit")) // this is temporary
                {
                    IRMessage newMessage = new IRMessage(new List<double>(new double[] { 1000, 2000, 1000, 2000, 3000 }), true);
                    string encoded = newMessage.Encode();
                    message = "transmit:" + encoded;
                }
                Console.WriteLine("sending: " + message);
                cloud.SendCloudToDeviceMessageAsync(message).Wait();
      
                message = Console.ReadLine();
            }*/

        }
        private static async Task InvokeMethod()
        {
            var methodInvocation = new CloudToDeviceMethod("writeLine") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            methodInvocation.SetPayloadJson("'a line to be written'");

            var response = await serviceClient.InvokeDeviceMethodAsync("MainDevice", methodInvocation);

            Console.WriteLine("Response status: {0}, payload:", response.Status);
            Console.WriteLine(response.GetPayloadAsJson());
        }
    }
}