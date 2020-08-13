using UnityEngine;
using System.Collections;

public class NewGame : MonoBehaviour {
	
	const int lvl_start = 1;
	public int levelid = 0;
	Material normal;
	public Material selected;
	public TextMesh map_txt;
	public string lvl_name;
	
	void OnEnable()
	{
		if(normal != null)
			renderer.material = normal;
	}
	
	void Start()
	{
		normal = renderer.material;
	}
	
	void OnMouseDown()
	{
		renderer.material = normal;
		Application.LoadLevel(lvl_start+levelid);
	}

	void OnMouseEnter () {
		renderer.material = selected;
		map_txt.text = lvl_name;
	}

	void OnMouseExit () {
		renderer.material = normal;
		map_txt.text = null;
	}
}
