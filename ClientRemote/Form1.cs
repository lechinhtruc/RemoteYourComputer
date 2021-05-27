using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientRemote
{
    public partial class Form1 : Form
    {
        #region Form Code
        public Form1()
        {
            InitializeComponent();
            notifyIcon1.Visible = false;  
            TrayMenuContext();
            GC.Collect();
        }

        private void StartupWithWindows()
        {
            if (!System.IO.File.Exists(startUpFolderPath + "\\" + Application.ProductName + ".lnk"))
            {
                WshShell wshShell = new WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut;


                // Create the shortcut
                shortcut =
                  (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(
                    startUpFolderPath + "\\" +
                    Application.ProductName + ".lnk");

                shortcut.TargetPath = Application.ExecutablePath;
                shortcut.WorkingDirectory = Application.StartupPath;
                shortcut.Description = "ClientRemote";

                shortcut.Save();
            }
        }

        public static void AddApplicationToAllUserStartup()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("ClientRemote", "\"" + Application.ExecutablePath + "\"");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (ini.Read("Setting", "Auto") == "true")
                {
                    username = ini.Read("Setting", "User");
                    password = ini.Read("Setting", "Pass");
                    textBox1.Text = username;
                    textBox2.Text = password;
                   
                        AddApplicationToAllUserStartup();
                   
                    
                    Login(username, password);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            GC.Collect();
        }

        private void TrayMenuContext()
        {
            this.notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.notifyIcon1.ContextMenuStrip.Items.Add("HIỆN ỨNG DỤNG", Properties.Resources._32687_200, this.SHOW_APP);
            this.notifyIcon1.ContextMenuStrip.Items.Add("KẾT NỐI", Properties.Resources.plugin);
            this.notifyIcon1.ContextMenuStrip.Items.Add("THOÁT", Properties.Resources.cancel_button, this.ExitProgram);
            this.notifyIcon1.ContextMenuStrip.Items.Add("THÔNG TIN", Properties.Resources.information, this.Information);
            GC.Collect();
        }

        void RefreshContextMenu(int TrangThai)
        {
           

            switch (TrangThai)
            {
                case 1:
                    notifyIcon1.ContextMenuStrip.Items[1].Click -= disconnect_;
                    notifyIcon1.ContextMenuStrip.Items[1].Text = "KẾT NỐI";
                    notifyIcon1.ContextMenuStrip.Items[1].Image = Properties.Resources.plugin;
                    notifyIcon1.ContextMenuStrip.Items[1].Click += SHOW_APP;
                    GC.Collect();
                    
                    break;
                case 0:
                    notifyIcon1.ContextMenuStrip.Items[1].Click -= SHOW_APP;
                    notifyIcon1.ContextMenuStrip.Items[1].Text = "NGẮT KẾT NỐI";
                    notifyIcon1.ContextMenuStrip.Items[1].Image = Properties.Resources.plug;
                    notifyIcon1.ContextMenuStrip.Items[1].Click += disconnect_;
                    GC.Collect();
                    
                    break;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void ExitProgram(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Information(object sender, EventArgs e)
        {
            Process.Start("https://www.facebook.com/EdwardLe3010/");
        }

        private void disconnect_(object sender, EventArgs e)
        {
           
            Disconnect();
            
            GC.Collect();

            return;
        }

        void ShowApp()
        {
            notifyIcon1.Visible = false;
            this.Visible = true;
            GC.Collect();
        }

        private void SHOW_APP(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Visible = true;
            GC.Collect();
        }

        void HIDE_APP()
        {
            //if(LoginStatus == "true")
            //{
            //    RefreshContextMenu(0);
            //}
            //else
            //{
            //    RefreshContextMenu(1);
            //}
            notifyIcon1.Visible = true;
            this.Visible = false;
            notifyIcon1.ShowBalloonTip(5000);
            GC.Collect();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                GC.Collect();
            }
            if(this.Visible == false)
            {
                if (LoginStatus == "true")
                {
                    RefreshContextMenu(0);
                }
                else
                {
                    RefreshContextMenu(1);
                }
                notifyIcon1.Visible = true;
                this.Visible = false;
                notifyIcon1.ShowBalloonTip(5000);
                GC.Collect();
            }
           
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowApp();
        }

        #endregion

        #region Variable 

        Ini ini = new Ini(Application.StartupPath + "\\config.remote");

        string startUpFolderPath =
             Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        string username = null;

        string password = null;

        string LoginStatus = null;

        WebClient client = new WebClient();

        string IP = "http://www.remotemycomputer.tk/";

        string LoginIP = "loginProcess.php";

        string queryCheck = "queryCheck.php";

        #endregion

        #region Login And Disconnect Event
        private void Login(string Username, string Password)
        {
            var data = new NameValueCollection();
            data["computername"] = Username;
            data["password"] = Password;
            var response = client.UploadValues(IP + LoginIP, "POST", data);
            string responseInString = Encoding.UTF8.GetString(response);
            if (responseInString == "LOGINSUCCES")
            {
                if (ini.Read("Setting", "Auto") == "true")
                {
                    ini.Write("Setting" , "User" , Username);
                    ini.Write("Setting", "Pass", Password);
                }
                label4.Text = "CONNECTED";
                label4.ForeColor = Color.Green;
                button1.Enabled = false;
                button2.Enabled = true;
                LoginStatus = "true";
                backgroundWorker1.RunWorkerAsync();
               
            }
            else
            {
                MessageBox.Show("SAI TÀI KHOẢN HOẶC MẬT KHẨU!", "Messege", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

        }

        
        private void Disconnect()
        {
            
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
                username = null;
                password = null;
                LoginStatus = null;
                textChange = new Thread(Textchange);
                textChange.Start();
            }
            if(notifyIcon1.Visible == true)
            {
                notifyIcon1.ShowBalloonTip(5000, "THÔNG BÁO", "ĐÃ NGẮT KẾT NỐI TỚI MÁY CHỦ!", ToolTipIcon.Info);
            }    
            RefreshContextMenu(1);
            GC.Collect();
          
        }

        Thread textChange;

        
        void Textchange()
        {
            label4.Invoke(new MethodInvoker(delegate
            {
                label4.ForeColor = Color.Red;
                label4.Text = "DISCONNECT";
            }));
            button1.Invoke(new MethodInvoker(delegate
            {
                button1.Enabled = true;
            }));
            button2.Invoke(new MethodInvoker(delegate
            {
                button2.Enabled = false;

            }));
         
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
        }

        #endregion

        #region Button Click Event
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                username = textBox1.Text.ToString();
                password = textBox2.Text.ToString();
                Login(username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR");
                return;
            }
        }//login btn

        private void button2_Click(object sender, EventArgs e)
        {
            Disconnect();
        }//disconnect btn

        private void button3_Click(object sender, EventArgs e) // setting btn
        {
            Form2 setting = new Form2();
            setting.Show();
        }

        private void button4_Click(object sender, EventArgs e) // about btn
        {
            Process.Start("https://www.facebook.com/EdwardLe3010/");
        }

        #endregion

        #region Query Execution
        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        public void ShutdownPC()
        {
            Process.Start("shutdown", "/s /t 15");
        }

        public void RestartPC()
        {
            Process.Start("shutdown", "/r /t 5");
        }
        #endregion

        #region Check Query
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            var data = new NameValueCollection();
            data["computername"] = username;
            var response = client.UploadValues(IP + queryCheck, "POST", data);
            string responseInString = Encoding.UTF8.GetString(response);
            switch (responseInString)
            {
                case "SHUTDOWN":
                    ShutdownPC();
                    GC.Collect();
                    break;
                case "LOCK":
                    LockWorkStation();
                    GC.Collect();
                    break;
                case "Disconnect":
                    Disconnect();
                    GC.Collect();
                    break;
                case "RESTART":
                    RestartPC();
                    GC.Collect();
                    break;
            }
            
            Thread.Sleep(1000);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Application.DoEvents();
            GC.Collect();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(LoginStatus =="true")
            {
                
                backgroundWorker1.RunWorkerAsync();
                GC.Collect();

            }
        }




        #endregion

       
    }
}