using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public class SniperGun : Weapon
{
	public int m_iAmmoMaxInAClip;
	public int m_iMaxClip;
	public float m_fReloadTime;
	Transform MuzzleFlash;
	GameObject Trailbullet;

	List<RecoilPart> recoils = new List<RecoilPart>();

	
	//some runtime changing variables
	int ammoLeft;
	int clipsLeft;
	bool hasShot;
	int LightIndex;
	bool isReloading;
	int BulletsInARow;
	float TimerRecoil;
	Quaternion targetRotation;
	Transform layerCameraShoot;
	
	Vector3 BasePosition;
	Vector3 ScopePosition;

	Vector3 targetPosition;

	bool scopeTriggered;

	float scopePercent;

	float baseFoV;
	float scopeFoV;
	float targetFoV;

	public Texture2D m_tSniperTexture;

	List<Component> meshRend = new List<Component>();

	
	public SniperGun (string [] StrToParse)
	{
		try
		{
			m_sWeaponName = StrToParse[0];	
			int.TryParse(StrToParse[1],out WeaponID);
			if(WeaponID == 1 || WeaponID == 4 || WeaponID == 6) Default = true;
			else Default = false;
			
			//int
			int.TryParse(StrToParse[3],out m_iAmmoMaxInAClip);
			int.TryParse(StrToParse[4],out m_iMaxClip);
			int.TryParse(StrToParse[5],out m_iPrice);
			
			
			//float
			m_sStats.FireRate = float.Parse(StrToParse[6]);
			m_fReloadTime = float.Parse(StrToParse[7]);

			string [] recoilsStrings = StrToParse[15].Split('#');
			
			foreach(string rec in recoilsStrings)
			{
				recoils.Add(new RecoilPart(rec.Split('|')));
			}
			
			m_sCrossHair.Initalize(float.Parse(StrToParse[8]), float.Parse(StrToParse[9]),float.Parse(StrToParse[10]),float.Parse(StrToParse[11]));
			m_sStats.Initialize(float.Parse(StrToParse[6]), float.Parse(StrToParse[13]),float.Parse(StrToParse[14]),float.Parse(StrToParse[12]));
			
			m_tIconTexture = Resources.Load(m_sWeaponName+"Texture2D") as Texture2D;	
			m_tSniperTexture = Resources.Load("SniperTexture") as Texture2D;
		}
		catch
		{
			Debug.Log("failled filling up weapon");
		}
	}
	
	public SniperGun(SniperGun copy) 
	{
		this.m_sWeaponName = copy.m_sWeaponName;
		this.WeaponID = copy.WeaponID;
		this.Default = copy.Default;
		
		this.m_iAmmoMaxInAClip = copy.m_iAmmoMaxInAClip;
		this.m_iMaxClip = copy.m_iMaxClip;
		this.m_iPrice = copy.m_iPrice;
		this.m_fReloadTime = copy.m_fReloadTime;
		
		
		//inherited members
		m_sCrossHair.Initalize(copy.m_sCrossHair.CrossHairBasicOffSet, copy.m_sCrossHair.CrossHairOffSetModifier,
		                       copy.m_sCrossHair.CrossHairStability, copy.m_sCrossHair.CrossHairMaxOffSet);
		
		m_sStats.Initialize(copy.m_sStats.FireRate, copy.m_sStats.WeaponDamage, copy.m_sStats.Force, copy.m_sStats.Range);
		
		this.m_tIconTexture = copy.m_tIconTexture;
		
		ammoLeft = m_iAmmoMaxInAClip;
		clipsLeft = m_iMaxClip;
	}
	
	public override bool CanAction1 ()
	{
		if(Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Shoot) 
		   && !hasShot && ammoLeft > 0 && !isReloading && m_sTransforms.componentsManager.m_pStatsSynchronizer.iStanceID != 2
		   && !m_sTransforms.componentsManager.m_pWeaponHandler.isSwitching) return true;
		
		return false;
	}
	
	public override IEnumerator LaunchAction1 ()
	{
		m_sTransforms.Weapon.root.networkView.RPC("Action1", RPCMode.All);
		HitDamageInfo Attack = AeRaycasts.ShootingRaycast(m_sTransforms.WeaponSpawnPoint, Vector3.forward, m_sStats.Range, m_sStats.Force, (int)m_sStats.WeaponDamage, WeaponID);
		AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud.AddKnockBack();
		m_sTransforms.Weapon.Translate(new Vector3(0,0.001f,0.080f));

		if(Attack.isEnnemy) AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().SendAttack(Attack);

		yield return new WaitForSeconds(0.15f);

		m_sTransforms.weaponAnimator.SetTrigger("ReloadChamber");
	}
	
	public override IEnumerator DoAction1CallBack ()
	{
		ammoLeft--;
		HitDamageInfo HitInfo = AeRaycasts.ShootingRaycast(m_sTransforms.WeaponSpawnPoint, Vector3.forward, m_sStats.Range, m_sStats.Force, (int) m_sStats.WeaponDamage, WeaponID);
		DoRecoil();
		GameObject TrailBull = MonoBehaviour.Instantiate(Trailbullet,MuzzleFlash.position,MuzzleFlash.rotation) as GameObject;
		m_sTransforms.Sounds.SoundAction1.Play();

		
		MonoBehaviour.Destroy(TrailBull,3.0f);
		
		if(HitInfo.physicsFailed)
		{
			Vector3 forward = m_sTransforms.WeaponSpawnPoint.TransformDirection(Vector3.forward);
			TrailBull.transform.LookAt((m_sTransforms.WeaponSpawnPoint.position + (forward * m_sStats.Range)));
		}
		else TrailBull.transform.LookAt(HitInfo.hit.point);
		
		if(HitInfo.isEnnemy || HitInfo.damageLocation == "PlayerRag" || HitInfo.damageLocation == "Player")
		{
			if(HitInfo.isEnnemy)
			{
				if(AeTools.damageLocationToInt(HitInfo.damageLocation) != -1) AeTools.CreateParticle(m_sTransforms.Particles.Blood,HitInfo,true);
				else AeTools.CreateParticle(m_sTransforms.Particles.Metal,HitInfo,false);
			}
			else AeTools.CreateParticle(m_sTransforms.Particles.Blood,HitInfo,true);
		}

		else if(HitInfo.damageLocation == "Untagged") AeTools.CreateParticle(m_sTransforms.Particles.Concrete,HitInfo,true);
		else if(HitInfo.damageLocation == "Wood") AeTools.CreateParticle(m_sTransforms.Particles.Wood,HitInfo,true);
		else if(HitInfo.damageLocation == "Metal") AeTools.CreateParticle(m_sTransforms.Particles.Metal,HitInfo,true);

		MuzzleFlash.localRotation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.forward);
		LightIndex++;
		if(LightIndex > m_sTransforms.Lights.Count - 1) LightIndex = 0;
		
		hasShot = true;
		m_sTransforms.Lights[LightIndex].gameObject.SetActive(true);
		MuzzleFlash.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.05f);
		m_sTransforms.Lights[LightIndex].gameObject.SetActive(false);
		MuzzleFlash.gameObject.SetActive(false);
		yield return new WaitForSeconds(m_sStats.FireRate - 0.05f);
		hasShot = false;
	}

	void DoRecoil ()
	{
		TimerRecoil = m_sStats.FireRate + 0.07f;
		Vector3 spawnpoint = m_sTransforms.WeaponSpawnPoint.localEulerAngles;
		Vector3 recoil = new Vector3(spawnpoint.x,spawnpoint.y,spawnpoint.z);
		BulletsInARow++;
		
		
		foreach(RecoilPart reco in recoils)
		{
			if(BulletsInARow >= reco.startingPart && BulletsInARow < reco.endingPart)
			{
				float recoilMultiplier = m_sTransforms.componentsManager.m_pMovements.currentStance.recoilMultiplier;
				recoil = new Vector3(spawnpoint.x + ((reco.x * recoilMultiplier) * 0.8f), spawnpoint.y + ((reco.y * recoilMultiplier) * 0.8f), spawnpoint.z);
				targetRotation *= Quaternion.Euler(reco.x * recoilMultiplier * 0.5f, reco.y * recoilMultiplier * 0.5f, 0);
			}
		}
		m_sTransforms.WeaponSpawnPoint.localEulerAngles = recoil;
	}
	
	public override bool CanAction2 ()
	{
		if(Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Reload)
		   && ammoLeft < m_iAmmoMaxInAClip && clipsLeft > 0 && !isReloading) return true;

		return false;

	}
	
	public override IEnumerator LaunchAction2 ()
	{
		m_sTransforms.Weapon.root.networkView.RPC("Action2",RPCMode.All);
		yield return null;
	}
	
	public override IEnumerator DoAction2CallBack ()
	{
		isReloading = true;
		m_sTransforms.componentsManager.m_pAnimator.PlayReload();
		m_sTransforms.Sounds.SoundAction2.Play();
		yield return new WaitForSeconds(m_fReloadTime);
		ammoLeft = m_iAmmoMaxInAClip;
		clipsLeft--;
		isReloading = false;
	}
	
	public override bool StatusAction3 ()
	{
		if(Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control. Parry)
		   && m_sTransforms.componentsManager.m_pStatsSynchronizer.iStanceID != 2
		   && !m_sTransforms.componentsManager.m_pWeaponHandler.isSwitching) return true;
		
		return false;
	}

	public override IEnumerator ProcessAction3 (bool status)
	{
		if(status)
		{
			targetFoV = scopeFoV;
			targetPosition = ScopePosition;
			scopePercent += Time.deltaTime * 4.0f;
		}
		else
		{
			targetFoV = baseFoV;
			targetPosition = BasePosition;
			scopePercent -= Time.deltaTime * 4.0f;
		}

		scopePercent = Mathf.Clamp(scopePercent,0.0f,1.0f);

		bool changeState = scopeTriggered;

		if(scopePercent > 0.92f) changeState = true;
		else changeState = false;

		if(changeState != scopeTriggered)
		{
			foreach(Component c in meshRend) c.renderer.enabled = !scopeTriggered;

			m_sTransforms.componentsManager.m_pNetworkCaller.networkView.RPC("Action3", RPCMode.All);
		}

		yield return null;
	}
	
	public override IEnumerator DoAction3CallBack ()
	{
		scopeTriggered = !scopeTriggered;
		/*here we set the animation to what we need in fact, there is no animation to set locally, 
		only on multiplayer, and since the multiplayer part isnt ready, we dont do anything right there for now*/
		yield return null;
	}

	public override void Update (float MouseX, float MouseY, float HorizontalInput, float VerticalInput, bool JumpInput)
	{
		if(BulletsInARow > 0) TimerRecoil -= Time.deltaTime;
		if(TimerRecoil <= 0.0f)
		{
			BulletsInARow = 0;
			m_sTransforms.WeaponSpawnPoint.localEulerAngles = new Vector3(0,0,0);
			targetRotation = Quaternion.Euler(0,0,targetRotation.eulerAngles.z);
		}
		
		targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, (-HorizontalInput) * 2);
		
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

		Vector3	Recalculate = new Vector3(targetPosition.x + InputX, targetPosition.y + InputY, targetPosition.z + InputZ);
		m_sTransforms.Weapon.localPosition = Vector3.Lerp(m_sTransforms.Weapon.localPosition, Recalculate, Time.deltaTime * 2.0f);

		layerCameraShoot.localRotation = Quaternion.Slerp(layerCameraShoot.localRotation, targetRotation, 4.0F * Time.deltaTime);
	}
	
	public override bool CanSwitch ()
	{
		if(!isReloading) return true;
		else return false;
	}
	
	public override float FadeAway ()
	{
		scopePercent = 0.0f;
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
			if(!m_sTransforms.Particles.Initialize(Weapon.transform)) return false;
			if(!m_sTransforms.Sounds.Initialize(Weapon.transform)) return false;
			
			MuzzleFlash = Weapon.transform.Find("MuzzleFlash");
			MuzzleFlash.gameObject.SetActive(false);
			
			m_sTransforms.WeaponSpawnPoint = localHolder.transform.parent.parent.FindChild("SpawnPoint");
			
			
			BasePosition = m_sTransforms.Weapon.transform.localPosition;
			ScopePosition = m_sTransforms.Weapon.FindChild("Scope").transform.position;

			meshRend.AddRange(m_sTransforms.Weapon.GetComponentsInChildren<SkinnedMeshRenderer>());
			meshRend.AddRange(m_sTransforms.Weapon.GetComponentsInChildren<MeshRenderer>());

			Trailbullet = Resources.Load("TrailBullet") as GameObject;
			layerCameraShoot = m_sTransforms.componentsManager.m_pMovements.GetGrandChild(9);
			m_sTransforms.Weapon.gameObject.SetActive(false);
			
			return true;
		}
		catch
		{
			Debug.Log("failed while creating and assigning weapon");
			return false;
		}
	}
}
