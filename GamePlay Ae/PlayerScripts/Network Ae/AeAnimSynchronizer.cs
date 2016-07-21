using UnityEngine;
using System.Collections;

public class AeAnimSynchronizer : MonoBehaviour 
{
	public Animator AnimatorCameraLocomotion;
	public Animator AnimatorCameraSlideW;
	public Animator MainCam;
	public Animator AnimatorObject;
	public Transform Spine;

	[HideInInspector] public Animator CurrentWeaponAnimator;
	[HideInInspector] public Quaternion Rotation;

	ComponentsManager manager;
	

	void Awake ()
	{
		Quaternion parentRotInv = Quaternion.Inverse(transform.root.rotation); 
		Rotation = parentRotInv * Spine.rotation; 
		manager = GetComponent<ComponentsManager>();
	}

	

	void Update () 
	{

		//MULTI AND LOCAL PART
		AnimatorObject.SetInteger("StanceID",manager.m_pStatsSynchronizer.iStanceID);
		AnimatorObject.SetFloat("Direction",manager.m_pStatsSynchronizer.hor);
		AnimatorObject.SetFloat("Ver",manager.m_pStatsSynchronizer.ver);
		AnimatorObject.SetInteger("CurrentWeapon",manager.m_pWeaponHandler.currentWeapon);


		if(CurrentWeaponAnimator)
		{
			CurrentWeaponAnimator.SetInteger("uStanceID",manager.m_pStatsSynchronizer.iStanceID);
		}


		//LOCAL PART ONLY

		if(networkView.isMine)
		{
			AnimatorCameraLocomotion.SetInteger("StanceID",manager.m_pStatsSynchronizer.iStanceID);
			AnimatorCameraSlideW.SetInteger("StanceID",manager.m_pStatsSynchronizer.iStanceID);
		}
	}

	public void PlayFallHigh ()
	{
		MainCam.SetTrigger("FallHighLock");
	}
	public void PlayFall ()
	{
		AnimatorCameraSlideW.SetTrigger("FallMid");
	}
	public void PlayReload ()
	{
		try
		{
			CurrentWeaponAnimator.SetTrigger("Reload");
		}
		catch
		{
			//			Debug.Log("No Reload");
		}
	}
	public void PlayMagicShoot ()
	{
		try
		{
			CurrentWeaponAnimator.SetTrigger("MagicShot");
		}
		catch
		{
			//			Debug.Log("No magic shot");
		}
	}
	public void PlayAction1 ()
	{
		try
		{
			CurrentWeaponAnimator.SetTrigger("Attack");
		}
		catch
		{
			//			Debug.Log("No Shoot");
		}
	}
	public void PlayAction3 ()
	{
		try
		{
			CurrentWeaponAnimator.SetBool("Parry",!CurrentWeaponAnimator.GetBool("Parry"));
		}
		catch
		{
//			Debug.Log("no parry");
		}
	}
	public void PlayShow ()
	{
		try
		{
			CurrentWeaponAnimator.SetTrigger("Show");
		}
		catch
		{
//			Debug.Log("No Show");
		}
	}
	public void PlayHide ()
	{
		try
		{
			CurrentWeaponAnimator.SetTrigger("Hide");
		}
		catch
		{
//			Debug.Log("No Hide");
		}
	}
	public void PlayJump ()
	{
		try
		{
			CurrentWeaponAnimator.SetTrigger("Jump");
		}
		catch
		{
//			Debug.Log("No Jump");
		}
	}

	void LateUpdate ()
	{
		if(!networkView.isMine) Spine.rotation = manager.m_pWeaponHandler.transform.rotation * Rotation;
	}
}
