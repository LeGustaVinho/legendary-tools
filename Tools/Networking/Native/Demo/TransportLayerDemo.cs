using System.IO;
using System.Net;
using LegendaryTools.Networking;
using UnityEngine;

public class TransportLayerDemo : MonoBehaviour
{
    public bool isReliable;
    public bool isServer;
    public string ServerExternalIP;
    public string ServerInternalIP;
    public int stringSize = 1;
    public int TCPPort;
    private TcpProtocol TcpProtocol = new TcpProtocol();
    public int UDPPort;

    private UdpProtocol UdpProtocol = new UdpProtocol();
    private UPnP upnp = new UPnP();

    private void OnPacketReceived2(Buffer buffer, IPEndPoint source)
    {
        BinaryReader reader = buffer.BeginReading();
        reader.ReadByte();

        OnMessageReceived(reader.ReadString());

        buffer.Recycle();
    }

    private void OnPacketReceived(Buffer buffer, TcpProtocol source)
    {
        BinaryReader reader = buffer.BeginReading();
        reader.ReadByte();

        OnMessageReceived(reader.ReadString());

        buffer.Recycle();
    }

    private string BigString()
    {
        string result = string.Empty;

        for (int i = 0; i < stringSize; i++)
        {
            result += "A";
        }

        return result;
    }

    public void Send()
    {
        string message = isServer ? "I am server. " + BigString() : "I am client. " + BigString();

        Buffer buffer = Buffer.Create();
        BinaryWriter writer = buffer.BeginPacket(Packet.Empty);
        writer.Write(message);
        buffer.EndPacket();

        Debug.Log("Buffer size: " + buffer.size);

        if (!isReliable)
        {
            UdpProtocol.Send(buffer, NetworkUtility.ResolveEndPoint(ServerExternalIP, UDPPort));
        }
        else
        {
            if (!isServer)
            {
                TcpProtocol.SendTcpPacket(buffer);
            }
            else
            {
                TcpProtocol.SendToClient(0, buffer);
            }
        }
    }

    public void ConnectTCP()
    {
        if (!isServer)
        {
            TcpProtocol.OnClientPacketReceived += OnPacketReceived2;
            TcpProtocol.Connect(NetworkUtility.ResolveEndPoint(ServerExternalIP, TCPPort),
                NetworkUtility.ResolveEndPoint(ServerInternalIP, TCPPort));
        }
    }

    public void HostTCP()
    {
        if (isServer)
        {
            TcpProtocol.OnListenerPacketReceived += OnPacketReceived;
            TcpProtocol.StartListener(TCPPort);
            upnp.OpenTCP(TCPPort);
        }
    }

    public void HostUDP()
    {
        UdpProtocol.OnPacketReceived += OnPacketReceived2;
        UdpProtocol.Start(UDPPort);
        upnp.OpenUDP(UDPPort);
    }

    private void OnMessageReceived(string message)
    {
        Debug.Log("OnMessageReceived: " + message);
    }

    private void Update()
    {
        //TCP
        //TcpProtocol.UpdateListener();
        //TcpProtocol.UpdateClient();
    }

    private void OnDestroy()
    {
        TcpProtocol.Disconnect();
        UdpProtocol.Stop();
        upnp.Close();
    }

    private void OnApplicationQuit()
    {
        TcpProtocol.Disconnect();
        UdpProtocol.Stop();
        upnp.Close();

        Debug.Log("Application ending after " + Time.time + " seconds");
    }
}