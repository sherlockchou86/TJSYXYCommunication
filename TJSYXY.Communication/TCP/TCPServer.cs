using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace TJSYXY.Communication.TCP
{
    /// <summary>
    /// 说明：
    /// TCP通信服务器
    /// 信息：
    /// 周智 2015.07.20
    /// </summary>
    class TCPServer
    {
        private Socket _socket; //服务端监听socket
        private string _server_id;  //服务器ID
        private int _client_index;  //当前连入客户端index

        private ConcurrentDictionary<int, int> _pulse_time = new ConcurrentDictionary<int, int>();
        private BackgroundWorker _pulse_test = new BackgroundWorker();

        private static Dictionary<string, TCPServer> _tcpServers;
        /// <summary>
        /// 服务器列表
        /// </summary>
        public static Dictionary<string,TCPServer> TCPServers
        {
            get
            {
                if (_tcpServers == null)
                {
                    _tcpServers = new Dictionary<string, TCPServer>();
                }
                return _tcpServers;
            }
        }
        /// <summary>
        /// 服务器状态
        /// </summary>
        public bool Runing
        {
            get;
            set;
        }

        private int _pulse = 5; 
        /// <summary>
        /// 服务器端心跳检测时间
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

        /// <summary>
        /// 接收到TCP消息时激发该事件
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
        /// 在心跳检测发现断线时激发该事件
        /// </summary>
        public event TCPClientDisConnected4PulseEventHandler TCPClientDisConnected4Pulse;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="server_id">服务器ID</param>
        public TCPServer(string server_id)
        {
            _server_id = server_id;
            _pulse_test.DoWork += new DoWorkEventHandler(_pulse_test_DoWork);
            _pulse_test.RunWorkerAsync();
        }
        /// <summary>
        /// 心跳检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pulse_test_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_pulse_test.CancellationPending)
            {
                foreach (KeyValuePair<int, int> p in _pulse_time)
                {
                    _pulse_time[p.Key] = p.Value - 1;
                    if (_pulse_time[p.Key] == 0)  //心跳检测断线
                    {
                        if (TCPClientDisConnected4Pulse != null)
                        {
                            TCPClientDisConnected4Pulse(_server_id, p.Key);
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 按照给定的端口号开启服务器
        /// </summary>
        /// <param name="port">侦听端口号</param>
        public void Start(int port)
        {
            if (!Runing)
            {
                string ip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                if (_socket == null)
                {
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                _socket.Listen(256);
                _socket.BeginAccept(new AsyncCallback(OnAccept), null);  //开始第一次接受客户端连接
                Runing = true;
            }
        }
        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Stop()
        {
            if (Runing)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                Runing = false;
            }
        }

        /// <summary>
        /// 客户端连入回调方法
        /// </summary>
        /// <param name="ar"></param>
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket new_socket = _socket.EndAccept(ar);
                TCPEndPoint end = new TCPEndPoint();
                end.Socket = new_socket;
                end.UID = _client_index++;
                end.Socket.BeginReceive(end.Buffer, 0, 1024, SocketFlags.None, new AsyncCallback(OnReceive), end);  //开始第一次数据接收
                _socket.BeginAccept(new AsyncCallback(OnAccept), null);  //开始接受下一次客户端连接
                _pulse_time.TryAdd(end.UID, _pulse); //加入心跳检测
                if (TCPClientConnected != null)  //通知新客户端连入
                {
                    TCPClientConnectedEventArgs args = new TCPClientConnectedEventArgs();
                    args.End = end;
                    args.Time = DateTime.Now;
                    TCPClientConnected(_server_id, args);
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// 接收数据回调方法
        /// </summary>
        /// <param name="ar">回调参数</param>
        private void OnReceive(IAsyncResult ar)
        {
            TCPEndPoint end = ar.AsyncState as TCPEndPoint;
            try
            {
                int real_recv = end.Socket.EndReceive(ar);
                end.MStream.Write(end.Buffer, 0, real_recv);  //写入消息缓冲区
                //尝试读取一条完整消息
                ZMessage msg;
                while (end.MStream.ReadMessage(out msg))
                {
                    if ((Msg)msg.head == Msg.Default)  //心跳包 跳过
                    {
                        foreach (KeyValuePair<int, int> p in _pulse_time)
                        {
                            if (p.Key == end.UID)
                            {
                                _pulse_time[p.Key] = _pulse; //重置
                                break;
                            }
                        }
                        continue;
                    }
                    //处理消息
                    if (TCPMessageReceived != null)
                    {
                        TCPMessageReceivedEventArgs args = new TCPMessageReceivedEventArgs();
                        args.Msg = (Msg)msg.head;
                        args.Time = DateTime.Now;
                        args.End = end;
                        args.Data = msg.content;
                        TCPMessageReceived.BeginInvoke(_server_id, args, null, null);  //激发事件，通知事件注册者处理消息
                    }
                }
                end.Socket.BeginReceive(end.Buffer, 0, 1024, SocketFlags.None, new AsyncCallback(OnReceive), end);  //开始下一次数据接收
            }
            catch
            {
                if (TCPClientDisConnected != null)  //通知客户端断开
                {
                    TCPClientDisConnectedEventArgs args = new TCPClientDisConnectedEventArgs();
                    args.End = end;
                    args.Time = DateTime.Now;
                    TCPClientDisConnected(_server_id, args);
                }
                int tmp;
                _pulse_time.TryRemove(end.UID,out tmp);
            }
        }

        /// <summary>
        /// 给指定终端同步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        /// <param name="end">指定终端</param>
        public void Send(Msg msg, byte[] data, TCPEndPoint end)
        {
            byte[] buffer2send = new byte[5 + data.Length];  // 1 + 4 + data
            BinaryWriter sWriter = new BinaryWriter(new MemoryStream(buffer2send));

            sWriter.Write((byte)msg);
            sWriter.Write(data.Length);
            sWriter.Write(data);
            sWriter.Close();

            end.Socket.Send(buffer2send);  //同步
        }
        /// <summary>
        /// 给指定终端异步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        /// <param name="end">指定终端</param>
        /// <param name="callback">回调方法</param>
        public void SendAsync(Msg msg, byte[] data, TCPEndPoint end, AsyncCallback callback)
        {
            byte[] buffer2send = new byte[5 + data.Length];  // 1 + 4 + data
            BinaryWriter sWriter = new BinaryWriter(new MemoryStream(buffer2send));

            sWriter.Write((byte)msg);
            sWriter.Write(data.Length);
            sWriter.Write(data);
            sWriter.Close();

            end.Socket.BeginSend(buffer2send, 0, buffer2send.Length, SocketFlags.None, callback, end);  //异步
        }
    }
}
