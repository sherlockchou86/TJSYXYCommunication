using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace TJSYXY.Communication.TCP
{
    /// <summary>
    /// 说明：
    /// TCP通信客户端
    /// 信息：
    /// 周智 2015.07.20
    /// </summary>
    class TCPClient
    {
        private Socket _socket;  //客户端socket
        private string _client_id;  //客户端ID

        private BackgroundWorker _pulse_test = new BackgroundWorker();

        private int _pulse = 3;
        /// <summary>
        /// 心跳包发送时间间隔（应小于服务器端设置的Pulse）
        /// </summary>
        public int Pulse
        {
            get
            {
                return _pulse;
            }
            set
            {
                _pulse = value;
            }
        }

        private static Dictionary<string, TCPClient> _tcpClients;
        /// <summary>
        /// 客户端列表
        /// </summary>
        public static Dictionary<string, TCPClient> TCPClients
        {
            get
            {
                if (_tcpClients == null)
                {
                    _tcpClients = new Dictionary<string, TCPClient>();
                }
                return _tcpClients;
            }
        }

        /// <summary>
        /// 客户端连接状态
        /// </summary>
        public bool Connected
        {
            get;
            set;
        }
        /// <summary>
        /// 接收到服务器的消息时激发该事件
        /// </summary>
        public event TCPMessageReceivedEventHandler TCPMessageReceived;
        /// <summary>
        /// 客户端连入服务器时激发该事件
        /// </summary>
        public event TCPClientConnectedEventHandler TCPClientConnected;
        /// <summary>
        /// 客户端断开服务器时激发该事件
        /// </summary>
        public event TCPClientDisConnectedEventHandler TCPClientDisConnected;
        /// <summary>
        /// 在心跳包发送时检测出客户端断线时激发该事件
        /// </summary>
        public event TCPClientDisConnected4PulseEventHandler TCPClientDisConnected4Pulse;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="client_id">客户端ID</param>
        public TCPClient(string client_id)
        {
            _client_id = client_id;
            _pulse_test.DoWork += new DoWorkEventHandler(_pulse_test_DoWork);
            _pulse_test.RunWorkerAsync();
        }
        /// <summary>
        /// 心跳包发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pulse_test_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_pulse_test.CancellationPending)
            {
                try
                {
                    if (Connected)
                    {
                        Send(Msg.Default, new byte[] { 0 });  //心跳包发送
                    }
                }
                catch
                {
                    if (TCPClientDisConnected4Pulse != null)
                    {
                        TCPClientDisConnected4Pulse(_client_id, -1);
                    }
                }
                Thread.Sleep(_pulse * 1000);
            }
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        public void Connect(string ip, int port)
        {
            if (!Connected)
            {
                if (_socket == null)
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                _socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                Connected = true;
                TCPEndPoint end = new TCPEndPoint();
                end.Socket = _socket;
                end.Socket.BeginReceive(end.Buffer, 0, 1024, SocketFlags.None, new AsyncCallback(OnReceive), end);
                if (TCPClientConnected != null)  //通知新客户端连入
                {
                    TCPClientConnectedEventArgs args = new TCPClientConnectedEventArgs();
                    args.End = end;
                    args.Time = DateTime.Now;
                    TCPClientConnected(_client_id, args);
                }
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            if (Connected)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                Connected = false;
            }
        }
        /// <summary>
        /// 接收数据回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceive(IAsyncResult ar)
        {
            TCPEndPoint end = ar.AsyncState as TCPEndPoint;
            try
            {
                int real_recv = end.Socket.EndReceive(ar);
                end.MStream.Write(end.Buffer, 0, real_recv); //写入消息缓冲区
                //尝试读取一条完整消息
                ZMessage msg;
                while (end.MStream.ReadMessage(out msg))
                {
                    //处理消息
                    if (TCPMessageReceived != null)
                    {
                        TCPMessageReceivedEventArgs args = new TCPMessageReceivedEventArgs();
                        args.Msg = (Msg)msg.head;
                        args.Time = DateTime.Now;
                        args.End = end;
                        args.Data = msg.content;
                        TCPMessageReceived.BeginInvoke(_client_id, args, null, null);  //激发事件，通知事件注册者处理消息
                    }
                }
                end.Socket.BeginReceive(end.Buffer, 0, 1024, SocketFlags.None, new AsyncCallback(OnReceive), end);
            }
            catch
            {
                if (TCPClientDisConnected != null)  //通知客户端断开
                {
                    TCPClientDisConnectedEventArgs args = new TCPClientDisConnectedEventArgs();
                    args.End = end;
                    args.Time = DateTime.Now;
                    TCPClientDisConnected(_client_id, args);
                }     
            }
        }

        /// <summary>
        /// 向服务器同步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        public void Send(Msg msg, byte[] data)
        {
            byte[] buffer2send = new byte[5 + data.Length];  // 1 + 4 + data
            BinaryWriter sWriter = new BinaryWriter(new MemoryStream(buffer2send));

            sWriter.Write((byte)msg);
            sWriter.Write(data.Length);
            sWriter.Write(data);
            sWriter.Close();

            _socket.Send(buffer2send);  //同步
        }
        /// <summary>
        /// 向服务器异步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        /// <param name="callback">回调方法</param>
        public void SendAsync(Msg msg, byte[] data, AsyncCallback callback)
        {
            byte[] buffer2send = new byte[5 + data.Length];  // 1 + 4 + data
            BinaryWriter sWriter = new BinaryWriter(new MemoryStream(buffer2send));

            sWriter.Write((byte)msg);
            sWriter.Write(data.Length);
            sWriter.Write(data);
            sWriter.Close();

            _socket.BeginSend(buffer2send, 0, buffer2send.Length, SocketFlags.None, callback, _socket);  //异步
        }
    }
}
