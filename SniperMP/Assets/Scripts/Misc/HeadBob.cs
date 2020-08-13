using UnityEngine;
using System.Collections;

public class HeadBob : MonoBehaviour {
	
	Vector3 tmp;
	Transform mainobj;
	Vector3 def_pos;
	Vector3[] move_vector = new Vector3[4];
	float limit = 0.05f;
	float speed = 0.0025f;
	float speed2 = 0.004f;
	float distance = 0.0f;
	bool ismove = false;
	bool moved = false;
	bool[] isdown = new bool[2];
	float tmpy = 0.0f;
	float[] limits = new float[4];
	
	void Start()
	{
		mainobj = transform.root;
		def_pos = transform.localPosition;
		move_vector[0] = new Vector3(speed,0,0);
		move_vector[1] = new Vector3(speed2,0,0);
		move_vector[2] = new Vector3(0,speed,0);
		move_vector[3] = new Vector3(0,speed2,0);
		limits[0] = def_pos.x+limit;
		limits[1] = def_pos.x-limit;
		limits[2] = def_pos.y+limit/2;
		limits[3] = def_pos.y-limit/2;
	}
	
	void FixedUpdate()
	{
		ismove = Input.GetButton("Horizontal") || Input.GetButton("Vertical");
		distance = Vector3.Distance(tmp,mainobj.position);
		if(ismove)
		{
			if(distance > 0.15f)
			{
				transform.localPosition += isdown[0] ? -move_vector[1] : move_vector[1];
				transform.localPosition += isdown[1] ? -move_vector[3] : move_vector[3];
				DoBob();
			}
			else if(distance > 0.05f)
			{
				transform.localPosition += isdown[0] ? -move_vector[0] : move_vector[0];
				transform.localPosition += isdown[1] ? -move_vector[2] : move_vector[2];
				DoBob();
			}
			moved = true;
		}
		else if(moved)
		{
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,def_pos,0.005f);
			if(transform.localPosition == def_pos)
				moved = false;
		}
		tmp = mainobj.position;
	}
	
	void DoBob()
	{
		tmpy = transform.localPosition.x;
		if(tmpy > limits[0]) isdown[0] = true;
		else if(tmpy < limits[1]) isdown[0] = false;
		tmpy = transform.localPosition.y;
		if(tmpy > limits[2]) isdown[1] = true;
		else if(tmpy < limits[3]) isdown[1] = false;
	}
}