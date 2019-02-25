using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udp_client
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
                IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[0], 11000);

                Socket sender = new Socket(endPoint.Address.AddressFamily,
                    SocketType.Dgram,
                    ProtocolType.Udp);
                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {

                    sender.SendTo(fileLen, endPoint);
                    sender.SendTo(lenBytes, endPoint);
                    sender.SendTo(fName, endPoint);
                    sender.SendTo(File.ReadAllBytes(fileName), endPoint);
                    Console.WriteLine("Sended...");

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
            catch (FileNotFoundException e)
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
