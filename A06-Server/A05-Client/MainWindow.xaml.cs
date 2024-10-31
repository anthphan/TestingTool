/* 
* FILE : MainWindow.xaml.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 15, 2023  
* DESCRIPTION :
* The functions in this file are used to handle the client side connection of the
* word guessing game. It gets values from the user to connect to the server.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A05_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Client Client { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Client = new Client();
        }


        // FUNCTION    : Button_Click
        // DESCRIPTION : This function submits values from all textboxes on the MainWindow
        // PARAMETERS  : object sender, RoutedEventArgs e
        // RETURNS     : Nothing
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 127.0.0.1
            // 13000

            // check if ip address is empty
            if(string.IsNullOrEmpty(IPAddress.Text))
            {
                // prompt user to enter missing value
                MessageBox.Show("Please enter an IP Address.");
                return;
            }

            // check if port number is empty
            if (string.IsNullOrEmpty(portNumber.Text))
            {
                // prompt user to enter missing value
                MessageBox.Show("Please enter a port number.");
                return;
            }

            // check if name is empty
            if (string.IsNullOrEmpty(username.Text))
            {
                // prompt user to enter missing value
                MessageBox.Show("Please enter a username");
                return;
            }

            // check if time limit is empty
            if (string.IsNullOrEmpty(time.Text))
            {
                // prompt user to enter missing value
                MessageBox.Show("Please enter a time limit");
                return;
            }

            // set values from textboxes
            string ip = IPAddress.Text;
            int port = int.Parse(portNumber.Text);
            string name = username.Text;
            string timeLimit = time.Text;

            // connect to server using message protocol 
            // string message = "{connect}" + " " + {name} + " " + {timeLimit};
            string message = "connect" + " " + name + " " + timeLimit;

            // get message back from server
            string received = Client.ConnectClient(ip, port, message);

            // check to see if it can connect to server
            if (received != null)
            {            // split message into values
                string[] receivedMsg = received.Split(' ');
                // all names inputs are unique
                if (receivedMsg[0] == "exists")
                {
                    // prompt user to enter different name
                    MessageBox.Show("Name taken! Please Choose a different name.");
                    return;
                }

                // pass values to next window - gameWindow
                gameWindow GameWindow = new gameWindow(ip, port, message);
                // display gameWindow
                GameWindow.Show();

                // parse message from server. server sent back the {80 char string} {word count}
                string[] msg = received.Split(' ');
                // set random 80 char string to ranChar
                GameWindow.randChar.Content = msg[1];
                // set wordCount
                GameWindow.wordCount.Content = msg[2];

                // set the timer from the user input
                GameWindow.timerCounter.Content = timeLimit;

                // close main window
                this.Close();
            }
            // can't connect to server
            else
            {
                MessageBox.Show("Can't connect to server.");
                return;
            }


        }
    }
}
