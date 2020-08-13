using UnityEngine;
using System.Collections;

public class HudHide : MonoBehaviour {
	
	public Transform[] hud;
	public Transform[] hud2;
	int hide = 0;
	
	public void HideHud()
	{
		if(hide == 0)
			foreach(Transform h in hud)
				h.gameObject.SetActiveRecursively(false);
		else if(hide == 1)
			foreach(Transform h in hud2)
				h.gameObject.SetActiveRecursively(false);
		else
		{
			foreach(Transform h in hud)
				h.gameObject.SetActiveRecursively(true);
			foreach(Transform h in hud2)
				h.gameObject.SetActiveRecursively(true);
		}
		hide += 1;
		if(hide > 2) hide = 0;
	}
}