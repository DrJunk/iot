using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Gpio;
using System.Diagnostics;

namespace IoT_Device
{
    class IRControl
    {
        static string debugString;

        const int IR_LED_PIN = 4;
        const int IR_RCV_PIN = 26;
        const int STATUS_LED_PIN = 17;
        const int BTN_PIN = 22;

        static GpioController gpio;
        static GpioPin irLED;
        static GpioPin irReceiver;
        static GpioPin statusLED;
        static GpioChangeReader irReceiverReader;

        static Stopwatch stopwatch;

        public static void Init()
        {
            debugString = "";
            stopwatch = new Stopwatch();
            InitGPIO();
        }

        public static void Close()
        {
            irLED.Dispose();
            irReceiver.Dispose();
        }

        private static void InitGPIO()
        {
            gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                irLED = null;
                debugString = "There is no GPIO controller on this device.\n";
                return;
            }

            irReceiver = gpio.OpenPin(IR_RCV_PIN, GpioSharingMode.Exclusive);
            irReceiver.SetDriveMode(GpioPinDriveMode.InputPullUp);
            irReceiverReader = new GpioChangeReader(irReceiver);
            irReceiverReader.Polarity = GpioChangePolarity.Both;

            irLED = gpio.OpenPin(IR_LED_PIN); 
            irLED.Write(GpioPinValue.Low);
            irLED.SetDriveMode(GpioPinDriveMode.Output);

            statusLED = gpio.OpenPin(STATUS_LED_PIN);
            statusLED.Write(GpioPinValue.Low);
            statusLED.SetDriveMode(GpioPinDriveMode.Output);

            debugString = "GPIO pin initialized correctly.\n";
        }

        public static void Transmit(IRMessage message) {
            statusLED.Write(GpioPinValue.High);
            stopwatch.Start();
            bool flag = true;
            foreach (double d in message.intervalList)
            {
                if (flag)
                    Send1(d);
                else
                    Send0(d);
                flag = !flag;
            }
            stopwatch.Stop();
            statusLED.Write(GpioPinValue.Low);
        }

        public static void StartRecording()
        {
            statusLED.Write(GpioPinValue.High);
            irReceiverReader.Start();
        }

        public static IRMessage EndRecording() {
            double newTime, oldTime;
            List<double> intervalList = new List<double>();
            GpioChangeRecord changeRecord;

            statusLED.Write(GpioPinValue.Low);

            irReceiverReader.Stop();

            if (irReceiverReader.IsEmpty)
                return null;

            changeRecord = irReceiverReader.GetNextItem();
            oldTime = changeRecord.RelativeTime.TotalMilliseconds;

            while (!irReceiverReader.IsEmpty)
            {
                changeRecord = irReceiverReader.GetNextItem();
                newTime = changeRecord.RelativeTime.TotalMilliseconds;
                intervalList.Add(newTime - oldTime);
                oldTime = newTime;
            }

            if (!intervalList.Any())
                return null;
            return new IRMessage(intervalList);
        }

        private static void Send0(double length)
        {
            irLED.Write(GpioPinValue.Low);
            double last = 0, current;
            last = stopwatch.Elapsed.TotalMilliseconds;
            current = stopwatch.Elapsed.TotalMilliseconds;
            while (current - last < length)
            {
                current = stopwatch.Elapsed.TotalMilliseconds;
            }
        }

        private static void Send1(double length)
        {
            irLED.Write(GpioPinValue.High);
            double last = 0, current;
            last = stopwatch.Elapsed.TotalMilliseconds;
            current = stopwatch.Elapsed.TotalMilliseconds;
            while (current - last < length)
            {
                current = stopwatch.Elapsed.TotalMilliseconds;
            }
            irLED.Write(GpioPinValue.Low);
        }
    }
}