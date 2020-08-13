using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FileConfigManager
{
    class FCM
    {
        char cfgmark = '=';

        /// <summary>
        /// Reading Data from a file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <param name="data">The name of the data in the file</param>
        /// <returns>The value of the data</returns>
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
    }
}