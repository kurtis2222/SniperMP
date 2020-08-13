using UnityEngine;
using System.Collections;

public class LevelList : MonoBehaviour {
	
	public GameObject[] list;
	int numb = 0;
	Material normal;
	public Material selected;
	public TextMesh map_txt;
	public TextMesh page_txt;
	
	void OnEnable()
	{
		map_txt.gameObject.active = true;
		page_txt.gameObject.active = true;
		page_txt.text = numb+1 + "/" + list.Length;
		map_txt.text = null;
		numb = 0;
		list[numb].SetActiveRecursively(true);
	}
	
	void Start()
	{
		normal = renderer.material;
	}
	
	void OnMouseDown()
	{
		list[numb].SetActiveRecursively(false);
		
		numb++;
		if(numb > list.Length-1)
			numb = 0;
		
		list[numb].SetActiveRecursively(true);
		page_txt.text = numb+1 + "/" + list.Length;
	}
	
	void OnMouseEnter () {
		renderer.material = selected;
	}

	void OnMouseExit () {
		renderer.material = normal;
	}
}