using UnityEngine;
using System.Collections;

public class HudStamina : MonoBehaviour
{	
	float st_max;
	float st, st_old;
	Rect data;
	float tmp;
	
	public void GetInfo()
	{
		enabled = true;
	}
	
	void Start()
	{
		data = guiTexture.pixelInset;
		st_max = guiTexture.pixelInset.width;
		st = MainScript.stamina;
		st_old = st;
	}
	
	void FixedUpdate ()
	{
		st = MainScript.stamina;
		if(st_old != st)
		{
			tmp = st / 1000 * st_max;
			data.width = tmp;
			guiTexture.pixelInset = data;
		}
		st_old = st;
	}
}