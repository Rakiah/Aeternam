using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[SerializeField]
public class MeleeGun : Weapon
{
	//some runtime changing variables
	Transform ParryObject;
	Vector3 BasePosition;
	bool hasAttack;
	bool isParrying;
	bool isBigAttack;

	public MeleeGun(string [] StrToParse)
	{
		
		m_sWeaponName = StrToParse[0];
		int.TryParse(StrToParse[1],out WeaponID);
		if(WeaponID == 1 || WeaponID == 4 || WeaponID == 6) Default = true;
		else Default = false;
		
		//int
		int.TryParse(StrToParse[5],out m_iPrice);

		//inherited members
		m_sCrossHair.Initalize(float.Parse(StrToParse[8]), float.Parse(StrToParse[9]),float.Parse(StrToParse[10]),float.Parse(StrToParse[11]));
		m_sStats.Initialize(float.Parse(StrToParse[6]), float.Parse(StrToParse[13]),float.Parse(StrToParse[14]),float.Parse(StrToParse[12]));
		m_tIconTexture = Resources.Load(m_sWeaponName+"Texture2D") as Texture2D;	
	}
	
	public MeleeGun(MeleeGun copy)
	{
		this.m_sWeaponName = copy.m_sWeaponName;
		this.WeaponID = copy.WeaponID;
		this.Default = copy.Default;
		
		this.m_iPrice = copy.m_iPrice;
		
		//inherited members
		m_sCrossHair.Initalize(copy.m_sCrossHair.CrossHairBasicOffSet, copy.m_sCrossHair.CrossHairOffSetModifier,
		                       copy.m_sCrossHair.CrossHairStability, copy.m_sCrossHair.CrossHairMaxOffSet);
		
		m_sStats.Initialize(copy.m_sStats.FireRate, copy.m_sStats.WeaponDamage, copy.m_sStats.Force, copy.m_sStats.Range);
		
		this.m_tIconTexture = copy.m_tIconTexture;
	}
	
	public override bool CanAction1 ()
	{
		if(Input.GetKeyDown(AeProfils.m_pAeProfils.CurrentProfil.control.Shoot) 
		   && !hasAttack && !isParrying && !m_sTransforms.componentsManager.m_pWeaponHandler.isSwitching) return true;
		
		return false;
	}
	
	public override IEnumerator LaunchAction1 ()
	{
		m_sTransforms.Weapon.root.networkView.RPC("Action1", RPCMode.All);

		yield return new WaitForSeconds(0.2f);
		HitDamageInfo Attack = AeRaycasts.ShootingRaycast(m_sTransforms.WeaponSpawnPoint, Vector3.forward, m_sStats.Range, m_sStats.Force, (int)m_sStats.WeaponDamage, WeaponID);
		
		
		if(Attack.isEnnemy) AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().SendAttack(Attack);
		yield return null;
	}
	
	public override IEnumerator DoAction1CallBack ()
	{
		hasAttack = true;

		m_sTransforms.componentsManager.m_pAnimator.PlayAction1();
		m_sTransforms.Sounds.SoundAction1.Play();

		yield return new WaitForSeconds(0.2f);

		HitDamageInfo HitInfo = AeRaycasts.ShootingRaycast(m_sTransforms.WeaponSpawnPoint, Vector3.forward, m_sStats.Range, m_sStats.Force, (int) m_sStats.WeaponDamage, WeaponID);

		if(HitInfo.isEnnemy || HitInfo.damageLocation == "PlayerRag" || HitInfo.damageLocation == "Player")
		{
			GameObject Impact = MonoBehaviour.Instantiate(m_sTransforms.Particles.Blood, HitInfo.hit.point + (HitInfo.hit.normal * 0.01f),Quaternion.LookRotation(HitInfo.hit.normal)) as GameObject;
			MonoBehaviour.Destroy(Impact,2.0f);
		}
		else if(HitInfo.damageLocation == "Untagged")
		{
			GameObject Impact = MonoBehaviour.Instantiate(m_sTransforms.Particles.Concrete, HitInfo.hit.point + (HitInfo.hit.normal * 0.01f),Quaternion.LookRotation(HitInfo.hit.normal)) as GameObject;
			Impact.transform.GetChild(1).gameObject.SetActive(false);
			MonoBehaviour.Destroy(Impact,2.0f);
		}
		else if(HitInfo.damageLocation == "Wood")
		{
			GameObject Impact = MonoBehaviour.Instantiate(m_sTransforms.Particles.Wood, HitInfo.hit.point + (HitInfo.hit.normal * 0.01f),Quaternion.LookRotation(HitInfo.hit.normal)) as GameObject;
			Impact.transform.GetChild(1).gameObject.SetActive(false);
			MonoBehaviour.Destroy(Impact,2.0f);
		}
		
		else if(HitInfo.damageLocation == "Metal")
		{
			GameObject Impact = MonoBehaviour.Instantiate(m_sTransforms.Particles.Metal, HitInfo.hit.point + (HitInfo.hit.normal * 0.01f),Quaternion.LookRotation(HitInfo.hit.normal)) as GameObject;
			Impact.transform.GetChild(1).gameObject.SetActive(false);
			MonoBehaviour.Destroy(Impact,2.0f);
		}
		
		else if(HitInfo.damageLocation == "WeaponsParry")
		{
			GameObject Impact = MonoBehaviour.Instantiate(m_sTransforms.Particles.Metal, HitInfo.hit.point + (HitInfo.hit.normal * 0.01f),Quaternion.LookRotation(HitInfo.hit.normal)) as GameObject;
			Impact.transform.GetChild(1).gameObject.SetActive(false);
			MonoBehaviour.Destroy(Impact,2.0f);
		}

		
		yield return new WaitForSeconds(m_sStats.FireRate);
		
		hasAttack = false;
		
	}
	
	public override bool CanAction2 ()
	{
//		if(Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Reload) && !isBigAttack) return true;
//		
		return false;
	}
	
	public override IEnumerator LaunchAction2 ()
	{
//		m_sTransforms.Weapon.root.networkView.RPC("Action2",RPCMode.All);
		isBigAttack = true;
		yield return null;
	}
	
	public override IEnumerator DoAction2CallBack ()
	{
//		isReloading = true;
//		m_sTransforms.AnimationHandler.PlayReload();
//		m_sTransforms.Sounds.SoundAction2.Play();
		yield return new WaitForSeconds(0.1f);
//		ammoLeft = m_iAmmoMaxInAClip;
//		clipsLeft--;
//		isReloading = false;
	}
	
	public override bool StatusAction3 ()
	{
		if(Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Parry) && !hasAttack)	return true;
		
		return false;
	}
	
	public override IEnumerator ProcessAction3 (bool status)
	{
		if(status != isParrying) m_sTransforms.componentsManager.m_pNetworkCaller.networkView.RPC("Action3", RPCMode.All);
		
		yield return null;
	}

	public override IEnumerator DoAction3CallBack ()
	{
		m_sTransforms.componentsManager.m_pAnimator.PlayAction3();
		isParrying = !isParrying;

		yield return new WaitForSeconds(0.3f);
		if(ParryObject != null) ParryObject.gameObject.SetActive(isParrying);
	}

	
	public override void Update (float MouseX, float MouseY, float HorizontalInput, float VerticalInput, bool JumpInput)
	{
		MouseX /= AeProfils.m_pAeProfils.CurrentProfil.control.Sensitivity;
		MouseY /= AeProfils.m_pAeProfils.CurrentProfil.control.Sensitivity;
		
		float InputX = -(MouseX - HorizontalInput) * 0.07f;
		float InputY = MouseY * 0.07f;
		float InputZ = -VerticalInput * 0.07f;
		
		float value = 0.15f;
		
		if (InputX > value)
			InputX = value;
		
		if (InputX < -value)
			InputX = -value;
		
		if (InputY > value)
			InputY = value;
		
		if (InputY < -value)
			InputY = -value;
		
		if (InputZ > value)
			InputZ = value;
		
		if (InputZ < -value)
			InputZ = -value;
		
		
		Vector3 Recalculate = new Vector3(BasePosition.x+InputX, BasePosition.y+InputY, BasePosition.z+InputZ);
		m_sTransforms.Weapon.localPosition = Vector3.Lerp(m_sTransforms.Weapon.localPosition, Recalculate, Time.deltaTime * 2.0f);
	}
	
	public override bool CanSwitch ()
	{
		if(!isBigAttack) return true;
		else return false;
	}
	
	public override float FadeAway ()
	{
		m_sTransforms.Weapon.gameObject.SetActive(true);
		m_sTransforms.componentsManager.m_pAnimator.CurrentWeaponAnimator = m_sTransforms.weaponAnimator;
		m_sTransforms.componentsManager.m_pAnimator.PlayHide();
		return 0.4f;
	}
	
	public override float FadeIn ()
	{
		m_sTransforms.componentsManager.m_pAnimator.CurrentWeaponAnimator = m_sTransforms.weaponAnimator;
		m_sTransforms.componentsManager.m_pAnimator.PlayShow();
		return 1.0f;
	}
	
	public override bool Initialize (bool local, GameObject multiHolder, GameObject localHolder)
	{
		try
		{
			string path = string.Empty;
			GameObject holder = null;
			
			
			if(local) { path = "_prefab"; holder = localHolder; } else { path = "_prefabMulti"; holder = multiHolder; }
			
			GameObject Weapon = MonoBehaviour.Instantiate(Resources.Load(m_sWeaponName + path) as GameObject, holder.transform.position, holder.transform.rotation) as GameObject;
			
			Vector3 localScale = Weapon.transform.localScale;
			Weapon.transform.parent = holder.transform;
			Weapon.transform.localScale = localScale;
			
			if(!m_sTransforms.Initialize(Weapon.transform)) return false;
			if(!m_sTransforms.Sounds.Initialize(Weapon.transform)) return false;
			if(!m_sTransforms.Particles.Initialize(Weapon.transform)) return false;

			ParryObject = Weapon.transform.FindChild("ParryObject");
			m_sTransforms.WeaponSpawnPoint = localHolder.transform.parent.parent.FindChild("SpawnPoint");
			

			BasePosition = m_sTransforms.Weapon.transform.localPosition;

			m_sTransforms.Weapon.gameObject.SetActive(false);

			return true;
		}
		catch
		{
			Debug.Log("error while adding weapon");
			return false;
		}
	}
}