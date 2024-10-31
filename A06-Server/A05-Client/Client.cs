/* 
* FILE : Client.cs
* PROJECT : PROG2121 - Assignment #5
* PROGRAMMER : Anthony Phan 
* FIRST VERSION : Nov 15, 2023  
* DESCRIPTION :
* The functions in this file are used to handle connecting the client to the server.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace A05_Client
{
    //
    // The following code was extracted from the MSDN site:
    //https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-5.0
    //
    public class Client
    {
        internal string ConnectClient(String server, Int32 port, string message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 portNumber = port;
                TcpClient client = new TcpClient(server, portNumber);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", data);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Recieved: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
                return responseData;
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
