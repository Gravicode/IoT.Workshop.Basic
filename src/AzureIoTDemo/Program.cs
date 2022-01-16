using Microsoft.Extensions.Logging;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Json;
using nanoFramework.Logging.Serial;
using nanoFramework.Networking;
using System;
using System.IO.Ports;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using nanoFramework.M2Mqtt;
using nanoFramework.M2Mqtt.Messages;
using System.Diagnostics;

namespace AzureIoTDemo
{
    public class Program
    {

        public static void Main()
        {
            //masih bermasalah saat generate sas token, masalah di compute hash di HMA...
            var demo = new DemoAzureIoT();
            demo.Run();
            Thread.Sleep(Timeout.Infinite);
            
        }
        
        /*
        void Debug.WriteLine(string message)
        {
            logger?.LogDebug(message);
        }*/
    }
    public class DemoAzureIoT
    {
        const string DeviceID = "esp32";
        const string IotBrokerAddress = "FreeDeviceHub.azure-devices.net";
        const string SasKey = "U2hhcmVkQWNjZXNzU2lnbmF0dXJlIHNyPUZyZWVEZXZpY2VIdWIuYXp1cmUtZGV2aWNlcy5uZXQlMkZkZXZpY2VzJTJGZXNwMzImc2lnPXprV3RNZWRpZG1XUnZHVXBKRHBNTjRGdE5IUXQlMkJCNmd0anFpQmZ2R1RDZyUzRCZzZT0xNjQyMzc2MjE4";
        //const string SasKey = "SharedAccessSignature sr=FreeDeviceHub.azure-devices.net%2Fdevices%2Fesp32&sig=zkWtMedidmWRvGUpJDpMN4FtNHQt%2BB6gtjqiBfvGTCg%3D&se=1642376218";
        //HostName=FreeDeviceHub.azure-devices.net;DeviceId=esp32;SharedAccessSignature=
        const string Ssid = "wifi lemot";
        const string Password = "123qweasd";

        string telemetryTopic = $"devices/{DeviceID}/messages/events/";
        const string TwinReportedPropertiesTopic = "$iothub/twin/PATCH/properties/reported/";
        const string TwinDesiredPropertiesTopic = "$iothub/twin/GET/";

        // One minute unit
        DateTime allupOperation = DateTime.UtcNow;
        int sleepTimeMinutes = 60000;
        int minutesToGoToSleep = 2;
        SerialLogger logger = null;
        ushort messageID = ushort.MaxValue;
        bool twinReceived = false;
        bool messageReceived = false;

        public void Run()
        {
            try
            {
                // Use Debug.WriteLine to show messages in serial COM2 as debug won't work when the device will wake up from deep sleep
                // Set the GPIO 16 and 17 for the serial port COM2
                //Configuration.SetPinFunction(16, DeviceFunction.COM2_RX);
                //Configuration.SetPinFunction(17, DeviceFunction.COM2_TX);
                //SerialPort serial = new("COM2");
                //serial.BaudRate = 115200;
                //logger = new SerialLogger(ref serial, "My logger");
                //logger.MinLogLevel = LogLevel.Debug;
                Debug.WriteLine("Program Started, connecting to WiFi.");

                // As we are using TLS, we need a valid date & time
                // We will wait maximum 1 minute to get connected and have a valid date
                CancellationTokenSource cs = new(sleepTimeMinutes);
                var success = NetworkHelper.ConnectWifiDhcp(Ssid, Password, setDateTime: true, token: cs.Token);
                if (!success)
                {
                    Debug.WriteLine($"Can't connect to wifi: {NetworkHelper.ConnectionError.Error}");
                    if (NetworkHelper.ConnectionError.Exception != null)
                    {
                        Debug.WriteLine($"NetworkHelper.ConnectionError.Exception");
                    }

                    GoToSleep();
                }

                // Reset the time counter if the previous date was not valid
                if (allupOperation.Year < 2018)
                {
                    allupOperation = DateTime.UtcNow;
                }

                Debug.WriteLine($"Date and time is now {DateTime.UtcNow}");

                // nanoFramework socket implementation requires a valid root CA to authenticate with.
                // This can be supplied to the caller (as it's doing on the code bellow) or the Root CA has to be stored in the certificate store
                // Root CA for Azure from here: https://github.com/Azure/azure-iot-sdk-c/blob/master/certs/certs.c
                // We are storing this certificate in the resources
                X509Certificate azureRootCACert = new X509Certificate(Resources.GetBytes(Resources.BinaryResources.AzureRoot));

                // Creates MQTT Client with default port 8883 using TLS protocol
                MqttClient mqttc = new MqttClient(
                    IotBrokerAddress,
                    8883,
                    true,
                    azureRootCACert,
                    null,
                    MqttSslProtocols.TLSv1_2);

                // Handler for received messages on the subscribed topics
                mqttc.MqttMsgPublishReceived += ClientMqttMsgReceived;
                // Handler for publisher
                mqttc.MqttMsgPublished += ClientMqttMsgPublished;

                // Now connect the device
                var code = mqttc.Connect(
                    DeviceID,
                    $"{IotBrokerAddress}/{DeviceID}/api-version=2020-09-30",
                    GetSharedAccessSignature(null, SasKey, $"{IotBrokerAddress}/devices/{DeviceID}", new TimeSpan(24, 0, 0)),
                    false,
                    MqttQoSLevel.ExactlyOnce,
                    false, "$iothub/twin/GET/?$rid=999",
                    "Disconnected",
                    false,
                    60
                    );

                //If we are connected, we can move forward
                if (mqttc.IsConnected)
                {
                    Debug.WriteLine("subscribing to topics");
                    mqttc.Subscribe(
                        new[] {
                $"devices/{DeviceID}/messages/devicebound/#",
                "$iothub/twin/res/#"
                        },
                        new[] {
                    MqttQoSLevel.AtLeastOnce,
                    MqttQoSLevel.AtLeastOnce
                        }
                    );

                    Debug.WriteLine("Getting twin properties");
                    mqttc.Publish($"{TwinDesiredPropertiesTopic}?$rid={Guid.NewGuid()}", Encoding.UTF8.GetBytes(""), MqttQoSLevel.AtLeastOnce, false);
                    var readResult = new FakeSensor();

                    CancellationTokenSource cstwins = new(10000);
                    CancellationToken tokentwins = cstwins.Token;
                    while (!twinReceived && !tokentwins.IsCancellationRequested)
                    {
                        tokentwins.WaitHandle.WaitOne(200, true);
                    }

                    if (tokentwins.IsCancellationRequested)
                    {
                        Debug.WriteLine("No twin received on time");
                    }

                    Debug.WriteLine("Sending twin properties");
                    mqttc.Publish($"{TwinReportedPropertiesTopic}?$rid={Guid.NewGuid()}", Encoding.UTF8.GetBytes($"{{\"Firmware\":\"nanoFramework\",\"TimeToSleep\":{minutesToGoToSleep}}}"), MqttQoSLevel.AtLeastOnce, false);


                    // Print out the measured data
                    Debug.WriteLine($"Temperature: {readResult.Temperature}\u00B0C");
                    Debug.WriteLine($"Pressure: {readResult.Pressure}hPa");

                    //Publish telemetry data
                    messageID = mqttc.Publish(telemetryTopic, Encoding.UTF8.GetBytes($"{{\"Temperature\":{readResult.Temperature},\"Pressure\":{readResult.Pressure}}}"), MqttQoSLevel.ExactlyOnce, false);
                    Debug.WriteLine($"Message ID for telemetry: {messageID}");

                    // Wait for the message or cancel if waiting for too long
                    CancellationToken token = new CancellationTokenSource(5000).Token;
                    while (!messageReceived && !token.IsCancellationRequested)
                    {
                        token.WaitHandle.WaitOne(200, true);
                    }

                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("No telemetry confirmation received");
                    }
                }
            }
            catch
            {
                // We won't do anything
                // This global try catch is to make sure whatever happen, we will safely be able to go
                // To sleep
            }

            // Just go to sleep when we will arrive at this point
            GoToSleep();
        }
        void GoToSleep()
        {
            Debug.WriteLine($"Full operation took: {DateTime.UtcNow - allupOperation}");
            Debug.WriteLine($"Set wakeup by timer for {minutesToGoToSleep} minutes to retry.");
            Sleep.EnableWakeupByTimer(new TimeSpan(0, 0, minutesToGoToSleep, 0));
            Debug.WriteLine("Deep sleep now");
            Sleep.StartDeepSleep();
        }

        void ClientMqttMsgReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                Debug.WriteLine($"Message received on topic: {e.Topic}");
                string message = Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
                Debug.WriteLine($"and message length: {message.Length}");

                if (e.Topic.StartsWith("$iothub/twin/res/204"))
                {
                    Debug.WriteLine("and received confirmation for desired properties.");
                }
                else if (e.Topic.StartsWith("$iothub/twin/"))
                {
                    if (e.Topic.IndexOf("res/400/") > 0 || e.Topic.IndexOf("res/404/") > 0 || e.Topic.IndexOf("res/500/") > 0)
                    {
                        Debug.WriteLine("and was in the error queue.");
                    }
                    else
                    {
                        Debug.WriteLine("and was in the success queue.");
                        if (message.Length > 0)
                        {
                            // skip if already received in this session
                            if (!twinReceived)
                            {
                                try
                                {
                                    TwinProperties twin = (TwinProperties)JsonConvert.DeserializeObject(message, typeof(TwinProperties));
                                    minutesToGoToSleep = twin.desired.TimeToSleep != 0 ? twin.desired.TimeToSleep : minutesToGoToSleep;
                                    twinReceived = true;
                                }
                                catch
                                {
                                    // We will ignore
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in event: {ex}");
            }
        }

        void ClientMqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Debug.WriteLine($"Response from publish with message id: {e.MessageId}");
            if (e.MessageId == messageID)
            {
                messageReceived = true;
            }
        }

        string GetSharedAccessSignature(string keyName, string sharedAccessKey, string resource, TimeSpan tokenTimeToLive)
        {
            // http://msdn.microsoft.com/en-us/library/azure/dn170477.aspx
            // the canonical Uri scheme is http because the token is not amqp specific
            // signature is computed from joined encoded request Uri string and expiry string

            var exp = DateTime.UtcNow.ToUnixTimeSeconds() + (long)tokenTimeToLive.TotalSeconds;

            string expiry = exp.ToString();
            string encodedUri = HttpUtility.UrlEncode(resource);

            var hmacsha256 = new HMACSHA256(Convert.FromBase64String(sharedAccessKey));
            byte[] hmac = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(encodedUri + "\n" + expiry));
            string sig = Convert.ToBase64String(hmac);

            if (keyName != null)
            {
                return String.Format(
                "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
                encodedUri,
                HttpUtility.UrlEncode(sig),
                HttpUtility.UrlEncode(expiry),
                HttpUtility.UrlEncode(keyName));
            }
            else
            {
                return String.Format(
                    "SharedAccessSignature sr={0}&sig={1}&se={2}",
                    encodedUri,
                    HttpUtility.UrlEncode(sig),
                    HttpUtility.UrlEncode(expiry));
            }
        }

    }
    public class FakeSensor
    {
        Random rnd=new Random();    
        public double Temperature { get { return 10 + rnd.NextDouble() * 30; } }
        public double Pressure { get { return rnd.NextDouble() * 100; } }
    }
}
