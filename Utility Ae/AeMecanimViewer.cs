using UnityEngine;
using System.Collections;

public class AeMecanimViewer : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.GetComponent<Animator>().SetBool("Next",false);
		this.GetComponent<Animator>().SetBool("Back",false);
		if(Input.GetMouseButtonDown(0))
		{
			Debug.Log("Wtf?");
			this.GetComponent<Animator>().SetBool("Next",true);
		}
		else if(Input.GetMouseButtonDown(1))
		{
			this.GetComponent<Animator>().SetBool("Back",true);
		}
	}
}
