using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;
using System.Collections.Generic;

namespace IRPiWebApp
{
    class IoTHubCloud
    {
		static ServiceClient serviceClient;
        static string deviceID = "MainDevice";
		const string connectionString = "HostName=MainIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=f4RkS/lSdFcJcSIoRqaKKKgWnGhlYOe5GSHORqLIZxA=";

        private static void createService() {
            if (serviceClient == null)
            {
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            }	
        }

		public static async Task InvokeMethod(string methodName)
		{
            createService();
            var methodInvocation = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(30) };
			
            // temp
            IRMessage msg = new IRMessage(new List<double>(new double[] { 1000, 2000, 1000, 2000, 3000 }), true);

			methodInvocation.SetPayloadJson("'" + msg.Encode() + "'");

			var response = await serviceClient.InvokeDeviceMethodAsync(deviceID, methodInvocation);

			Console.WriteLine("Response status: {0}, payload:", response.Status);
			Console.WriteLine(response.GetPayloadAsJson());
		}
	}
}
