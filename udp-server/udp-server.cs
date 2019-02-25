using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udp_server
{
    public class Server
    {
        public static void StartListening()
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint endPoint = new IPEndPoint(hostEntry.AddressList[0], 11000);

                Socket listener = new Socket(endPoint.Address.AddressFamily,
                    SocketType.Dgram,
                    ProtocolType.Udp);

                IPEndPoint sender = new IPEndPoint(IPAddress.IPv6Any, 0);
                EndPoint senderRemote = (EndPoint)sender;

                listener.Bind(endPoint);

                Console.WriteLine("Waiting to receive datagrams from client...");

                byte[] filelen = new byte[8];
                int bytesrec = listener.ReceiveFrom(filelen, ref senderRemote);
                long len = (filelen[0] << 56) | (filelen[1] << 48) | (filelen[2] << 40) | (filelen[3] << 32)
                    | (filelen[4] << 24) | (filelen[5] << 16) | (filelen[6] << 8) | (filelen[7]);

                byte[] namelen = new byte[4];
                bytesrec = listener.ReceiveFrom(namelen, ref senderRemote);
                int lenname = (namelen[0] << 24) | (namelen[1] << 16) | (namelen[2] << 8) | (namelen[3]);

                byte[] name = new byte[lenname];
                bytesrec = listener.ReceiveFrom(name, ref senderRemote);

                byte[] file = new byte[len];
                bytesrec = listener.ReceiveFrom(file, ref senderRemote);
 
                using (BinaryWriter br = new BinaryWriter(File.Open(Encoding.ASCII.GetString(name), FileMode.Create)))
                {
                    br.Write(file);
                }

                Console.WriteLine("File {0} received, restarting...\n", Encoding.ASCII.GetString(name));
                listener.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("System failed, try again.\n");
            }
        }

        public static int Main(String[] args)
        {
            Console.Write("Start...\n");
            while (true)
            {
                StartListening();
                Console.WriteLine("Restarting...\n");
            }
            return 0;
        }
    }
}
