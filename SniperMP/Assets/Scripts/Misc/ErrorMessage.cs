using UnityEngine;
using System.Collections;

public class ErrorMessage : MonoBehaviour
{	
	GUIStyle labelstyle = new GUIStyle();
	string message = null;
	
	public void ShowError(string str)
	{
		labelstyle.fontSize = 16;
		labelstyle.fontStyle = FontStyle.Bold;
		labelstyle.normal.textColor = Color.white;
		message = str;
		StartCoroutine(AutoDestroy());
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(0,0,512,32),message,labelstyle);
	}
	
	IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(5.0f);
		Destroy(gameObject);
	}
}