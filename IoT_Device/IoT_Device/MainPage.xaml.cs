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
        static IRMessage recordedByButton;
        static string buttonString = "Message";

        public MainPage()
        {
            this.InitializeComponent();

            Unloaded += MainPage_Unloaded;

            /*
            Task<string> task = hub.ReceiveCloudToDeviceMessageAsync();
            task.ContinueWith(x => HandleMessage(x.Result));
            */

        }

        /* this is how we boogie (wait)
        public async void Test()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            MessageButton.Content = "wuuttt";
        }*/

        /*
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
        }*/

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            messageButton.Content = buttonString;
            //hub.SendDeviceToCloudMessageAsync("I am Laplace, and I got " + messageButton.Content.ToString());
        }

        private void Button_Record(object sender, RoutedEventArgs e)
        {
            if (recordButton.Content.ToString().Equals("Start Recording"))
            {
                IRControl.StartRecording();
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
                IRControl.Transmit(recordedByButton);
            
            //IRMessage newMessage = new IRMessage(new List<double>(new double[] { 1, 0.5, 1, 0.7, 1.3 }), true);
           // irc.Transmit(newMessage);
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            IRControl.Close();
            IoTHubDevice.deleteClient();
        }
    }
}
