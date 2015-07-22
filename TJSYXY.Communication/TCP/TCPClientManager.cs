using System;
using System.Collections.Generic;
using System.Text;

namespace TJSYXY.Communication.TCP
{
    /// <summary>
    /// 说明：
    /// TCP通信客户端的代理
    /// 信息：
    /// 周智 2015.07.20
    /// </summary>
    public class TCPClientManager
    {
        private string _client_id;  //要代理的客户端ID

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="client_id">要代理的客户端ID，若不存在，则使用该client_id创建</param>
        public TCPClientManager(string client_id)
        {
            _client_id = client_id;
            if (!TCPClient.TCPClients.ContainsKey(_client_id))
            {
                TCPClient.TCPClients.Add(_client_id, new TCPClient(_client_id));
            }
        }
        /// <summary>
        /// 客户端连接状态
        /// </summary>
        public bool Connected
        {
            get
            {
                return TCPClient.TCPClients[_client_id].Connected;
            }
        }
        /// <summary>
        /// 心跳包发送时间间隔，默认为3秒（应小于服务器端Pulse）
        /// </summary>
        public int Pulse
        {
            get
            {
                return TCPClient.TCPClients[_client_id].Pulse;
            }
            set
            {
                TCPClient.TCPClients[_client_id].Pulse = value;
            }
        }
        /// <summary>
        /// 检查指定客户端是否存在
        /// </summary>
        /// <param name="client_id"></param>
        /// <returns></returns>
        public static bool ClientExist(string client_id)
        {
            return TCPClient.TCPClients.ContainsKey(client_id);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        public void Connect(string ip, int port)
        {
            TCPClient.TCPClients[_client_id].Connect(ip, port);
        }
        /// <summary>
        /// 断开服务器
        /// </summary>
        public void DisConnect()
        {
            TCPClient.TCPClients[_client_id].DisConnect();
        }
        /// <summary>
        /// 向服务器同步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        public void Send(Msg msg, byte[] data)
        {
            TCPClient.TCPClients[_client_id].Send(msg, data);
        }
        /// <summary>
        /// 向服务器异步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        /// <param name="callback">回调方法</param>
        public void SendAsync(Msg msg, byte[] data, AsyncCallback callback)
        {
            TCPClient.TCPClients[_client_id].SendAsync(msg, data, callback);
        }

        /// <summary>
        /// 接收到服务器的消息时激发该事件
        /// </summary>
        public event TCPMessageReceivedEventHandler TCPMessageReceived
        {
            add
            {
                TCPClient.TCPClients[_client_id].TCPMessageReceived += value;
            }
            remove
            {
                TCPClient.TCPClients[_client_id].TCPMessageReceived -= value;
            }
        }
        /// <summary>
        /// 客户端连入服务器时激发该事件
        /// </summary>
        public event TCPClientConnectedEventHandler TCPClientConnected
        {
            add
            {
                TCPClient.TCPClients[_client_id].TCPClientConnected += value;
            }
            remove
            {
                TCPClient.TCPClients[_client_id].TCPClientConnected -= value;
            }
        }
        /// <summary>
        /// 客户端断开服务器时激发该事件
        /// </summary>
        public event TCPClientDisConnectedEventHandler TCPClientDisConnected
        {
            add
            {
                TCPClient.TCPClients[_client_id].TCPClientDisConnected += value;
            }
            remove
            {
                TCPClient.TCPClients[_client_id].TCPClientDisConnected -= value;
            }
        }
        /// <summary>
        /// 在心跳包发送时检测出断线时激发该事件
        /// </summary>
        public event TCPClientDisConnected4PulseEventHandler TCPClientDisConnected4Pulse
        {
            add
            {
                TCPClient.TCPClients[_client_id].TCPClientDisConnected4Pulse += value;
            }
            remove
            {
                TCPClient.TCPClients[_client_id].TCPClientDisConnected4Pulse -= value;
            }
        }
    }
}
