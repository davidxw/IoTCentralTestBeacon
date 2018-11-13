using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;

namespace IoTCentralTestBeacon
{
    class Program
    {
        //static string DeviceConnectionString = "HostName=saas-iothub-69ec5798-e8bd-4978-bb95-617bd8d6215b.azure-devices.net;DeviceId=a782b00e-c332-4de0-a9ad-135ab777e057;SharedAccessKey=hRFNzPsrcj8Wb0TG14SaUJVSQDe5rFEQuevuVLvwpE4=";
        // this is a PER DEVICE connection string - if you want to connect a second device then I can generate a second string
        static string DeviceConnectionString = "HostName=saas-iothub-69ec5798-e8bd-4978-bb95-617bd8d6215b.azure-devices.net;DeviceId=15842b1e-a8d7-4555-b678-e18c0a77e436;SharedAccessKey=brMCxqLb2yc1AQKISEXiQgmx0x+BOwXlF3Mo8zHcBNI=";
        static DeviceClient Client = null;
        static CancellationTokenSource cts;
        private static double currentBatteryLevel = 0.0;
        private const double batteryDecrement = 1.0;
        private const double maxBatteyLevel = 100.0;


        static void Main(string[] args)
        {
            Console.WriteLine("Send simulated beacon battery level to IoT Central, Ctrl+C to cancel");

            try
            {
                InitClient();

                SendDeviceProperties();

                SendTelemetryAsync();

                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error: {0}", ex.Message);
            }
        }

        public static void InitClient()
        {
            try
            {
                Console.WriteLine("Connecting to hub");
                Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Mqtt);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        public static async void SendDeviceProperties()
        {
            try
            {
                Console.WriteLine("Sending device properties:");
                
                TwinCollection reportedProperties = new TwinCollection();
               
                reportedProperties["bluetoothAddress"] = "sampleAddress"; //text
                reportedProperties["beaconType"] = "sampleType"; //text
                reportedProperties["rssi"] = "rrrr"; //text
                reportedProperties["uuid"] = "uuuuu"; //text

                Console.WriteLine(JsonConvert.SerializeObject(reportedProperties));

                await Client.UpdateReportedPropertiesAsync(reportedProperties);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        private static async void SendTelemetryAsync()
        {
            Random rand = new Random();

            try
            {
                while (true)
                {
                    var nextBatteryLevel = getNextBatteryLevel();

                    var telemetryDataPoint = new
                    {
                        batteryLevel = nextBatteryLevel,
                        txPower = -30.0 - (rand.NextDouble() * 10)
                    };

                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    var message = new Message(Encoding.ASCII.GetBytes(messageString));

                    await Client.SendEventAsync(message);

                    Console.WriteLine("{0} > Sending telemetry: {1}", DateTime.Now, messageString);

                    await Task.Delay(5000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Intentional shutdown: {0}", ex.Message);
            }
        }

        private static double getNextBatteryLevel()
        {
            currentBatteryLevel -= batteryDecrement;

            if (currentBatteryLevel <= 0.0)
                currentBatteryLevel = maxBatteyLevel;

            return currentBatteryLevel;

        }
    }
}
