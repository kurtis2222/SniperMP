using UnityEngine;
using System.Collections;

public class HudAmmo : MonoBehaviour
{	
	float tmp;
	float st_max;
	Rect data;
	
	void Start()
	{
		data = guiTexture.pixelInset;
		st_max = guiTexture.pixelInset.width;
	}
	
	public void ChangeValue (int input, int max)
	{
		if(max == 0) tmp = 0;
		else tmp = (float)input / (float)max * st_max;
		data.width = tmp;
		guiTexture.pixelInset = data;
	}
	
	public void ChangeValue2 (int input, int max)
	{
		if(data != default(Rect))
		{
			if(max == 0) tmp = 0;
			else tmp = (float)input / (float)max * st_max;
			data.width = tmp;
			guiTexture.pixelInset = data;
		}
	}
}