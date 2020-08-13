using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileConfigManager
{
    class FCM
    {
        char cfgmark = '=';
		
        public void ReadAllData(string filename, out string[] data, out string[] value)
        {
            List<string> tmp = new List<string>();
            List<string> tmp2 = new List<string>();
            string tmpstr = null;
            int idx;
            StreamReader sr = new StreamReader(filename, Encoding.Default);
            while (sr.Peek() > -1)
            {
                tmpstr = sr.ReadLine();
                if (tmpstr.Length == 0 || tmpstr[0] == '#') continue;

                if (tmpstr.Contains(cfgmark.ToString()))
                {
                    idx = tmpstr.IndexOf(cfgmark);
                    tmp.Add(tmpstr.Substring(0, idx));
                    tmp2.Add(tmpstr.Substring(idx + 1));
                }
            }
            data = tmp.ToArray();
            value = tmp2.ToArray();
            sr.Close();
        }
		
        public string ReadData(string filename, string data)
        {
            StreamReader sr = new StreamReader(filename, Encoding.Default);
            string line = null;
            while (sr.Peek() > -1)
            {
                line = sr.ReadLine();
                if (line.Length == 0 || line[0] == '#') continue;

                if (line.StartsWith(data + cfgmark))
                {
                    sr.Close();
                    return line.Substring(data.Length + 1, line.Length - (data.Length + 1));
                }
            }
            sr.Close();
            return null;
        }

        public void ChangeAllData(string filename, string[] data, string[] newval)
        {
            string value = null;
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            string tmp = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            for (int i = 0; i < data.Length; i++)
            {
                value = ReadData(filename, data[i]);
                tmp = tmp.Replace("\n" + data[i] + cfgmark + value, "\n" + data[i] + cfgmark + newval[i]);
            }
            fs = new FileStream(filename, FileMode.Truncate, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(tmp);
            sw.Close();
            fs.Close();
        }
    }
}