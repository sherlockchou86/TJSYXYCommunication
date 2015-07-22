using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TJSYXY.Communication.UDP
{
    /// <summary>
    /// UDP消息参数
    /// </summary>
    public class UDPMessageReceivedEventArgs : EventArgs
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
            get;
            set;
        }
        /// <summary>
        /// 远程IP
        /// </summary>
        public string RemoteIP
        {
            set;
            get;
        }
        /// <summary>
        /// 远程端口
        /// </summary>
        public int RemotePort
        {
            set;
            get;
        }
        /// <summary>
        /// 消息接收时间
        /// </summary>
        public DateTime Time
        {
            set;
            get;
        }
    }
    /// <summary>
    /// 表示处理UDP消息的方法
    /// </summary>
    /// <param name="csID">激发该事件的客户端ID</param>
    /// <param name="args">消息参数</param>
    public delegate void UDPMessageReceivedEventHandler(string csID, UDPMessageReceivedEventArgs args);
}
