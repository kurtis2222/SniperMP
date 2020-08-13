using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace fbx_mat
{
    class Program
    {
        static void Main(string[] args)
        {
            string fname;
            char[] filter;
            int len;
            byte tmp;
            List<string> list = new List<string>();
            Console.Write("Fájl neve:");
            fname = Console.ReadLine();
            Console.Write("Kiterjesztés:");
            filter = Console.ReadLine().ToCharArray();
            len = filter.Length;
            BinaryReader br = new BinaryReader(
                new FileStream(fname, FileMode.Open, FileAccess.Read),
                Encoding.Default
                );

            long i = 0;
            int j;
            while (br.PeekChar() > -1)
            {
                br.BaseStream.Position = i;
                if (br.ReadChars(len).SequenceEqual(filter))
                {
                    i = br.BaseStream.Position;
                    j = 0;
                    tmp = 0xFF;
                    while (i - j > 0 && tmp != 0x0)
                    {
                        j++;
                        br.BaseStream.Position = i - j;
                        tmp = br.ReadByte();
                        //Skip duplicates
                        if (tmp == 0x5C) goto skip;
                    }
                    //Go forward
                    if (j > 0)
                        j-=1;
                    if (i - j > 0)
                    {
                        br.BaseStream.Position = i - j;
                        list.Add(new string(br.ReadChars(j)));
                        br.BaseStream.Position = i;
                    }
                }
            skip:
                i++;
            }
            br.Close();
            //Clear duplicates
            list = list.Distinct().ToList();
            StreamWriter sw = new StreamWriter(fname + ".lst", false, Encoding.Default);
            foreach (string l in list)
                sw.WriteLine(l);
            sw.Close();
        }
    }
}