/* 
* FILE : clientHandler.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 24, 2023  
* DESCRIPTION :
* The functions in this file are used to handle the client values on the server side.
* us accordingly.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A06_Server
{
    internal class clientHandler
    {
        public string UserName { get; set; }
        public string UserGuess { get; set; }
        public TcpClient TcpClient { get; set; }
        public string timeLimit { get; set; }
        public string[] wordList { get; set; }
        public int[] wordCount { get; set; }


        // FUNCTION    : clientHandler
        // DESCRIPTION : This function gets values for user and tcpClient
        // PARAMETERS  : string userName, TcpClient tcpClient
        // RETURNS     : Nothing
        public clientHandler(string userName, TcpClient tcpClient)
        {
            UserName = userName;
            TcpClient = tcpClient;
        }
    }
}
