using UnityEngine;
using System.Collections;

public class AeTrailBullet : MonoBehaviour 
{
	public float speed;
	public float TimerAppear = 0.2f;
	void Start () 
	{
	
	}
	void Awake ()
	{
		Invoke("ShowAgain",TimerAppear);
	}
	void ShowAgain ()
	{
		this.renderer.enabled = true;
	}
	void Update () 
	{
		transform.position += transform.TransformDirection(Vector3.forward * speed);
	}

	void OnTriggerEnter(Collider other)
	{
		Destroy(this.gameObject);
	}
}
