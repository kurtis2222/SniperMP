using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour
{
	void OnTriggerEnter(Collider col)
	{
		if(col.name.Contains("Player"))
			col.GetComponent<MainScript>().SendDamage(null,100);
	}
}