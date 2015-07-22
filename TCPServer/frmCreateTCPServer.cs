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

namespace TCPServer
{
    public partial class frmCreateTCPServer : Form
    {
        public frmCreateTCPServer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (TCPServerManager.ServerExist(textBox2.Text))
            {
                MessageBox.Show("服务器已存在!");
                return;
            }

            TCPServerManager manager = new TCPServerManager(textBox2.Text);  //创建服务器
            manager.Start(int.Parse(textBox1.Text)); //启动服务器

            frmTCPServer frmtcpserver = new frmTCPServer(textBox2.Text, int.Parse(textBox1.Text));
            frmtcpserver.Show();
        }
    }
}
