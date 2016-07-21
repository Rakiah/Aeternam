using UnityEngine;
using System.Collections;

public class AeSplashScreen : MonoBehaviour 
{

	void Start () 
	{
		Invoke("Load",0.1f);
	}
	
	void Update () 
	{
		
	}
	void Load ()
	{
		Application.LoadLevel(1);
	}
}
