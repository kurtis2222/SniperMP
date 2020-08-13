using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour
{
	//Stuff
	public GameObject skin;
	public GameObject player;
	static GameObject gamecontrol;
	static GUIText deathmsg;
	static GUIText killedmsg;
	public int count = 0;
	GameObject scoreboard;
	static GUIText resmsg;
	static GUIText quitmsg;
	bool quit_warning = false;
	static Transform hud_items;
	
	public AudioClip[] step_snd;
	static AudioClip[] weapon_shoot = new AudioClip[11];
	static AudioClip[] weapon_reload = new AudioClip[11];
	public AudioClip death_snd;
	public AudioClip melee_hit;
	public AudioClip[] quotes;
	GameObject gren;
	
	//Taunt system
	int taunt_cooldown = 0;
	int t1 = 0, t2 = 0;
	int sc_cool = 0;
	
	//Death Snd
	int death_len;
	GameObject tmpdeath;
	public int ktime = 0;
	
	//Death List
	const int MAX_LINES = 5;
	int line_count = 0;
	string dtmpstr = null;
	
	//Head bob
	Vector3 oldpos;
	int stepnumb = 0;
	
	//Music
	bool isplaying = true;
	
	//Scope
	bool iszooming = false;
	MouseLook[] ms;
	Camera[] cams;
	GUITexture hud;
	GameObject scope;
	
	//Gun Flash Light
	public Light weap_light;
	
	bool[] isdown = new bool[9];
	float msens = 5.0f;
	float ssens = 1.0f;
	bool isinv = false;
	
	//Music
	public AudioClip[] music_list;
	int mus_tr = 0;
	bool mus_onstart = false;
	
	//HP System
	HudAmmo hud_hp;
	GUITexture hud_hit;
	int hp = 100;
	public static int stamina = 1000;
	bool isdead = false;
	bool autorespawn = false;
	
	//Skin weapon change
	HudwIcon hud_wicon;
	public GameObject[] weapon_obj;
	
	//Projectiles, particles
	public GameObject grenade;
	public GameObject[] particles;
	
	//Player name
	public string player_name = "Player";
	
	//Sync helpers
	NetworkView sync_state;
	NetworkView sync_weapon;
	
	[RPC]
	void LoadPlayerName(string pname)
	{
		player_name = pname;
	}
	
	void Start()
	{
		if(skin == null && Network.isServer)
		{
			hud_items = GameObject.Find("hud_items").transform;
			gamecontrol = GameObject.Find("GameControl");
			deathmsg = hud_items.Find("deathmsg").guiText;
			killedmsg = hud_items.Find("killedmsg").guiText;
			resmsg = hud_items.Find("HudRespawn").guiText;
			quitmsg = hud_items.Find("HudWarning").guiText;
			enabled = false;
			return;
		}
		skin.animation["fire"].layer = skin.animation["bat"].layer = skin.animation["rel"].layer = 1;
		if(Application.loadedLevel == 0)
			Destroy(gameObject);
		else
		{
			if(networkView.isMine)
			{
				stamina = 1000;
				hud_items = GameObject.Find("hud_items").transform;
				gamecontrol = GameObject.Find("GameControl");
				deathmsg = hud_items.Find("deathmsg").guiText;
				killedmsg = hud_items.Find("killedmsg").guiText;
				resmsg = hud_items.Find("HudRespawn").guiText;
				quitmsg = hud_items.Find("HudWarning").guiText;
				//Load camera
				GameObject tmp = (GameObject)GameObject.Instantiate(Resources.Load("cam_helper"));
				tmp.transform.parent = transform;
				tmp.transform.localPosition = new Vector3(0f,0.9f,0f);
				tmp.transform.rotation = transform.rotation;
				tmp.transform.localScale = transform.localScale;
				player = tmp.transform.GetChild(0).gameObject;
				weap_light = player.transform.Find("WeaponLight").light;
				GetComponent<PlayerChecker>().proj_point = player.transform.Find("proj_point");
				GetComponent<FPSWalkerEnhanced>().maincam = tmp;
				tmp = null;
				//Game mode
				if(Network.isServer)
				{
					if(CFGLoader.old_school)
					{
						GameObject.Instantiate(Resources.Load("OldSchool/oldschool_spawns" + Application.loadedLevel.ToString()));
						player.GetComponent<WeaponScript>().OldSchool();
					}
					else if(CFGLoader.instagib)
						player.GetComponent<WeaponScript>().InstagibMod();
					else if(CFGLoader.filt_weapons != "0000000000")
						player.GetComponent<WeaponScript>().FilterWeapons(CFGLoader.filt_weapons);
				}
				else
					gamecontrol.GetComponent<NetScore>().ReqMode();
				player.GetComponent<WeaponScript>().enabled = true;
				//Game mode end
				if(Application.loadedLevel == 12)
				{
					GetComponent<FPSWalkerEnhanced>().runSpeed = 10;
					GetComponent<FPSWalkerEnhanced>().jumpSpeed = 14;	
				}
				gameObject.layer = 9;
				Destroy(GetComponent<SphereCollider>());
				StartCoroutine(HideMSG());
				player.GetComponent<WeaponScript>().enabled = true;
				oldpos = transform.position;
				Screen.showCursor = false;
				Screen.lockCursor = true;
				hud_items.Find("Hud").Find("HudST").GetComponent<HudStamina>().GetInfo();
				StartCoroutine(StepTimer());
				ms = GetComponentsInChildren<MouseLook>();
				cams = GetComponentsInChildren<Camera>();
				weap_light.renderMode = LightRenderMode.ForcePixel;
				hud = hud_items.Find("HudCross").guiTexture;
				scope = hud_items.Find("HudSniper").gameObject;
				scope.SetActiveRecursively(false);
				scoreboard = hud_items.Find("HudScore").gameObject;
				scoreboard.SetActiveRecursively(false);
				hud_hp = hud_items.Find("Hud").Find("HudHP").GetComponent<HudAmmo>();
				hud_hit = hud_items.Find("HudHit").guiTexture;
				hud_wicon = hud_items.Find("HudWeapon").Find("HudWIcon").GetComponent<HudwIcon>();
				death_len = NetworkMenu.death_snd.Length;
				skin.transform.Find("Root").renderer.enabled = false;
				msens = CFGLoader.mouse_sens;
				ssens = CFGLoader.scope_sens;
				mus_onstart = CFGLoader.music;
				mus_tr = CFGLoader.track;
				autorespawn = CFGLoader.autorespawn;
				isinv = CFGLoader.inv_mouse;
				player_name = gamecontrol.GetComponent<NetworkMenu>().player_name;
				sync_weapon = transform.Find("syn1").networkView;
				sync_state = transform.Find("syn2").networkView;
				sync_weapon.RPC("Call1", RPCMode.OthersBuffered, 6);
				networkView.RPC("LoadPlayerName",RPCMode.OthersBuffered,player_name);
				if(CFGLoader.player_check)
					GetComponent<PlayerChecker>().enabled = true;
				foreach(MouseLook m in ms)
				{
					m.sensitivityX = msens;
					m.sensitivityY = (isinv) ? -msens : msens;
				}
				audio.clip = music_list[mus_tr];
				if(mus_onstart)
				{
					audio.Play();
					isplaying = true;
				}
				else
				{
					audio.Stop();
					isplaying = false;
				}
				weapon_shoot = player.GetComponent<WeaponScript>().weapon_shoot;
				weapon_reload = player.GetComponent<WeaponScript>().weapon_reload;
			}
			else
			{
				Destroy(player);
				audio.Stop();
				GetComponent<FPSWalkerEnhanced>().enabled = false;
				GetComponent<MouseLook>().enabled = false;
				GetComponent<MainScript>().enabled = false;
				GetComponentInChildren<MouseLook>().enabled = false;
			}
		}
	}
	
	void FixedUpdate()
	{
		if(quit_warning) return;
		
		if(Input.GetButton("Score"))
		{
			if(!isdown[0])
			{
				scoreboard.SetActiveRecursively(!scoreboard.active);
				isdown[0] = true;
			}
		}
		else isdown[0] = false;
		
		if(Input.GetButton("Talk"))
		{
			if(!isdown[1])
				isdown[1] = true;
		}
		else if(!Input.GetButton("Talk") && isdown[1])
		{
			gamecontrol.GetComponent<ChatScript>().enabled = true;
			isdown[1] = false;
		}
		
		if(isdead) goto end;
		
		if(Input.GetButton("Music"))
		{
			if(!isdown[2])
			{
				if(!isplaying) audio.Play();
				else audio.Stop();
				isplaying = !isplaying;
				isdown[2] = true;
			}
		}
		else isdown[2] = false;
		
		if(Input.GetButton("AltFire"))
		{
			if(!isdown[3])
			{
				switch(player.GetComponent<WeaponScript>().weaponid)
				{
					case 4:
					case 8:
					{
						weap_light.enabled = !weap_light.enabled;
						break;
					}
					case 6:
					{
						iszooming = !iszooming;
						if(iszooming)
						{
							hud.enabled = false;
							scope.SetActiveRecursively(true);
							foreach(Camera c in cams)
								c.fov = 5.0f;
							foreach(MouseLook m in ms)
							{
								m.sensitivityX = ssens;
								m.sensitivityY = (isinv) ? -ssens : ssens;
							}
						}
						else
						{
							hud.enabled = true;
							scope.SetActiveRecursively(false);
							foreach(Camera c in cams)
								c.fov = 60.0f;
							foreach(MouseLook m in ms)
							{
								m.sensitivityX = msens;
								m.sensitivityY = (isinv) ? -msens : msens;
							}
						}
						break;
					}
				}
				isdown[3] = true;
			}
		}
		else isdown[3] = false;
		
		if(Input.GetButton("NextTrack"))
		{
			if(!isdown[4])
			{
				mus_tr++;
				if(mus_tr > 3 || mus_tr < 0) mus_tr = 0;
				audio.clip = music_list[mus_tr];
				isdown[4] = true;
				audio.Play();
				isplaying = true;
			}
		}
		else isdown[4] = false;
		
		if(Input.GetButton("Taunt"))
		{
			if(!isdown[5] && taunt_cooldown == 0)
			{
			retry:
				t1 = Random.Range(0,quotes.Length);
				if(t1 == t2) goto retry;
				t2 = t1;
				networkView.RPC("PlayTaunt",RPCMode.All,t1);
				taunt_cooldown = 5;
				isdown[5] = true;
			}
		}
		else isdown[5] = false;
		
		if(Input.GetButton("Suicide"))
		{
			if(!isdown[6] && sc_cool == 0)
			{
				sc_cool = 30;
				gamecontrol.GetComponent<NetScore>().ServAddScore(
					gamecontrol.GetComponent<NetworkMenu>().player_name, -1);
				SendDamage(null,100);
				isdown[6] = true;
			}
		}
		else isdown[6] = false;
		
	end:
		if(Input.GetButton("Respawn"))
		{
			if(!isdown[7] && isdead)
			{
				GetComponent<FPSWalkerEnhanced>().ResetFall();
				Transform tmp = gamecontrol.GetComponent<NetworkMenu>().GetSpawn();
				transform.position = tmp.position;
				transform.rotation = tmp.rotation;
				hp = 100;
				hud_hp.ChangeValue(hp,100);
				CanControlPlayer(true);
				isdead = false;
				isdown[7] = true;
			}
		}
		else isdown[7] = false;
		
		if(Input.GetButton("HideHud"))
		{
			if(!isdown[8])
			{
				hud_items.GetComponent<HudHide>().HideHud();
				isdown[8] = true;
			}
		}
		else isdown[8] = false;
		
		//Quit warning
		if(Input.GetKey(KeyCode.Escape) && !quit_warning)
		{
			quitmsg.enabled = true;
			quit_warning = true;
		}
	}
	
	void Update()
	{
		if(quit_warning)
		{
			if(Input.GetKeyUp(KeyCode.I))
			{
				Network.Disconnect();
				Application.LoadLevel(0);
			}
			if(Input.GetKeyUp(KeyCode.N))
			{
				quitmsg.enabled = false;
				quit_warning = false;
			}
		}
	}
	
	public void DisableSniper()
	{
		hud.enabled = true;
		scope.SetActiveRecursively(false);
		foreach(Camera c in cams)
			c.fov = 60.0f;
		foreach(MouseLook m in ms)
		{
			m.sensitivityX = msens;
			m.sensitivityY = (isinv) ? -msens : msens;
		}
	}
	
	IEnumerator StepTimer()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.4f);
			if(Vector3.Distance(transform.position,oldpos) > 2.5f && GetComponent<FPSWalkerEnhanced>().Grounded)
			{
				if(stamina > 0) stamina -= 20;
				if(stamina < 0) stamina = 0;
				MakeStep();
			}
			else
			{
				if(stamina < 1000) stamina += 20;
				if(stamina > 1000) stamina = 1000;
			}
			
			oldpos = transform.position;
			if(oldpos.y < -4 && !isdead) SendDamage(null,100);
		}
	}
	
	void MakeStep()
	{
		if(stepnumb == step_snd.Length-1) stepnumb = 0;
		else stepnumb += 1;
		audio.PlayOneShot(step_snd[stepnumb]);
		networkView.RPC("SendStep",RPCMode.Others,stepnumb);
	}
	
	[RPC]
	void SendStep(int stepnumb)
	{
		audio.PlayOneShot(step_snd[stepnumb]);
	}
	
	public void DoDamage(NetworkViewID id, int dam)
	{
		networkView.RPC("SendDamage",id.owner,gamecontrol.GetComponent<NetworkMenu>().player_name,dam,false);
	}
	
	public void DoServerDamage(NetworkViewID id, int dam)
	{
		networkView.RPC("SendAltDamage",id.owner,dam);
	}
	
	[RPC]
	void SendAltDamage(int dam)
	{
		SendDamage(null, dam);
	}
	
	[RPC]
	public void SendDamage(string pname, int dam, bool isbot = false)
	{
		if(networkView.isMine)
		{
			if(!isdead)
			{
				hp-=dam;
				hud_hp.ChangeValue(hp,100);
				if(pname != null || isbot)
					hud_hit.enabled = true;
				if(hp <= 0)
				{
					hp = 0;
					hud_hp.ChangeValue(hp,100);
					if(pname != null)
					{
						gamecontrol.GetComponent<NetScore>().ServAddScore(pname,1);
						//Send player killer's name
						killedmsg.text = "Megölt " + pname;
						killedmsg.enabled = true;
						gamecontrol.GetComponent<NetworkMenu>().plobj.GetComponent<MainScript>().ktime = 5;
						if(Network.isServer)
							networkView.RPC("ShowKilledMessage",gamecontrol.GetComponent<NetScore>().GetPlayerViewID(pname).owner,gamecontrol.GetComponent<NetworkMenu>().player_name);
						else
							networkView.RPC("SendMessageToKiller",RPCMode.Server,pname,gamecontrol.GetComponent<NetworkMenu>().player_name);
					}
					if(pname == null)
					{
						if(isbot)
						{
							killedmsg.text = "Megölt egy bot";
							killedmsg.enabled = true;
							gamecontrol.GetComponent<NetworkMenu>().plobj.GetComponent<MainScript>().ktime = 5;
							networkView.RPC("DeathMessage3",RPCMode.All,gamecontrol.GetComponent<NetworkMenu>().player_name);
						}
						else
							networkView.RPC("DeathMessage2",RPCMode.All,gamecontrol.GetComponent<NetworkMenu>().player_name);
					}
					else
						networkView.RPC("DeathMessage",RPCMode.All,pname,gamecontrol.GetComponent<NetworkMenu>().player_name);
					if(iszooming)
					{
						iszooming = false;
						hud.enabled = true;
						scope.SetActiveRecursively(false);
						foreach(Camera c in cams)
							c.fov = 60.0f;
						foreach(MouseLook m in ms)
						{
							m.sensitivityX = msens;
							m.sensitivityY = (isinv) ? -msens : msens;
						}
					}
					weap_light.enabled = false;
					//Play Death snd
					networkView.RPC("PlayDeathSnd",RPCMode.Others,Random.Range(0,death_len));
					audio.PlayOneShot(death_snd);				
					player.GetComponent<WeaponScript>().ResetAmmo();
					stamina = 1000;
					//Send death message and block keys
					if(autorespawn)
					{
						GetComponent<FPSWalkerEnhanced>().ResetFall();
						isdead = true;
						Transform tmp = gamecontrol.GetComponent<NetworkMenu>().GetSpawn();
						transform.position = tmp.position;
						transform.rotation = tmp.rotation;
						hp = 100;
						hud_hp.ChangeValue(hp,100);
						isdead = false;
					}
					else
					{
						isdead = true;
						CanControlPlayer(false);
					}
				}
			}
		}
		else networkView.RPC("SendDamage",networkView.owner,pname,dam,isbot);
	}
	
	[RPC]
	void SendMessageToKiller(string pname, string dname)
	{
		if(pname == gamecontrol.GetComponent<NetworkMenu>().player_name)
			ShowKilledMessage(dname);
		else
			networkView.RPC("ShowKilledMessage",gamecontrol.GetComponent<NetScore>().GetPlayerViewID(pname).owner,dname);
	}
	
	[RPC]
	void ShowKilledMessage(string pname)
	{
		killedmsg.text = "Megölted " + pname + "-t";
		killedmsg.enabled = true;
		gamecontrol.GetComponent<NetworkMenu>().plobj.GetComponent<MainScript>().ktime = 5;
	}
	
	public void SendBotMessage(NetworkViewID id)
	{
		if(id.isMine)
			ShowKilledMessage2();
		else	
			networkView.RPC("ShowKilledMessage2",id.owner);
	}
	
	[RPC]
	void ShowKilledMessage2()
	{
		killedmsg.text = "Megöltél egy botot";
		killedmsg.enabled = true;
		gamecontrol.GetComponent<NetworkMenu>().plobj.GetComponent<MainScript>().ktime = 5;
	}
	
	[RPC]
	void PlayDeathSnd(int numb)
	{
		tmpdeath = (GameObject)GameObject.Instantiate(NetworkMenu.deathpr,
			transform.position, transform.rotation);
		tmpdeath.audio.PlayOneShot(NetworkMenu.death_snd[numb]);
		tmpdeath = null;
	}
	
	public bool GiveHP()
	{
		if(!isdead && hp < 100)
		{
			hp = 100;
			hud_hp.ChangeValue(hp,100);
			return true;
		}
		else return false;
	}
	
	void CanControlPlayer(bool input)
	{
		Network.RemoveRPCs(sync_state.viewID);
		sync_state.RPC("Call2",RPCMode.OthersBuffered,input);
		GetComponent<FPSWalkerEnhanced>().cancontrol = input;
		if(!input) GetComponent<FPSWalkerEnhanced>().ResetCrouch();
		player.GetComponent<WeaponScript>().enabled = input;
		player.GetComponent<WeaponScript>().ShowWeapon(input);
		GetComponent<MouseLook>().enabled = input;
		player.GetComponent<MouseLook>().enabled = input;
		resmsg.enabled = !input;
	}
	
	public void PlayerCollisions(bool input)
	{
		GetComponent<CharacterController>().enabled = input;
		GetComponent<SphereCollider>().enabled = input;
		skin.animation.Play((input ? "idle" : "death"));
	}
	
	public void SendCustomMessage(RPCMode mode, string message)
	{
		networkView.RPC("CustomMessage",mode,message);
	}
	
	[RPC]
	void CustomMessage(string messsage)
	{
		if(deathmsg == null)
			deathmsg = GameObject.Find("deathmsg").guiText;
		deathmsg.enabled = true;
		deathmsg.text += messsage + "\n";
		CheckTextLines();
	}
	
	[RPC]
	void DeathMessage(string killer, string player)
	{
		deathmsg.enabled = true;
		deathmsg.text += killer + " megölte " + player + "-t\n";
		CheckTextLines();
	}
	
	[RPC]
	void DeathMessage2(string player)
	{
		deathmsg.enabled = true;
		deathmsg.text += player + " meghalt\n";
		CheckTextLines();
	}
	
	[RPC]
	void DeathMessage3(string player)
	{
		deathmsg.enabled = true;
		deathmsg.text += player + "-t megölte egy bot\n";
		CheckTextLines();
	}
	
	void CheckTextLines()
	{
		dtmpstr = deathmsg.text;
		line_count = 0;
		foreach (char c in dtmpstr)
			if (c == '\n') line_count++;
		if(line_count > MAX_LINES)
		{
			dtmpstr = dtmpstr.Remove(0,dtmpstr.IndexOf('\n',0)+1);
			deathmsg.text = dtmpstr;
		}
		dtmpstr = null;
		line_count = 0;
	}
	
	IEnumerator HideMSG()
	{
		while(true)
		{
			yield return new WaitForSeconds(1.0f);
			if(taunt_cooldown > 0) taunt_cooldown-=1;
			if(sc_cool > 0) sc_cool-=1;
			if(ktime > 0)
			{
				ktime-=1;
				if(ktime == 0)
					killedmsg.enabled = false;
			}
		}
	}
	
	public void GetAnim(string anim)
	{
		if(anim != skin.animation.clip.name)
			networkView.RPC("SendAnim", RPCMode.All, anim);
	}
	
	[RPC]
	void SendAnim(string anim)
	{
		if(anim == "fire" || anim == "bat" || anim == "rel")
		{
			skin.animation.Stop(anim);
			skin.animation.Play(anim);
		}
		else
		{
			skin.animation.clip = skin.animation.GetClip(anim);
			skin.animation.CrossFade(anim);
		}
	}
	
	public void GetShot(int wid)
	{
		networkView.RPC("SendShot", RPCMode.Others, wid);
	}
	
	[RPC]
	void SendShot(int wid)
	{
		if(wid == -1)
			audio.PlayOneShot(melee_hit);
		else
		{
			if(wid != 0)
			{
				light.enabled = true;
				weapon_obj[wid].transform.GetChild(0).renderer.enabled = true;
				StartCoroutine(ResetGunFire(0.05f, wid));
			}
			audio.PlayOneShot(weapon_shoot[wid]);
		}
	}
	
	public void GetReload(int wid)
	{
		networkView.RPC("SendReload", RPCMode.Others, wid);
	}
	
	[RPC]
	void SendReload(int wid)
	{
		audio.PlayOneShot(weapon_reload[wid]);
	}
	
	IEnumerator ResetGunFire(float seconds, int wid)
	{
		yield return new WaitForSeconds(seconds);
		weapon_obj[wid].transform.GetChild(0).renderer.enabled = false;
		light.enabled = false;
	}
	
	public void ChangeWeapon(int weaponid)
	{
		hud_wicon.ChangeIcon(weaponid);
		Network.RemoveRPCs(sync_weapon.viewID);
		sync_weapon.RPC("Call1", RPCMode.OthersBuffered, weaponid);
	}
	
	public void SendChangeWeapon(int weaponid)
	{
		for(int i = 0; i < weapon_obj.Length; i++)
			weapon_obj[i].renderer.enabled = false;
		weapon_obj[weaponid].renderer.enabled = true;
	}
	
	[RPC]
	void PlayTaunt(int numb)
	{
		audio.PlayOneShot(quotes[numb]);
	}
	
	public void RequestObject(int prt, Vector3 pos)
	{
		networkView.RPC("LoadObject",RPCMode.All,prt,pos);
	}
	
	[RPC]
	void LoadObject(int prt, Vector3 pos)
	{
		GameObject.Instantiate(particles[prt],pos,new Quaternion(0,0,0,0));
	}
	
	public void RequestGrenade(Vector3 pos, Quaternion rot)
	{
		if(Network.isServer)
		{
			gren = (GameObject)Network.Instantiate(grenade,pos,rot,0);
			gren.GetComponent<GrenadeScript>().id = networkView.viewID;
			gren = null;
		}
		else
			networkView.RPC("ServerLoadGren",RPCMode.Server,pos,rot);
	}
	
	[RPC]
	void ServerLoadGren(Vector3 pos, Quaternion rot)
	{
		gren = (GameObject)Network.Instantiate(grenade,pos,rot,0);
		gren.GetComponent<GrenadeScript>().id = networkView.viewID;
		gren = null;
	}
	
	public void TeleportPlayer(NetworkPlayer pl, Vector3 pos)
	{
		networkView.RPC("DoTeleport",pl,pos);
	}
	
	[RPC]
	void DoTeleport(Vector3 pos)
	{
		transform.position = pos;
	}
}