using UnityEngine;
using System.Collections;

public class AeMenuCS : MonoBehaviour
{
	public float velMag;
	public bool grounded;
	public float hor;
	public float ver;
	public bool running;
	public float speed;
	public float velPercent;

	public AeMenuPlayerMovement AeM;

	void Update()
	{
		hor = AeM.inputX;
		ver = AeM.inputY;
		running = AeM.running;
		grounded = AeM.controller.isGrounded;
		velMag = AeM.controller.velocity.magnitude;
		speed = AeM.speed;
		velPercent = velMag / speed;
	}
}