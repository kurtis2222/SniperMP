using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
	static GUIStyle labelstyle;
	FileConfigManager.FCM cfg = new FileConfigManager.FCM ();
	const string filename = "config.ini";
	string[] data;
	string[] val;
	string pname = "Player";
	string mouse_sens = "5.0";
	string scope_sens = "1.0";
	string cl_rate = "20";
	string maxfps = "125";
	bool inv_mouse = false;
	bool auto_reload = false;
	bool music = true;
	bool squote = true;
	bool altshotgun = false;
	bool autorespawn = false;
	bool player_check = true;
	int track = 1;
	float volume = 0.5f;
	bool autoswitch = true;
	
	//Skin select
	int skin = 0;
	string[] skins = new string[]
	{
		"Dominick",
		"Nico",
		"Rendőr",
		"Tommy",
		"Olasz1",
		"Olasz2",
		"Kínai",
		"Jack"
	};
	
	public GameObject options;
	public Object image;
	
	void Start ()
	{
		if(labelstyle == null)
		{
			labelstyle = new GUIStyle();
			labelstyle.fontSize = 16;
			labelstyle.fontStyle = FontStyle.Bold;
			labelstyle.normal.textColor = Color.white;
		}
		pname = CFGLoader.pname;
		cl_rate = CFGLoader.crate.ToString();
		maxfps = CFGLoader.maxfps.ToString();
		mouse_sens = CFGLoader.mouse_sens.ToString();
		scope_sens = CFGLoader.scope_sens.ToString();
		inv_mouse = CFGLoader.inv_mouse;
		auto_reload = CFGLoader.auto_reload;
		altshotgun = CFGLoader.altshotgun;
		music = CFGLoader.music;
		track = CFGLoader.track;
		squote = CFGLoader.squote;
		autorespawn = CFGLoader.autorespawn;
		player_check = CFGLoader.player_check;
		skin = CFGLoader.skin;
		autoswitch = CFGLoader.autoswitch;
		volume = CFGLoader.volume;
	}
	
	int width, height;
	
	void OnGUI ()
	{
		width = Screen.width/2;
		height = Screen.height/2;
		GUI.Label(new Rect(width - 128, height - 96, 256, 32), "Multiplayer név", labelstyle);
		pname = GUI.TextField (new Rect(width + 16, height - 96, 128, 24), pname, 16);
		GUI.Label(new Rect(width - 128, height - 64, 256, 32), "Egér érzékenység", labelstyle);
		mouse_sens = GUI.TextField (new Rect(width + 16, height - 64, 32, 24), mouse_sens, 4);
		GUI.Label(new Rect(width + 64, height - 64, 256, 32), "Szkóp érzékenység", labelstyle);
		scope_sens = GUI.TextField (new Rect(width + 224, height - 64, 32, 24), scope_sens, 4);
		GUI.Label(new Rect(width - 128, height - 32, 96, 32), "Hangerő", labelstyle);
		volume = GUI.HorizontalSlider(new Rect(width - 48, height - 32, 96, 24),volume,0,1);
		GUI.Label(new Rect(width + 64, height - 32, 256, 32), "Kliens ráta", labelstyle);
		cl_rate = GUI.TextField(new Rect(width + 224, height - 32, 32, 24), cl_rate, 2);
		inv_mouse = GUI.Toggle(new Rect(width - 128, height, 96, 24), inv_mouse, "Inverz egér");
		auto_reload = GUI.Toggle(new Rect(width - 32, height, 100, 24), auto_reload, "Auto újratöltés");
		music = GUI.Toggle(new Rect(width + 80, height, 144, 24), music, "Zene (játék közben)");
		squote = GUI.Toggle(new Rect(width - 128, height + 32, 144, 24), squote, "Stella beszólások");
		altshotgun = GUI.Toggle(new Rect(width + 128, height + 32, 144, 24), altshotgun, "Alternatív Sörétes");
		autorespawn = GUI.Toggle(new Rect(width - 128, height + 64, 144, 24), autorespawn, "Automata újraéledés");
		player_check = GUI.Toggle(new Rect(width - 128, height + 96, 144, 24), player_check, "Játékos nevek");
		GUI.Label(new Rect(width - 128, height + 128, 64, 24), "MaxFPS");
		maxfps = GUI.TextField(new Rect(width - 64, height + 128, 48, 24), maxfps, 4);
		autoswitch = GUI.Toggle(new Rect(width - 128, height + 160, 144, 24), autoswitch, "Felvételkor váltás");
		
		if(GUI.Button(new Rect(width + 32, height + 64, 128, 24),"Külső: " + skins[skin].ToString()))
		{
			if(skin > skins.Length-2)
				skin=0;
			else
				skin++;
		}
		else if(GUI.Button(new Rect(width + 32, height + 32, 64, 24),"Zene: " + track.ToString()))
		{
			if(track > 3)
				track=1;
			else
				track++;
		}
		else if (GUI.Button(new Rect(width, height + 96, 128, 32), "Mentés és Kilépés"))
		{
			if(System.IO.File.Exists(filename))
			{
				CFGLoader.pname = pname;
				int.TryParse(cl_rate, out CFGLoader.crate);
				int.TryParse(maxfps, out CFGLoader.maxfps);
				float.TryParse(mouse_sens, out CFGLoader.mouse_sens);
				float.TryParse(scope_sens, out CFGLoader.mouse_sens);
				CFGLoader.inv_mouse = inv_mouse;
				CFGLoader.auto_reload = auto_reload;
				CFGLoader.altshotgun = altshotgun;
				CFGLoader.music = music;
				CFGLoader.track = track;
				CFGLoader.squote = squote;
				CFGLoader.autorespawn = autorespawn;
				CFGLoader.player_check = player_check;
				CFGLoader.skin = skin;
				CFGLoader.autoswitch = autoswitch;
				CFGLoader.volume = volume;
				if(CFGLoader.crate < 10)
				{
					cl_rate = "10";
					CFGLoader.crate = 10;
				}
				else if(CFGLoader.crate > 30)
				{
					cl_rate = "30";
					CFGLoader.crate = 30;
				}
				//
				cfg.ReadAllData(filename, out data, out val);
				for (int i = 0; i < data.Length; i++) {
					if (data[i] == "Name")
						val[i] = pname;
					else if(data[i] == "MouseSens")
						val[i] = mouse_sens.ToString();
					else if(data[i] == "ScopeSens")
						val[i] = scope_sens.ToString();
					else if(data[i] == "InvertMouse")
						val[i] = inv_mouse.ToString();
					else if(data[i] == "AutoReload")
						val[i] = auto_reload.ToString();
					else if(data[i] == "Music")
						val[i] = music.ToString();
					else if(data[i] == "Track")
						val[i] = track.ToString();
					else if(data[i] == "StellaQuote")
						val[i] = squote.ToString();
					else if(data[i] == "MossbergSG")
						val[i] = altshotgun.ToString();
					else if(data[i] == "AutoRespawn")
						val[i] = autorespawn.ToString();
					else if(data[i] == "ShowNames")
						val[i] = player_check.ToString();
					else if(data[i] == "PlayerSkin")
						val[i] = skin.ToString();
					else if(data[i] == "AutoSwitch")
						val[i] = autoswitch.ToString();
					else if(data[i] == "Volume")
						val[i] = volume.ToString();
					else if(data[i] == "ClientRate")
						val[i] = cl_rate;
					else if(data[i] == "MaxFPS")
						val[i] = maxfps;
				}
				cfg.ChangeAllData(filename, data, val);
				gameObject.renderer.material.mainTexture = (Texture)image;
				foreach (GameObject h in options.GetComponent<MenuScript>().menu_hide)
					h.SetActiveRecursively(true);
				enabled = false;
				Application.targetFrameRate = CFGLoader.maxfps;
				AudioListener.volume = volume;
			}
		}
		else if (GUI.Button(new Rect(width, height + 128, 128, 32), "Kilépés")) {
			gameObject.renderer.material.mainTexture = (Texture)image;
			foreach(GameObject h in options.GetComponent<MenuScript>().menu_hide)
				h.SetActiveRecursively(true);
			enabled = false;
		}
	}
}