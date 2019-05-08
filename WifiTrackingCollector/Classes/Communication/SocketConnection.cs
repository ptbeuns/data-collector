using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace DataCollector
{
    class SocketConnection
    {
        public Socket Socket { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public int Port { get; private set; }
        public string Message { get; private set; }

        private byte[] bytes = new byte[1024];

        public SocketConnection(IPAddress ipaddress, int port)
        {
            IPAddress = ipaddress ?? throw new ArgumentNullException();
            Port = port;
            Socket = null;
        }

        public void ConnectSocket()
        {
            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress, Port);
                Socket = new Socket(IPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Socket.Connect(iPEndPoint);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public bool SendMessage(string msg)
        {
            if (Socket != null)
            {
                try
                {
                    if (Socket.Poll(1000, SelectMode.SelectWrite))
                    {
                        byte[] message = Encoding.ASCII.GetBytes(MessageParser.Encode(msg));
                        int bytesSent = Socket.Send(message);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("sendmessageellse");
                        return false;
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("Socketexceptie");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool ReceiveMessage()
        {
            if (Socket != null)
            {
                if (Socket.Poll(1000, SelectMode.SelectRead))
                {
                    int bytesRec = Socket.Receive(bytes);
                    Message = MessageParser.Parse(Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
