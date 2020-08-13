using UnityEngine;
using System.Collections;

public class BossScript : MonoBehaviour {
	
	public Object[] weapon;
	public AudioClip[] weapon_snd;
	
	Collider bounds;
	Transform proj_point;
	Transform target;
	bool alert = false;
	float self_y;
	Vector3 tmpmove;
	System.Random rnd = new System.Random();
	
	int hp = 100000;
	
	//Weapons
	int shots = 0;
	Vector3 height_add = new Vector3(0,4,0);
	Vector3 spiral_shot = new Vector3(0,0,0);
	Vector3 spiral_add = new Vector3(0,45,0);
	
	public GameObject orig_skin;
	public GameObject alt_skin;
	public GUIText gui_hp;
	
	void Start()
	{
		if(Network.isClient)
			enabled = false;
		else
		{
			self_y = transform.position.y;
			bounds = transform.parent.Find("bounds").collider;
			proj_point = transform.Find("proj_point");
		}
	}
	
	public void SetBossHP(int health)
	{
		hp = health;	
	}

	void FixedUpdate()
	{
		if(alert)
		{
			if(!target)
				target = GameObject.Find("GameControl").GetComponent<NetworkMenu>().plobj.transform;
			transform.LookAt(new Vector3(target.position.x,self_y,target.position.z));
			proj_point.LookAt(target);
			tmpmove = transform.position+transform.TransformDirection(0,0,1);
			if(bounds.bounds.Contains(tmpmove))
				transform.position = tmpmove;
		}
	}
	
	IEnumerator AttackTimer()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.3f);
			if(shots < 5)
			{
				Network.Instantiate(weapon[0],proj_point.position,proj_point.rotation * Quaternion.Euler(0,10,0),0);
				Network.Instantiate(weapon[0],proj_point.position,proj_point.rotation,0);
				Network.Instantiate(weapon[0],proj_point.position,proj_point.rotation * Quaternion.Euler(0,-10,0),0);
				networkView.RPC("SendSND",RPCMode.All,0);
				shots+=1;
				if(shots > 4)
					StartCoroutine(ResetAttack());
			}
		}
	}
	
	IEnumerator ResetAttack()
	{
		yield return new WaitForSeconds(3);
		shots = 0;
	}
	
	IEnumerator SpecAttackTimer()
	{
		while(true)
		{
			yield return new WaitForSeconds(rnd.Next(5,15));
			DoAttack(rnd.Next(0,5));
		}
	}
	
	IEnumerator ResetSkin()
	{
		yield return new WaitForSeconds(5);
		networkView.RPC("ShowNico",RPCMode.All,false);
	}
	
	void DoAttack(int input)
	{
		switch(input)
		{
			case 0:
			{
				networkView.RPC("SendSND",RPCMode.All,1);
				spiral_shot = new Vector3(0,0,0);
				for(int i = 0; i < 8; i++)
				{
					Network.Instantiate(
					weapon[1],
					transform.position+height_add,
					Quaternion.Euler(spiral_shot),
					0);
					spiral_shot += spiral_add;			
				}
				break;
			}
			case 1:
			{
				networkView.RPC("SendSND",RPCMode.All,1);
				for(int i = 0; i < 16; i++)
				{
					Network.Instantiate(weapon[1],new Vector3(rnd.Next(-60,60),100,rnd.Next(-50,50)),Quaternion.Euler(90,0,0),0);
				}
				break;
			}
			case 2:
			{
				networkView.RPC("SendSND",RPCMode.All,2);
				spiral_shot = new Vector3(0,0,0);
				for(int i = 0; i < 8; i++)
				{
					Network.Instantiate(
					weapon[2],
					transform.position+height_add,
					Quaternion.Euler(spiral_shot),
					0);
					spiral_shot += spiral_add;			
				}
				break;
			}
			case 3:
			{
				networkView.RPC("ShowNico",RPCMode.All,true);
				StartCoroutine(ResetSkin());
				networkView.RPC("SendSND",RPCMode.All,2);
				for(int i = 0; i < 16; i++)
				{
					Network.Instantiate(weapon[2],new Vector3(rnd.Next(-60,60),100,rnd.Next(-50,50)),Quaternion.Euler(90,0,0),0);
				}
				break;
			}
			case 4:
			{
				networkView.RPC("SendSND",RPCMode.All,1);
				((GameObject)Network.Instantiate(weapon[1],new Vector3(0,100,0),Quaternion.Euler(90,0,0),0)).GetComponent<BossHeatseek>().TargetPlayer(target);
				break;
			}
		}
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(col.name.Contains("Player"))
			col.GetComponent<MainScript>().SendDamage(null,100);
	}
	
	public void SendSetTarget(Transform player, int damage)
	{
		if(Network.isClient)
			networkView.RPC("SetTarget",RPCMode.Server,player.networkView.viewID, damage);
		else
			SetTarget(player.networkView.viewID, damage);
	}
	
	[RPC]
	void SetTarget(NetworkViewID player, int damage)
	{
		if(!alert)
		{
			StartCoroutine(AttackTimer());
			StartCoroutine(SpecAttackTimer());
			alert = true;
		}
		target = NetworkView.Find(player).transform;
		hp -= damage;
		networkView.RPC("ShowHP",RPCMode.All,hp);
		if(hp <= 0)
			networkView.RPC("KillBoss",RPCMode.AllBuffered);
	}
	
	[RPC]
	void ShowHP(int bosshp)
	{
		gui_hp.text = "Stella HP: " + bosshp;
	}
	
	[RPC]
	void KillBoss()
	{
		orig_skin.transform.localPosition = new Vector3(0,-0.88f,0);
		orig_skin.transform.localRotation = Quaternion.Euler(0,0,-90);
		Destroy(this);
		Destroy(GetComponent<BoxCollider>());
	}
	
	[RPC]
	void SendSND(int snd)
	{
		audio.PlayOneShot(weapon_snd[snd]);
	}
	
	[RPC]
	void ShowNico(bool input)
	{
		orig_skin.renderer.enabled = !input;
		alt_skin.renderer.enabled = input;
	}
}