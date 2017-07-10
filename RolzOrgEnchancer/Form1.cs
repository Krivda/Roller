using System;
using System.Windows.Forms;

namespace RolzOrgEnchancer
{
    public partial class Form1 : Form, ILogger
    {                      
        public Form1()
        {            
            RolzWebBrowser.SetBrowserFeatureControl();
            InitializeComponent();
        }

        public void Log(string log_message)
        {
            textBox2.AppendText(log_message + "\r\n");
        }

        public void LogRoomLog(string room_log)
        {
            textBox1.Text = "";
            textBox1.AppendText(room_log);
        }

        public void UpdateActionQueueDepth(int depth)
        {
            toolStripLabel1.Text = "depth=" + depth.ToString("000");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction("Action1");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction("Action2");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction("Action3");
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction("Action4");
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction("Action5");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RoomBot.OnGuiTick();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RoomBot.OnGuiStarted(webBrowser1);
        }

    }
}
