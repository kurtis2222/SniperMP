using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SniperMPded
{
    class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler();
        static EventHandler hnd;
        static Process proc;
        static FileConfigManager.FCM config = new FileConfigManager.FCM();
        const string filename = "config.ini";

        static void Main(string[] args)
        {
            Console.Title = "SniperMP v2.4 Dedicated Server";
            if (File.Exists("SniperMP.exe"))
            {
                if (!File.Exists(filename))
                {
                    Console.WriteLine("Hiba: Nem található a " + filename + " nevű fájl!");
                    Console.ReadKey(true);
                    return;
                }
                if (config.ReadData(filename, "AdminPass").Length <= 0)
                {
                    Console.WriteLine("Hiba: Nincs admin jelszó megadva!");
                    Console.ReadKey(true);
                    return;
                }
                Console.Write("Pálya:");
                int lvl;
                if (!int.TryParse(Console.ReadLine(), out lvl)) lvl = 1;
                if (lvl < 1) lvl = 1;
                else if (lvl > 12) lvl = 12;
                hnd += new EventHandler(AppClose);
                SetConsoleCtrlHandler(hnd, true);
                proc = new Process();
                proc.StartInfo.FileName = "SniperMP.exe";
                proc.StartInfo.Arguments = "-batchmode -nographics " + lvl.ToString();
                proc.Start();
                Console.WriteLine("SniperMP Dedikált Szerver v2.4, elindítva.");
                proc.WaitForExit();
            }
            else
            {
                Console.WriteLine("Hiba: Nem található a SniperMP.exe fájl!");
                Console.ReadKey(true);
            }
        }

        private static bool AppClose()
        {
            if (proc != null)
            {
                proc.Kill();
                bool tmp = false;
                bool.TryParse(config.ReadData(filename, "SrvVis"), out tmp);
                if (tmp)
                {
                    System.Collections.Specialized.NameValueCollection post_valid =
                        new System.Collections.Specialized.NameValueCollection()
			        {
				        {"valid","sniper"},
                        {"port",config.ReadData(filename,"Port")}
			        };
                    try
                    {
                        System.Net.WebClient wc = new System.Net.WebClient();
                        wc.UploadValues(config.ReadData(filename, "SrvList") + "/remsvr.php", post_valid);
                        wc.Dispose();
                    }
                    catch { }
                }
            }
            return false;
        }
    }
}