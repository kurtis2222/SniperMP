using UnityEngine;
using System.Collections;

public class ChatGUI : MonoBehaviour {

	GUIText chat;
	
	const int MAX_LINES = 5;
	
	int lines;
	string tmp = null;
	
	public void LoadChat()
	{
		chat = GameObject.Find("HudChat").guiText;
	}
	
	public void MessageTrigger(string msg)
	{
		networkView.RPC("ShowChat",RPCMode.All,msg);
	}
	
	public void MessageTrigger2(string msg)
	{
		ShowChat(msg);
	}
	
	[RPC]
	void ShowChat(string msg)
	{
		chat.enabled = true;
		chat.text += msg + "\n";
		//Checking lines
		tmp = chat.text;
		lines = 0;
		foreach (char c in tmp)
			if (c == '\n') lines++;
		if(lines > MAX_LINES)
		{
			tmp = tmp.Remove(0,tmp.IndexOf('\n',0)+1);
			chat.text = tmp;
		}
		tmp = null;
		lines = 0;
	}
}