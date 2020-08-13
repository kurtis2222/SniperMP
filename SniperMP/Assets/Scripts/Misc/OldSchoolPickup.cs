using UnityEngine;
using System.Collections;

public class OldSchoolPickup : MonoBehaviour {
	
	public enum pickup_wp
	{
		Glock = 0,
		SnW = 1,
		Mossberg = 2,
		SIG = 3,
		Ingram = 4,
		M14 = 5,
		HKG8 = 6,
		P90 = 7,
		SawedOff = 8,
		Grenade = 9,
		Medipack = 10
	}
	
	public pickup_wp weapon;
	
	OldSchool control;
	GameObject pickup;
	int time = 0;
	
	void Start ()
	{
		control = transform.parent.GetComponent<OldSchool>();
		pickup = (GameObject)GameObject.Instantiate(control.pickups[(int)weapon],
			transform.position + new Vector3(0,1,0),
			new Quaternion(-0.7f,0,0,0.7f));
		pickup.transform.parent = gameObject.transform;
		StartCoroutine(Respawn());
	}
	
	void OnTriggerEnter(Collider col)
	{
		if(col.name.Contains("Player") && time == 0)
		{
			if(col.networkView.isMine)
			{
				if((int)weapon != 10)
				{
					if(col.GetComponentInChildren<WeaponScript>().GivePickup((int)weapon,control.ammo[(int)weapon]))
					{
						audio.PlayOneShot(control.sounds[0]);
						control.ShowPickup((int)weapon);
						pickup.renderer.enabled = false;
						time = 30;
					}
				}
				else
				{
					if(col.GetComponent<MainScript>().GiveHP())
					{
						audio.PlayOneShot(control.sounds[2]);
						control.ShowPickup((int)weapon);
						pickup.renderer.enabled = false;
						time = 30;
					}
				}
			}
		}
	}
	
	IEnumerator Respawn()
	{
		while(true)
		{
			yield return new WaitForSeconds(1.0f);
			if(time > 0)
			{
				time--;
				if(time == 0)
				{
					pickup.renderer.enabled = true;
					audio.PlayOneShot(control.sounds[1]);
				}
			}
		}
	}
}