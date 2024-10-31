/* 
* FILE : MyGameServer.cs
* PROJECT : PROG2121 - Assignment #6
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 24, 2023  
* DESCRIPTION :
* The functions in this file are used start the server on service start and stop 
* the server on service stop.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A06_Server
{
    //cd C:\Windows\Microsoft.NET\Framework\v4.0.30319
    public partial class MyGameServer : ServiceBase
    {
        private Listener listener;
        private Thread listenerThread;
        public MyGameServer()
        {
            InitializeComponent();
        }

        // FUNCTION    : OnStart
        // DESCRIPTION : This function is used to make a new thread start
        //               start the server when the service is started.
        // PARAMETERS  : string[] args
        // RETURNS     : Nothing
        protected override void OnStart(string[] args)
        {
            try
            {
                Logger.Log("[SERVICE STARTING] Service starting.");
                listener = new Listener();
                listenerThread = new Thread(listener.StartListener);
                listenerThread.Start();
            }
            catch (Exception ex)
            {
                Logger.Log($"[SERVICE STARTING ERROR] {ex.Message}");
            }
        }

        // FUNCTION    : OnStop
        // DESCRIPTION : This function is used to stop the server when the
        //               service is stopped
        // PARAMETERS  : Nothing
        // RETURNS     : Nothing
        protected override void OnStop()
        {
            try
            {
                Logger.Log("[SERVICE STOPPING] Service stopping.");

                listener.StopListener();
            }
            catch (Exception ex)
            {
                Logger.Log($"[SERVICE STOPPING ERROR] { ex.Message}");
            }
        }


    }
}
