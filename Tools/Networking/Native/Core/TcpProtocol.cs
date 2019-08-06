#define DEBUG

using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace LegendaryTools.Networking
{
    /// <summary>
    /// Common network communication-based logic: sending and receiving of data via TCP.
    /// </summary>

    public delegate void OnTcpListenerPacketReceivedEventHandler(Buffer buffer, TcpProtocol source);
    public delegate void OnTcpClientPacketReceivedEventHandler(Buffer buffer, IPEndPoint source);
    public delegate void OnClientConnectEventHandler(int id, TcpProtocol client);

    public class TcpProtocol
    {
        bool multiThreaded = true;

        #region TcpClient Vars

        /// <summary>
        /// Protocol version.
        /// </summary>

        public const int VERSION = 1;
        public const int BUFFER_MAX_SIZE = 8192;

        static protected object clientLockObj = new int();
        static protected int connectionCounter = 0;

        /// <summary>
        /// All players have a unique identifier given by the server.
        /// </summary>
        public int Id = 1;

        public enum ConnectionStatus
        {
            NotConnected,
            Connecting,
            Verifying,
            Connected,
        }

        /// <summary>
        /// Current connection stage.
        /// </summary>

        public ConnectionStatus Status = ConnectionStatus.NotConnected;

        /// <summary>
        /// IP end point of whomever we're connected to.
        /// </summary>

        public IPEndPoint EndPoint;

        /// <summary>
        /// Timestamp of when we received the last message.
        /// </summary>

        public long LastReceivedTime = 0;

        /// <summary>
        /// How long to allow this player to go without packets before disconnecting them.
        /// This value is in milliseconds, so 1000 means 1 second.
        /// </summary>
#if UNITY_EDITOR
        public long TimeoutTime = 60000;
#else
	    public long TimeoutTime = 20000;
#endif

        // Incoming and outgoing queues
        Queue<Buffer> inQueue = new Queue<Buffer>();
        Queue<Buffer> outQueue = new Queue<Buffer>();

        // Buffer used for receiving incoming data
        byte[] temp = new byte[BUFFER_MAX_SIZE];

        // Current incoming buffer
        Buffer receiveBuffer;
        int expected = 0;
        int offset = 0;
        Socket socket;
        bool noDelay = false;
        IPEndPoint fallback;
        ListLessGarb<Socket> connectingList = new ListLessGarb<Socket>();
        Thread clientThread;

        // Static as it's temporary
        static Buffer buffer;

        public event OnTcpClientPacketReceivedEventHandler OnClientPacketReceived;

        /// <summary>
        /// Whether the connection is currently active.
        /// </summary>

        public bool IsConnected { get { return Status == ConnectionStatus.Connected; } }

        /// <summary>
        /// Whether we are currently trying to establish a new connection.
        /// </summary>

        public bool IsTryingToConnect { get { return connectingList.size != 0; } }

        /// <summary>
        /// Enable or disable the Nagle's buffering algorithm (aka NO_DELAY flag).
        /// Enabling this flag will improve latency at the cost of increased bandwidth.
        /// http://en.wikipedia.org/wiki/Nagle's_algorithm
        /// </summary>
        public bool NoDelay
        {
            get
            {
                return noDelay;
            }
            set
            {
                if (noDelay != value)
                {
                    noDelay = value;
#if !UNITY_WINRT
                    socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, noDelay);
#endif
                }
            }
        }

        /// <summary>
        /// Connected target's address.
        /// </summary>

        public string IpAddress { get { return (EndPoint != null) ? EndPoint.ToString() : "0.0.0.0:0"; } }

        #endregion

        #region TcpListener Vars

        static protected object listenerLockObj = new int();

        /// <summary>
        /// List of players in a consecutive order for each looping.
        /// </summary>

        ListLessGarb<TcpProtocol> clients = new ListLessGarb<TcpProtocol>();

        /// <summary>
        /// Dictionary list of players for easy access by ID.
        /// </summary>

        internal Dictionary<int, TcpProtocol> clientsDictionary = new Dictionary<int, TcpProtocol>();

        TcpListener listener;
        Thread listenerThread;
        int listenerPort = 0;
        long time = 0;

        public event OnTcpListenerPacketReceivedEventHandler OnListenerPacketReceived;
        public event OnClientConnectEventHandler OnClientConnect;

        /// <summary>
        /// Whether the server is currently actively serving players.
        /// </summary>

        public bool IsActive { get { return listenerThread != null; } }

        /// <summary>
        /// Whether the server is listening for incoming connections.
        /// </summary>

        public bool IsListening { get { return (listener != null); } }

        /// <summary>
        /// Port used for listening to incoming connections. Set when the server is started.
        /// </summary>

        public int Port { get { return (listener != null) ? listenerPort : 0; } }

        #endregion

        #region TcpClient Methods

        /// <summary>
        /// Try to establish a connection with the specified address.
        /// </summary>

        public void Connect(IPEndPoint externalIP) { Connect(externalIP, null); }

        /// <summary>
        /// Try to establish a connection with the specified remote destination.
        /// </summary>

        public void Connect(IPEndPoint externalIP, IPEndPoint internalIP)
        {
            Disconnect();

            Buffer.Recycle(inQueue);
            Buffer.Recycle(outQueue);

            // Some routers, like Asus RT-N66U don't support NAT Loopback, and connecting to an external IP
            // will connect to the router instead. So if it's a local IP, connect to it first.
            if (internalIP != null && NetworkUtility.GetSubnet(NetworkUtility.localAddress) == NetworkUtility.GetSubnet(internalIP.Address))
            {
                EndPoint = internalIP;
                fallback = externalIP;

#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:Connect(" + externalIP.ToString() + "," + internalIP.ToString() + ") -> Dont support loopback.");
#endif
            }
            else
            {
                EndPoint = externalIP;
                fallback = internalIP;

#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:Connect(" + externalIP.ToString() + "," + internalIP.ToString() + ") -> Support loopback.");
#endif
            }

            ConnectToTcpEndPoint();
        }

        /// <summary>
        /// Try to establish a connection with the current tcpEndPoint.
        /// </summary>

        bool ConnectToTcpEndPoint()
        {
            if (EndPoint != null)
            {
                Status = ConnectionStatus.Connecting;

                try
                {
                    lock (connectingList)
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        connectingList.Add(socket);
                    }
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:ConnectToTcpEndPoint() -> Connecting to endpoint.");
#endif
                    IAsyncResult result = socket.BeginConnect(EndPoint, OnConnectResult, socket);
                    Thread th = new Thread(CancelConnect);
                    th.Start(result);

                    return true;
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    Error(ex.Message);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("[Client][TcpProtocol:ConnectToTcpEndPoint()] -> Unable to resolve the specified address.");
                Error("Unable to resolve the specified address");
            }

            return false;
        }

        /// <summary>
        /// Try to establish a connection with the fallback end point.
        /// </summary>

        bool ConnectToFallback()
        {
            EndPoint = fallback;
            fallback = null;

            bool connectResult = ConnectToTcpEndPoint();

#if DEBUG
            UnityEngine.Debug.Log("[Client][TcpProtocol:ConnectToFallback()] -> Fallback result: " + connectResult);
#endif

            return (EndPoint != null) && connectResult;
        }

        /// <summary>
        /// Default timeout on a connection attempt it something around 15 seconds, which is ridiculously long.
        /// </summary>

        void CancelConnect(object obj)
        {
            IAsyncResult result = (IAsyncResult)obj;
#if !UNITY_WINRT
            if (result != null && !result.AsyncWaitHandle.WaitOne(3000, true))
            {
                try
                {
                    Socket sock = (Socket)result.AsyncState;

                    if (sock != null)
                    {
                        sock.Close();

                        lock (connectingList)
                        {
                            // Last active connection attempt
                            if (connectingList.size > 0 && connectingList[connectingList.size - 1] == sock)
                            {
                                socket = null;

                                if (!ConnectToFallback())
                                {
                                    UnityEngine.Debug.LogError("[Client][TcpProtocol:ConnectToFallback()] -> Unable to connect");
                                    Error("Unable to connect");
                                    Close(false);
                                }
                            }

                            connectingList.Remove(sock);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
#endif
        }

        /// <summary>
        /// Connection attempt result.
        /// </summary>

        void OnConnectResult(IAsyncResult result)
        {
            Socket sock = (Socket)result.AsyncState;

            // Windows handles async sockets differently than other platforms, it seems.
            // If a socket is closed, OnConnectResult() is never called on Windows.
            // On the mac it does get called, however, and if the socket is used here
            // then a null exception gets thrown because the socket is not usable by this point.
            if (sock == null)
            {
                UnityEngine.Debug.LogError("[Client][TcpProtocol:OnConnectResult() -> (socket)result.AsyncState is null");
                return;
            }

            if (socket != null && sock == socket)
            {
                bool success = true;
                string errMsg = "Failed to connect";

                try
                {
#if !UNITY_WINRT
                    sock.EndConnect(result);
#endif
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);

                    if (sock == socket) socket = null;
                    sock.Close();
                    errMsg = ex.Message;
                    success = false;
                }

                if (success)
                {
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:OnConnectResult() -> Connetion successful. Sending request to verify ID.");
#endif
                    // Request a connection ID
                    Status = ConnectionStatus.Verifying;
                    BinaryWriter writer = BeginSend(Packet.RequestID);
                    writer.Write(VERSION);
                    EndSend();
                    StartReceiving();
                }
                else if (!ConnectToFallback())
                {
                    UnityEngine.Debug.LogError("[Client][TcpProtocol:ConnectToFallback() -> " + errMsg);
                    Error(errMsg);
                    Close(false);
                }
            }

            // We are no longer trying to connect via this socket
            lock (connectingList) connectingList.Remove(sock);
        }

        /// <summary>
        /// Disconnect the instance, freeing all resources.
        /// </summary>

        public void Disconnect() { Disconnect(false); }

        /// <summary>
        /// Disconnect the instance, freeing all resources.
        /// </summary>

        public void Disconnect(bool notify)
        {
            if (!IsConnected)
            {
#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:Disconnect(" + notify + ") -> Not connected.");
#endif
                return;
            }

            try
            {
                lock (connectingList)
                {
                    //close all connections first
                    for (int i = connectingList.size; i > 0;)
                    {
                        Socket sock = connectingList[--i];
                        connectingList.RemoveAt(i);
                        if (sock != null) sock.Close();
                    }
                }

                // Stop the worker thread
                if (clientThread != null)
                {
                    clientThread.Abort();
                    clientThread = null;
                }

                if (socket != null)
                {
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:Disconnect(" + notify + ") -> Disconnected.");
#endif
                    Close(notify || socket.Connected);
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                lock (connectingList) connectingList.Clear();
                socket = null;
            }
        }

        /// <summary>
        /// Close the connection.
        /// </summary>

        public void Close(bool notify)
        {
            Status = ConnectionStatus.NotConnected;

            //recycle buffer
            if (receiveBuffer != null)
            {
                receiveBuffer.Recycle();
                receiveBuffer = null;
            }

            // Stop the worker thread
            if (clientThread != null)
            {
                clientThread.Abort();
                clientThread = null;
            }

            if (socket != null)
            {
                try
                {
                    if (socket.Connected)
                        socket.Shutdown(SocketShutdown.Both);

                    socket.Close();

#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:Close(" + notify + ") -> Closed.");
#endif
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
                socket = null;

                if (notify)
                {
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:Close(" + notify + ") -> Sending disconnect message.");
#endif

                    Buffer buffer = Buffer.Create();
                    buffer.BeginPacket(Packet.Disconnect);
                    buffer.EndTcpPacketWithOffset(4);
                    lock (inQueue) inQueue.Enqueue(buffer);
                }
            }
        }

        /// <summary>
        /// Release the buffers.
        /// </summary>

        public void Release()
        {
#if DEBUG
            UnityEngine.Debug.Log("[Client][TcpProtocol:Release() -> Released.");
#endif
            Close(false);
            Buffer.Recycle(inQueue);
            Buffer.Recycle(outQueue);
        }

        /// <summary>
        /// Begin sending a new packet to the server.
        /// </summary>

        public BinaryWriter BeginSend(Packet type)
        {
            buffer = Buffer.Create(false);
            return buffer.BeginPacket(type);
        }

        /// <summary>
        /// Send the outgoing buffer.
        /// </summary>

        public void EndSend()
        {
            buffer.EndPacket();
            SendTcpPacket(buffer);
            buffer = null;
        }

        /// <summary>
        /// Send the specified packet. Marks the buffer as used.
        /// </summary>

        public void SendTcpPacket(Buffer buffer)
        {
            buffer.MarkAsUsed();

            if (socket != null && socket.Connected)
            {
                buffer.BeginReading();

                lock (outQueue)
                {
                    outQueue.Enqueue(buffer);

                    if (outQueue.Count == 1)
                    {
                        try
                        {
#if DEBUG
                            UnityEngine.Debug.Log("[Client][TcpProtocol:SendTcpPacket(" + buffer.size + ") -> Sending packed.");
#endif
                            // If it's the first packet, let's begin the send process
#if !UNITY_WINRT
                            socket.BeginSend(buffer.buffer, buffer.position, buffer.size, SocketFlags.None, OnSend, buffer);
#endif
                        }
                        catch (System.Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                            Error(ex.Message);
                            Close(false);
                            Release();
                        }
                    }
                }
            }
            else
            {
#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:SendTcpPacket(" + buffer.size + ") -> Socket is null or not connected.");
#endif
                buffer.Recycle();
            }
        }

        /// <summary>
        /// Send completion callback. Recycles the buffer.
        /// </summary>

        void OnSend(IAsyncResult result)
        {
            if (Status == ConnectionStatus.NotConnected)
            {
#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:OnSend() -> Not connected.");
#endif
                return;
            }

            int bytes;

            try
            {
#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:OnSend() -> End send successful.");
#endif
#if !UNITY_WINRT
                bytes = socket.EndSend(result);
#endif
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                bytes = 0;
                Close(true);
                Error(ex.Message);
                return;
            }

            lock (outQueue)
            {
                // The buffer has been sent and can now be safely recycled
                outQueue.Dequeue().Recycle();
#if !UNITY_WINRT
                if (bytes > 0 && socket != null && socket.Connected)
                {
                    // If there is another packet to send out, let's send it
                    Buffer next = (outQueue.Count == 0) ? null : outQueue.Peek();

                    if (next != null)
                    {
                        try
                        {
                            socket.BeginSend(next.buffer, next.position, next.size, SocketFlags.None, OnSend, next);
#if DEBUG
                            UnityEngine.Debug.Log("[Client][TcpProtocol:OnSend() -> Sending another packet.");
#endif
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                            Error(ex.Message);
                            Close(false);
                        }
                    }
                }
                else
                {
#if DEBUG
                    UnityEngine.Debug.LogWarning("[Client][TcpProtocol:OnSend() -> Socket is null.");
#endif
                    Close(true);
                }
#endif
            }
        }

        /// <summary>
        /// Start receiving incoming messages on the current socket.
        /// </summary>

        public void StartReceiving(bool multiThreaded = true) { StartReceiving(null, multiThreaded); }

        /// <summary>
        /// Start receiving incoming messages on the specified socket (for example socket accepted via Listen).
        /// </summary>

        public void StartReceiving(Socket socket, bool multiThreaded = true)
        {
            this.multiThreaded = multiThreaded;

            if (socket != null)
            {
                Close(false);
                this.socket = socket;

#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:StartReceiving() -> Changing socket.");
#endif
            }

            if (this.socket != null && this.socket.Connected)
            {
                // We are not verifying the connection
                Status = ConnectionStatus.Verifying;

                // Save the timestamp
                LastReceivedTime = DateTime.UtcNow.Ticks / 10000;

                // Save the address
                EndPoint = (IPEndPoint)this.socket.RemoteEndPoint;

                if (multiThreaded)
                {
                    clientThread = new Thread(ThreadProcessListenerPackets);
                    clientThread.Start();
                }

                // Queue up the read operation
                try
                {
#if !UNITY_WINRT
                    this.socket.BeginReceive(temp, 0, temp.Length, SocketFlags.None, OnReceive, this.socket);
#endif
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:StartReceiving() -> Begin receive.");
#endif
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    Error(ex.Message);
                    Disconnect(true);
                }
            }
            else
            {
#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:StartReceiving() -> Socket is null or not connected.");
#endif
            }
        }

        /// <summary>
        /// Extract the first incoming packet.
        /// </summary>

        public bool ReceivePacket(out Buffer buffer)
        {
            if (inQueue.Count != 0)
            {
                lock (inQueue)
                {
                    buffer = inQueue.Dequeue();
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:ReceivePacket(" + buffer.size + ")] - Receiving packet ...");
#endif
                    return true;
                }
            }

            buffer = null;
            return false;
        }

        /// <summary>
        /// Receive incoming data.
        /// </summary>

        void OnReceive(IAsyncResult result)
        {
            if (Status == ConnectionStatus.NotConnected)
            {
#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:OnReceive() -> Not connected.");
#endif
                return;
            }

            int bytes = 0;
            Socket socket = (Socket)result.AsyncState;

            try
            {
#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:OnReceive() -> EndReceive.");
#endif
#if !UNITY_WINRT
                bytes = socket.EndReceive(result);
#endif
                if (this.socket != socket)
                {
#if DEBUG
                    UnityEngine.Debug.LogWarning("[Client][TcpProtocol:OnReceive() -> Current socket is not equals (Socket)result.AsyncState.");
#endif
                    return;
                }
            }
            catch (System.Exception ex)
            {
                if (this.socket != socket)
                {
#if DEBUG
                    UnityEngine.Debug.LogWarning("[Client][TcpProtocol:OnReceive() -> Current socket is not equals (Socket)result.AsyncState.");
#endif
                    return;
                }

                UnityEngine.Debug.LogException(ex);
                Error(ex.Message);
                Disconnect(true);
                return;
            }

            LastReceivedTime = DateTime.UtcNow.Ticks / 10000;

            if (bytes == 0)
            {
#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:OnReceive() -> Bytes received is 0.");
#endif
                Close(true);
            }
            else if (ProcessBuffer(bytes))
            {
                if (Status == ConnectionStatus.NotConnected)
                {
#if DEBUG
                    UnityEngine.Debug.LogWarning("[Client][TcpProtocol:OnReceive() -> Not connected.");
#endif
                    return;
                }

                try
                {
#if !UNITY_WINRT
                    // Queue up the next read operation
                    socket.BeginReceive(temp, 0, temp.Length, SocketFlags.None, OnReceive, socket);
#endif
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:OnReceive() -> Begin receive again.");
#endif
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    Error(ex.Message);
                    Close(false);
                }
            }
            else
            {
#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:OnReceive() -> ???");
#endif
                Close(true);
            }
        }

        /// <summary>
        /// See if the received packet can be processed and split it up into different ones.
        /// </summary>

        bool ProcessBuffer(int bytes)
        {
            if (receiveBuffer == null)
            {
                // Create a new packet buffer
                receiveBuffer = Buffer.Create();
                receiveBuffer.BeginWriting(false).Write(temp, 0, bytes);
                expected = 0;
                offset = 0;
#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessBuffer(" + bytes + ") -> ReceivedBuffer is null, then creating new buffer.");
#endif
            }
            else
            {
                // Append this data to the end of the last used buffer
                receiveBuffer.BeginWriting(true).Write(temp, 0, bytes);
#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessBuffer(" + bytes + ") -> Appending ReceivedBuffer.");
#endif
            }

            for (int available = receiveBuffer.size - offset; available >= 4;)
            {
                // Figure out the expected size of the packet
                if (expected == 0)
                {
                    expected = receiveBuffer.PeekInt(offset);

                    if (expected < 0 || expected > 16777216)
                    {
#if DEBUG
                        UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessBuffer(" + bytes + ") -> ???");
#endif
                        // HTTP Get: 542393671
                        Close(true);
                        return false;
                    }
                }

                // The first 4 bytes of any packet always contain the number of bytes in that packet
                available -= 4;

                // If the entire packet is present
                if (available == expected)
                {
                    // Reset the position to the beginning of the packet
                    receiveBuffer.BeginReading(offset + 4);

                    // This packet is now ready to be processed
                    lock (inQueue) inQueue.Enqueue(receiveBuffer);

                    receiveBuffer = null;
                    expected = 0;
                    offset = 0;
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessBuffer(" + bytes + ") -> Entire packet is present.");
#endif
                    break;
                }
                else if (available > expected)
                {
                    // There is more than one packet. Extract this packet fully.
                    int realSize = expected + 4;
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessBuffer(" + bytes + ") -> There is more than one packet. Extract this packet fully. RealSize = " + realSize + " | available(" + available + ") > mExpected(" + expected + ")");
#endif
                    Buffer temp = Buffer.Create();

                    // Extract the packet and move past its size component
                    BinaryWriter bw = temp.BeginWriting(false);
                    bw.Write(receiveBuffer.buffer, offset, realSize);
                    temp.BeginReading(4);

                    // This packet is now ready to be processed
                    lock (inQueue) inQueue.Enqueue(temp);

                    // Skip this packet
                    available -= expected;
                    offset += realSize;
                    expected = 0;
                }
                else
                {
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessBuffer(" + bytes + ") -> available(" + available + ") < mExpected(" + expected + ")");
#endif
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// Add an error packet to the incoming queue.
        /// </summary>

        public void Error(string error) { Error(Buffer.Create(), error); }

        /// <summary>
        /// Add an error packet to the incoming queue.
        /// </summary>

        void Error(Buffer buffer, string error)
        {
            buffer.BeginPacket(Packet.Error).Write(error);
            buffer.EndTcpPacketWithOffset(4);
            lock (inQueue) inQueue.Enqueue(buffer);
        }

        /// <summary>
        /// Verify the connection.
        /// </summary>

        public bool VerifyRequestID(Buffer buffer, bool uniqueID)
        {
            BinaryReader reader = buffer.BeginReading();
            Packet request = (Packet)reader.ReadByte();

            if (request == Packet.RequestID)
            {
                if (reader.ReadInt32() == VERSION)
                {
                    lock (listenerLockObj)
                    {
                        Id = uniqueID ? ++connectionCounter : 0;
                    }

                    Status = TcpProtocol.ConnectionStatus.Connected;

                    BinaryWriter writer = BeginSend(Packet.ResponseID);
                    writer.Write(VERSION);
                    writer.Write(Id);
                    writer.Write((Int64)(System.DateTime.UtcNow.Ticks / 10000));
                    EndSend();
#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:VerifyRequestID()] -> Protocol marked as connected in server. !");
#endif
                    return true;
                }
                else
                {
                    BinaryWriter writer = BeginSend(Packet.ResponseID);
                    writer.Write(0);
                    EndSend();
#if DEBUG
                    UnityEngine.Debug.LogWarning("[Client][TcpProtocol:VerifyRequestID()] -> Incorrect version.");
#endif
                    Close(false);
                }
            }
            else
            {
#if DEBUG
                UnityEngine.Debug.LogWarning("[Client][TcpProtocol:VerifyRequestID(" + buffer.size + ", " + uniqueID + ") -> Packet is not Packet.RequestID");
#endif
            }

            return false;
        }

        /// <summary>
        /// Verify the connection.
        /// </summary>

        public bool VerifyResponseID(Packet packet, BinaryReader reader)
        {
            if (packet == Packet.ResponseID)
            {
                int serverVersion = reader.ReadInt32();

                if (serverVersion != 0 && serverVersion == VERSION)
                {
                    Id = reader.ReadInt32();
                    Status = ConnectionStatus.Connected;

#if DEBUG
                    UnityEngine.Debug.Log("[Client][TcpProtocol:VerifyRequestID()] -> Protocol marked as connected in client. !");
#endif

                    return true;
                }
                else
                {
                    Id = 0;
                    UnityEngine.Debug.LogError("[Client][TcpProtocol:VerifyResponseID() -> Version mismatch! Server is running a different protocol version!");
                    Error("Version mismatch! Server is running a different protocol version!");
                    Close(false);

                    return false;
                }
            }

            UnityEngine.Debug.LogError("[Client][TcpProtocol:VerifyResponseID() -> Expected a response ID, got " + packet);
            Error("Expected a response ID, got " + packet);
            Close(false);

            return false;
        }

        /// <summary>
        /// Call after shutting down the listener.
        /// </summary>

        public static void ResetConnectionsCounter()
        {
#if DEBUG
            UnityEngine.Debug.Log("[Client][TcpProtocol:ResetConnectionsCounter()] - Reseted.");
#endif
            connectionCounter = 0;
        }

        /// <summary>
        /// Process a single incoming packet. Returns whether we should keep processing packets or not.
        /// </summary>

        bool ProcessListenerPacket(Buffer buffer, IPEndPoint ip)
        {
#if DEBUG
            UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessListenerPacket()] - Processing. Status: " + Status.ToString());
#endif

            // Verification step must be passed first
            if (Status == TcpProtocol.ConnectionStatus.Verifying)
            {
                BinaryReader reader = buffer.BeginReading();
                if (buffer.size == 0) return true;

                int packetID = reader.ReadByte();
                Packet response = (Packet)packetID;

                if (response == Packet.ResponseID)
                {
                    if (VerifyResponseID(response, reader))
                    {
#if DEBUG
                        UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessListenerPacket()] - Verified. Id: " + Id);
#endif
                        return true;
                    }
                    else
                    {
#if DEBUG
                        UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessListenerPacket()] - Not Verified.");
#endif
                        return false;
                    }
                }
            }
            else if (Status == ConnectionStatus.Connected)
            {
#if DEBUG
                UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessListenerPacket()] - Packet received.");
#endif
                if (OnClientPacketReceived != null)
                    OnClientPacketReceived.Invoke(buffer, ip);
            }
#if DEBUG
            UnityEngine.Debug.Log("[Client][TcpProtocol:ProcessListenerPacket()] - Processed. Status: " + Status.ToString());
#endif
            return true;
        }

        /// <summary>
        /// Process all incoming packets.
        /// </summary>

        public void ThreadProcessListenerPackets()
        {
            if (multiThreaded)
            {
                while (true)
                {
                    if (!ProcessListenerPackets())
                        Thread.Sleep(1);
                }
            }
            else
                ProcessListenerPackets();
        }

        bool ProcessListenerPackets()
        {
#if DEBUG
            UnityEngine.Debug.Log("[Client] - Running ThreadProcessListenerPackets.");
#endif
            bool received = false;

            lock (listenerLockObj)
            {
                Buffer buffer = null;

                //receive all packets
                while (ReceivePacket(out buffer))
                {
                    try
                    {
                        received = ProcessListenerPacket(buffer, null);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogException(ex);
                    }

                    buffer.Recycle();
                }
            }

            return received;
        }

        /// <summary>
        /// Process incoming packets in the Unity Update function.
        /// </summary>
        public void UpdateClient()
        {
            if (clientThread == null && socket != null) ThreadProcessListenerPackets();
        }

        public bool CheckClientThread()
        {
            return clientThread != null && clientThread.IsAlive;
        }

        #endregion

        #region TcpListener Methods

        /// <summary>
        /// Start listening to incoming connections on the specified port.
        /// </summary>

        public bool StartListener(int tcpPort, bool multiThreaded = true)
        {
            this.multiThreaded = multiThreaded;
            StopListener();

            try
            {
                listenerPort = tcpPort;
                listener = new TcpListener(IPAddress.Any, tcpPort);
                listener.Start(50);
            }
            catch (System.Exception ex)
            {
                Error(ex.Message);
                return false;
            }
#if DEBUG
            UnityEngine.Debug.Log("[Listener][TcpProtocol:StartListener()] - Game server started on port " + tcpPort + " using protocol version " + VERSION);
#endif
            if (multiThreaded)
            {
                listenerThread = new Thread(ThreadProcessClientPackets);
                listenerThread.Start();
            }

            return true;
        }

        /// <summary>
        /// Stop listening to incoming connections and disconnect all players.
        /// </summary>

        public void StopListener()
        {
#if DEBUG
            UnityEngine.Debug.Log("[Listener][TcpProtocol:StopListener()] - Stopped.");
#endif

            // Stop the worker thread
            if (listenerThread != null)
            {
                listenerThread.Abort();
                listenerThread = null;
            }

            // Stop listening
            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }

            // Player counter should be reset
            TcpProtocol.ResetConnectionsCounter();
        }

        /// <summary>
        /// Stop listening to incoming connections but keep the server running.
        /// </summary>

        public void RefuseConnections() { listenerPort = 0; }

        /// <summary>
        /// Thread that will be processing incoming data.
        /// </summary>

        void ThreadProcessClientPackets()
        {
            if(multiThreaded)
            {
                while(true)
                {
                    if (!ProcessClientPackets())
                        Thread.Sleep(1);
                }
            }
            else
                ProcessClientPackets();
        }

        bool ProcessClientPackets()
        {
            bool received = false;

            lock (listenerLockObj)
            {
                Buffer buffer;
                time = DateTime.UtcNow.Ticks / 10000;

                // Stop the listener if the port is 0 (MakePrivate() was called)
                if (listenerPort == 0)
                {
                    if (listener != null)
                    {
                        listener.Stop();
                        listener = null;
                    }
                }
                else
                {
                    // Add all pending connections
                    while (listener != null && listener.Pending())
                    {
                        AddClient(listener.AcceptSocket());
                    }
                }
#if DEBUG
                UnityEngine.Debug.Log("[Listener] - Running ThreadProcessClientPackets. Clients: " + clients.size);
#endif
                // Process player connections next
                for (int i = 0; i < clients.size;)
                {
                    TcpProtocol client = clients[i];

                    // Process up to 100 packets at a time
                    for (int b = 0; b < 100 && client.ReceivePacket(out buffer); ++b)
                    {
                        if (buffer.size > 0)
                        {
                            if (multiThreaded)
                            {
                                try
                                {
                                    if (ProcessClientPacket(buffer, client))
                                        received = true;
                                }
                                catch (System.Exception ex)
                                {
                                    UnityEngine.Debug.LogException(ex);
                                    Error("(Listener ThreadFunction Process) " + ex.Message + "\n" + ex.StackTrace);
                                    RemoveClient(client);
                                }
                            }
                            else
                            {
                                if (ProcessClientPacket(buffer, client))
                                    received = true;
                            }
                        }

                        buffer.Recycle();
                    }

                    // Time out -- disconnect this player
//                    if (client.Status == ConnectionStatus.Connected)
//                    {
//                        // If the player doesn't send any packets in a while, disconnect him
//                        if (client.TimeoutTime > 0 && client.LastReceivedTime + client.TimeoutTime < time)
//                        {
//#if DEBUG
//                            UnityEngine.Debug.LogWarning("[TcpProtocol:StopListener()] - Client " + client.IpAddress + " has timed out");
//#endif
//                            RemoveClient(client);
//                            continue;
//                        }
//                    }
//                    else if (client.LastReceivedTime + 2000 < time)
//                    {
//#if DEBUG
//                        UnityEngine.Debug.LogWarning("[TcpProtocol:StopListener()] - Client " + client.IpAddress + " has timed out");
//#endif
//                        RemoveClient(client);
//                        continue;
//                    }
                    ++i;
                }
            }

            return received;
        }

        /// <summary>
        /// Call this function when you've disabled multi-threading.
        /// </summary>

        public void UpdateListener() { if (listenerThread == null && listener != null) ThreadProcessClientPackets(); }

        /// <summary>
        /// Add a new player entry.
        /// </summary>

        TcpProtocol AddClient(Socket socket)
        {
            TcpProtocol client = new TcpProtocol();
            client.StartReceiving(socket);
            clients.Add(client);
            return client;
        }

        /// <summary>
        /// Remove the specified player.
        /// </summary>

        void RemoveClient(TcpProtocol client)
        {
            if (client != null)
            {
                client.Release();
                clients.Remove(client);

                if (client.Id != 0)
                {
                    if (clientsDictionary.Remove(client.Id))
                    {

                    }

                    client.Id = 0;
                }
            }
        }

        /// <summary>
        /// Retrieve a player by their ID.
        /// </summary>

        TcpProtocol GetClient(int id)
        {
            TcpProtocol p = null;
            clientsDictionary.TryGetValue(id, out p);
            return p;
        }

        /// <summary>
        /// Send a buffer to player by ID
        /// </summary>
        public void SendToClient(int id, Buffer buffer)
        {
            if(clientsDictionary.ContainsKey(id))
                clientsDictionary[id].SendTcpPacket(buffer);
        }

        /// <summary>
	    /// Receive and process a single incoming packet.
	    /// Returns 'true' if a packet was received, 'false' otherwise.
	    /// </summary>

        bool ProcessClientPacket(Buffer buffer, TcpProtocol client)
        {
#if DEBUG
            UnityEngine.Debug.Log("[Listener][TcpProtocol:ProcessClientPacket()] - Processing. Status: " + client.Status.ToString());
#endif
            // If the player has not yet been verified, the first packet must be an ID request
            if (client.Status == TcpProtocol.ConnectionStatus.Verifying)
            {
                if (client.VerifyRequestID(buffer, true))
                {
#if DEBUG
                    UnityEngine.Debug.Log("[Listener][TcpProtocol:ProcessClientPacket()] - Client verified. Id: " + client.Id);
#endif
                    clientsDictionary.Add(client.Id, client);

                    if (OnClientConnect != null)
                        OnClientConnect.Invoke(client.Id, client);

                    return true;
                }

                RemoveClient(client);
                return false;
            }
            else if (client.Status == TcpProtocol.ConnectionStatus.Connected)
            {
                UnityEngine.Debug.Log("[Listener][TcpProtocol:ProcessClientPacket()] - Packet received.");

                if (OnListenerPacketReceived != null)
                    OnListenerPacketReceived.Invoke(buffer, client);
            }

#if DEBUG
            UnityEngine.Debug.Log("[Listener][TcpProtocol:ProcessClientPacket()] - Processed. Status: " + client.Status.ToString());
#endif
            return true;
        }

        #endregion
    }
}