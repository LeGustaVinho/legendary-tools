using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace LegendaryTools.Networking
{
    public enum QoSType
    {
        /// <summary>
        /// There is no guarantee of delivery or ordering.
        /// </summary>
        Unreliable,
        /// <summary>
        /// Each message is guaranteed to be delivered and in order.
        /// </summary>
        Reliable, 
    }

    public class NetworkServer
    {
        public int listenPort
        {
            get { return TcpProtocol.IsListening ? TcpProtocol.Port : 0; }
        }

        public List<TcpProtocol> connections
        {
            get { return new List<TcpProtocol>(TcpProtocol.clientsDictionary.Values); }
        }

        private UdpProtocol UdpProtocol = new UdpProtocol();
        private TcpProtocol TcpProtocol = new TcpProtocol();
        private UPnP upnp = new UPnP();

        public NetworkServer()
        {
            this.TcpProtocol.OnListenerPacketReceived += OnListenerPacketReceived;
            this.TcpProtocol.OnClientPacketReceived += OnClientPacketReceived;
            this.TcpProtocol.OnClientConnect += OnClientConnect;

            this.UdpProtocol.OnPacketReceived += OnUnreliablePacketReceived;
        }

        /// <summary>
        /// This starts the server listening for connections on the specified port.
        /// </summary>
        public void Listen(int tcpPort, int udpPort, bool multiThreaded = true)
        {
            if(tcpPort == udpPort)
            {
                return;
            }

            TcpProtocol.StartListener(tcpPort, multiThreaded);
            UdpProtocol.Start(udpPort, multiThreaded);
            upnp.OpenTCP(tcpPort);
            upnp.OpenUDP(udpPort);
        }

        /// <summary>
        /// This stops a server from listening.
        /// </summary>
        public void Stop()
        {
            TcpProtocol.Disconnect();
            UdpProtocol.Stop();
            upnp.Close();
        }

        /// <summary>
        /// This function pumps the server causing incoming network data to be processed, and pending outgoing data to be sent.
        /// Should be called when multi thread is false
        /// </summary>
        public void Update()
        {
            TcpProtocol.UpdateListener();
            TcpProtocol.UpdateClient();

            UdpProtocol.Update();
        }

        public void SendBytesTo(int id, byte[] bytes, Packet type, QoSType qos)
        {
            Buffer buffer = Buffer.Create();
            BinaryWriter writer = buffer.BeginPacket(type);
            writer.Write(bytes.Length);
            writer.Write(bytes);
            buffer.EndPacket();

            if (TcpProtocol.clientsDictionary.ContainsKey(id))
            {
                switch (qos)
                {
                    case QoSType.Reliable: TcpProtocol.SendToClient(id, buffer); break;
                    case QoSType.Unreliable: UdpProtocol.Send(buffer, 
                        NetworkUtility.ResolveEndPoint(TcpProtocol.clientsDictionary[id].IpAddress, 
                        UdpProtocol.listeningPort));
                    break;
                }
            }
        }

        protected virtual void OnClientConnect(int id, TcpProtocol client)
        {
            
        }

        protected virtual void OnClientPacketReceived(Buffer buffer, IPEndPoint source)
        {
            
        }

        protected virtual void OnListenerPacketReceived(Buffer buffer, TcpProtocol source)
        {
            
        }

        protected virtual void OnUnreliablePacketReceived(Buffer buffer, IPEndPoint source)
        {
            
        }
    }
}