using System;
using System.Collections.Generic;
using System.Text;

namespace TJSYXY.Communication.TCP
{
    /// <summary>
    /// 说明：
    /// TCP通信服务器的代理
    /// 信息：
    /// 周智  2015.07.20
    /// </summary>
    public class TCPServerManager
    {
        private string _server_id;  //当前要代理的服务器ID

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="server_id">要代理的服务器id，若不存在，则使用server_id创建</param>
        public TCPServerManager(string server_id)
        {
            _server_id = server_id;
            if (!TCPServer.TCPServers.ContainsKey(server_id))
            {
                TCPServer.TCPServers.Add(server_id, new TCPServer(server_id));
            }
        }
        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="port">侦听端口号</param>
        public void Start(int port)
        {
            TCPServer.TCPServers[_server_id].Start(port);
        }
        /// <summary>
        /// 关闭服务器，只结束侦听
        /// </summary>
        public void Stop()
        {
            TCPServer.TCPServers[_server_id].Stop();
        }
        /// <summary>
        /// 向指定终端同步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        /// <param name="end">指定终端</param>
        public void Send(Msg msg, byte[] data, TCPEndPoint end)
        {
            TCPServer.TCPServers[_server_id].Send(msg, data, end);
        }
        /// <summary>
        /// 向指定终端异步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">消息数据正文</param>
        /// <param name="end">指定终端</param>
        /// <param name="callback">回调方法</param>
        public void SendAsync(Msg msg, byte[] data, TCPEndPoint end, AsyncCallback callback)
        {
            TCPServer.TCPServers[_server_id].SendAsync(msg, data, end, callback);
        }

        /// <summary>
        /// 服务器接收到消息时激发该事件
        /// </summary>
        public event TCPMessageReceivedEventHandler TCPMessageReceived
        {
            add
            {
                TCPServer.TCPServers[_server_id].TCPMessageReceived += value;
            }
            remove
            {
                TCPServer.TCPServers[_server_id].TCPMessageReceived -= value;
            }
        }
        /// <summary>
        /// 客户端连入服务器时激发该事件
        /// </summary>
        public event TCPClientConnectedEventHandler TCPClientConnected
        {
            add
            {
                TCPServer.TCPServers[_server_id].TCPClientConnected += value;
            }
            remove
            {
                TCPServer.TCPServers[_server_id].TCPClientConnected -= value;
            }
        }
        /// <summary>
        /// 客户端断开服务器时激发该事件
        /// </summary>
        public event TCPClientDisConnectedEventHandler TCPClientDisConnected
        {
            add
            {
                TCPServer.TCPServers[_server_id].TCPClientDisConnected += value;
            }
            remove
            {
                TCPServer.TCPServers[_server_id].TCPClientDisConnected -= value;
            }
        }
        /// <summary>
        /// 在心跳检测发现断线时激发该事件
        /// </summary>
        public event TCPClientDisConnected4PulseEventHandler TCPClientDisConnected4Pulse
        {
            add
            {
                TCPServer.TCPServers[_server_id].TCPClientDisConnected4Pulse += value;
            }
            remove
            {
                TCPServer.TCPServers[_server_id].TCPClientDisConnected4Pulse -= value;
            }
        }

        /// <summary>
        /// 服务器状态
        /// </summary>
        public bool Runing
        {
            get
            {
                return TCPServer.TCPServers[_server_id].Runing;
            }
        }
        /// <summary>
        /// 服务器心跳检测时间，默认为5秒（应大于客户端的Pulse）
        /// </summary>
        public int Pulse
        {
            get
            {
                return TCPServer.TCPServers[_server_id].Pulse;
            }
            set
            {
                TCPServer.TCPServers[_server_id].Pulse = value;
            }
        }
        /// <summary>
        /// 检查指定服务器是否存在
        /// </summary>
        /// <param name="server_id"></param>
        /// <returns></returns>
        public static bool ServerExist(string server_id)
        {
            return TCPServer.TCPServers.ContainsKey(server_id);
        }
    }
}
