using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeMenuMouseLookY : MonoBehaviour
{

	public float sensitivityY = 15F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	public float rotationY = 0F;
	
	private List<float> rotArrayY = new List<float>();
	float rotAverageY = 0F;
	
	public float frameCounter = 20;
	
	Quaternion originalRotation;

	public AeMainMenu MainMenu;
	
	void Update ()
	{
		sensitivityY = MainMenu.Sensibility;
		sensitivityY = 5.0F;

		rotAverageY = 0f;
		if(rotationY < minimumY)
		{
			rotationY = minimumY;
		}
		if(rotationY > maximumY)
		{
			rotationY = maximumY;
		}

		rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

		rotArrayY.Add(rotationY);
		
		if (rotArrayY.Count >= frameCounter) 
		{
			rotArrayY.RemoveAt(0);
		}
		for(int j = 0; j < rotArrayY.Count; j++) 
		{
			rotAverageY += rotArrayY[j];
		}
		rotAverageY /= rotArrayY.Count;
		
		rotAverageY = ClampAngle (rotAverageY, minimumY, maximumY);
		
		Quaternion yQuaternion = Quaternion.AngleAxis (rotAverageY, Vector3.left);
		transform.localRotation = originalRotation * yQuaternion;
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
