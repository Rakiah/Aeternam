using UnityEngine;
using System.Collections;

public class AeControllerStats : MonoBehaviour
{
	public float velMag;
	public float height;
	public Vector3 center;
	public float hor;
	public float ver;
	public float mouseX;
	public float mouseY;
	public float health;
	public bool running;
	public bool Parrying;
	public float speed;
	public float velPercent;
	public int iStanceID;

	ComponentsManager manager;

	void Awake ()
	{
		manager = transform.root.GetComponent<ComponentsManager>();
	}

	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		
		// Send data to server
		if (stream.isWriting)
		{
			stream.Serialize(ref velMag);
			stream.Serialize(ref height);
			stream.Serialize(ref center);
			stream.Serialize(ref hor);
			stream.Serialize(ref ver);
			stream.Serialize(ref Parrying);
			stream.Serialize(ref iStanceID);
			stream.Serialize(ref health);
		}
		else
		{
			stream.Serialize(ref velMag);
			stream.Serialize(ref height);
			stream.Serialize(ref center);
			stream.Serialize(ref hor);
			stream.Serialize(ref ver);
			stream.Serialize(ref Parrying);
			stream.Serialize(ref iStanceID);
			stream.Serialize(ref health);
		}
	}
	
	void Update()
	{

		if (networkView.isMine)
		{
			mouseX = manager.m_pMouseX.rotationX;
			mouseY = manager.m_pMouseY.rotationY;
			hor = manager.m_pMovements.inputX;
			ver = manager.m_pMovements.inputY;
			velMag = manager.controller.velocity.magnitude;
			iStanceID = manager.m_pMovements.iStanceID;
			velPercent = velMag / speed;
		}
		else
		{
			manager.m_pMouseX.rotationX = mouseX;
			manager.m_pMouseY.rotationY = mouseY;
			manager.m_pMovements.inputX = hor;
			manager.m_pMovements.inputY = ver;
			manager.m_pMovements.iStanceID = iStanceID;
			velPercent = velMag / speed;
		}
	}
}
