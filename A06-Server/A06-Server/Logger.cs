/* 
* FILE : Logger.cs
* PROJECT : PROG2121 - Assignment #6
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 24, 2023  
* DESCRIPTION :
* The functions in this file are used to log messages to a text file.
* Each entry has a date and time of the event and a log message.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace A06_Server
{
    internal class Logger
    {
        private static readonly string logFilePath = ConfigurationManager.AppSettings["LogFilePath"];

        // FUNCTION    : Log
        // DESCRIPTION : This function is used to log an entry to a file.
        // PARAMETERS  : string message
        // RETURNS     : Nothing
        public static void Log(string message)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss - ");
            string logEntry = date + message + "\r\n";
            File.AppendAllText(logFilePath, logEntry);
        }
    }
}
