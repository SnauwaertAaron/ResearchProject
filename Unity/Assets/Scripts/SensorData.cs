using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using Newtonsoft.Json;
using Random = System.Random;
using System.Text;
using System.Security.Cryptography;

public class SensorData : MonoBehaviour
{
    Rigidbody drone;
    //GameObject[] remote;
    void Awake()
    {
        drone = GetComponent<Rigidbody>();
        //remote = GameObject.FindGameObjectsWithTag("Remote");
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("WriteToFile", 0.0f, 0.5f);
        InvokeRepeating("GenrateSensorData", 0.0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Creating function to check key-value already exists and  add or alter (error is thrown when usisng dict.Add when key-value already exists)
    void AddFloatToDictionary(string key, float value)
    {
        if (sensorData.ContainsKey(key))
            sensorData[key] = value;
        else
           sensorData.Add(key, value);
    }
    void AddStringToDictionary(string key, string value)
    {
        if (sensorData.ContainsKey(key))
            sensorData[key] = value;
        else
            sensorData.Add(key, value);
    }

    int CalculateSignal(int distance)
    {
        if (0 <= distance && distance < 1000)
            return 5;

        else if (1000 <= distance && distance < 2000)
            return 4;

        else if (2000 <= distance && distance < 3000)
            return 3;

        else if (3000 <= distance && distance < 4000)
            return 2;

        else if (4000 <= distance && distance < 5000)
            return 1;

        else
            return 0;

    }

    string CalculateCardinalDirection(float facing)
    {
        if (22.5 <= facing && facing < 67.5)
            return "NE";

        else if (67.5 <= facing && facing < 112.5)
            return "E";

        else if (112.5 <= facing && facing < 157.5)
            return "SE";

        else if (157.5 <= facing && facing < 202.5)
            return "S";

        else if (202.5 <= facing && facing < 247.5)
            return "SW";

        else if (247.5 <= facing && facing < 292.5)
            return "W";

        else if (292.5 <= facing && facing < 337.5)
            return "NW";

        else
            return "N";
    }

    float generateWindspeed()
    {
        float randwindspeed = (float)rd.NextDouble()/10;
        int randInt = rd.Next(0, 2);
        if (randInt == 0)
            windSpeed -= randwindspeed;
        else
            windSpeed += randwindspeed;
        return windSpeed;
    }

    Random rd = new Random();
    Dictionary<string, dynamic> sensorData = new Dictionary<string, dynamic>();
    public string sensorDataJson = "";
    float batteryPercentage = 100.00f;
    float altitude = 0.00f;
    float windSpeed = 3f;
    void GenrateSensorData()
    {
        // Creating a random to make the battery drainage non linear.
        float randBatteryPerc = rd.Next(0, 3);

        // Generate battery
        batteryPercentage -= (randBatteryPerc / 10);
        AddFloatToDictionary("batteryPercentage", (float)Math.Round(batteryPercentage, 2));

        // Capture altitude
        altitude = (float)Math.Round((drone.position.y - 3.75) / 100, 2);
        AddFloatToDictionary("altitude", altitude);

        // Capture speed
        float speed = (float)Math.Round(drone.velocity.magnitude / 20, 2);
        AddFloatToDictionary("speed", speed);

        float distanceToRemote = Vector3.Distance(GameObject.FindGameObjectWithTag("Remote").transform.position, drone.position);
        distanceToRemote = (float)Math.Round(distanceToRemote / 100, 2);
        AddFloatToDictionary("distanceToRemote", distanceToRemote);

        AddFloatToDictionary("signalStrength", CalculateSignal((int)Math.Floor(distanceToRemote)));

        float facing = drone.transform.eulerAngles.y;
        AddStringToDictionary("cardinalDirection", CalculateCardinalDirection(facing));

        AddFloatToDictionary("windspeed", (float)Math.Round(generateWindspeed(), 2));

        sensorDataJson = JsonConvert.SerializeObject(sensorData);
        Debug.Log(sensorDataJson);
    }

    string required = System.IO.File.ReadAllText(@"D:\School\SEM7\ResearchProject\ResearchProjectSimulation\Assets\Resources\EK.txt");
    void WriteToFile()
    {
        // Encrypt the plain text and write it to a file
        byte[] encrypted = EncryptStringToBytes_Aes(sensorDataJson, required);
        File.WriteAllBytes("Assets/Resources/SensorData.bin", encrypted);
    }

    public string GetSensorDataJson()
    {
        return sensorDataJson;
    }
    static byte[] EncryptStringToBytes_Aes(string plainText, string password)
    {
        byte[] encrypted;
        byte[] salt = new byte[] { 9, 2, 0, 0, 8, 7, 3, 1 };

        using (Aes aes = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt);
            aes.Key = pdb.GetBytes(32);
            aes.IV = pdb.GetBytes(16);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    encrypted = ms.ToArray();
                }
            }
        }

        return encrypted;
    }
}
