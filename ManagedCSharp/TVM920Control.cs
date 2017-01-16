using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCSharp
{
    static class TVM920Control
    {
        static UdpClient UDP;
        static IPEndPoint EP;

        static TVM920Control()
        {
            UDP = new UdpClient();
            EP = new IPEndPoint(IPAddress.Parse("192.168.0.10"), 8701);
            UDP.Client.SendTimeout = 1000;
            UDP.Client.ReceiveTimeout = 1000;
            UDP.Connect(EP);
        }

        static private void SendUDP(byte[] data)
        {
            try
            {
                UDP.Send(data, data.Length);
            }
            catch
            {

            }
        }

        static private byte[] ReceiveUDP()
        {
            try
            {
                return UDP.Receive(ref EP);
            }
            catch
            {

            }

            return new byte[] { };
        }

        static public void DownLightOn()
        {
            // GPIO Set
            SendUDP(new byte[] { 0x16, 0x00, 0x00, 0x00, 0x80, 0x01, 0x00, 0x00 });

            // GPIO Clear
            SendUDP(new byte[] { 0x17, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 });
        }

        static public void UpLightOn()
        {
            // GPIO Set
            SendUDP(new byte[] { 0x16, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00 });

            // GPIO Clear
            SendUDP(new byte[] { 0x17, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00 });
        }
    }
}
