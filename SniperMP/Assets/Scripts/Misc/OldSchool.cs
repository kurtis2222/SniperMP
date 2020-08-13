using UnityEngine;
using System.Collections;

public class OldSchool : MonoBehaviour {

	public GameObject[] pickups;
	public Texture[] icons;
	public AudioClip[] sounds = new AudioClip[3];
	
	public int[] ammo;
	
	GUITexture hud_pickup;
	
	void Start()
	{
		ammo = new int[]
		{
			68,
			24,
			16,
			120,
			100,
			20,
			50,
			50,
			30,
			-1,
			-1
		};
	}
	
	public void ShowPickup(int id)
	{
		if(hud_pickup == null)
			hud_pickup = GameObject.Find("HudPickup").guiTexture;
		
		hud_pickup.texture = icons[id];
		hud_pickup.enabled = true;
		StartCoroutine(HideIcon());
	}
	
	IEnumerator HideIcon()
	{
		yield return new WaitForSeconds(1.0f);
		hud_pickup.enabled = false;
	}
}