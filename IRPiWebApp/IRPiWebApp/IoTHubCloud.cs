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
		const string connectionString = "HostName=MainIoTHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=f4RkS/lSdFcJcSIoRqaKKKgWnGhlYOe5GSHORqLIZxA=";

        private static void CreateService() {
            if (serviceClient == null)
            {
                serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            }	
        }

		public static async Task InvokeTransmit(string deviceID, string irMessageCode)
		{
            CreateService();

            var methodInvocation = new CloudToDeviceMethod("Transmit") { ResponseTimeout = TimeSpan.FromSeconds(30) };
			methodInvocation.SetPayloadJson("'" + irMessageCode + "'");

            try
            {
                var response = await serviceClient.InvokeDeviceMethodAsync(deviceID, methodInvocation);
            }
            catch(Exception e)
            {

            }
		}

        public static async Task InvokeStartRecording(string deviceID)
        {
            CreateService();
            var methodInvocation = new CloudToDeviceMethod("StartRecording") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            var response = await serviceClient.InvokeDeviceMethodAsync(deviceID, methodInvocation);
        }

        public static async Task InvokeEndRecording(string deviceID, string productName, string actionName)
        {
            CreateService();
            var methodInvocation = new CloudToDeviceMethod("EndRecording") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            methodInvocation.SetPayloadJson("'" + productName + ";" + actionName + "'");
            var response = await serviceClient.InvokeDeviceMethodAsync(deviceID, methodInvocation);
        }
    }
}
