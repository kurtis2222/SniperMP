using UnityEngine;
using System.Collections;

public class NewGMenu : MonoBehaviour {
	
	public GameObject newgame;
	public GameObject lvl_menu;
	
	void Start ()
	{
	
	}
	
	void FixedUpdate ()
	{
		if(Input.GetKey(KeyCode.Escape))
		{
			foreach (GameObject h in newgame.GetComponent<MenuScript>().menu_hide)
				h.SetActiveRecursively (true);
			lvl_menu.SetActiveRecursively(false);
			enabled = false;
		}
	}
}
