using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {
	
	public GameObject menu;
	public GameObject[] menu_hide;
	public GameObject menu_show;
	public int menuid;
	public Object[] image;
	
	void Start()
	{
		renderer.material.mainTexture = (Texture)image[0];
		Screen.showCursor = true;
		Screen.lockCursor = false;
	}
	
	// Use this for initialization
	void OnMouseEnter () {
		renderer.material.mainTexture = (Texture)image[1];
	}
	
	// Update is called once per frame
	void OnMouseExit () {
		renderer.material.mainTexture = (Texture)image[0];
	}
	
	void OnMouseDown()
	{
		renderer.material.mainTexture = (Texture)image[0];
		switch(menuid)
		{
			case 0:
				{
					menu.GetComponent<NewGMenu>().enabled = true;
					menu_show.SetActiveRecursively(true);
					break;
				}
			case 1:
				{
					Application.Quit();
					break;
				}
			case 2:
				{
					menu.GetComponent<OptionsMenu>().enabled = true;
					menu.renderer.material.mainTexture = (Texture)image[2];
					break;
				}
			case 3:
				{
					menu_show.SetActiveRecursively(true);
					GameObject.Find("GameControl").GetComponent<NetworkMenu>().isenabled = true;
					break;
				}
			case 4:
				{
					menu.GetComponent<ServOptMenu>().enabled = true;
					menu.renderer.material.mainTexture = (Texture)image[2];
					break;
				}
		}
		foreach(GameObject h in menu_hide)
			h.SetActiveRecursively(false);
	}
}