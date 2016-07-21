using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeNetShoot : MonoBehaviour 
{
	public List<Weapon> WeaponInventory = new List<Weapon> ();

	public Gauntlet gauntlet;

	public int currentWeapon = 0;
	
	public Transform SpawnPoint;
	public Camera GunCamera;
	public bool isSwitching = false;

	public Weapon GetCurrentWeapon  ()
	{
		return WeaponInventory[currentWeapon];
	}

	public void ActionsController (bool Action1, bool Action2, bool Action3, bool ActionGauntlet)
	{
		if(Action1) StartCoroutine(GetCurrentWeapon().LaunchAction1());
		if(Action2) StartCoroutine(GetCurrentWeapon().LaunchAction2());
		if(ActionGauntlet) StartCoroutine(gauntlet.LaunchAction());

		StartCoroutine(GetCurrentWeapon().ProcessAction3(Action3));

	}

	public void RealisticMovements(float MouseX, float MouseY, float HorizontalInput, float VerticalInput, bool JumpInput)
	{
		if(WeaponInventory.Count > 0) GetCurrentWeapon().Update(MouseX, MouseY, HorizontalInput, VerticalInput, JumpInput);
	}

	public void SwitchWeaponController (bool PickRifle, bool PickSword, float ScrollWheel)
	{
		if(WeaponInventory.Count > 0)
		{
			if(GetCurrentWeapon().CanSwitch())
			{
				if(ScrollWheel < 0.0f)
				{
					int nextWeapon = currentWeapon + 1;
					if(nextWeapon > WeaponInventory.Count - 1) nextWeapon = 0;
					transform.root.networkView.RPC("SwitchGuns", RPCMode.All, currentWeapon);
				}
				else if(ScrollWheel > 0.0f)
				{
					int nextWeapon = currentWeapon - 1;
					if(nextWeapon < 0) nextWeapon = WeaponInventory.Count - 1;
					transform.root.networkView.RPC("SwitchGuns",RPCMode.All, currentWeapon);
				}
				else if(PickRifle && currentWeapon != 0)
				{
					transform.root.networkView.RPC("SwitchGuns",RPCMode.All, 0);
				}
				else if(PickSword && currentWeapon != 1)
				{
					transform.root.networkView.RPC("SwitchGuns",RPCMode.All, 1);
				}
			}
		}
	}

	public IEnumerator SwitchGunsCallBack(int GunID)
	{
		isSwitching = true;
		yield return new WaitForSeconds(GetCurrentWeapon().FadeAway());
		GetCurrentWeapon().m_sTransforms.Weapon.gameObject.SetActive(false);
		currentWeapon = GunID;
		GetCurrentWeapon().m_sTransforms.Weapon.gameObject.SetActive(true);
		yield return new WaitForSeconds(GetCurrentWeapon().FadeIn());
		isSwitching = false;

		if(networkView.isMine) AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud.SwappedGun();
	}
}
