﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;


namespace IoT_Device
{
    class IoTHubDevice
    {
        /*
        const string deviceConnectionString = "HostName=MainIoTHub.azure-devices.net;DeviceId=MainDevice;SharedAccessKey=c76PJgKGJQ4fkWJxrvexsHQUs08IxR3ufSaWs/dBMDw=";
        string iotHubUri; 
        string deviceId;
        string deviceKey;
        DeviceClient deviceClient;

        public IoTHubDevice()
        {
            //deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
            iotHubUri = "MainIoTHub.azure-devices.net";
            deviceId = "MainDevice";
            deviceKey = "c76PJgKGJQ4fkWJxrvexsHQUs08IxR3ufSaWs/dBMDw=";
            deviceClient = DeviceClient.Create(iotHubUri,
            AuthenticationMethodFactory.
                CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey),
            TransportType.Http1);
        }

        public async void SendDeviceToCloudMessageAsync(string message)
        {
            var msg = new Message(Encoding.ASCII.GetBytes(message));
            await deviceClient.SendEventAsync(msg);
        }


        /*
        public async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }*/

        static string deviceConnectionString = "HostName=MainIoTHub.azure-devices.net;DeviceId=MainDevice;SharedAccessKey=c76PJgKGJQ4fkWJxrvexsHQUs08IxR3ufSaWs/dBMDw=";
        static DeviceClient Client = null;

        private static void createClient()
        {
            Client = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

            // setup callback for methods
            Client.SetMethodHandlerAsync("writeLine", WriteLineToConsole, null);
            Client.SetMethodHandlerAsync("transmit", IRControl.Transmit, null);
        }

	    public static Task<MethodResponse> WriteLineToConsole(MethodRequest methodRequest, object userContext)
	    {
	        buttonString += "\n" + methodRequest.DataAsJson + "\n Returning response for method " + methodRequest.Name;
	        string result = "'Input was written to log.'";
	        recordedByButton = new IRMessage(methodRequest.DataAsJson.Substring(1, methodRequest.DataAsJson.Length - 2));
	        return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
	    }

	    public static deleteClient()
	    {
	        Client.SetMethodHandlerAsync("writeLine", null, null).Wait();
	        Client.CloseAsync().Wait();
	    }
	}
}
