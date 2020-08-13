using UnityEngine;
using System.Collections;

public class ImpactEffect : MonoBehaviour {
	
	public float sec_wait = 0.5f;
	public AudioClip[] impact_snd;
	
	void Start()
	{
		if (impact_snd.Length > 1) audio.PlayOneShot(impact_snd[Random.Range(0,impact_snd.Length)]);
		else audio.PlayOneShot(impact_snd[0]);
		if(!networkView)
			StartCoroutine(AutoDestroy());
		else if(Network.isServer)
			StartCoroutine(AutoDestroy2());
	}
	
	IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(sec_wait);
		Destroy(gameObject);
	}
	
	IEnumerator AutoDestroy2()
	{
		yield return new WaitForSeconds(sec_wait);
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(gameObject);
		Destroy(gameObject);
	}
}