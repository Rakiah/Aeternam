using UnityEngine;
using System.Collections;

public class AeOnMouse : MonoBehaviour 
{
	public float Rotation = 180;
	Quaternion TransRot;
	void Start () 
	{
		TransRot.y = Rotation;
		TransRot = this.transform.rotation;
		transform.rotation = TransRot;
	}

	void Update () 
	{
		transform.Rotate(0,0.5f,0);
	}

	void OnMouseOver ()
	{
		if(Input.GetMouseButton(0))
		{
			transform.Rotate(0,2.5f,0);
		}
		else if(Input.GetMouseButton(1))
		{
			transform.Rotate(0,-2.5f,0);
		}
	}
}
