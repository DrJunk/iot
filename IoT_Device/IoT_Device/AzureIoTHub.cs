using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace IoT_Device
{
    class AzureIoTHub
    {
        private static void CreateClient()
        {
            if (deviceClient == null)
            {
                // create Azure IoT Hub client from embedded connection string
                deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
            }
        }

        static DeviceClient deviceClient = null;

        //
        // Note: this connection string is specific to the device "MainDevice". To configure other devices,
        // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
        //
        const string deviceConnectionString = "HostName=MainIoTHub.azure-devices.net;DeviceId=MainDevice;SharedAccessKey=c76PJgKGJQ4fkWJxrvexsHQUs08IxR3ufSaWs/dBMDw=";

        //
        // To monitor messages sent to device "kraaa" use iothub-explorer as follows:
        //    iothub-explorer monitor-events --login HostName=MainIoTHub.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=ux6FNXUNTEm5Eq3e22z2K2TzT1qX0GtHahialxtaz5Q= "MainDevice"
        //

        // Refer to http://aka.ms/azure-iot-hub-vs-cs-2017-wiki for more information on Connected Service for Azure IoT Hub

        public static async Task SendDeviceToCloudMessageAsync(string str)
        {
            CreateClient();
            var message = new Message(Encoding.ASCII.GetBytes(str));
            await deviceClient.SendEventAsync(message);
        }

        public static async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            CreateClient();

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
        }

        private static Task<MethodResponse> OnTransmitCalled(MethodRequest methodRequest, object userContext)
        {
            IRMessage msg = new IRMessage(methodRequest.DataAsJson.Substring(1, methodRequest.DataAsJson.Length - 2));
            IRControl.Transmit(msg);
            string result = "'Transmitted IR signal'";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        private static Task<MethodResponse> OnStartRecordingCalled(MethodRequest methodRequest, object userContext)
        {
            IRControl.StartRecording();
            string result = "'Started Recording'";
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        private static Task<MethodResponse> OnEndRecordingCalled(MethodRequest methodRequest, object userContext)
        {
            try
            {
                IRMessage msg = IRControl.EndRecording();
                if(msg == null)
                    return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("'The IR reciver did not receive a message'"), 500));
                string result = "'" + msg.Encode() + "'";
                SendDeviceToCloudMessageAsync(methodRequest.DataAsJson + ";" + msg.Encode()).Wait();
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            catch(Exception e)
            {
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes("'Error in OnEndRecordingCalled:" + e.Message + "'"), 500));
            }
        }

        public static async Task RegisterDirectMethodsAsync()
        {
            CreateClient();
            // setup callback for methods
            await deviceClient.SetMethodHandlerAsync("StartRecording", OnStartRecordingCalled, null);
            await deviceClient.SetMethodHandlerAsync("EndRecording", OnEndRecordingCalled, null);
            await deviceClient.SetMethodHandlerAsync("Transmit", OnTransmitCalled, null);
        }
    }
}