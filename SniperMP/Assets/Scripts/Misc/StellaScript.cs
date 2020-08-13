using UnityEngine;
using System.Collections;

public class StellaScript : MonoBehaviour {
	
	public AudioClip[] quotes;
	int tmp = 0, old_tmp = 0;
	
	public void LoadEnabled(bool input)
	{
		if(input)
			StartCoroutine(RandomQuote());
	}
	
	IEnumerator RandomQuote()
	{
		while(true)
		{
			yield return new WaitForSeconds(Random.Range(90,120));
		retry:
			tmp = Random.Range(0,quotes.Length);
			if(tmp == old_tmp) goto retry;
			old_tmp = tmp;
			audio.PlayOneShot(quotes[tmp]);
		}
	}
}
