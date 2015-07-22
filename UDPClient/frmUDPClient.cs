using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TJSYXY.Communication.UDP;
using TJSYXY.Communication;

namespace UDPClient
{
    public partial class frmUDPClient : Form
    {
        private string _client_id;
        private int _port;

        public frmUDPClient()
        {
            InitializeComponent();
        }
        public frmUDPClient(string client_id, int port)
            : this()
        {
            _client_id = client_id;
            _port = port;
        }
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmUDPClient_Load(object sender, EventArgs e)
        {
            Text = "UDPClient " + _client_id + ":" + _port;

            //注册事件
            UDPClientManager manager = new UDPClientManager(_client_id);
            manager.UDPMessageReceived += new UDPMessageReceivedEventHandler(manager_UDPMessageReceived);
        }
        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="csID"></param>
        /// <param name="args"></param>
        void manager_UDPMessageReceived(string csID, UDPMessageReceivedEventArgs args)
        {
            this.Invoke((Action)delegate()
            {
                if (args.Msg == Msg.Zmsg1)
                {
                    textBox1.AppendText(args.Time.ToLongTimeString() + " " + args.RemoteIP + ":" + args.RemotePort + " 发送文本：\n"
                        + Encoding.Unicode.GetString(args.Data) + "\n");
                }
            });
        }

        /// <summary>
        /// 发送文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            UDPClientManager manager = new UDPClientManager(_client_id);
            manager.SendTo(Msg.Zmsg1, Encoding.Unicode.GetBytes(textBox2.Text), textBox4.Text, int.Parse(textBox5.Text));
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            UDPClientManager manager = new UDPClientManager(_client_id);
            manager.Stop();
        }

    }
}
