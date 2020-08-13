using UnityEngine;
using System.Collections;

public class SpawnGizmo : MonoBehaviour {
	
	public Color gizmo_color = Color.red;
	
	void OnDrawGizmos () {
		Gizmos.DrawRay(transform.position, transform.forward);
		Gizmos.color = gizmo_color;
		Gizmos.DrawCube(transform.position, new Vector3(1,2,1));
	}
}
