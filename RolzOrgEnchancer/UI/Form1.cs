using System;
using System.Windows.Forms;
using RolzOrgEnchancer.Interfaces;

namespace RolzOrgEnchancer.UI
{
    public partial class Form1 : Form, IFormUpdate
    {
        public Form1()
        {
            RolzWebBrowser.SetBrowserFeatureControl();
            InitializeComponent();
        }

        #region IFormUpdate interface
        public void AddToLog(string logMessage)
        {
            textBox2.AppendText(logMessage + "\r\n");
        }

        public void UpdateRoomLog(string roomLog)
        {
            textBox1.Text = "";
            textBox1.AppendText(roomLog);
        }

        public void UpdateActionQueueDepth(int depth)
        {
            toolStripLabel1.Text = @"depth=" + depth.ToString("000");
        }
        #endregion

        #region Handlers
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction(1);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction(2);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction(3);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction(4);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction(5);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RoomBot.OnGuiTick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RoomBot.OnGuiStarted(webBrowser1, this);
        }
        #endregion
    }
}
