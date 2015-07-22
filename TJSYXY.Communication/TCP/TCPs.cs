using System;
using System.Collections.Generic;
using System.Text;

namespace TJSYXY.Communication.TCP
{
    /// <summary>
    /// TCP消息参数
    /// </summary>
    public class TCPMessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public Msg Msg
        {
            get;
            set;
        }
        /// <summary>
        /// 消息数据
        /// </summary>
        public byte[] Data
        {
            set;
            get;
        }
        /// <summary>
        /// 终端
        /// </summary>
        public TCPEndPoint End
        {
            get;
            set;
        }
        /// <summary>
        /// 消息接收时间
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 表示处理TCP消息的方法
    /// </summary>
    /// <param name="csID">激发该事件的服务器（或客户端）ID</param>
    /// <param name="args">消息参数</param>
    public delegate void TCPMessageReceivedEventHandler(string csID, TCPMessageReceivedEventArgs args);

    /// <summary>
    /// TCP通信时客户端连入事件参数
    /// </summary>
    public class TCPClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// 新连入的终端
        /// </summary>
        public TCPEndPoint End
        {
            get;
            set;
        }
        /// <summary>
        /// 连入时间
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 表示处理客户端连入服务器这一事件的方法
    /// </summary>
    /// <param name="csID">激发该事件的服务器（或客户端）ID</param>
    /// <param name="args">消息参数</param>
    public delegate void TCPClientConnectedEventHandler(string csID, TCPClientConnectedEventArgs args);

    /// <summary>
    /// TCP通信时客户端断开事件参数
    /// </summary>
    public class TCPClientDisConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// 断开服务器的终端（已经失效）
        /// </summary>
        public TCPEndPoint End
        {
            get;
            set;
        }
        /// <summary>
        /// 断开时间
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 表示处理客户端断开服务器这一事件的方法
    /// </summary>
    /// <param name="csID">激发该事件的服务器（或客户端）ID</param>
    /// <param name="args">消息参数</param>
    public delegate void TCPClientDisConnectedEventHandler(string csID,TCPClientDisConnectedEventArgs args);

    /// <summary>
    /// 表示处理在心跳检测时发现断线这一事件的方法
    /// </summary>
    /// <param name="csID">激发该事件的服务器（或客户端）ID</param>
    /// <param name="uid">消息参数</param>
    public delegate void TCPClientDisConnected4PulseEventHandler(string csID, int uid);
}
