using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TJSYXY.Communication;
using TJSYXY.Communication.TCP;

namespace TCPClient
{
    public partial class frmCreateTCPClient : Form
    {
        public frmCreateTCPClient()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 创建客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (TCPClientManager.ClientExist(textBox3.Text))
            {
                MessageBox.Show("客户端已存在！");
                return;
            }

            TCPClientManager manager = new TCPClientManager(textBox3.Text);  //创建客户端
            manager.Connect(textBox1.Text, int.Parse(textBox2.Text));

            frmTCPClient frmtcpclient = new frmTCPClient(textBox3.Text);
            frmtcpclient.Show();
        }
    }
}
