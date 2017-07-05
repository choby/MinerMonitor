using MinerMonitor.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinerMonitor
{
    public partial class Form1 : Form
    {
        BrowserTask task;

        int restartCount = 0;


        public Form1()
        {
            InitializeComponent();
        }

        #region 初始化
        private void Form1_Load(object sender, EventArgs e)
        {
          
            task = new BrowserTask();



            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000, "系统启动中", "系统启动中", ToolTipIcon.Info);
            notifyIcon1.DoubleClick += NotifyIcon1_DoubleClick;
            comboBox1.SelectedIndex = 0;

            //
            this.RefreshAsync();

            //定时器
            timer1.Interval = int.Parse(comboBox1.SelectedItem.ToString()) * 60 * 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Enabled = true;


            //一个测试功能，不需要的
            //timer2.Enabled = true;
            //FindRepeatItemCode();

        }


        #endregion

        #region 按钮操作：刷新,时钟操作；通知栏双击
        private void button1_Click(object sender, EventArgs e)
        {
            this.RefreshAsync();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.RefreshAsync();
        }

        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }
        #endregion

        #region 共享方法：刷新
        new async Task RefreshAsync()
        {
            
            this.lblLoading.Text = "正在捞鱼...";

            var awaiter= task.BrowserAsync().GetAwaiter();
            awaiter.OnCompleted(async () =>
            {
                dataGridView1.DataSource = task.list;

                this.lblLoading.Text = "捞到两只小虾米！";
                timer1.Interval = int.Parse(comboBox1.SelectedItem.ToString()) * 60 * 1000;

                if ((task.list.Select(d => (DateTime.Now - d.LastSubmitTime).TotalMinutes).FirstOrDefault() >= 10))
                {
                    var restarted = await task.RestartAsync();
                    if (restarted)
                    {
                        notifyIcon1.ShowBalloonTip(120000, "服务器重启成功", "服务器重启成功", ToolTipIcon.Info);
                        ++restartCount;
                        this.lblRestartCount.Text = restartCount.ToString();
                        //NotifyIcon1_DoubleClick(null, null);
                    }
                    else
                    {
                        notifyIcon1.ShowBalloonTip(120000, "服务器重启失败", "服务器重启失败，稍后会自动重试", ToolTipIcon.Error);
                    }
                }
            });
        }




        #endregion

        #region 窗体事件

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //注意判断关闭事件Reason来源于窗体按钮，否则用菜单退出时无法退出
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;    //取消"关闭窗口"事件                 
                this.WindowState = FormWindowState.Minimized;    //使关闭时窗口向右下角缩小的效果  
                notifyIcon1.Visible = true;
                this.Hide();
                return;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)    //最小化到系统托盘   
            {
                notifyIcon1.Visible = true;    //显示托盘图标   
                this.Hide();    //隐藏窗口    
            }
        }



        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            //string url = "http://" + BrowserTask.mainUrl + row.Cells[1].Value.ToString();
            //System.Diagnostics.Process.Start(url);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        #endregion

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dataGridView1.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        


       
       


    }
}
