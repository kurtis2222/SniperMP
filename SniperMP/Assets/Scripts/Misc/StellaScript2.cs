using UnityEngine;
using System.Collections;

public class StellaScript2 : MonoBehaviour {
	
	public AudioClip[] quotes;
	int tmp = 0, old_tmp = 0;
	
	void Start()
	{
		StartCoroutine(RandomQuote());
	}
	
	IEnumerator RandomQuote()
	{
		while(true)
		{
			yield return new WaitForSeconds(Random.Range(30,60));
		retry:
			tmp = Random.Range(0,quotes.Length);
			if(tmp == old_tmp) goto retry;
			old_tmp = tmp;
			audio.PlayOneShot(quotes[tmp]);
		}
	}
}
