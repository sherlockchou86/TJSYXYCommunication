using System;
using System.Collections.Generic;
using System.Text;

namespace TJSYXY.Communication.UDP
{
    /// <summary>
    /// 说明：
    /// UDP客户端的代理
    /// 信息：
    /// 周智 2015.07.20
    /// </summary>
    public class UDPClientManager
    {
        private string _client_id; //要代理的客户端ID

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="client_id">要代理的客户端ID，若不存在，则使用该client_id创建</param>
        public UDPClientManager(string client_id)
        {
            _client_id = client_id;
            if (!UDPClient.UDPClients.ContainsKey(_client_id))
            {
                UDPClient.UDPClients.Add(_client_id, new UDPClient(_client_id));
            }
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        /// <param name="port">监听端口号</param>
        public void Start(int port)
        {
            UDPClient.UDPClients[_client_id].Start(port);
        }
        /// <summary>
        /// 接收到消息时激发该事件
        /// </summary>
        public event UDPMessageReceivedEventHandler UDPMessageReceived
        {
            add
            {
                UDPClient.UDPClients[_client_id].UDPMessageReceived += value;
            }
            remove
            {
                UDPClient.UDPClients[_client_id].UDPMessageReceived -= value;
            }
        }
        /// <summary>
        /// 检查客户端是否存在
        /// </summary>
        /// <param name="client_id">要检查的客户端ID</param>
        /// <returns></returns>
        public static bool ClientExist(string client_id)
        {
            return UDPClient.UDPClients.ContainsKey(client_id);
        }
        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="msg">消息类型</param>
        /// <param name="data">数据正文</param>
        /// <param name="remoteIP">远程IP</param>
        /// <param name="remotePort">远程端口</param>
        public void SendTo(Msg msg, byte[] data, string remoteIP, int remotePort)
        {
            UDPClient.UDPClients[_client_id].SendTo(msg, data, remoteIP, remotePort);
        }
        /// <summary>
        /// 客户端端口监听状态
        /// </summary>
        public bool Runing
        {
            get
            {
                return UDPClient.UDPClients[_client_id].Runing;
            }
        }
        /// <summary>
        /// 停止监听端口
        /// </summary>
        public void Stop()
        {
            UDPClient.UDPClients[_client_id].Stop();
        }
    }
}
