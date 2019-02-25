using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace tcp_client
{
    public class Client
    {
        public static void StartClient(string fileName)
        {
            // Connect to a remote device.  
            try
            {
                FileInfo info = new FileInfo(fileName);
                byte[] fName = Encoding.ASCII.GetBytes(info.Name);
                int len = fName.Length;

                byte[] lenBytes = BitConverter.GetBytes(len);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(lenBytes);

                byte[] fileLen = BitConverter.GetBytes(info.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(fileLen);

                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    sender.Send(fileLen);
                    sender.Send(lenBytes);
                    sender.Send(fName);                   
                    sender.SendFile(fileName);
                    Console.WriteLine("Sended...");

                    // Receive the response from the remote device.
                    byte[] bytes = new byte[1024];
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Result = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : invalid argument");
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : no connection or port is unavailable");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : something went wrong");
                }

            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine("File not found, try again");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("System failed, try again.\n" + e.ToString());
                return;
            }
        }

        public static int Main(String[] args)
        {
            Console.Write("Start...\n");
            while (true)
            {
                Console.Write("Enter path to copied file: ");
                string path = Console.ReadLine();
                StartClient(path);
                Console.WriteLine("Restarting...\n");
            }
        }
    }
}
