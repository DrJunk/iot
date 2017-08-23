using System;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoT_Device
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AzureIoTHub hub;
        IRControl irc;
        IRMessage recorded;

        public MainPage()
        {
            this.InitializeComponent();

            irc = new IRControl();
            hub = new AzureIoTHub();
            Unloaded += MainPage_Unloaded;



            /* this code gave me cancer
            Task<string> task = hub.ReceiveCloudToDeviceMessageAsync();
            task.ContinueWith(x => HandleMessage(x.Result));
            */

        }

        /* this is how we boogie
        public async void Test()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            MessageButton.Content = "wuuttt";
        }*/

        public void HandleMessage(string message)
        {
            messageButton.Content = message;
            /*
            Task<string> task = hub.ReceiveCloudToDeviceMessageAsync();
            task.ContinueWith(x => HandleMsg(x.Result));*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task t = hub.SendDeviceToCloudMessageAsync("I am Laplace, and I got " + messageButton.Content.ToString());
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
                mainText.Text = msg.ToString() + "\n";
                mainText.Text += msg.ParseToBits() + "\n" + msg.ParseToRemoteButton() + "\n";
                recorded = msg;
            }
        }

        private void Button_Transmit(object sender, RoutedEventArgs e)
        {
            irc.Transmit(recorded);
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            irc.Close();
        }
    }
}
