using UnityEngine;
using System.Collections;

public class HudwIcon : MonoBehaviour {

	public Texture[] icon;
	
	public void ChangeIcon(int id)
	{
		guiTexture.texture = icon[id];
	}
}
