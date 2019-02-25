using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp_server
{
    public class Server
    {
        public static void StartListening()
        {

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    // An incoming connection needs to be processed. 
                    byte[] fileLen = new byte[8];
                    int bytesRec = handler.Receive(fileLen);
                    long len = (fileLen[0] << 56) | (fileLen[1] << 48) | (fileLen[2] << 40) | (fileLen[3] << 32)
|                              (fileLen[4] << 24) | (fileLen[5] << 16) | (fileLen[6] << 8) | (fileLen[7]);

                    
                    byte[] nameLen = new byte[4];
                    bytesRec = handler.Receive(nameLen);
                    int lenName = (nameLen[0] << 24) | (nameLen[1] << 16) | (nameLen[2] << 8) | (nameLen[3]);

                    byte[] name = new byte[lenName];
                    bytesRec = handler.Receive(name);

                    byte[] file = new byte[len];
                    bytesRec = handler.Receive(file);

                    // Write file  
                    using (BinaryWriter br = new BinaryWriter(File.Open(Encoding.ASCII.GetString(name), FileMode.Create)))
                    {
                        br.Write(file);
                    }

                    Console.WriteLine("File {0} received, restarting...\n", Encoding.ASCII.GetString(name));

                        // Echo the data back to the client.  
                    byte[] msg = Encoding.ASCII.GetBytes("success");

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Main(String[] args)
        {
            Console.WriteLine("Start...\n");
            StartListening();
            return 0;
        }
    }
}