using UnityEngine;
using System.Collections;

public class TextureOffset : MonoBehaviour {
	
	public float speed;
	float offset;
	
	void FixedUpdate()
	{
		if(offset >= 1) offset = 0;
		offset += speed;
		gameObject.renderer.material.mainTextureOffset = new Vector2(offset,offset);
	}
}
