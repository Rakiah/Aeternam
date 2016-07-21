using UnityEngine;
using System.Collections;

public class AeHeadName : MonoBehaviour 
{
	GameObject PlayerToFollow;
	[HideInInspector] public TextMesh TextHead;
	float headTimer = 0.0f;

	void Awake () 
	{
		TextHead = GetComponent<TextMesh>();
	}
	
	void Update () 
	{
		if(headTimer > 0.0f)
		{
			headTimer -= Time.deltaTime;
		}
		else
		{
			if(TextHead.renderer.enabled)
			{
				TextHead.renderer.enabled = false;
			}
		}
		if(AeCore.m_pCoreGame.MyStats.InstantiatedPlayer)
		{
			PlayerToFollow = AeCore.m_pCoreGame.MyStats.InstantiatedPlayer;
			if(PlayerToFollow)
			{
				TextHead.transform.rotation = PlayerToFollow.transform.rotation;
			}
		}
	}

	public void CheckHeadInput ()
	{
		var layerMask = ~(1 << 8);
		RaycastHit hit;
		Vector3 forward = TextHead.transform.TransformDirection(Vector3.forward);
		if(Physics.Raycast(AeCore.m_pCoreGame.MyStats.PlayerComponents.NormalCam.transform.position,forward,out hit,Mathf.Infinity,layerMask))
		{
			if(AeTools.isEnnemy(hit.collider.tag))
			{
				hit.collider.transform.root.GetComponentInChildren<AeHeadName>().TimerHead();
			}
		}
	}

	public void TimerHead()
	{
		headTimer = 2.0f;
		TextHead.renderer.enabled = true;
	}
}
