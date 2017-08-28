using System;
using System.Text;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoT_Device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IoTHubDevice hub;
        IRControl irc;
        static IRMessage recordedByButton;

        static string buttonString = "Message";
        static string deviceConnectionString = "HostName=MainIoTHub.azure-devices.net;DeviceId=MainDevice;SharedAccessKey=c76PJgKGJQ4fkWJxrvexsHQUs08IxR3ufSaWs/dBMDw=";
        static DeviceClient Client = null;

        public MainPage()
        {
            this.InitializeComponent();

            irc = new IRControl();
            //hub = new IoTHubDevice();
            Unloaded += MainPage_Unloaded;
            //Console.WriteLine("WTF");

            /*
            Task<string> task = hub.ReceiveCloudToDeviceMessageAsync();
            task.ContinueWith(x => HandleMessage(x.Result));
            */
            try
            {
                //Console.WriteLine("Connecting to hub");
                Client = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

                // setup callback for "writeLine" method
                Client.SetMethodHandlerAsync("writeLine", WriteLineToConsole, null);
                //Console.WriteLine("Waiting for direct method call\n Press enter to exit.");

                /*
                // as a good practice, remove the "writeLine" handler
                Client.SetMethodHandlerAsync("writeLine", null, null).Wait();
                Client.CloseAsync().Wait();
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        /* this is how we boogie (wait)
        public async void Test()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            MessageButton.Content = "wuuttt";
        }*/


        static Task<MethodResponse> WriteLineToConsole(MethodRequest methodRequest, object userContext)
        {
            buttonString += "\n" + methodRequest.DataAsJson + "\n Returning response for method " + methodRequest.Name;
            string result = "'Input was written to log.'";
            recordedByButton= new IRMessage(methodRequest.DataAsJson.Substring(1, methodRequest.DataAsJson.Length-2));
            return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
        }

        public void HandleMessage(string message)
        {
            buttonString = message;
            IRMessage irMsg;
            Task<string> task;
            if (message.StartsWith("transmit:"))
            {
                irMsg = new IRMessage(message.Substring("transmit:".Length));
                irc.Transmit(irMsg);
                hub.SendDeviceToCloudMessageAsync("OK");
            }
            else if (message.StartsWith("startRecording"))
            {
                irc.StartRecording();
                hub.SendDeviceToCloudMessageAsync("OK");
            }
            else if (message.StartsWith("endRecording"))
            {
                irMsg = irc.EndRecording();
                hub.SendDeviceToCloudMessageAsync("irMsg:" + irMsg.Encode());
            }
            else
            {
                hub.SendDeviceToCloudMessageAsync("wtf do you want");
            }
            task = hub.ReceiveCloudToDeviceMessageAsync();
            task.ContinueWith(x => HandleMessage(x.Result));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            messageButton.Content = buttonString;
            //hub.SendDeviceToCloudMessageAsync("I am Laplace, and I got " + messageButton.Content.ToString());
        }

        private void Button_Record(object sender, RoutedEventArgs e)
        {
            if (recordButton.Content.ToString().Equals("Start Recording"))
            {
                irc.StartRecording();
                recordButton.Content = "Stop Recording";
            }
            else
            {
                IRMessage msg = irc.EndRecording();
                recordButton.Content = "Start Recording";
                if (msg != null)
                {
                    mainText.Text = msg.ToString() + "\n";
                    mainText.Text += msg.ParseToBits() + "\n" + msg.ParseToRemoteButton() + "\n";
                }

                else
                    mainText.Text = "Nothing was recorded.";
                recordedByButton = msg;
            }
        }

        private void Button_Transmit(object sender, RoutedEventArgs e)
        {
            if(recordedByButton != null)
                irc.Transmit(recordedByButton);
            
            //IRMessage newMessage = new IRMessage(new List<double>(new double[] { 1, 0.5, 1, 0.7, 1.3 }), true);
           // irc.Transmit(newMessage);
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            irc.Close();
        }
    }
}
