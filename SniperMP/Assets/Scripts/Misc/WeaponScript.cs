using UnityEngine;
using System.Collections;

public class WeaponScript : MonoBehaviour
{
    bool blockshot = false;
    GameObject audiosrc;
    MainScript controller;

    public GameObject[] weapon = new GameObject[2];
    public AudioClip[] weapon_shoot = new AudioClip[2];
    public AudioClip[] weapon_reload = new AudioClip[2];
    public AudioClip melee_hit;
    public Texture mosberg_sg;

    public Transform proj_point;
    RaycastHit hit = new RaycastHit();
    Renderer gunflash;

    GUIText gui_cammo;
    HudAmmo gui_ammo;
    GUIText gui_mammo;

    int[] mammo = new int[10];
    int[] cammo = new int[10];

    int[] max_ammo;
    int[] clip_ammo;

    float[] fire_reset;
    float[] fire_delay;
    float[] reload_delay;

    int[,] damage;

    bool[] isauto;
    bool isfiredown = false;
    bool auto_reload = false;
    bool ismelee = false;
    bool altshotgun = false;
    public bool oldschool = false;

    Vector3 normal_pos;
    Vector3 shoot_pos;
    Vector3 reload_pos;
	public Transform wp_pivot;
	bool changing = false;
	bool dir = false;
	int oldid = -1;

    public int weaponid = 0;
    float mwheel = 0;

    //Grenade
    bool used_grenade = false;
	bool disable_gren = false;
	
	//
    bool isdead = false;
	bool autoswitch = true;
	
	//Other
	int spec = 0;
	string tmp = null;
	bool isdown = false;

    void Start()
    {
        //Load arrays
        max_ammo = new int[]
		{
			1,
			170,
			36,
			50,
			240,
			300,
			100,
			150,
			100,
			50
		};

        clip_ammo = new int[]
		{
			1,
			17,
			6,
			8,
			30,
			50,
			20,
			50,
			50,
			2
		};

        fire_reset = new float[]
		{
			0.1f,
			0.10f,
			0.15f,
			0.15f,
			0.02f,
			0.02f,
			0.2f,
			0.05f,
			0.05f,
			0.2f
		};

        fire_delay = new float[]
		{
			0.15f,
			0.15f,
			0.3f,
			0.35f,
			0.027f,
			0.027f,
			0.3f,
			0.027f,
			0.027f,
			0.3f
		};

        reload_delay = new float[]
		{
			1.5f,
			1.0f,
			2.5f,
			1.5f,
			1.5f,
			1.5f,
			1.5f,
			2.0f,
			2.0f,
			1.5f
		};

        damage = new int[,] 
		{
			{75,150},
			{10,20},
			{50,70},
			{80,150},
			{10,25},
			{5,15},
			{50,150},
			{10,25},
			{10,15},
			{80,120}
		};

        isauto = new bool[]
		{
			false,
			false,
			false,
			false,
			true,
			true,
			false,
			true,
			true,
			false
		};
        //Load arrays end
		audiosrc = transform.root.gameObject;
        controller = audiosrc.GetComponent<MainScript>();
        weaponid = 6;
        altshotgun = CFGLoader.altshotgun;
		autoswitch = CFGLoader.autoswitch;
        if (altshotgun)
            weapon[3].renderer.materials[1].mainTexture = mosberg_sg;
        auto_reload = CFGLoader.auto_reload;
        weapon[weaponid].renderer.enabled = true;
        for (int i = 0; i < mammo.Length; i++)
        {
            mammo[i] = max_ammo[i];
            cammo[i] = clip_ammo[i];
            normal_pos = wp_pivot.localPosition;
            shoot_pos = normal_pos - new Vector3(0, 0, 0.1f);
            reload_pos = normal_pos - new Vector3(0, 0.3f, 0);
        }
		Transform temp = GameObject.Find("HudWeapon").transform;
        gui_ammo = temp.Find("HudAmmo").GetComponent<HudAmmo>();
        gui_ammo.GetComponent<HudAmmo>().enabled = true;
        gui_cammo = temp.Find("HudCAmmo").guiText;
        gui_mammo = temp.Find("HudMAmmo").guiText;
        gui_cammo.text = cammo[weaponid].ToString("00");
        gui_mammo.text = mammo[weaponid].ToString("000");
		temp = null;
		if(spec == 1) OldSchool();
		else if(spec == 2) InstagibMod();
		else if(spec == 3) FilterWeapons(tmp);
    }

    public void ResetAmmo()
    {
        if (oldschool)
        {
            used_grenade = true;
            for (int i = 0; i < mammo.Length; i++)
            {
                if (i < 2)
                {
                    mammo[i] = max_ammo[i];
                    cammo[i] = clip_ammo[i];
                }
                else
                {
                    mammo[i] = 0;
                    cammo[i] = 0;
                }
            }
            FastChangeWeapon(1);
            gui_ammo.ChangeValue2(cammo[weaponid], clip_ammo[weaponid]);
        }
        else
        {
            for (int i = 0; i < mammo.Length; i++)
            {
                mammo[i] = max_ammo[i];
                cammo[i] = clip_ammo[i];
            }
            used_grenade = false;
            gui_ammo.ChangeValue2(cammo[weaponid], clip_ammo[weaponid]);
            gui_cammo.text = cammo[weaponid].ToString("00");
            gui_mammo.text = mammo[weaponid].ToString("000");
        }
    }

    void ChangeWeapon(int wid)
    {
        if (weaponid != wid)
        {
            if (wid == 0) ismelee = true;
            else ismelee = false;
            blockshot = true;
			if (gunflash != null) gunflash.enabled = false;
			audiosrc.light.enabled = false;
			audiosrc.GetComponent<MainScript>().weap_light.enabled = false;
            controller.DisableSniper();
            oldid = weaponid;
            weaponid = wid;
            changing = true;
        }
    }

    void ChangeWeapon2(int wid, int old)
    {
        if (wid == 0) ismelee = true;
        else ismelee = false;
        blockshot = true;
		if (gunflash != null) gunflash.enabled = false;
		audiosrc.light.enabled = false;
		audiosrc.GetComponent<MainScript>().weap_light.enabled = false;
        controller.DisableSniper();
		oldid = old;
        weaponid = wid;
        changing = true;
    }

    void FastChangeWeapon(int wid)
    {
        if (weaponid != wid)
        {
            if (wid == 0) ismelee = true;
            else ismelee = false;
            controller.DisableSniper();
            isfiredown = true;
            weapon[weaponid].renderer.enabled = false;
            wp_pivot.localPosition = normal_pos;
            if (gunflash != null) gunflash.enabled = false;
            weaponid = wid;
            weapon[weaponid].renderer.enabled = true;
            gui_cammo.text = cammo[weaponid].ToString("00");
            gui_mammo.text = mammo[weaponid].ToString("000");
            controller.ChangeWeapon(weaponid);
            isfiredown = false;
			audiosrc.GetComponent<MainScript>().weap_light.enabled = false;
        }
    }

    public void OldSchool()
    {
		if(spec == 1 || (Network.isClient && enabled))
		{
        	oldschool = true;
        	ResetAmmo();
			FastChangeWeapon(1);
			spec = 0;
		}
		else spec = 1;
    }
	
	public void InstagibMod()
	{
		if(spec == 2 || (Network.isClient && enabled))
		{
			for(int i = 1; i < max_ammo.Length; i++)
			{
				if(i != 6)
				{
					max_ammo[i] = 0;
					clip_ammo[i] = 0;
				}
				else
				{
					damage[i,0] = 100;
					damage[i,1] = 100;
					max_ammo[i] = 0;
					clip_ammo[i] = 999;
				}
			}
			damage[0,0] = 100;
			damage[0,1] = 100;
			disable_gren = true;
			ResetAmmo();
			spec = 0;
		}
		else spec = 2;
	}
	
	public void FilterWeapons(string filter)
	{
		if(spec == 3 || (Network.isClient && enabled))
		{
			for(int i = 0; i < filter.Length-1; i++)
			{
				if(filter[i] == '1')
				{
					max_ammo[i+1] = 0;
					clip_ammo[i+1] = 0;
				}
			}
			if(filter[9] == '1') disable_gren = true;
			ResetAmmo();
			if(spec != 0)
			{
				spec = 0;
				tmp = null;
			}
		}
		else
		{
			tmp = filter;
			spec = 3;
		}
	}

    public bool GivePickup(int wid, int ammount)
    {
        if (wid < 9)
        {
            if (mammo[wid + 1] < max_ammo[wid + 1])
            {
                if (max_ammo[wid + 1] - mammo[wid + 1] < ammount)
                {
					if(autoswitch)
                    	FastChangeWeapon(wid + 1);
                    mammo[wid + 1] += max_ammo[wid + 1] - mammo[wid + 1];
                    gui_mammo.text = mammo[weaponid].ToString("000");
                    if (cammo[wid + 1] == 0)
                    {
                        mammo[wid + 1] -= (clip_ammo[wid + 1] - cammo[wid + 1]);
                        cammo[wid + 1] = clip_ammo[wid + 1];
                        gui_cammo.text = cammo[weaponid].ToString("00");
                        gui_mammo.text = mammo[weaponid].ToString("000");
                    }
                    gui_ammo.ChangeValue(cammo[weaponid], clip_ammo[weaponid]);
                }
                else if (max_ammo[wid + 1] - mammo[wid + 1] >= ammount)
                {
					if(autoswitch)
                   		FastChangeWeapon(wid + 1);
                    mammo[wid + 1] += ammount;
                    gui_mammo.text = mammo[weaponid].ToString("000");
                    if (cammo[wid + 1] == 0)
                    {
                        mammo[wid + 1] -= (clip_ammo[wid + 1] - cammo[wid + 1]);
                        cammo[wid + 1] = clip_ammo[wid + 1];
                        gui_cammo.text = cammo[weaponid].ToString("00");
                        gui_mammo.text = mammo[weaponid].ToString("000");
                    }
                    gui_ammo.ChangeValue(cammo[weaponid], clip_ammo[weaponid]);
                }
                else return false;
            }
            else return false;
        }
        else
        {
            if (used_grenade)
                used_grenade = false;
            else
                return false;
        }
        return true;
    }

    public void ShowWeapon(bool input)
    {
        if (input)
            weapon[weaponid].renderer.enabled = input;
        else
        {
			if(changing)
				weapon[oldid].renderer.enabled = input;
            weapon[weaponid].renderer.enabled = input;
            if (gunflash != null) gunflash.enabled = input;
            audiosrc.light.enabled = input;
        }
        isdead = !input;
    }

    //It only works fine with this Update
    void Update()
    {
        if (!blockshot && !isfiredown)
        {
            mwheel = Input.GetAxis("Mouse ScrollWheel");
            if (mwheel < 0)
            {
                if (weaponid >= 9)
                    weaponid = 0;
                else
                    weaponid++;
                ChangeWeapon2(weaponid, weaponid - 1 < 0 ? 9 : weaponid - 1);
            }
            else if (mwheel > 0)
            {
                if (weaponid <= 0)
                    weaponid = 9;
                else
                    weaponid--;
                ChangeWeapon2(weaponid, weaponid + 1 > 9 ? 0 : weaponid + 1);
            }
        }
    }

    void DoMeleeAttack()
    {
        controller.GetAnim("bat");
        weapon[weaponid].animation.Play("st_hit");
        BlockWeapon(fire_delay[weaponid]);
        audiosrc.audio.PlayOneShot(weapon_shoot[weaponid]);
        controller.GetShot(weaponid);
        Physics.Raycast(proj_point.position, proj_point.forward, out hit, 2.0f, 9);
        if (hit.collider)
        {
            if (hit.collider.gameObject.name.StartsWith("Player") && !hit.collider.networkView.isMine)
            {
                hit.collider.GetComponent<MainScript>().DoDamage(
                    hit.collider.networkView.viewID,
					Random.Range(damage[weaponid, 0], damage[weaponid, 1]));
                controller.RequestObject(0, hit.point);
            }
            else if (hit.collider.name == "bus")
                controller.RequestObject(0, hit.point);
			else if (hit.collider.name == "Bot")
			{
				controller.RequestObject(0, hit.point);
				hit.collider.GetComponent<BotScript>().SendSetTarget(controller.transform,
					Random.Range(damage[weaponid, 0], damage[weaponid, 1]));
			}
            else
            {
                audiosrc.audio.PlayOneShot(melee_hit);
                controller.GetShot(-1);
            }
        }
        isfiredown = true;
    }

    void DoShot()
    {
		controller.GetAnim("fire");
        gunflash = weapon[weaponid].transform.GetChild(0).renderer;
        cammo[weaponid] -= 1;
        gui_ammo.ChangeValue(cammo[weaponid], clip_ammo[weaponid]);
        gui_cammo.text = cammo[weaponid].ToString("00");
        audiosrc.light.enabled = true;
        gunflash.enabled = true;
        audiosrc.audio.PlayOneShot(weapon_shoot[weaponid]);
        wp_pivot.localPosition = shoot_pos;
        controller.GetShot(weaponid);
        if(!isauto[weaponid]) StartCoroutine(ResetGunFire(fire_reset[weaponid]));
        BlockWeapon(fire_delay[weaponid]);
		if(weaponid == 1 || weaponid == 6)
			Physics.Raycast(proj_point.position, proj_point.forward, out hit, 500.0f, 9);
		else
        	Physics.Raycast(proj_point.position, proj_point.TransformDirection(-0.02f+0.04f/Random.Range(1,11),-0.02f+0.04f/Random.Range(1,11),1f), out hit, 500.0f, 9);
        if (hit.collider)
        {
            if (hit.collider.gameObject.name.Contains("Player") && !hit.collider.networkView.isMine)
            {
                hit.collider.GetComponent<MainScript>().DoDamage(
                    hit.collider.networkView.viewID,
					Random.Range(damage[weaponid, 0], damage[weaponid, 1]));
                controller.RequestObject(0, hit.point);
            }
            else if (hit.collider.name == "bus")
                controller.RequestObject(0, hit.point);
			else if (hit.collider.name == "Bot")
			{
				controller.RequestObject(0, hit.point);
				hit.collider.GetComponent<BotScript>().SendSetTarget(controller.transform,
					Random.Range(damage[weaponid, 0], damage[weaponid, 1]));
			}
			else if (hit.collider.name == "stella_boss")
			{
				controller.RequestObject(0, hit.point);
				hit.collider.GetComponent<BossScript>().SendSetTarget(controller.transform,
					Random.Range(damage[weaponid, 0], damage[weaponid, 1]));
			}
            else
                controller.RequestObject(1, hit.point);
        }
        isfiredown = true;
    }

    void FixedUpdate()
    {
        if (!blockshot)
        {
			isfiredown = Input.GetButton("Fire1");
            if (!isfiredown)
            {
                if (Input.GetKey(KeyCode.Alpha1)) ChangeWeapon(0);
                else if (Input.GetKey(KeyCode.Alpha2)) ChangeWeapon(1);
                else if (Input.GetKey(KeyCode.Alpha3)) ChangeWeapon(2);
                else if (Input.GetKey(KeyCode.Alpha4)) ChangeWeapon(3);
                else if (Input.GetKey(KeyCode.Alpha5)) ChangeWeapon(4);
                else if (Input.GetKey(KeyCode.Alpha6)) ChangeWeapon(5);
                else if (Input.GetKey(KeyCode.Alpha7)) ChangeWeapon(6);
                else if (Input.GetKey(KeyCode.Alpha8)) ChangeWeapon(7);
                else if (Input.GetKey(KeyCode.Alpha9)) ChangeWeapon(8);
                else if (Input.GetKey(KeyCode.Alpha0)) ChangeWeapon(9);
            }
			if (isauto[weaponid] && audiosrc.light.enabled)
			{
				wp_pivot.localPosition = normal_pos;
				audiosrc.light.enabled = false;
				weapon[weaponid].transform.GetChild(0).renderer.enabled = false;
			}
			
            if (isfiredown && ismelee)
                DoMeleeAttack();
            else if (isfiredown && cammo[weaponid] > 0 && !ismelee)
                DoShot();
            else if (Input.GetButton("Reload") || (cammo[weaponid] <= 0 && auto_reload) && !ismelee)
            {
                if (mammo[weaponid] > 0 && cammo[weaponid] < clip_ammo[weaponid])
                {
                    if (mammo[weaponid] > (clip_ammo[weaponid] - cammo[weaponid]))
                    {
                        mammo[weaponid] -= (clip_ammo[weaponid] - cammo[weaponid]);
                        cammo[weaponid] = clip_ammo[weaponid];
                    }
                    else
                    {
                        cammo[weaponid] = mammo[weaponid] - (clip_ammo[weaponid] - cammo[weaponid]);
                        cammo[weaponid] = clip_ammo[weaponid] + cammo[weaponid];
                        mammo[weaponid] = 0;
                    }
					controller.GetAnim("rel");
                    gui_cammo.text = cammo[weaponid].ToString("00");
                    gui_mammo.text = mammo[weaponid].ToString("000");
                    gui_ammo.ChangeValue(cammo[weaponid], clip_ammo[weaponid]);
                    audiosrc.audio.PlayOneShot(weapon_reload[weaponid]);
                    controller.GetReload(weaponid);
                    wp_pivot.localPosition = reload_pos;
                    BlockWeapon2(reload_delay[weaponid]);
                }
            }
            else if (Input.GetButton("Grenade") && !disable_gren)
            {
                if (!used_grenade)
                {
                    used_grenade = true;
                    controller.RequestGrenade(proj_point.position, proj_point.rotation);
                }
            }
        }
		else if(changing)
		{
			wp_pivot.localPosition = Vector3.MoveTowards(wp_pivot.localPosition, dir ? normal_pos : reload_pos, 0.025f);
			if(!dir && wp_pivot.localPosition == reload_pos)
			{
				weapon[oldid].renderer.enabled = false;
				if (!isdead)
	            	weapon[weaponid].renderer.enabled = true;
				gui_ammo.ChangeValue(cammo[weaponid], clip_ammo[weaponid]);
		        gui_cammo.text = cammo[weaponid].ToString("00");
		        gui_mammo.text = mammo[weaponid].ToString("000");
				controller.ChangeWeapon(weaponid);
				dir = true;
			}
			if(dir && wp_pivot.localPosition == normal_pos)
			{
		        blockshot = false;
				changing = false;
				dir = false;
			}
		}
		
		if(Input.GetButton("Respawn"))
		{
			if(!isdown)
			{
				if(Physics.Raycast(proj_point.position,proj_point.forward,out hit,2.0f))
				{
					if(hit.collider.name == "squote")
						hit.collider.GetComponent<StellaTaunt>().DoTaunt();
					else if(hit.collider.name == "fquote" || hit.collider.name == "bus")
						hit.collider.GetComponent<NormalQuote>().DoQuote();
				}
				isdown = true;
			}
		}
		else isdown = false;
		
		if(!controller.GetComponent<FPSWalkerEnhanced>().Grounded)
		{
			if(!Input.GetButton("Crouch")) controller.GetAnim("fall");
			else controller.GetAnim("cr");
		}
		else if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
			if (Input.GetButton("Crouch")) controller.GetAnim("crw");
			else if (Input.GetButton("Run")) controller.GetAnim("run");
			else controller.GetAnim("walk");
		}
		else if(Input.GetButton("Crouch")) controller.GetAnim("cr");
		else controller.GetAnim("idle");
    }
	
	//Shooting
    void BlockWeapon(float seconds)
    {
        blockshot = true;
		StartCoroutine(WeaponSleep(seconds));
    }
	
	IEnumerator WeaponSleep(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		if (!ismelee)
            wp_pivot.localPosition = normal_pos;
		StartCoroutine(UnBlockWeapon(seconds));
	}

    IEnumerator UnBlockWeapon(float seconds)
    {
        yield return new WaitForSeconds(seconds);
		if(enabled)
		{
			if (Input.GetButton("Fire1") && ismelee)
			{
				weapon[weaponid].animation.Play("st_idle");
	            DoMeleeAttack();
			}
	        else if (Input.GetButton("Fire1") && cammo[weaponid] > 0 && !ismelee)
	            DoShot();
			else
			{
				if(ismelee)
					weapon[weaponid].animation.Play("st_idle");
	        	blockshot = false;
			}
		}
		else
			blockshot = false;
    }
	
	IEnumerator ResetGunFire(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        audiosrc.light.enabled = false;
        gunflash.enabled = false;
    }
	
	//Reloading
	void BlockWeapon2(float seconds)
    {
        blockshot = true;
		StartCoroutine(UnBlockWeapon2(seconds));
    }
	
	IEnumerator UnBlockWeapon2(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        blockshot = false;
        if (!ismelee)
            wp_pivot.localPosition = normal_pos;
        else
            weapon[weaponid].animation.Play("st_idle");
    }
}