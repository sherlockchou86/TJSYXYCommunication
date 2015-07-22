using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace TJSYXY.Communication.TCP
{
    /// <summary>
    /// 说明：
    /// TCP通信过程中的终端
    /// 信息：
    /// 周智 2015.07.20
    /// </summary>
    public class TCPEndPoint
    {
        private Socket _socket;  //负责通信的Socket
        private byte[] _buffer = new byte[1024];  //接收数据系统缓冲区
        private ZMessageStream _mStream = new ZMessageStream();  //消息缓冲区

        /// <summary>
        /// 负责通信的Socket
        /// </summary>
        internal Socket Socket
        {
            get
            {
                return _socket;
            }
            set
            {
                _socket = value;
            }
        }
        /// <summary>
        /// 接收数据系统缓冲区
        /// </summary>
        internal byte[] Buffer
        {
            get
            {
                return _buffer;
            }
        }
        /// <summary>
        /// 消息缓冲区
        /// </summary>
        internal ZMessageStream MStream
        {
            get
            {
                return _mStream;
            }
        }
        /// <summary>
        /// 终端唯一标识[服务器端有效]
        /// </summary>
        public int UID
        {
            get;
            set;
        }
        /// <summary>
        /// 远程终端IP
        /// </summary>
        public string RemoteIP
        {
            get
            {
                try
                {
                    return ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 远程终端Port
        /// </summary>
        public int RemotePort
        {
            get
            {
                try
                {
                    return ((IPEndPoint)_socket.RemoteEndPoint).Port;
                }
                catch
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// 尝试关闭当前Socket连接
        /// </summary>
        public void TryClose()
        {
            try
            {
                _socket.Close();
            }
            catch
            {

            }
        }
    }
}
