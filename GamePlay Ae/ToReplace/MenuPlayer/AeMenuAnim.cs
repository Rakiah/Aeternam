using UnityEngine;
using System.Collections;

public class AeMenuAnim : MonoBehaviour 
{

	public AeMenuCS AeCS;
	public Animator AnimatorObject;
	
	void Awake () 
	{
		
	}
	
	
	
	void FixedUpdate () 
	{
		AnimatorObject.SetBool("Running",AeCS.running);
		AnimatorObject.SetBool("Jump",!AeCS.grounded);
		AnimatorObject.SetFloat("Speed",AeCS.velMag);
		AnimatorObject.SetFloat("Direction",AeCS.hor);
		AnimatorObject.SetFloat("Ver",AeCS.ver);
	}
}
