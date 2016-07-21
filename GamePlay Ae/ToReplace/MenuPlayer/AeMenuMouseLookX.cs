using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeMenuMouseLookX : MonoBehaviour 
{
	
	public float sensitivityX = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	
	public float rotationX = 0F;
	
	private List<float> rotArrayX = new List<float>();
	float rotAverageX = 0F;    
	
	public float frameCounter = 20;


	public AeMainMenu MainMenu;
	
	Quaternion originalRotation;
	
	void Update ()
	{
		sensitivityX = MainMenu.Sensibility;
		sensitivityX = 5.0F;

		rotAverageX = 0f;

		rotationX += Input.GetAxis("Mouse X") * sensitivityX;
		
		rotArrayX.Add(rotationX);
		
		if (rotArrayX.Count >= frameCounter)
		{
			rotArrayX.RemoveAt(0);
		}
		for(int i = 0; i < rotArrayX.Count; i++) 
		{
			rotAverageX += rotArrayX[i];
		}
		rotAverageX /= rotArrayX.Count;
		
		rotAverageX = ClampAngle (rotAverageX, minimumX, maximumX);
		
		Quaternion xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.up);
		transform.localRotation = originalRotation * xQuaternion;
	}
	
	void Start ()
	{
		MainMenu = GameObject.Find("MainMenu").GetComponent<AeMainMenu>();
		originalRotation = transform.localRotation;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F)) {
			if (angle < -360F) {
				angle += 360F;
			}
			if (angle > 360F) {
				angle -= 360F;
			}         
		}
		return Mathf.Clamp (angle, min, max);
	}
}
