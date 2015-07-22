using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TJSYXY.Communication
{
    /// <summary>
    /// 说明：
    /// 消息缓冲区，TCP通信中确保能接收到完整的消息包
    /// 信息：
    /// 周智 2015.07.20
    /// </summary>
    class ZMessageStream
    {
        private byte[] buffer;
        private int length;
        private int capacity;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ZMessageStream()
        {
            buffer = new byte[0];
            length = 0;
            capacity = 0;
        }


        /// <summary>
        /// 从缓冲区中读初一条完整消息
        /// </summary>
        /// <param name="msg">读出的完整消息</param>
        /// <returns>如果为true,读取了完整消息</returns>
        public bool ReadMessage(out ZMessage msg)    //从流中读取完整ZMessage对象
        {
            ZMessage temp = new ZMessage();
            if (length >= 5)
            {
                MemoryStream stream = new MemoryStream(buffer);
                BinaryReader reader = new BinaryReader(stream);

                temp.head = reader.ReadByte();
                temp.length = reader.ReadInt32();

                if (temp.length <= (length - 5))
                {
                    temp.content = reader.ReadBytes(temp.length);
                    reader.Close();
                    Remove(temp.length + 5);
                    msg = temp;
                    return true;
                }
                else
                {
                    msg = null;
                    return false;
                }

            }
            else
            {
                msg = null;
                return false;
            }
        }


        /// <summary>
        /// 移出缓冲区
        /// </summary>
        /// <param name="count">要移出缓冲区的字节数</param>
        public void Remove(int count)
        {
            if (count <= length)
            {
                byte[] b = new byte[length - count];
                Buffer.BlockCopy(buffer, count, b, 0, length - count);
                length = capacity = length - count;
                buffer = b;
            }
            else
            {
                buffer = new byte[0];
                length = capacity = 0;
            }
        }


        /// <summary>
        /// 将字节流写入缓冲区
        /// </summary>
        /// <param name="bufferEx">要写入缓冲区的字节流</param>
        /// <param name="offset">写入字节流的开始位置</param>
        /// <param name="count">写入字节大小</param>
        public void Write(byte[] bufferEx, int offset, int count)
        {
            if (count > bufferEx.Length - offset)
            {
                count = bufferEx.Length - offset;
            }
            EnsureCapacity(length + count);//再写入之前，判断容量大小

            Buffer.BlockCopy(bufferEx, offset, buffer, length, count);
            length += count;

        }


        /// <summary>
        /// 确保容量足够大
        /// </summary>
        /// <param name="count"></param>
        public void EnsureCapacity(int count)
        {
            if (count <= capacity)
            {
                return;
            }
            if (count < 2 * capacity)
            {
                count = 2 * capacity;
            }

            byte[] bufferEx = new byte[count];
            capacity = count;
            Buffer.BlockCopy(buffer, 0, bufferEx, 0, length);
            buffer = bufferEx;
        }
    }
}
