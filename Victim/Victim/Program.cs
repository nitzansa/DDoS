using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Collections;


namespace Victim
{
    class Program
    {
        String pass = "";
        TcpListener tcpListener;
        ArrayList botsTime = new ArrayList();
        int botsConnections;

        public void creatpass()
        {
            Random r = new Random();
            const String fromChars = "abcdefghijklmnopkrstuvwxyz";
            pass = new string(Enumerable.Repeat(fromChars, 6).Select(s => s[r.Next(s.Length)]).ToArray());
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var IP in host.AddressList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                {
                    return IP.ToString();
                }
            }
            throw new Exception("No valid network adapters in the system!");
        }

        public void creatTcp()
        {
            try
            {
                //creat pass
                Random r = new Random();
                const String fromChars = "abcdefghijklmnopkrstuvwxyz";
                pass = new string(Enumerable.Repeat(fromChars, 6).Select(s => s[r.Next(s.Length)]).ToArray());

                //get randoom port
                int port = new Random().Next(1024, 65537);
                tcpListener = new TcpListener(port);
                tcpListener.Start();

                //start watch
                Stopwatch sw = new Stopwatch();
                sw.Start();

                Console.WriteLine("Server listening on port " + port + ", password id " + pass + " IPAdress " + IPAddress.Parse(GetLocalIPAddress()));
                while (true)
                {

                    TcpClient test = tcpListener.AcceptTcpClient();
                    Byte[] data = new Byte[256];
                    NetworkStream stream = test.GetStream();

                    //write mwssege
                    byte[] messege = Encoding.ASCII.GetBytes("Please enter your password\r\n");

                    try
                    {
                        stream.Write(messege, 0, messege.Length);
                    }
                    catch
                    {
                        Console.WriteLine("stream not writable");
                        //stream.Close();
                        return;
                    }

                    stream.Read(data, 0, 6);
                    String asciiData = Encoding.ASCII.GetString(data, 0, data.Length);
                    if (asciiData.Contains(this.pass))
                    {
                        //Console.WriteLine("Hacked succeeded your the king welcom to our CKingiot grop!");
                        Byte[] toSend = Encoding.ASCII.GetBytes("Access granted");
                        stream.Write(toSend, 0, toSend.Length);
                    }
                    else
                    {
                        //Console.WriteLine("Hacked fail, try again!");
                        test.Close();
                    }

                    //hacked messege
                    try
                    {
                        byte[] hack = new byte[44];
                        stream.Read(hack, 0, 44);
                        String msg = Encoding.ASCII.GetString(hack, 0, hack.Length);
                        if (msg.Contains("Hacked by "))
                        {
                            botsTime.Add(DateTime.UtcNow);
                            botsConnections++;
                            if (botsConnections >= 10)
                            {
                                if (((DateTime)botsTime[botsConnections - 1] - (DateTime)botsTime[botsConnections - 10]).TotalSeconds <= 1)
                                {
                                    Console.WriteLine("hacked by");
                                }
                            }

                        }


                    }
                    catch
                    {
                        Console.WriteLine("stream not readable");
                        stream.Close();
                        return;
                    }

                }
            }
            catch
            {

            }
        }

        private void checkPassword(String password, NetworkStream stream)
        {
            if (password.Equals(this.pass))
            {
                Console.WriteLine("Hacked succeeded your the king welcom to our CKingiot grop!");
                Byte[] toSend = Encoding.ASCII.GetBytes("Access granted");
                stream.Write(toSend, 0, toSend.Length);
            }
            else
            {
                Console.WriteLine("Hacked fail, try again!");
            }
        }


        static void Main(string[] args)
        {
            Program v = new Program();
            //v.creatpass();
            v.creatTcp();
        }
    }
}
