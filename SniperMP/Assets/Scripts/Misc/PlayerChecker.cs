using UnityEngine;
using System.Collections;

public class PlayerChecker : MonoBehaviour {
	
	public Transform proj_point;
	RaycastHit hit = new RaycastHit();
	
	GUIText hud_name;
	
	void Start()
	{
		hud_name = GameObject.Find("HudName").guiText;
		hud_name.material.color = Color.red;
	}
	
	void FixedUpdate ()
	{
		if(Physics.Raycast(proj_point.transform.position,proj_point.transform.forward,out hit,50.0f,9))
		{
			if(hit.collider.gameObject.name.Contains("Player"))
			{
				if(!hud_name.enabled)
					hud_name.enabled = true;
				hud_name.text = hit.collider.GetComponent<MainScript>().player_name;
			}
			else if(hud_name.enabled)
				hud_name.enabled = false;
		}
		else if(hud_name.enabled)
			hud_name.enabled = false;
	}
}