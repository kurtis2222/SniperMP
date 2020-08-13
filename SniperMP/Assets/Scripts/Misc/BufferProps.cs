using UnityEngine;
using System.Collections;

public class BufferProps : MonoBehaviour
{
	[RPC]
	void Call1(int data)
	{
		transform.parent.GetComponent<MainScript>().SendChangeWeapon(data);
	}
	
	[RPC]
	void Call2(bool data)
	{
		transform.parent.GetComponent<MainScript>().PlayerCollisions(data);
	}
}
