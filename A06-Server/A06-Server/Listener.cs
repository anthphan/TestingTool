/* 
* FILE : Listener.cs
* PROJECT : PROG2121 - Assignment #6
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 24, 2023  
* DESCRIPTION :
* The functions in this file are used to handle most of the server side of the application.
* Deals with all the processing of values sent from the client
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace A06_Server
{
    internal class Listener
    {
        private TcpListener server;
        private static bool stopListener = false;
        private static readonly List<clientHandler> users = new List<clientHandler>();
        //
        // The most of this code was extracted from the MSDN site:
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-5.0
        //

        // FUNCTION    : StartListener
        // DESCRIPTION : This function is used to start the server
        // PARAMETERS  : Nothing
        // RETURNS     : Nothing
        internal void StartListener()
        {
            try
            {
                string webIP = ConfigurationManager.AppSettings["ServerIP"];
                string webPort = ConfigurationManager.AppSettings["ServerPort"];


                IPAddress localAddr = IPAddress.Parse(webIP);
                Int32 port = Int32.Parse(webPort);

                server = new TcpListener(localAddr, port);
                Logger.Log($"[SERVER STARTING] Server starting on {localAddr}:{port}");
                // start listening for client requests
                server.Start();

                // enter the listening loop
                while (!stopListener)
                {
                    // perform a blocking call to accept requests.
                    // you could also user server.AcceotSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    // Console.WriteLine("Connected");
                    ParameterizedThreadStart ts = new ParameterizedThreadStart(Worker);
                    Thread clientThread = new Thread(ts);
                    clientThread.Start(client);
                }

            }
            catch (Exception ex)
            {
                Logger.Log($"[SERVER ERROR] {ex.Message}");
            }
            finally
            {
                Logger.Log("[SERVER STOPPING] Server stopping");
                // stop listening for new clients
                StopListener();
            }
        }

        // FUNCTION    : StopListener
        // DESCRIPTION : This function is used to stop the server
        // PARAMETERS  : Nothing
        // RETURNS     : Nothing
        internal void StopListener()
        {
            try
            {
                if (server != null)
                {
                    stopListener = true;
                    server.Stop();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"[SERVER STOPPING ERROR] {ex.Message}");
            }
        }

        // FUNCTION    : Worker
        // DESCRIPTION : This function gets values for user and tcpClient
        // PARAMETERS  : Object o
        // RETURNS     : Nothing
        public void Worker(Object o)
        {
            TcpClient client = (TcpClient)o;
            // buffer for reading data
            Byte[] bytes = new Byte[1024];
            String data = null;

            data = null;

            // get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;

            // loop to receive all the data sent by the client
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // translate data bytes ti ASCII string
                data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                // Console.WriteLine("Received: {0}", data);

                // string message = "{connect}" + " " + name + " " + timeLimit;
                string[] message = data.Split(' ');
                string messageType = message[0];
                string user = message[1];

                // check the message type
                // connect protocol
                if (messageType == "connect")
                {
                    // check to see if the user exists 
                    clientHandler existingUser = users.FirstOrDefault(x => x.UserName == user);

                    // if the user exists
                    if (existingUser != null)
                    {
                        // send response to client saying user exists
                        string exists = "exists" + " " + user + " " + 1;
                        byte[] existsMsg = System.Text.Encoding.ASCII.GetBytes(exists);

                        // send back a response
                        stream.Write(existsMsg, 0, existsMsg.Length);
                        // Console.WriteLine("Sent: {0}", exists);
                    }
                    // if user doesn't exist 
                    else
                    {
                        // add new user
                        NewUser(client, message);
                    }
                }

                // guess protocol
                if (messageType == "guess")
                {
                    // get word count after submitting  user's guess
                    int wordCounter = UserGuess(message);

                    // send response to client
                    string guessDetails = string.Format("guess {0} {1}", user, wordCounter);
                    byte[] response = System.Text.Encoding.ASCII.GetBytes(guessDetails);

                    // send back a response
                    stream.Write(response, 0, response.Length);
                    // Console.WriteLine("Sent: {0}", guessDetails);

                }

                // exists protocol
                if (messageType == "exists")
                {
                    // check to see if user exists
                    clientHandler existingUser = users.FirstOrDefault(x => x.UserName == user);
                    // remove user that exists
                    users.Remove(existingUser);

                    // send response saying user was removed
                    string removed = "removed" + " " + user + " " + 1;
                    byte[] removedMsg = System.Text.Encoding.ASCII.GetBytes(removed);

                    // send back a response
                    stream.Write(removedMsg, 0, removedMsg.Length);
                    // Console.WriteLine("Sent: {0}", removed);
                }

                // quitConfirmation protocol
                if (messageType == "quitConfirmation")
                {
                    // send response saying user wants to quit
                    string quit = "quit" + " " + user;
                    byte[] quitMsg = System.Text.Encoding.ASCII.GetBytes(quit);

                    // send back a response
                    stream.Write(quitMsg, 0, quitMsg.Length);
                    // Console.WriteLine("Sent: {0}", quit);
                }

                // quit protocol
                if (messageType == "quit")
                {
                    // end game and remove user
                    EndGame(client);
                }

            }
            // shutdown and end connection
            client.Close();
        }

        // FUNCTION    : NewUser
        // DESCRIPTION : This function is used to add a new user
        // PARAMETERS  : TcpClient tcpClient, string[] message
        // RETURNS     : Nothing
        private void NewUser(TcpClient tcpClient, string[] message)
        {
            // set values for new user
            string user = message[1];
            // load all lines from file
            string[] words = gameEngine.loadWords();
            // first line is the random 80 char string
            string randChar = words[0];
            // second line is the word count
            string wordCount = words[1];
            // all lines after that are the words
            string[] answers = words.Skip(2).ToArray();

            // instantiate new user
            clientHandler newUser = new clientHandler(user, tcpClient);

            // set word list for the user
            newUser.wordList = answers;
            // add user
            users.Add(newUser);

            // create response to client for the game details - rand char string and word count
            string startGame = string.Format("play {0} {1}", randChar, wordCount);
            byte[] gameDetails = System.Text.Encoding.ASCII.GetBytes(startGame);

            // send back the response
            newUser.TcpClient.GetStream().Write(gameDetails, 0, gameDetails.Length);
            // Console.WriteLine("Sent: {0}", startGame);

        }

        // FUNCTION    : UserGuess
        // DESCRIPTION : This function is used to process the users guess
        // PARAMETERS  : string[] message
        // RETURNS     : Nothing
        private int UserGuess(string[] message)
        {
            // set values from client message
            string user = message[1];
            string guess = message[2];

            // check if user exists
            clientHandler foundUser = users.FirstOrDefault(u => u.UserName == user);

            if (foundUser != null)
            {
                // check to see if word matches the users word list
                bool match = CheckGuess(guess, foundUser.wordList);

                // if it is a match
                if (match)
                {
                    // remove the guess from the list
                    List<string> words = foundUser.wordList.ToList();
                    words.Remove(guess);
                    // update wordlist
                    foundUser.wordList = words.ToArray();
                }
            }
            // return word count of the word list
            return foundUser.wordList.Length;
        }

        // FUNCTION    : CheckGuess
        // DESCRIPTION : This function is used to check to see if the user's guess is in the word list
        // PARAMETERS  : string userGuess, string[] wordList
        // RETURNS     : Nothing
        private bool CheckGuess(string userGuess, string[] wordList)
        {
            // check to see if the user guess matches any in the word list
            return wordList.Any(word => string.Equals(userGuess, word, StringComparison.OrdinalIgnoreCase));
        }

        // FUNCTION    : EndGame
        // DESCRIPTION : This function is used to remove the user from the game and end the game
        // PARAMETERS  : TcpClient client
        // RETURNS     : Nothing
        private void EndGame(TcpClient client)
        {
            // check to see if user exists
            clientHandler removeUser = users.Find(u => u.TcpClient == client);

            // user exists
            if (removeUser != null)
            {
                // remove the user
                users.Remove(removeUser);
            }
        }
    }
}
