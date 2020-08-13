using UnityEngine;
using System.Collections;

public class MenuVer : MonoBehaviour
{	
	static GUIStyle labelstyle;
	
	void Start()
	{
		if(labelstyle == null)
		{
			labelstyle = new GUIStyle();
			labelstyle.fontSize = 32;
			labelstyle.fontStyle = FontStyle.Bold;
			labelstyle.normal.textColor = Color.white;
		}
		//Change volume on load
		AudioListener.volume = CFGLoader.volume;
		Application.targetFrameRate = CFGLoader.maxfps;
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,Screen.height-32,384,32),"Verzió: 2.4",labelstyle);
	}
}