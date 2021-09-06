//Code written by Albert liu and Alex Hudson
//Last updated 12/5/2019
//Skeleton Code provided by professor Kopta

using System;
using System.Collections.Generic;
using System.Threading;
using Control;
using Model;

namespace Server
{
    class ServerStarter
    {
        private string setting_path;
        private ServerController sc;
        private int fps;

        /// <summary>
        /// Creates and starts a server
        /// </summary>
        public ServerStarter()
        {
            setting_path = "..\\..\\..\\..\\Resources\\ServerResources\\";
            sc = new ServerController(setting_path + "settings.xml");
            fps = 1000 / ServerController.TheWorld.GetFrame();
        }

        /// <summary>
        /// Display the frame rate once every 1000 milisecond/1 second on the server screen
        /// </summary>
        /// <param name="fps"></param>
        private static void DisplayFPS(int fps)
        {
            while (true)
            {
                Thread.Sleep(1000); 
                Console.WriteLine("FPS: " + fps);
                ServerController.TimeSpan += 1000; 
            }
        }

        static void Main(string[] args)
        {
            ServerStarter ss = new ServerStarter();
            ss.sc.SetGames(DBManager.GetGamesData());

            Thread httpserver = new Thread(ss.sc.StartHttpServer);
            Thread server = new Thread(ss.sc.StartServer);
            httpserver.Start();
            server.Start();

            // thread for displaying FPS
            Thread fps_displayer = new Thread(() => DisplayFPS(ss.fps));
            fps_displayer.Start();

            // thread for timer  
            Thread timer = new Thread(ss.sc.SetTimer);  
            timer.Start();  

            Console.ReadLine();
            ss.sc.EndServer();
            Environment.Exit(0);

        }
    }
}
