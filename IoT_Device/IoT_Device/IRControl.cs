using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Pwm;
using Windows.Devices;
using Windows.Devices.Gpio;
using System.Diagnostics;

namespace IoT_Device
{
    class IRControl
    {
        string debugString;

        private const int IR_LED_PIN = 4;
        private const int IR_RCV_PIN = 18;

        private GpioController gpio;
        private GpioPin irLED;
        private GpioPin irReceiver;
        private GpioChangeReader irReceiverReader;

        private Stopwatch stopwatch;

        public IRControl()
        {
            debugString = "";
            stopwatch = new Stopwatch();
            InitGPIO();
        }

        public void Close()
        {
            irLED.Dispose();
            irReceiver.Dispose();
        }

        private void InitGPIO()
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

            //irReceiver.ValueChanged += OnChange;

            debugString = "GPIO pin initialized correctly.\n";
            //flipsAmount = 0;
        }

        public void Transmit(IRMessage message) {
            stopwatch.Start();
            bool flag = message.startingState;
            foreach (double d in message.intervalList)
            {
                if (flag)
                    Send1(d);
                else
                    Send0(d);
                flag = !flag;
            }
            stopwatch.Stop();
        }

        public void StartRecording()
        {
            irReceiverReader.Start();
        }

        public IRMessage EndRecording() {
            double newTime, oldTime;
            List<double> intervalList = new List<double>();
            GpioChangeRecord changeRecord;

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
            return new IRMessage(intervalList, true);
        }

        private void Send0(double length)
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

        private void Send1(double length)
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
            /*
            double last = 0, current;
            double start = stopwatch.Elapsed.TotalMilliseconds;
            last = stopwatch.Elapsed.TotalMilliseconds;
            for (int i = 0; stopwatch.Elapsed.TotalMilliseconds - start < length; i++)
            {
                current = stopwatch.Elapsed.TotalMilliseconds;
                // Should be: 0.0131 for 38kHz.  But there is overhead so we acctualy measure a littel bit less
                while (current - last < 0.0085)
                {
                    current = stopwatch.Elapsed.TotalMilliseconds;
                }
                if (i % 2 == 0)
                    irLED.Write(GpioPinValue.High);
                else
                    irLED.Write(GpioPinValue.Low);

                last = stopwatch.Elapsed.TotalMilliseconds;
            }
        }*/
    }
}
