/* 
* FILE : gameWindow.xaml.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 15, 2023  
* DESCRIPTION :
* The functions in this file are used to handle the client side game window of the
* word guessing game. It communicates with the server to get values and update the
* us accordingly.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace A05_Client
{
    /// <summary>
    /// Interaction logic for gameWindow.xaml
    /// </summary>
    public partial class gameWindow : Window
    {
        public Client Client { get; set; }
        private readonly string ip;
        private readonly int port;
        private readonly string username;
        private int timeLimit;
        private int oldTimeLimit;
        private readonly DispatcherTimer timer;

        // FUNCTION    : gameWindow
        // DESCRIPTION : This function gets values from previous window and sets them.
        //               It also starts the timer.
        // PARAMETERS  : string ip, int port, string message
        // RETURNS     : Nothing
        public gameWindow(string ip, int port, string message)
        {
            InitializeComponent();
            Client = new Client();
            // set values
            this.ip = ip;
            this.port = port;
            // split the message string
            string[] split = message.Split(' ');
            this.username = split[1];
            this.timeLimit = int.Parse(split[2]);
            oldTimeLimit = int.Parse(split[2]);

            // instatiate new timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += UpdateTimer;
            // start timer
            timer.Start();
        }


        // FUNCTION    : Button_Click
        // DESCRIPTION : This function submits the value from the textbox as the guess
        //               to the server. The guess is sent in the message protocol
        // PARAMETERS  : object sender, RoutedEventArgs e
        // RETURNS     : Nothing
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // set value of textbox
            string guess = GuessBox.Text;
            // message protocol
            string message = "guess" + " " + username + " " + guess;

            // get message from server
            string received = Client.ConnectClient(ip, port, message);



            if (received != null)
            {
                // split message
                string[] msg = received.Split(' ');

                // update the word count
                wordCount.Content = msg[2];

                // if the word count is zero that means all words have been guessed
                if (int.Parse(msg[2]) == 0)
                {
                    timer.Stop();
                    // prompt user to play again or quit
                    MessageBoxResult result = MessageBox.Show("WINNER! Play again?", "Game over!", MessageBoxButton.YesNo);

                    // reset the timer
                    timeLimit = oldTimeLimit;
                    // message protocol for dealing with existing users
                    message = "exists" + " " + username;
                    // get response from server to reset existing user
                    Client.ConnectClient(ip, port, message);

                    // if the user wants to play again
                    if (result == MessageBoxResult.Yes)
                    {
                        // message protocol for connecting to server
                        message = "connect" + " " + username + " " + timeLimit;
                        // get another response from server - start new game
                        received = Client.ConnectClient(ip, port, message);
                        // split message
                        string[] receivedMsg = received.Split(' ');
                        // set values 
                        randChar.Content = receivedMsg[1];
                        wordCount.Content = receivedMsg[2];
                        timerCounter.Content = timeLimit;
                        timer.Start();
                    }
                    else
                    {
                        // quit application
                        Quit();
                    }
                }

            }
            else
            {
                MessageBox.Show("Can't connect to server.");
                Close();
            }

        }

        // FUNCTION    : UpdateTimer
        // DESCRIPTION : This function updates the timer and counts down from the time limit value that
        //               was submitted. If the timer hits 0 its game over and prompt the user to play again
        // PARAMETERS  : object sender, RoutedEventArgs e
        // RETURNS     : Nothing
        private void UpdateTimer(object sender, EventArgs e)
        {
            // update timer display
            timerCounter.Content = timeLimit.ToString();

            // if timer is 0 game over
            if (timeLimit == 0)
            {
                // prompt user to play again or quit
                MessageBoxResult result = MessageBox.Show("Game over! Play again?", "Game over!", MessageBoxButton.YesNo );

                // reset the timer
                timeLimit = oldTimeLimit;
                // message protocol for dealing with existing users
                string message = "exists" + " " + username;
                // get response from server to reset existing user
                Client.ConnectClient(ip, port, message);

                // if the user wants to play again
                if (result == MessageBoxResult.Yes)
                {
                    // message protocol for connecting to server
                    message = "connect" + " " + username + " " + timeLimit;
                    // get another response from server - start new game
                    string received = Client.ConnectClient(ip, port, message);
                    // split message
                    string[] receivedMsg = received.Split(' ');
                    // set values 
                    randChar.Content = receivedMsg[1];
                    wordCount.Content = receivedMsg[2];
                    timerCounter.Content = timeLimit;
                }
                else
                {
                    // quit application
                    Quit();
                }

            }
            else
            {
                // decrement timer
                timeLimit--;
            }
        }

        // FUNCTION    : Button_Click_1
        // DESCRIPTION : This function is used for whenever the user wants to disconnect or quit the game.
        // PARAMETERS  : object sender, RoutedEventArgs e
        // RETURNS     : Nothing

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // confirm if the user wants to quit 
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit?", "Quit", MessageBoxButton.YesNo);

            // the user wants to quit
            if (result == MessageBoxResult.Yes)
            {
                Quit();
            }
        }

        // FUNCTION    : Quit
        // DESCRIPTION : This function is used for whenever the user wants to disconnect or quit the game.
        // PARAMETERS  : object sender, RoutedEventArgs e
        // RETURNS     : Nothing
        private void Quit()
        {
            // message protocol for quitting game
            string message = "quitConfirmation" + " " + username;
            // get message from server
            string received = Client.ConnectClient(ip, port, message);


            if (received != null)
            {
                // split message to confirm the quit
                string[] msg = received.Split(' ');

                // user really wants to quit
                if (msg[0] == "quit")
                {
                    Close();
                }
            }
            else
            {
                Close();
            }

        }

        // FUNCTION    : GuessBox_KeyDown
        // DESCRIPTION : This function is used for when the user wants to submit a guess. User can 
        //               press enter to submit guess rather than pressing button
        // PARAMETERS  : object sender, RoutedEventArgs e
        // RETURNS     : Nothing
        private void GuessBox_KeyDown(object sender, KeyEventArgs e)
        {
            // if the key pressed is enter
            if (e.Key == Key.Enter)
            {
                // submit guess
                Button_Click(sender, e);
                // dont submit enter
                e.Handled = true;
            }
        }
    }
}
