using System.IO;
using System.Net;

namespace LegendaryTools.Networking
{
    public class NetworkClient
    {
        private TcpProtocol TcpProtocol = new TcpProtocol();
        private UdpProtocol UdpProtocol = new UdpProtocol();
        private UPnP upnp = new UPnP();

        public NetworkClient()
        {
            TcpProtocol.OnClientPacketReceived += OnClientPacketReceived;

            UdpProtocol.OnPacketReceived += OnUnreliablePacketReceived;
        }

        /// <summary>
        /// Connect client to a NetworkServer instance.
        /// </summary>
        public void Connect(string ip, int tcpPort, int udpPort)
        {
            TcpProtocol.Connect(NetworkUtility.ResolveEndPoint(ip, tcpPort));
            UdpProtocol.Start(udpPort);
        }

        public void Disconnect()
        {
            TcpProtocol.Disconnect();
            UdpProtocol.Stop();
            upnp.Close();
        }

        public void SendBytes(byte[] bytes, Packet type, QoSType qos)
        {
            Buffer buffer = Buffer.Create();
            BinaryWriter writer = buffer.BeginPacket(type);
            writer.Write(bytes.Length);
            writer.Write(bytes);
            buffer.EndPacket();

            switch (qos)
            {
                case QoSType.Reliable:
                    TcpProtocol.SendTcpPacket(buffer);
                    break;
                case QoSType.Unreliable:
                    UdpProtocol.Send(buffer,
                        NetworkUtility.ResolveEndPoint(TcpProtocol.IpAddress, UdpProtocol.listeningPort));
                    break;
            }
        }

        protected virtual void OnUnreliablePacketReceived(Buffer buffer, IPEndPoint source)
        {
        }

        protected virtual void OnClientPacketReceived(Buffer buffer, IPEndPoint source)
        {
        }
    }
}