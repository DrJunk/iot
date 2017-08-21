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
        string buttonText = "Button";

        public MainPage()
        {
            this.InitializeComponent();

            AzureIoTHub.Init();
            //AzureIoTHub.SendDeviceToCloudMessageAsync();
            Task<string> task = AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
            task.ContinueWith(x => HandleMsg(x.Result));
        }

        public void HandleMsg(string message)
        {
            buttonText = message;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageButton.Content = buttonText;
            AzureIoTHub.SendDeviceToCloudMessageAsync("I am Laplace, and I got " + buttonText);
        }
    }
}
