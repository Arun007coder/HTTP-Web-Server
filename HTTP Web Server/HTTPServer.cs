﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HTTP_Web_Server
{
    public class HTTPServer
    {
        public const string VERSION = "HTTP/1.1";
        public const string NAME = "C# web server v3.0.0";
        public static string MSG_DIR;
        public static string WEB_DIR;
        public static string LOG_DIR;
        public static string DEF_WEB_DIR = Environment.CurrentDirectory + "/Root/web/";
        public static string DEF_MSG_DIR = Environment.CurrentDirectory + "/Root/msg/";
        public static string DEF_LOG_DIR = Environment.CurrentDirectory + "/Root/logs/";
        public static string SET_WEB_DIR;
        public static string SET_MSG_DIR;
        public static string SET_LOG_DIR;
        private static IPAddress IPA;
        public static bool Lbool;
        public static bool CMDBool;
        public static string CMD1;
        public static string CMD2;
        public static string CMD3;
        public static string CMD1_Output;
        public static string CMD2_Output;
        public static string CMD3_output;
        public static string CMDEDIN6;
        public static string CMDEDIN7;
        public static string CMDEDIN5;

        private int Port;
        public static int EPort;
        public static bool CanFWD;
        public static System.Timers.Timer timer;
        private bool isRunning = false;
        public static string time;
        public static string FileName;
        public static string msg;
        public static string nme = " Log.txt";
        public static bool isHacking = false;
        //private static PortForward FWD;

        private TcpListener TL;

        


        public static void SetTimer()
        {
            // Create a timer with a 1 second interval.
            timer = new System.Timers.Timer(1000);
            timer.Enabled = true;
            // Hook up the Elapsed event for the timer. 
            timer.Elapsed += TimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public static void TimedEvent(object source, ElapsedEventArgs l)
        {
            string time = DateTime.Now.ToString("dd/MM/yyyy hh:mm.ss tt");
            FileName = time + nme;
        }

        public HTTPServer(int _port)
        {
            Port = _port;
            foreach(IPAddress IP in PortForward.ips)
            {
                if (IP.ToString().Contains("192.168.1"))
                {
                    TL = new TcpListener(IP, Port);
                    Console.WriteLine("The server is listening to " + IP + ":" + Port);
                }
            }
            
        }

        public void start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
            if (CanFWD)
            {
                PortForward.Makeport(Port, EPort);
            }
            else
            {
                Console.WriteLine("Port forwarding is disabled");
            }
        }

        public static void stop()
        {
            Console.WriteLine("Server is stopping ...");
            PortForward.REMport(EPort);
            log(WEB_DIR + @"\\CMDOUT\\CMD1_Output.txt", "null");
            log(WEB_DIR + @"\\CMDOUT\\CMD2_Output.txt", "null");
            log(WEB_DIR + @"\\CMDOUT\\CMD3_Output.txt", "null");
            Environment.Exit(0);

        }

        public static string GetLine(string txt, int lineNo)
        {
            //Console.WriteLine("debugreq : " + txt);
            string[] lines = txt.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        private void Run()
        {
            isRunning = true;
            TL.Start();

            while (isRunning == true)
            {



                try
                {

                    if (!isHacking)
                    {
                        Console.WriteLine("Waiting for connection ...");

                        TcpClient client = TL.AcceptTcpClient();

                        Console.WriteLine("Client connected!");

                        HandleClient(client);

                        client.Close();
                    }
                    else
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(30000);

                            isHacking = false;

                            Console.WriteLine("Waiting for connection ...");

                            TcpClient client = TL.AcceptTcpClient();

                            Console.WriteLine("Client connected!");

                            HandleClient(client);

                            client.Close();
                        });
                    }

                    
                }
                catch(IndexOutOfRangeException e)
                {
                    isHacking = true;
                    log(LOG_DIR +"Server was hacked or Index  outofbounds Exeption", e + "S002:Someone is trying to hack the server or trying to use nikto");;
                    Console.WriteLine("S002:Someone is trying to hack the server or trying to use nikto");
                    Console.WriteLine("Server is stopping because of security reasons.Refer Errors.txt");
                    Process.Start("notepad.exe", "Errors.txt");

                    Task.Factory.StartNew(() =>
                    {

                        System.Threading.Thread.Sleep(8000);
                        stop();
                    });
                }
                
            }

            isRunning = false;

            TL.Stop();
        }

        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());

            string msg = "";
            while (reader.Peek() != -1)
            {
                msg += reader.ReadLine() + "\n";
                string l = msg;
                //Console.WriteLine(l);
                
                CMDEDIN6 = GetLine(l, 5);
                CMDEDIN7 = GetLine(l, 6);
                CMDEDIN5 = GetLine(l, 4);
                log(LOG_DIR + "Debug Log.txt", CMDEDIN5 + "/n" + CMDEDIN6 + "/n" + CMDEDIN7);
                try
                {
                    if (msg == " ")
                    {
                        if (Lbool)
                        {
                            log(LOG_DIR + FileName, msg);
                        }
                    }
                    else
                    {
                        Console.WriteLine("logging error");
                    }

                }
                catch(NullReferenceException e)
                {
                    Console.WriteLine("loging service has occured an error");
                }
                
            }

            Debug.WriteLine("$ Request: \n" + msg);
            Console.WriteLine("$ Request: \n" + msg);
            Request req = Request.GetRequest(msg);
            Response resp = Response.From(req);
            resp.Post(client.GetStream());

        }

        


        public static void log(string _file, string _res)
        {
            string dir = LOG_DIR;
            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string Log = "$ Request : \n" + _res;
            using (StreamWriter sw = File.CreateText(_file))
            {
                sw.WriteLine(Log);
                sw.Close();
            }
        }

        public void CMDREQ(string _CMD)
        {
            string CMDCommands = _CMD;
            string CMD = "CMD.exe";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Windows\\System32\\cmd.exe",
                    Arguments = CMDCommands,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                }
            };

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
        }

        public static string CMDCOM(string _cmdreq)
        {
            Process q = new Process();

            q.StartInfo.UseShellExecute = false;
            q.StartInfo.RedirectStandardOutput = true;
            q.StartInfo.CreateNoWindow = true;
            q.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
            q.StartInfo.Arguments = _cmdreq;
            q.Start();

            return q.StandardOutput.ReadToEnd();
            q.WaitForExit();
        }

        public static void CMD(int slot)
        {
            if(slot == 0)
            {
                Console.WriteLine("Command is " + CMD1);
                CMD1_Output += CMDCOM("/c" + CMD1);
                Console.WriteLine(CMD1_Output);
                log(WEB_DIR + @"\\CMDOUT\\CMD1_Output.txt", CMD1_Output);
            }

            if(slot == 1)
            {
                Console.WriteLine("Command is " + CMD2);
                CMD2_Output += CMDCOM("/c" + CMD2);
                Console.WriteLine(CMD2_Output);
                log(WEB_DIR + @"\\CMDOUT\\CMD2_Output.txt", CMD2_Output);
            }

            if(slot == 2)
            {
                Console.WriteLine("Command is " + CMD3);
                CMD3_output += CMDCOM("/c" + CMD3);
                Console.WriteLine(CMD3_output);
                log(WEB_DIR + "\\CMDOUT\\CMD3_Output.txt", CMD3_output);
            }
        }

        public static void CLR(int slot)
        {
            if(slot == 0)
            {
                log(WEB_DIR + @"\\CMDOUT\\CMD1_Output.txt", "null");
            }

            if(slot == 1)
            {
                log(WEB_DIR + @"\\CMDOUT\\CMD2_Output.txt", "null");
            }

            if(slot == 2)
            {
                log(WEB_DIR + @"\\CMDOUT\\CMD3_Output.txt", "null");
            }

            Console.WriteLine(slot + " cleared");

        }

        public static void EXT()
        {
            Console.WriteLine("Closing server ... ");
            stop();
            Environment.Exit(0);
             
        }

        public static void CMDET(int _slot ,  string _COMM)
        {
            if(_slot == 0)
            {
                CMD1 = _COMM;
            }

            if(_slot == 1)
            {
                CMD2 = _COMM;
            }
            if(_slot == 2)
            {
                CMD3 = _COMM;
            }
        }

        



    }



}
