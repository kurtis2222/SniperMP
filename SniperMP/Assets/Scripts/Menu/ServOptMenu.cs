using UnityEngine;
using System.Collections;

public class ServOptMenu : MonoBehaviour
{
	static GUIStyle labelstyle;
	FileConfigManager.FCM cfg = new FileConfigManager.FCM();
	const string filename = "config.ini";
	string[] data;
	string[] val;
	string port = "5300";
	string maxplayers = "16";
	string serv_rate = "20";
	bool old_school = false;
	bool instagib = false;
	bool[] filt_weapons = new bool[10];
	string adminpass = null;
	string serv_name = null;
	bool srvvis = false;
	bool limping = false;
	string maxping = "1000";
	
	public GameObject options;
	public Object image;
	float width, height;
	
	void Start()
	{
		if(labelstyle == null)
		{
			labelstyle = new GUIStyle();
			labelstyle.fontSize = 16;
			labelstyle.fontStyle = FontStyle.Bold;
			labelstyle.normal.textColor = Color.white;
		}
		old_school = CFGLoader.old_school;
		instagib = CFGLoader.instagib;
		maxplayers = CFGLoader.maxplayers.ToString();
		port = CFGLoader.port.ToString();
		serv_rate = CFGLoader.srate.ToString();
		adminpass = CFGLoader.adminpass;
		limping = CFGLoader.limping;
		maxping = CFGLoader.maxping.ToString();
		for(int i = 0; i < CFGLoader.filt_weapons.Length; i++)
			filt_weapons[i] = CFGLoader.filt_weapons[i] == '1' ? true : false;
		srvvis = CFGLoader.srvvis;
		serv_name = CFGLoader.serv_name;
	}
	
	void OnGUI ()
	{
		width = Screen.width/2;
		height = Screen.height/2;
		GUI.Label(new Rect(width - 128, height - 128, 256, 32), "Szerver neve", labelstyle);
		serv_name = GUI.TextField(new Rect(width + 32, height - 128, 192, 24), serv_name, 24);
		GUI.Label(new Rect(width - 128, height - 96, 256, 32), "Max Játékosszám", labelstyle);
		maxplayers = GUI.TextField(new Rect(width + 32, height - 96, 32, 24), maxplayers, 2);
		srvvis = GUI.Toggle(new Rect(width + 128, height - 96, 96, 24), srvvis, "Nyilvános");
		GUI.Label(new Rect(width - 128, height - 64, 256, 32), "Portszám", labelstyle);
		port = GUI.TextField(new Rect(width + 32, height - 64, 48, 24), port, 5);
		GUI.Label(new Rect(width + 86, height - 64, 256, 32), "Szerver ráta", labelstyle);
		serv_rate = GUI.TextField(new Rect(width + 196, height - 64, 32, 24), serv_rate, 2);
		GUI.Label(new Rect(width - 128, height - 32, 256, 32), "Admin jelszó", labelstyle);
		adminpass = GUI.TextField(new Rect(width + 32, height - 32, 128, 24), adminpass, 16);
		
		old_school = GUI.Toggle(new Rect(width - 128, height, 120, 24), old_school, "OldSchool Mod");
		if(instagib && old_school) instagib = false;
		instagib = GUI.Toggle(new Rect(width, height, 96, 24), instagib, "Instagib Mod");
		if(instagib && old_school) old_school = false;
		
		limping = GUI.Toggle(new Rect(width + 112, height, 84, 24), limping, "Pinglimit");
		maxping = GUI.TextField(new Rect(width + 196, height, 48, 24), maxping, 4);
		
		//Filtered weapons
		GUI.Label(new Rect(width - 128, height + 32, 292, 32), "Letiltandó fegyverek (Normál Módban)", labelstyle);
		filt_weapons[0] = GUI.Toggle(new Rect(width - 128, height + 56, 64, 24), filt_weapons[0], "Glock");
		filt_weapons[1] = GUI.Toggle(new Rect(width - 64, height + 56, 64, 24), filt_weapons[1], "SnW");
		filt_weapons[2] = GUI.Toggle(new Rect(width, height + 56, 96, 24), filt_weapons[2], "FN Shotgun");
		filt_weapons[3] = GUI.Toggle(new Rect(width + 96, height + 56, 48, 24), filt_weapons[3], "SiG");
		filt_weapons[4] = GUI.Toggle(new Rect(width + 144, height + 56, 64, 24), filt_weapons[4], "Ingram");
		filt_weapons[5] = GUI.Toggle(new Rect(width - 128, height + 80, 64, 24), filt_weapons[5], "M14");
		filt_weapons[6] = GUI.Toggle(new Rect(width - 64, height + 80, 64, 24), filt_weapons[6], "HKG8");
		filt_weapons[7] = GUI.Toggle(new Rect(width, height + 80, 64, 24), filt_weapons[7], "P90");
		filt_weapons[8] = GUI.Toggle(new Rect(width + 96, height + 80, 128, 24), filt_weapons[8], "Lefűrészelt SG");
		filt_weapons[9] = GUI.Toggle(new Rect(width - 128, height + 104, 64, 24), filt_weapons[9], "Gránát");
		
		if(GUI.Button(new Rect(width, height + 112, 128, 32), "Mentés és Kilépés"))
		{
			if(System.IO.File.Exists(filename))
			{
				CFGLoader.old_school = old_school;
				CFGLoader.instagib = instagib;
				int.TryParse(maxplayers, out CFGLoader.maxplayers);
				int.TryParse(port,out CFGLoader.port);
				int.TryParse(serv_rate,out CFGLoader.srate);
				CFGLoader.adminpass = adminpass;
				CFGLoader.limping = limping;
				int.TryParse(maxping,out CFGLoader.maxping);
				CFGLoader.filt_weapons = null;
				for(int i = 0; i < CFGLoader.filt_weapons.Length; i++)
					CFGLoader.filt_weapons += filt_weapons[i] ? "1" : "0";
				CFGLoader.srvvis = srvvis;
				CFGLoader.serv_name = serv_name;
				if(CFGLoader.srate < 10)
				{
					serv_rate = "10";
					CFGLoader.srate = 10;
				}
				else if(CFGLoader.srate > 30)
				{
					serv_rate = "30";
					CFGLoader.srate = 30;
				}
				//
				cfg.ReadAllData (filename, out data, out val);
				for (int i = 0; i < data.Length; i++) {
					if (data[i] == "OldSchool")
						val[i] = old_school.ToString();
					else if(data[i] == "Instagib")
						val[i] = instagib.ToString();
					else if(data[i] == "MaxPlayers")
						val[i] = maxplayers;
					else if(data[i] == "Port")
						val[i] = port;
					else if(data[i] == "ServerRate")
						val[i] = serv_rate;
					else if(data[i] == "AdminPass")
						val[i] = adminpass;
					else if(data[i] == "WeaponFilter")
					{
						val[i] = null;
						for(int i2 = 0; i2 < filt_weapons.Length; i2++)
							val[i] += filt_weapons[i2] ? "1" : "0";
					}
				}
				cfg.ChangeAllData(filename, data, val);
				gameObject.renderer.material.mainTexture = (Texture)image;
				foreach (GameObject h in options.GetComponent<MenuScript>().menu_hide)
					h.SetActiveRecursively (true);
				enabled = false;
			}
		}
		else if(GUI.Button(new Rect(width, height + 144, 128, 32), "Kilépés")) {
			gameObject.renderer.material.mainTexture = (Texture)image;
			foreach(GameObject h in options.GetComponent<MenuScript>().menu_hide)
				h.SetActiveRecursively (true);
			enabled = false;
		}
	}
}