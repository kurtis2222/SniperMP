using UnityEngine;
using System.Collections;

public class TimedDestroy : MonoBehaviour {

	public float time = 1f;
	
	void Start ()
	{
		StartCoroutine(AutoDestroy());
	}
	
	IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}