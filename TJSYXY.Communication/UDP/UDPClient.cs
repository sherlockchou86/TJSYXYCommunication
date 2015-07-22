using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TJSYXY.Communication.UDP
{
    /// <summary>
    /// 说明：
    /// UDP通信客户端
    /// 信息：
    /// 周智 2015.07.20
    /// </summary>
    class UDPClient
    {
        private Socket _socket;  //客户端socket
        private string _client_id;  //客户端ID

        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0); //远程终端信息

        byte[] _buffer4recv = new byte[1024 * 64];  //数据接收系统缓冲区

        /// <summary>
        /// 客户端工作状态
        /// </summary>
        public bool Runing
        {
            get;
            set;
        }

        private static Dictionary<string, UDPClient> _udpClients;
        /// <summary>
        /// 客户端列表
        /// </summary>
        public static Dictionary<string, UDPClient> UDPClients
        {
            get
            {
                if (_udpClients == null)
                {
                    _udpClients = new Dictionary<string, UDPClient>();
                }
                return _udpClients;
            }
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="client_id">客户端ID</param>
        public UDPClient(string client_id)
        {
            _client_id = client_id;
        }
        /// <summary>
        /// 开启UDP监听
        /// </summary>
        /// <param name="port">监听端口号</param>
        public void Start(int port)
        {
            if (!Runing)
            {
                if (_socket == null)
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                string ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                _socket.BeginReceiveFrom(_buffer4recv, 0, 1024 * 64, SocketFlags.None, ref remoteEndPoint, new AsyncCallback(OnReceive), remoteEndPoint);
                Runing = true;
            }
        }
        /// <summary>
        /// 数据接收回调方法
        /// </summary>
        /// <param name="ar">回调参数</param>
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int real_recv = _socket.EndReceiveFrom(ar, ref remoteEndPoint);
                if (real_recv >= 1)
                {
                    if (UDPMessageReceived != null)
                    {
                        UDPMessageReceivedEventArgs args = new UDPMessageReceivedEventArgs();
                        args.Msg = (Msg)_buffer4recv[0];
                        args.RemoteIP = (remoteEndPoint as IPEndPoint).Address.ToString();
                        args.RemotePort = (remoteEndPoint as IPEndPoint).Port;
                        args.Time = DateTime.Now;
                        args.Data = new byte[real_recv - 1];
                        Buffer.BlockCopy(_buffer4recv, 1, args.Data, 0, real_recv - 1);
                        UDPMessageReceived(_client_id, args);
                    }
                }
                _socket.BeginReceiveFrom(_buffer4recv, 0, 1024 * 64, SocketFlags.None, ref remoteEndPoint, new AsyncCallback(OnReceive), remoteEndPoint);
            }
            catch
            {

            }
        }
        /// <summary>
        /// 接收到消息时激发该事件
        /// </summary>
        public event UDPMessageReceivedEventHandler UDPMessageReceived;
        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">数据正文</param>
        /// <param name="remoteIP">远程IP</param>
        /// <param name="remotePort">远程端口</param>
        public void SendTo(Msg msg, byte[] data,string remoteIP, int remotePort)
        {
            byte[] buffer2send = new byte[1 + data.Length];  // 1  + data
            BinaryWriter sWriter = new BinaryWriter(new MemoryStream(buffer2send));

            sWriter.Write((byte)msg);
            sWriter.Write(data);
            sWriter.Close();

            _socket.SendTo(buffer2send, new IPEndPoint(IPAddress.Parse(remoteIP), remotePort));  //同步发送数据
        }
        /// <summary>
        /// 停止监听端口
        /// </summary>
        public void Stop()
        {
            if (Runing)
            {
                _socket.Close();
                _socket = null;
                Runing = false;
            }
        }
    }
}
