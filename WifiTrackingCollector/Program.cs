﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace DataCollector
{
    class Program
    {
        static public List<string> WifiTrackers { get; private set; }
        private static readonly string configFile = @"TrainConfig.json";
        private static Train train;
        private static SocketState socketState;
        static void Main(string[] args)
        {
            ReadConfig();
            train.AutoDiscoverWiFiTrackers();
            SocketConnection s = new SocketConnection(IPAddress.Parse("192.168.1.30"), 4337);
            while (true)
            {
                switch (socketState)
                {
                    case SocketState.Initialize:
                        if (s.Socket == null)
                        {
                            s.ConnectSocket();
                            if (s.SendMessage("CONNECT:TRAIN"))
                            {
                                socketState = SocketState.Identifying;
                            }
                        }
                        break;
                    case SocketState.Identifying:
                        if(s.ReceiveMessage())
                        {
                            if (s.Message == "ACK")
                            {
                                s.SendMessage("IAM:" + train.RideNr);
                                socketState = SocketState.AwaitAck;
                            }
                            else if (s.Message == "NACK")
                            {
                                socketState = SocketState.Initialize;
                            }
                        }
                        break;
                    case SocketState.AwaitAck:
                        s.ReceiveMessage();
                        if (s.Message == "ACK")
                        {
                            socketState = SocketState.MainLoop;
                        }
                        else if (s.Message == "NACK")
                        {
                            socketState = SocketState.Identifying;
                        }
                        break;
                    case SocketState.MainLoop:
                        string x = "";
                        foreach (Coupe coupe in train.Coupes)
                        {
                            coupe.CollectCrowd();
                            x += "," + coupe.UniqueMacCount;
                        }
                        s.SendMessage("OCCUPATION:" + train.TrainUnitNr + x);
                        Console.WriteLine("Occupation updated: " + x + " sleep for 30 seconds.");
                        System.Threading.Thread.Sleep(30000);

                        //Temp (voor demo?):
                        //while (s.Socket.IsBound)
                        //{
                        //    System.Threading.Thread.Sleep(5000);
                        //    s.SendMessage("OCCUPATION:" + train.TrainUnitNr + ",30,40,30");
                        //}
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ReadConfig()
        {
            if (!File.Exists(configFile))
            {
                Console.WriteLine("Train config file not found! Application exit.");
                Console.WriteLine(configFile);
                System.Threading.Thread.Sleep(3000);
                Environment.Exit(0);
            }
            Console.WriteLine("Reading config file from: " + configFile);
            train = JsonConvert.DeserializeObject<Train>(File.ReadAllText(configFile));
            //TODO server ip + port via config
        }
    }
}
