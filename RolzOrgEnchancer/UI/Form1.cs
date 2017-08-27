using System;
using System.Windows.Forms;

using Makedonsky.MapLogic.SpreadSheets;
using RollerEngine.SpreadSheets;
using RolzOrgEnchancer.Interfaces;

namespace RolzOrgEnchancer
{
    public partial class Form1 : Form, IFormUpdate
    {                      
        public Form1()
        {            
            RolzWebBrowser.SetBrowserFeatureControl();
            InitializeComponent();
        }

        #region IFormUpdate interface
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
        #endregion

        #region Handlers
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
            ApiTest.Test();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            RoomBot.OnGuiAction("Action5");
            var data = SpreadsheetService.GetNotEmptySpreadsheetRange("1tKXkAjTaUpIDkjmCi7w1QOVbnyYU2f-KOWEnl2EAIZg", "A1:J93", "Party sheet list");
            MessageBox.Show("Attributes:" + data[0][0]);
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
