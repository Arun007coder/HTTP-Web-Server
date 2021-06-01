﻿using System;
using System.IO;


namespace HTTP_Web_Server
{
    class Program
    {
        
        public int PT = 30;
        

         static void Main(string[] args)
        {
            HTTPServer.SetTimer();
            int PT = 0;
            string[] lines = File.ReadAllLines("port.txt");

            foreach (string line in lines)
                PT = int.Parse(line);

            Console.WriteLine("press ESC key to exit");
            Console.WriteLine("$ Server starting");
            HTTPServer server = new HTTPServer(PT);
            server.start();
            Console.WriteLine("$ Server started on port :" + PT);

            if(Console.ReadKey().Key == ConsoleKey.Escape)
            {
                server.stop();
            }

        }
    }
}
