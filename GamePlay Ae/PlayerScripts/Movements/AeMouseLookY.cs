using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeMouseLookY : MonoBehaviour 
{
    public float sensitivityY = 15F;
	
    public float minimumY = -60F;
    public float maximumY = 60F;

    public float rotationY = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 20;
	public bool NotScoping = true;

    Quaternion originalRotation;

	void Awake ()
	{
		originalRotation = transform.localRotation;
	}

	public void Move (float MouseY)
	{
		if(!NotScoping) MouseY = MouseY/5;

		if(rotationY > maximumY) rotationY = maximumY;
		else if(rotationY < minimumY) rotationY = minimumY;

		

		if(rotationY <= maximumY && rotationY >= minimumY) rotationY += MouseY;

		rotArrayY.Add(rotationY);
		
		if (rotArrayY.Count >= frameCounter) rotArrayY.RemoveAt(0);

		for(int j = 0; j < rotArrayY.Count; j++) rotAverageY += rotArrayY[j];
		
		rotAverageY /= rotArrayY.Count;
		
		rotAverageY = AeTools.ClampAngle (rotAverageY, minimumY, maximumY);
		
		Quaternion yQuaternion = Quaternion.AngleAxis (rotAverageY, Vector3.left);
		transform.localRotation = originalRotation * yQuaternion;
	}
}
