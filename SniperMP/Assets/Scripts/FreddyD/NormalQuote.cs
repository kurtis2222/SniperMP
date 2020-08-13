using UnityEngine;
using System.Collections;

public class NormalQuote : MonoBehaviour {

	public AudioClip quote;
	bool delayed = false;
	
	public void DoQuote()
	{
		if(!delayed)
		{
			audio.PlayOneShot(quote);
			delayed = true;
			StartCoroutine(Delay());
		}
	}
	
	IEnumerator Delay()
	{
		yield return new WaitForSeconds(5);
		delayed = false;
	}
}