using UnityEngine;
using System.Collections;

public class HudTextScale : MonoBehaviour {

	void Start () {
		if(Screen.width != 1024)
			guiText.fontSize = (int)Mathf.Ceil(guiText.fontSize * Screen.width/1024);
	}
}