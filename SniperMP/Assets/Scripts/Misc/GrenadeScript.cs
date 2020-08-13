using UnityEngine;
using System.Collections;

public class GrenadeScript : MonoBehaviour {
	
	public GameObject particle;
	public NetworkViewID id;
	
	void Start()
	{
		if(Network.isServer)
		{
			rigidbody.velocity = transform.TransformDirection(new Vector3 (0, 0, 30));
			rigidbody.angularVelocity = transform.TransformDirection(new Vector3(25, 0, 0));
			StartCoroutine(Detonate());
		}
	}
	
	IEnumerator Detonate()
	{
		yield return new WaitForSeconds(3.0f);
		if(id.isMine)
			SendExpDam(id);
		else
			networkView.RPC("SendExpDam",id.owner,id);
		Network.Instantiate(particle,transform.position,new Quaternion(0,0,0,0),0);
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(gameObject);
		Destroy(gameObject);
	}
	
	[RPC]
	void SendExpDam(NetworkViewID pid)
	{
		Collider[] other = Physics.OverlapSphere(transform.position,5.0f);
		for(int i = 0; i < other.Length; i++)
		{
			if(other[i].name.StartsWith("Player") && other[i].networkView.viewID.isMine)
				other[i].GetComponent<MainScript>().SendDamage(null,100);
			else if(other[i].name.StartsWith("Player") && !other[i].networkView.viewID.isMine)
				other[i].GetComponent<MainScript>().DoDamage(other[i].networkView.viewID,100);
			else if(other[i].name == "Bot")
				other[i].GetComponent<BotScript>().SendSetTarget(NetworkView.Find(pid).transform,100);
		}
	}
}