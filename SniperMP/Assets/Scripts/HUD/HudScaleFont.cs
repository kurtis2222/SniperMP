using UnityEngine;
using System.Collections;

public class HudScaleFont : MonoBehaviour {
	
	void Start()
	{
		float height = Screen.height;
		guiText.material.color = new Color(243f/255f,132f/255f,23f/255f,0.75f);
		if(height < 768)
		{
			guiText.fontSize = (int)(24 * (Screen.width+height)/1792);
			height /= 768;
			guiText.pixelOffset =
					new Vector2(guiText.pixelOffset.x * height,
						guiText.pixelOffset.y * height);
		}
		Destroy(this);
	}
}