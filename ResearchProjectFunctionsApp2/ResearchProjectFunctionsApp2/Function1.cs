using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Core;
using InfluxDB.Client.Writes;
using InfluxDB.Client.Api.Domain;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.IO;

namespace ResearchProjectFunctionsApp2
{   
    public class Function1
    {
        //cFunction to AES decrypt data
        static string DecryptStringFromBytes_Aes(byte[] encrypted, string password)
        {
            byte[] salt = new byte[] { 9, 2, 0, 0, 8, 7, 3, 1 };

            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            plaintext = sr.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        // Extract the encrypted data from their files
        static byte[] encryptedData = System.IO.File.ReadAllBytes("D:/School/SEM7/ResearchProject/InfludDB_token.bin");
        public static string password = System.IO.File.ReadAllText("D:/School/SEM7/ResearchProject/ResearchProjectSimulation/Assets/Resources/EK.txt");
        public static string influxToken = DecryptStringFromBytes_Aes(encryptedData, password);

        private static HttpClient client = new HttpClient();
        [FunctionName("Function1")]
        public async Task RunAsync([IoTHubTrigger("messages/events", Connection = "IoTHuBConnectionString")] EventData message, ILogger log)
        {
            string message_payload = Encoding.UTF8.GetString(message.Body.Array);
            log.LogInformation($"C# IoT Hub trigger function processed a message: {message_payload}");

            // Extract data from message
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(message_payload);

            //"hIBTZelEvwGJ40xavKqfqCJHPcsXO30HAFuSIqTkqhjZ455rjOPlUxiEA3V_f-Tdj9rK6ngA7D3Jr7yV0--ZYA=="
            var client = InfluxDBClientFactory.Create("https://westeurope-1.azure.cloud2.influxdata.com", influxToken);
            client.SetLogLevel(InfluxDB.Client.Core.LogLevel.Body);

            // Set the data to write to InfluxDB
            var point = PointData.Measurement("DroneData")
                .Field(data.Keys.ElementAt(0), data[data.Keys.ElementAt(0)])
                .Field(data.Keys.ElementAt(1), data[data.Keys.ElementAt(1)])
                .Field(data.Keys.ElementAt(2), data[data.Keys.ElementAt(2)])
                .Field(data.Keys.ElementAt(3), data[data.Keys.ElementAt(3)])
                .Field(data.Keys.ElementAt(4), data[data.Keys.ElementAt(4)])
                .Field(data.Keys.ElementAt(5), data[data.Keys.ElementAt(5)])
                .Field(data.Keys.ElementAt(6), data[data.Keys.ElementAt(6)])
                .Timestamp(DateTime.UtcNow, WritePrecision.Ns);
            //System.Diagnostics.Trace.WriteLine($"Data to write: {point.ToLineProtocol()}");

            // Exception handling
            try
            {
                // Async writing to Influx
                await client.GetWriteApiAsync().WritePointAsync(point, "ResearchProject", "MCT");
            }
            catch (Exception ex)
            {
                log.LogInformation("Error info:" + ex.Message);
            }
            finally
            {
                log.LogInformation("Written to InfluxDB");
            }

        }
    }
}