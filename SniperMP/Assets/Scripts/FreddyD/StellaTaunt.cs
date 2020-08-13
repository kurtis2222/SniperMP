using UnityEngine;
using System.Collections;

public class StellaTaunt : MonoBehaviour {

	public AudioClip[] quotes;
	bool delayed = false;
	int tmp, old = -1;
	
	public void DoTaunt()
	{
		if(!delayed)
		{
		retry:
			tmp = Random.Range(0,quotes.Length);
			if(tmp == old) goto retry;
			old = tmp;
			audio.PlayOneShot(quotes[tmp]);
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