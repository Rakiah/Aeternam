using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeMouseLookX : MonoBehaviour 
{
    public float sensitivityX = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;


    public float rotationX = 0F;

    private List<float> rotArrayX = new List<float>();
    public float rotAverageX = 0F;    

    public float frameCounter = 20;
	
	public bool NotScoping = true;

	public bool LockSlide = false;
	bool calculated;

    Quaternion originalRotation;

	void Awake ()
	{
		originalRotation = transform.localRotation;
	}

	public void Move (float MouseX)
	{
		if(!NotScoping) MouseX = MouseX/5;


		rotAverageX = 0f;
		
		rotationX += MouseX;
		
		rotArrayX.Add(rotationX);
		
		if (rotArrayX.Count >= frameCounter) rotArrayX.RemoveAt(0);

		for(int i = 0; i < rotArrayX.Count; i++) rotAverageX += rotArrayX[i];

		rotAverageX /= rotArrayX.Count;
		rotAverageX = AeTools.ClampAngle (rotAverageX, minimumX, maximumX);
		Quaternion xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.up);
		transform.localRotation = originalRotation * xQuaternion;
	}
}
