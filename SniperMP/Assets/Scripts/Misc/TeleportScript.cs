using UnityEngine;
using System.Collections;

public class TeleportScript : MonoBehaviour {
	
	public Transform teleport_pos;
	
	void OnTriggerEnter(Collider col)
	{
		if(col.name.Contains("Player"))
		{
			col.transform.position = teleport_pos.position;
			col.transform.rotation = teleport_pos.rotation;
		}
	}
}