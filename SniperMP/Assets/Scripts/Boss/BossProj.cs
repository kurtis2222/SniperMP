using UnityEngine;
using System.Collections;

public class BossProj : MonoBehaviour {
	
	public float speed = 5.0f;
	public float radius = 1.0f;
	public float time = 5.0f;
	public AudioClip impact_snd;
	public Renderer visible_obj;
	RaycastHit hit = new RaycastHit();
	bool isblown = false;
	
	void Start()
	{
		if(Network.isClient)
			enabled = false;
		else
		{
			rigidbody.velocity = transform.TransformDirection(0,0,speed);
			StartCoroutine(AutoDestroy());
		}
	}
	
	void FixedUpdate()
	{
		if(!isblown)
		{
			if(Physics.SphereCast(transform.position,radius,transform.forward,out hit,1.0f))
			{
				if(hit.collider.name.Contains("Player"))
				{
					if(hit.collider.networkView.isMine)
						hit.collider.GetComponent<MainScript>().SendDamage(null,100);
					else
						hit.collider.GetComponent<MainScript>().DoServerDamage(hit.collider.networkView.viewID,100);
				}
				foreach(Collider c in Physics.OverlapSphere(hit.point,5.0f))
				{
					if(c.name.Contains("Player"))
					{
						if(c.collider.networkView.isMine)
							c.collider.GetComponent<MainScript>().SendDamage(null,100);
						else
							c.collider.GetComponent<MainScript>().DoServerDamage(c.collider.networkView.viewID,100);
					}
				}
				networkView.RPC("SendSND",RPCMode.All);
				isblown = true;
				time = impact_snd.length;
				StartCoroutine(AutoDestroy2());
			}
		}
	}
	
	IEnumerator AutoDestroy()
	{
		yield return new WaitForSeconds(time);
		if(!isblown)
		{
			Network.RemoveRPCs(networkView.viewID);
			Network.Destroy(gameObject);
			Destroy(gameObject);
		}
	}
	
	IEnumerator AutoDestroy2()
	{
		yield return new WaitForSeconds(time);
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(gameObject);
		Destroy(gameObject);
	}
	
	[RPC]
	void SendSND()
	{
		if(visible_obj == null)
			renderer.enabled = false;
		else
			visible_obj.enabled = false;
		audio.PlayOneShot(impact_snd);
	}
}