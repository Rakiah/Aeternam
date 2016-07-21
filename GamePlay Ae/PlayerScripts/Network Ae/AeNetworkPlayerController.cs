using UnityEngine;
using System.Collections;

public class AeNetworkPlayerController : MonoBehaviour 
{
	GameObject TrailBullet;
	ComponentsManager manager;
	public Animator MecaAnimatorController;
	
	void Awake () 
	{
		manager = GetComponent<ComponentsManager>();
		if(!networkView.isMine)
		{
			MecaAnimatorController.SetBool("LocalPlayer", false);
		}
	}



	[RPC]
	void Action1 ()
	{
		StartCoroutine(manager.m_pWeaponHandler.GetCurrentWeapon().DoAction1CallBack());
	}
	[RPC]
	void Action2 ()
	{
		StartCoroutine(manager.m_pWeaponHandler.GetCurrentWeapon().DoAction2CallBack());
	}
	[RPC]
	void Action3 ()
	{
		StartCoroutine(manager.m_pWeaponHandler.GetCurrentWeapon().DoAction3CallBack());
	}

	[RPC]
	void ActionGauntlet()
	{
		StartCoroutine(manager.m_pWeaponHandler.gauntlet.ActionCallBack());
	}

	[RPC]
	void SwitchGuns (int NewGun)
	{
		manager.m_pWeaponHandler.StartCoroutine("SwitchGunsCallBack",NewGun);
	}

	[RPC]
	void TakeDamage (int PlayerAttackMeID,int DamageTaken,int WeaponUsed,int BodyPart)
	{
		float BodypartMultiplicator = 1.0f;
		float damageFloat = (float)DamageTaken;
		switch(BodyPart)
		{
			case 1 : 
			BodypartMultiplicator = 2.5f;
			break;
			case 2 : 
			BodypartMultiplicator = 1.0f;
			break;
			case 3 : 
			BodypartMultiplicator = 1.5f;
			break;
			case 4 : 
			BodypartMultiplicator = 0.7f;
			break;
			case 5 : 
			BodypartMultiplicator = 0.7f;
			break;
		}
		DamageTaken = (int)(damageFloat * BodypartMultiplicator);
		manager.m_pStats.CheckDamageAndHealth(PlayerAttackMeID,DamageTaken,BodyPart,WeaponUsed);
	}
	[RPC]
	void ShowPseudoHead (string PseudoHead)
	{
		manager.m_pNickNameHandler.TextHead.text = PseudoHead;
	}


	[RPC]
	void SetWeapon (int WeaponID)
	{
		manager.m_pWeaponHandler.WeaponInventory.Add(AeTools.CopyWeaponById(WeaponID));
		int listID = manager.m_pWeaponHandler.WeaponInventory.Count - 1;

		if(listID >= 0)
		{
			GameObject WepHolder = manager.m_pBones.WeaponLocalHolder.transform.FindChild((listID + 1).ToString()).gameObject;
			if(!manager.m_pWeaponHandler.WeaponInventory[listID].Initialize(networkView.isMine, manager.m_pBones.WeaponMultiHolder, WepHolder)) Debug.Log("Failed while adding weapon");
			
		}
	}

	[RPC]
	void SetGauntlet ()
	{
		manager.m_pWeaponHandler.gauntlet = new ElectricLineCatalyser();

		GameObject WepHolder = manager.m_pBones.WeaponLocalHolder.transform.FindChild("Gauntlet").gameObject;

		if(!manager.m_pWeaponHandler.gauntlet.Initialize(true, manager.m_pBones.WeaponMultiHolder, WepHolder)) Debug.Log("Failed while adding weapon");
		
	}
}
