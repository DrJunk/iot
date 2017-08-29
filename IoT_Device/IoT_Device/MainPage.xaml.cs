using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IoT_Device
{
    public sealed partial class MainPage : Page
    {
        static IRMessage recordedByButton;
        static string debugString = "Message";

        public MainPage()
        {
            this.InitializeComponent();
            IRControl.Init();
            AzureIoTHub.RegisterDirectMethodsAsync();

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
            messageButton.Content = debugString;
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
                IRMessage msg = IRControl.EndRecording();
                recordButton.Content = "Start Recording";
                if (msg != null)
                {
                    mainText.Text = msg.ToString() + "\n";
                    mainText.Text += msg.ParseToBits() + "\n";
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
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            IRControl.Close();
        }
    }
}
