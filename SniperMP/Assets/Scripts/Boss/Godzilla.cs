using UnityEngine;
using System.Collections;

public class Godzilla : MonoBehaviour {
	
	public void StartMove()
	{
		enabled = true;
	}
	
	void FixedUpdate ()
	{
		transform.position += new Vector3(0,0.01f,0);
	}
}