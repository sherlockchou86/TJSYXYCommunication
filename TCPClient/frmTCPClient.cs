using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TJSYXY.Communication.TCP;
using TJSYXY.Communication;
using System.IO;
using System.Drawing.Imaging;

namespace TCPClient
{
    public partial class frmTCPClient : Form
    {
        private string _client_id;

        public frmTCPClient()
        {
            InitializeComponent();
        }
        public frmTCPClient(string client_id)
            : this()
        {
            _client_id = client_id;
        }
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmTCPClient_Load(object sender, EventArgs e)
        {
            Text = "TCPClient " + _client_id;

            //注册事件
            TCPClientManager manager = new TCPClientManager(_client_id); //访问客户端
            manager.TCPMessageReceived += new TCPMessageReceivedEventHandler(manager_TCPMessageReceived);
            manager.TCPClientDisConnected4Pulse += new TCPClientDisConnected4PulseEventHandler(manager_TCPClientDisConnected4Pulse);
            manager.TCPClientDisConnected += new TCPClientDisConnectedEventHandler(manager_TCPClientDisConnected);
        }

        #region
        /// <summary>
        /// 断线
        /// </summary>
        /// <param name="csID"></param>
        /// <param name="args"></param>
        void manager_TCPClientDisConnected(string csID, TCPClientDisConnectedEventArgs args)
        {
            this.Invoke((Action)delegate()
            {
                textBox1.AppendText(args.Time.ToLongTimeString() + " 与服务器断开连接\n");
            });
        }
        /// <summary>
        /// 心跳包发送失败
        /// </summary>
        /// <param name="csID"></param>
        /// <param name="uid"></param>
        void manager_TCPClientDisConnected4Pulse(string csID, int uid)
        {
            this.Invoke((Action)delegate()
            {
                textBox1.AppendText("发送心跳包失败\n");
            });
        }
        /// <summary>
        /// 收到消息
        /// </summary>
        /// <param name="csID"></param>
        /// <param name="args"></param>
        void manager_TCPMessageReceived(string csID, TCPMessageReceivedEventArgs args)
        {
            this.Invoke((Action)delegate()
            {
                if (args.Msg == Msg.Zmsg1)  //文本
                {
                    textBox1.AppendText(args.Time.ToLongTimeString() + " " + args.End.RemoteIP + ":" + args.End.RemotePort + " 发送文本:\n"
                        + Encoding.Unicode.GetString(args.Data) + "\n");
                }
                if (args.Msg == Msg.Zmsg2)  //图片
                {
                    textBox1.AppendText(args.Time.ToLongTimeString() + " " + args.End.RemoteIP + ":" + args.End.RemotePort + " 发送图片:\n"
                        + "见右方-->\n");
                    Image image = Image.FromStream(new MemoryStream(args.Data));
                    pictureBox1.Image = image;
                }
            });
        }
        #endregion

        /// <summary>
        /// 发送文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            TCPClientManager manager = new TCPClientManager(_client_id);
            manager.Send(Msg.Zmsg1, Encoding.Unicode.GetBytes(textBox2.Text)); //同步发送文本
        }
        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "图片文件|*.jpg;*jpeg";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox3.Text = ofd.FileName;
                    Image image = Image.FromFile(textBox3.Text);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Jpeg);

                        TCPClientManager manager = new TCPClientManager(_client_id);
                        manager.SendAsync(Msg.Zmsg2, ms.ToArray(), null);  //异步发送图片
                    }
                }
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            TCPClientManager manager = new TCPClientManager(_client_id);
            manager.DisConnect();
            Close();
        }
    }
}
