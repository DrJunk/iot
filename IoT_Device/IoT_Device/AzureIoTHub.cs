using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;


class AzureIoTHub
{
    //
    // Note: this connection string is specific to the device "ourDeviceID". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "HostName=NumberAdder.azure-devices.net;DeviceId=ourDeviceID;SharedAccessKey=OHjmQWpHi0LFaiSHCKmNy1w9oZuwsMf8aw1oA7KMURo=";

    //
    // To monitor messages sent to device "ourDeviceID" use iothub-explorer as follows:
    //    iothub-explorer monitor-events --login HostName=NumberAdder.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=zD23fJNix+kLl2+SEJFvWBb/UQliKuxvslZYCByx6nA= "ourDeviceID"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

    DeviceClient deviceClient;

    public AzureIoTHub()
    {
        deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

    }

    public async Task SendDeviceToCloudMessageAsync(string message)
    {
        var msg = new Message(Encoding.ASCII.GetBytes(message));

        await deviceClient.SendEventAsync(msg);
    }

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
    }
}
