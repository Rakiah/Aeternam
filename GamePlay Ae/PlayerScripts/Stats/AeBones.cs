using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeBones : MonoBehaviour 
{
	public List<GameObject> Bones = new List<GameObject>();
	public List<GameObject> ToDiasbleLocally = new List<GameObject>();
	public GameObject HeadCamHolder;
	public GameObject WeaponMultiHolder;
	public GameObject WeaponLocalHolder;
	
	public void Initialize () 
	{
		foreach(GameObject Bone in Bones)
		{
			Bone.layer = 8;
		}
		foreach(GameObject Todisable in ToDiasbleLocally)
		{
			Todisable.SetActive(false);
		}
	}
	
	public void Raggdoll ()
	{
		GetComponentInChildren<Camera>().transform.parent = HeadCamHolder.transform;
		foreach(GameObject Bone in Bones)
		{
			Bone.tag = "PlayerRag";
			Bone.rigidbody.isKinematic = false;
			Bone.collider.isTrigger = false;
		}
	}
}
