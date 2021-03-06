﻿using System;
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

using Microsoft.IoT.Lightning.Providers;
using Windows.Devices.Pwm;
using Windows.Devices;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOT_HelloWorld
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int IR_LED_PIN = 4;
        private const int RCV_PIN = 18;
        private GpioPin irLED;
        private GpioPin irReceiver;
        private GpioChangeReader irReceiverReader;
        private DispatcherTimer timer;
        Stopwatch stopWatch;
        int flipsAmount;
        long oldFlipTime, currentFlipTime = 0;
        string debugString = "";
        string currentIRRawInput = "";
        Stopwatch watch = new Stopwatch();

        TimeSpan oldTime;

        public MainPage()
        {
            InitializeComponent();

            watch.Start();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();

            stopWatch = new Stopwatch();
            stopWatch.Start();
            Unloaded += MainPage_Unloaded;


            InitGPIO();
        }

        private async void InitGPIO()
        {
            if (LightningProvider.IsLightningEnabled)
            {
                LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();
                /*
                var pwmControllers = await PwmController.GetControllersAsync(LightningPwmProvider.GetPwmProvider());
                var pwmController = pwmControllers[1]; // the on-device controller
                pwmController.SetDesiredFrequency(40); // try to match 50Hz

                pwmLED = pwmController.OpenPin(IR_LED_PIN);
                pwmLED.SetActiveDutyCyclePercentage(0);
                pwmLED.Start();*/
            }

            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                irLED = null;
                debugString = "There is no GPIO controller on this device.\n";
                GpioStatus.Text = debugString;
                return;
            }

            irReceiver = gpio.OpenPin(RCV_PIN, GpioSharingMode.Exclusive);
            irReceiver.SetDriveMode(GpioPinDriveMode.InputPullUp);
            irReceiverReader = new GpioChangeReader(irReceiver);
            irReceiverReader.Start();
            irLED = gpio.OpenPin(IR_LED_PIN);
            
            irLED.Write(GpioPinValue.Low);
            irLED.SetDriveMode(GpioPinDriveMode.Output);
            
            //irReceiver.ValueChanged += OnChange;

            debugString = "GPIO pin initialized correctly.\n";
            GpioStatus.Text = debugString;

            flipsAmount = 0;
        }


        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                send0(0.7);
                send1(0.7);
            }
            for (int i = 0; i < 5; i++)
            {
                send0(2);
                send1(1);
            }

            irLED.Write(GpioPinValue.Low);
        }

        private void send1(double length)
        {
            double last = 0, current;
            double start = watch.Elapsed.TotalMilliseconds;
            last = watch.Elapsed.TotalMilliseconds;
            for (int i = 0; watch.Elapsed.TotalMilliseconds - start < length; i++)
            {
                current = watch.Elapsed.TotalMilliseconds;
                // Should be: 0.0131 for 38kHz.  But there is overhead so we acctualy measure a littel bit less
                while (current - last < 0.0085)
                {
                    current = watch.Elapsed.TotalMilliseconds;
                }
                if (i % 2 == 0)
                    irLED.Write(GpioPinValue.High);
                else
                    irLED.Write(GpioPinValue.Low);

                last = watch.Elapsed.TotalMilliseconds;
            }
        }

        private void send0(double length)
        {
            irLED.Write(GpioPinValue.Low);
            double last = 0, current;
            last = watch.Elapsed.TotalMilliseconds;
            current = watch.Elapsed.TotalMilliseconds;
            while (current - last < length)
            {
                current = watch.Elapsed.TotalMilliseconds;
            }
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
        }


        private void MainPage_Unloaded(object sender, object args)
        {
            irLED.Dispose();
            irReceiver.Dispose();
        }

        /*
        private void OnChange(object sender, object args)
        {
            currentFlipTime = stopWatch.ElapsedTicks;

            if (flipsAmount % 2 == 1)
            {
                if ((currentFlipTime - oldFlipTime) < 16000)
                    currentIRRawInput += "0";
                else if ((currentFlipTime - oldFlipTime) >= 16000)
                    currentIRRawInput += "1";
                else
                    currentIRRawInput += "?";
            }

            flipsAmount++;
            if (flipsAmount % 100 == 0)
            {
                //currentIRRawInput += "\n";
            }

            if ((currentFlipTime - oldFlipTime) >= 1000000)
            {
                debugString += parseIRInput(currentIRRawInput);
                debugString += currentIRRawInput+"\n";
                currentIRRawInput = "";
                flipsAmount = 0;
            }

            oldFlipTime = currentFlipTime;
        }
        */

        private string parseIRInput(string input)
        {
            if (input.StartsWith("100010000111011111010000001011111"))
                return "Up\n";
            if (input.StartsWith("100010000111011110000000011111111"))
                return "Down\n";
            if (input.StartsWith("100010000111011110001000011101111"))
                return "Left\n";
            if (input.StartsWith("100010000111011111000000001111111"))
                return "Right\n";
            if (input.StartsWith("100010000111011111101100000100111"))
                return "Off\n";
            if (input.StartsWith("100010000111011111111100000000111"))
                return "A\n";
            if (input.StartsWith("100010000111011110111100010000111"))
                return "B\n";
            if (input.StartsWith("100010000111011110101100010100111"))
                return "C\n";
            if (input.StartsWith("100010000111011110010000011011111"))
                return "Please don't press this button....\n";
            return "";
        }

        private void UpdateDebugString()
        {
            GpioStatus.Text = debugString;
        }

        private void Timer_Tick(object sender, object e)
        {
            while (!irReceiverReader.IsEmpty)
                HandleChange(irReceiverReader.GetNextItem());
            UpdateDebugString();
        }

        private void HandleChange(GpioChangeRecord changeRecord)
        {
            long ticksPassed = (changeRecord.RelativeTime - oldTime).Ticks;


            if (oldTime != null) {
                //debugString += ticksPassed + " ";
                debugString += (changeRecord.RelativeTime - oldTime).TotalMilliseconds + "," + ticksPassed + " ";
            }

            //if (flipsAmount % 2 == 1)
            {
                if (ticksPassed < 16000)
                    currentIRRawInput += "0";
                else if (ticksPassed >= 16000)
                    currentIRRawInput += "1";
                else
                    currentIRRawInput += "?";
            }

            flipsAmount++;
            if (flipsAmount % 100 == 0)
            {
                //currentIRRawInput += "\n";
            }

            if (ticksPassed >= 1000000)
            {
                debugString += parseIRInput(currentIRRawInput);
                debugString += currentIRRawInput + "\n";
                currentIRRawInput = "";
                flipsAmount = 0;
            }

            oldTime = changeRecord.RelativeTime;
        }
    }
}