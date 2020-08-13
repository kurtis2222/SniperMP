using UnityEngine;
using System.Collections;

public class HudScale : MonoBehaviour
{	
	void Awake()
	{
		float height = Screen.height;
		if(height < 768)
		{
			height /= 768;
			guiTexture.pixelInset =
				new Rect(guiTexture.pixelInset.x * height,
					guiTexture.pixelInset.y * height,
					guiTexture.pixelInset.width * height,
					guiTexture.pixelInset.height * height);
		}
		Destroy(this);
	}
}