using UnityEngine;
using System.Collections;

public class CFGLoader : MonoBehaviour
{	
	const string filename = "config.ini";

	public static float mouse_sens = 5.0f;
	public static float scope_sens = 1.0f;
	public static int track = 0;
	public static bool music = true;
	public static bool inv_mouse = false;
	public static bool auto_reload = false;
	public static bool squote = true;
	public static bool altshotgun = false;
	public static bool autorespawn = false;
	public static int skin = 0;
	public static string pname = "Player";
	public static bool old_school = false;
	public static bool instagib = false;
	public static int srate = 20;
	public static int crate = 20;
	public static int maxplayers = 16;
	public static int port = 5300;
	public static bool player_check = true;
	public static string filt_weapons = null;
	public static string adminpass = null;
	public static bool autoswitch = true;
	public static string serv_name = null;
	public static bool srvvis = false;
	public static bool dedicated = false;
	public static int lvl = 0;
	public static string srvlist = null;
	public static int maxfps = 125;
	public static bool limping = false;
	public static int maxping = 1000;
	public static float volume = 0.5f;
	
	GameObject temp;
	
	public static void LoadData()
	{
		if(System.IO.File.Exists(filename))
		{
			FileConfigManager.FCM cfg = new FileConfigManager.FCM();
			string[] data;
			string[] val;
			cfg.ReadAllData(filename,out data, out val);
			for(int i = 0; i < data.Length; i++)
			{
				if(data[i] == "Name") pname = val[i];
				else if(data[i] == "Music") bool.TryParse(val[i],out music);
				else if(data[i] == "Track") int.TryParse(val[i],out track);
				else if(data[i] == "MouseSens") float.TryParse(val[i],out mouse_sens);
				else if(data[i] == "ScopeSens") float.TryParse(val[i],out scope_sens);
				else if(data[i] == "InvertMouse") bool.TryParse(val[i],out inv_mouse);
				else if(data[i] == "AutoReload") bool.TryParse(val[i],out auto_reload);
				else if(data[i] == "MossbergSG") bool.TryParse(val[i],out altshotgun);
				else if(data[i] == "StellaQuote") bool.TryParse(val[i],out squote);
				else if(data[i] == "AutoRespawn") bool.TryParse(val[i],out autorespawn);
				else if(data[i] == "ShowNames") bool.TryParse(val[i],out player_check);
				else if(data[i] == "AutoSwitch") bool.TryParse(val[i],out autoswitch);
				else if(data[i] == "PlayerSkin") int.TryParse(val[i],out skin);
				else if(data[i] == "Volume") float.TryParse(val[i],out volume);
				else if(data[i] == "MaxFPS") int.TryParse(val[i],out maxfps);
				else if(data[i] == "ClientRate") int.TryParse(val[i],out crate);
				else if(data[i] == "ServerRate") int.TryParse(val[i],out srate);
				else if(data[i] == "MaxPlayers") int.TryParse(val[i],out maxplayers);
				else if(data[i] == "Port") int.TryParse(val[i],out port);
				else if(data[i] == "AdminPass") adminpass = val[i];
				else if(data[i] == "OldSchool") bool.TryParse(val[i],out old_school);
				else if(data[i] == "Instagib") bool.TryParse(val[i],out instagib);
				else if(data[i] == "WeaponFilter") filt_weapons = val[i];
				else if(data[i] == "SrvList") srvlist = val[i];
				else if(data[i] == "SrvVis") bool.TryParse(val[i],out srvvis);
				else if(data[i] == "SrvName") serv_name = val[i];
				else if(data[i] == "LimPing") bool.TryParse(val[i],out limping);
				else if(data[i] == "MaxPing") int.TryParse(val[i],out maxping);
			}
			if(track > 3 || track < 0) track = 0;
			if(skin > 7 || skin < 0) skin = 0;
			if(port < 1) port = 1;
			else if(port > 65535) port = 65535;
			if(maxplayers < 2) maxplayers = 2;
			else if(maxplayers > 32) maxplayers = 32;
			if(pname.Length > 16) pname = pname.Substring(0,16);
			if(crate < 10) crate = 10;
			else if(crate > 30) crate = 30;
			if(srate < 10) srate = 10;
			else if(srate > 30) srate = 30;
		}
		string[] tmp = System.Environment.GetCommandLineArgs();
		if(tmp.Length > 3)
		{
			if(dedicated = int.TryParse(tmp[3],out lvl))
			{
				if(lvl < 1) lvl = 1;
				else if(lvl > 12) lvl = 12;
			}
		}
	}
	
	void Awake()
	{
		if((temp = GameObject.Find("stella")) != null)
			temp.GetComponent<StellaScript>().LoadEnabled(squote);
		GetComponent<NetworkMenu>().LoadData();
	}
}