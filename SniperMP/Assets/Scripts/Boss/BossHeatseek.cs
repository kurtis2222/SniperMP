using UnityEngine;
using System.Collections;

public class BossHeatseek : MonoBehaviour {
	
	Transform target;
	
	public void TargetPlayer(Transform pl)
	{
		target = pl;
		enabled = true;
	}

	void FixedUpdate()
	{
		if(target)
		{
			transform.LookAt(target);
			transform.rigidbody.velocity = transform.TransformDirection(0,0,40);
		}
	}
}