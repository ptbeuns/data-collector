using System;
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
        private static readonly string configFile = AppDomain.CurrentDomain.BaseDirectory + @"\TrainConfig.json";
        private static Train train;
        private static SocketState socketState;
        static void Main(string[] args)
        {
            ReadConfig();
            train.AutoDiscoverWiFiTrackers();
            //Console.WriteLine(train.Coupes[0].CoupeNr);
            SocketConnection s = new SocketConnection(IPAddress.Parse("145.93.34.34"), 4337);
            while (true)
            {
                switch (socketState)
                {
                    case SocketState.Initialize:
                        Console.WriteLine("init");
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
                        else
                        {
                            socketState = SocketState.Initialize;
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
                        //Foreach coupe;
                        //Foreach wifitracker
                        //Temp:
                        while (s.Socket.IsBound)
                        {
                            System.Threading.Thread.Sleep(5000);
                            s.SendMessage("OCCUPATION:" + train.TrainUnitNr + ",30,40,30");
                        }
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
                Console.WriteLine("Train config file not found!");
                System.Threading.Thread.Sleep(3000);
                Environment.Exit(0);
            }
            Console.WriteLine("Reading config file from: " + configFile);
            train = JsonConvert.DeserializeObject<Train>(File.ReadAllText(configFile));
            //TODO server ip + port via config
        }

        //Temp:
        private static void ShowCoupeInfo()
        {
            Console.WriteLine(train.Coupes[0].WiFiTrackers[0].TrackerID + "" + train.Coupes[0].WiFiTrackers[0].ComPort);
            Console.WriteLine(train.Coupes[0].WiFiTrackers[1].TrackerID + "" + train.Coupes[0].WiFiTrackers[1].ComPort);

            Console.WriteLine(train.Coupes[1].WiFiTrackers[0].TrackerID + "" + train.Coupes[1].WiFiTrackers[0].ComPort);
            Console.WriteLine(train.Coupes[1].WiFiTrackers[1].TrackerID + "" + train.Coupes[1].WiFiTrackers[1].ComPort);
        }
    }
}
