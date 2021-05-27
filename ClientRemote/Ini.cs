using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientRemote
{
    class Ini
    {
        private string fileName
        {
            get;
            set;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filepath);

        public Ini(string fileName)
        {
            this.fileName = fileName;
        }

        public string Read(string section, string key)
        {
            StringBuilder stringBuilder = new StringBuilder(255);
            Ini.GetPrivateProfileString(section, key, "", stringBuilder, 255, this.fileName);
            return stringBuilder.ToString();
        }
        public void Write(string section, string key, string data)
        {
            Ini.WritePrivateProfileString(section, key, data, this.fileName);
        }
    }
}
