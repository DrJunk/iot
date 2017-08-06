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
using Windows.Devices.Gpio;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOT_HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 4;
        private const int PB_PIN = 18;
        private GpioPin pin;
        private GpioPin pushButton;
        private DispatcherTimer timer;
        private GpioPinValue pushButtonValue;
        Stopwatch stopWatch;
        int flips;
        long old,current = 0;
        string times = "";

        public MainPage()
        {
            InitializeComponent();
            
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();

            stopWatch = new Stopwatch();
            stopWatch.Start();

            Unloaded += MainPage_Unloaded;

            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                pin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            pushButton = gpio.OpenPin(PB_PIN);
            pin = gpio.OpenPin(LED_PIN);

            pushButton.SetDriveMode(GpioPinDriveMode.Input);
            pin.Write(GpioPinValue.Low);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            
            pushButton.ValueChanged += OnChange;

            GpioStatus.Text = "GPIO pin initialized correctly.";

            flips = 0;
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            pin.Dispose();
            pushButton.Dispose();
        }

        private void OnChange(object sender, object args)
        {
            current = stopWatch.ElapsedTicks;

            if (flips % 2 == 1)
            {
                if ((current - old) < 16000)
                    times += "0";
                else if ((current - old) >= 16000)
                    times += "1";
                else
                    times += "?";
            }
            //times += " " + (current - old);
            flips++;
            if (flips % 16 == 0)
                times += "\n";

            if ((current - old) >= 1000000)
            {
                times += "\n";//\n";
                flips = 0;
            }

            old = current;
        }

        private void FlipLED()
        {
            //if (pushButtonValue != pushButton.Read())
            pushButtonValue = pushButton.Read();

            if (pushButtonValue == GpioPinValue.High)
            {
                pin.Write(GpioPinValue.Low);
            }
            else if (pushButtonValue == GpioPinValue.Low)
            {
                pin.Write(GpioPinValue.High);
            }

            GpioStatus.Text = times;
        }

        private void Timer_Tick(object sender, object e)
        {
            FlipLED();
        }
    }
}
