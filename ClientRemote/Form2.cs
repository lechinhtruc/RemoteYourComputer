using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientRemote
{
    public partial class Form2 : Form
    {
        Ini ini = new Ini(Application.StartupPath + "\\config.remote");
        public Form2()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                ini.Write("Setting", "Auto" , "true");
            }
            else
            {
                ini.Write("Setting", "Auto", "false");
            }    
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if(ini.Read("Setting" , "Auto") == "true")
            {
                checkBox1.Checked = true;
            }    
            else
            {
                checkBox1.Checked = false;
            }    
        }
    }
}
