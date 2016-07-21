using UnityEngine;
using System.Collections;

public class AeMachineGun : MonoBehaviour 
{
//	public AeWeaponIG WeaponIG;
//	public bool CanShoot = true;
//	public bool isReloading = false;
//	public int LightIndex = 0;
//	Quaternion camPos;
//	Quaternion camDefaultRotation;
//	public float ShakeCam = 5.0f;
//	public Transform MainCamera;
//	public GameObject Trailbullet,bulletShell,ParryFlash;
//	float NextFireTime;
//	float NextMuzzleTime;
//	
//
//	public bool scopeIntoSights = false;
//	public float newField;
//	public float ScopeField = 8.0f;
//	public float curField = 70.0f;
//	public float baseField = 70.0f;
//	public float scopedInPercent = 0.0f;
//
//	
//	void Awake () 
//	{
//				
//		WeaponIG = GetComponent<AeWeaponIG>();
//		Trailbullet = Resources.Load("TrailBullet") as GameObject;
//		bulletShell = Resources.Load("bulletShell") as GameObject;
//		ParryFlash = Resources.Load("BulletHoleSword") as GameObject;
//	}
//
//	void Start ()
//	{
//		if(transform.root.networkView.isMine)
//		{
//			WeaponIG.Weapon.ShootSound.panLevel = 0.0f;
//			WeaponIG.Weapon.ReloadSound.panLevel = 0.0f;
//			if(GameObject.FindWithTag("MainCamera"))
//			{
//				MainCamera = GameObject.FindWithTag("MainCamera").transform;
//			}
//		}
//	}
//
//	void Update () 
//	{
//		if(transform.root.networkView.isMine)
//		{
//			WeaponIG.ShootSound.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeBruitage/4;
//			WeaponIG.ReloadSound.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeBruitage/4;
//			CheckIfCanShoot ();
//			CheckForReload  ();
//			CheckForScope();
//		}
//		else
//		{
//			WeaponIG.ShootSound.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeBruitage;
//			WeaponIG.ReloadSound.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeBruitage;
//		}
//		CheckFireTime   ();
//	}
//	
//
//	void CheckFireTime ()
//	{
//		if(NextFireTime > 0.0f)
//		{
//			NextFireTime -= Time.deltaTime;
//		}
//
//		if(NextMuzzleTime > 0.0f)
//		{
//			WeaponIG.WeaponTransform.Lights[LightIndex].gameObject.SetActive(true);
//			WeaponIG.WeaponTransform.m_tMuzzleFlash.gameObject.SetActive(true);
//			NextMuzzleTime -= Time.deltaTime;	
//		}
//
//		if(NextMuzzleTime <= 0.0f)
//		{
//			WeaponIG.WeaponTransform.m_tMuzzleFlash.gameObject.SetActive(false);
//			WeaponIG.WeaponTransform.Lights[0].gameObject.SetActive(false);
//			WeaponIG.WeaponTransform.Lights[1].gameObject.SetActive(false);
//			WeaponIG.WeaponTransform.Lights[2].gameObject.SetActive(false);
//		}
//	}
//
//	void CheckIfCanShoot()
//	{
//		if(isReloading || NextFireTime > 0.0f || WeaponIG.Weapon.m_iAmmoLeft <= 0 || WeaponIG.SwitchingGuns)
//		{
//			CanShoot = false;
//		}
//		else
//		{
//			CanShoot = true;	
//		}
//	}
//	
//	void CheckForScope ()
//	{
//		if(WeaponIG.Weapon.m_bScopePossible && !AeCore.m_pCoreGame.m_pNetworkHandler.m_pOptionsInGameMenu.showbox)
//		{
//			MainCamera.GetComponent<AeNetShoot>().GunCam.fieldOfView = MainCamera.GetComponent<Camera>().fieldOfView;
//			Vector3 defaultPos = WeaponIG.WeaponTransform.m_tUnScopePosition.localPosition;
//			Vector3 sightPos = WeaponIG.WeaponTransform.m_tScopePosition.localPosition;
//			
//			if (Input.GetButtonDown("Fire2")) 
//			{	
//				scopeIntoSights = true;
//				newField = ScopeField;
//			}	
//			else if (Input.GetButtonUp("Fire2")) 
//			{	
//				scopeIntoSights = false;
//				newField = baseField;
//			}	
//	
//	    	if(scopeIntoSights && !Mathf.Approximately(scopedInPercent,1.0f))
//			{
//				scopedInPercent += Time.deltaTime * 4.0f;
//			}
//			else if(!scopeIntoSights&& !Mathf.Approximately(scopedInPercent,0.0f)) 
//			{
//				scopedInPercent -= Time.deltaTime * 4.0f;
//			}
//	    	scopedInPercent = Mathf.Clamp(scopedInPercent,0.0f,1.0f);
//			
//	    	WeaponIG.WeaponTransform.m_tWeaponTransform.localPosition = Vector3.Lerp(defaultPos, sightPos, scopedInPercent);
//			
//			if(scopedInPercent >= 0.92f)
//			{
//				MainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(curField,newField,scopedInPercent);
//				transform.root.GetComponent<AeMouseLookX>().NotScoping = false;
//				MainCamera.GetComponent<AeMouseLookY>().NotScoping = false;
//				Component[] MeshRend;
//				Component [] MeshRend2;
//				MeshRend2 = WeaponIG.WeaponTransform.m_tWeaponTransform.GetComponentsInChildren<MeshRenderer>();
//				MeshRend = WeaponIG.WeaponTransform.m_tWeaponTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
//				foreach(SkinnedMeshRenderer mesh in MeshRend)
//				{ 
//					mesh.enabled = false;
//				}
//				foreach(MeshRenderer mesh in MeshRend2)
//				{
//					mesh.enabled = false;
//				}
//					transform.root.GetComponent<AeCrossHair>().ShowSniperScope = true;
//			}
//			else
//			{
//				transform.root.GetComponent<AeMouseLookX>().NotScoping = true;
//				MainCamera.GetComponent<AeMouseLookY>().NotScoping = true;
//				Component[] MeshRend;
//				Component [] MeshRend2;
//				MeshRend2 = WeaponIG.WeaponTransform.m_tWeaponTransform.GetComponentsInChildren<MeshRenderer>();
//				MeshRend =WeaponIG.WeaponTransform.m_tWeaponTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
//				foreach(SkinnedMeshRenderer mesh in MeshRend)
//				{ 
//					mesh.enabled = true;
//				}
//				foreach(MeshRenderer mesh in MeshRend2)
//				{
//					mesh.enabled = true;
//				}
//				transform.root.GetComponent<AeCrossHair>().ShowSniperScope = false;
//			}
//		}
//	}
//
//	void CheckForReload ()
//	{
//	}
//
//	void ActiveMuzzleFlash ()
//	{
//		if(LightIndex >= 2)
//		{
//			LightIndex = 0;
//		}
//		else
//		{
//			LightIndex++;
//		}
//		WeaponIG.WeaponTransform.m_tMuzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.forward);
//		NextMuzzleTime = 0.05f;
//	}
//
//	IEnumerator ApplyReload ()
//	{
//		isReloading = true;
//		WeaponIG.isReloading = true;
//		WeaponIG.NetShootHolder.AnimSync.PlayReload();
//		WeaponIG.ReloadSound.Play();
//		yield return new WaitForSeconds(1.5f);
//		WeaponIG.Weapon.m_iAmmoLeft = WeaponIG.Weapon.m_iAmmoMaxInAClip;
//		AeCore.m_pCoreGame.m_pNetworkHandler.InstantiatedPrefabHUD.ReloadedBullets();
//		WeaponIG.Weapon.m_iClipLeft--;
//		WeaponIG.isReloading = false;
//		isReloading = false;
//	}
//	
//
//	
//	public void Reload()
//	{
//		if(WeaponIG.Weapon.m_iAmmoLeft < WeaponIG.Weapon.m_iAmmoMaxInAClip && WeaponIG.Weapon.m_iClipLeft > 0 && !isReloading && !WeaponIG.SwitchingGuns)
//		{
//			StartCoroutine(ApplyReload());
//		}
//	}
//
//
//	public void Shoot ()
//	{
//		if(CanShoot)
//		{
//			WeaponIG.NetShootHolder.AnimSync.PlayShoot();
//			ApplyHit();
//
//			transform.root.networkView.RPC("ShootOverNetwork",RPCMode.All);
//			transform.root.GetComponent<AeCrossHair>().AddKnockBack();
//			WeaponIG.Weapon.m_iAmmoLeft--;
//			AeCore.m_pCoreGame.m_pNetworkHandler.InstantiatedPrefabHUD.DeleteLastBullet();
//			NextFireTime = WeaponIG.Weapon.m_fFireRate;
//			GetComponent<AeCosmeticRealism>().ApplyKickBack();
//		}
//	}
//
//	public void ApplyShootMisc()
//	{
//		ActiveMuzzleFlash();
//		GameObject TrailBull = Instantiate(Trailbullet,WeaponIG.WeaponTransform.m_tMuzzleFlash.position,WeaponIG.WeaponTransform.m_tWeaponSpawnPoint.rotation) as GameObject;
//
//
//		Vector3 forward = WeaponIG.WeaponTransform.m_tWeaponSpawnPoint.TransformDirection(Vector3.forward);
//		RaycastHit hit;
//		var layerMask = ~(1 << 8);
//		if(Physics.Raycast(WeaponIG.WeaponTransform.m_tWeaponSpawnPoint.position,forward,out hit,WeaponIG.Weapon.m_fRange,layerMask))
//		{
//			TrailBull.transform.LookAt(hit.point);
//		}
//		else
//		{
//			Vector3 ForwardPlusGunRange = WeaponIG.WeaponTransform.m_tWeaponSpawnPoint.TransformDirection(Vector3.forward);
//			TrailBull.transform.LookAt(WeaponIG.WeaponTransform.m_tWeaponSpawnPoint.position + ForwardPlusGunRange * WeaponIG.Weapon.m_fRange);
//		}
//		WeaponIG.ShootSound.Play();
//		if(!transform.root.networkView.isMine)
//		{
//			ApplyHitOverNetwork();
//		}
//	}
//
//	public void ApplyReloadMisc()
//	{
//	}
//
//	void HitParry(NetworkView ParryNView)
//	{
//		ParryNView.RPC("HitParry",RPCMode.All);
//	}
//
//	
//	void EmitParticles(RaycastHit hit, string WhichParticles)
//	{
//		EmitParticlesOverNetwork(hit.point,hit.normal,WhichParticles);
//	}
//	
//	public void EmitParticlesOverNetwork(Vector3 hitpoint,Vector3 hitnormal,string TypeParticles)
//	{
//		if(TypeParticles == "Blood")
//		{
//			if(WeaponIG.WeaponTransform.m_pBloodParticleEmit)
//			{
//				GameObject BulletHole = Instantiate(WeaponIG.BloodHole,hitpoint + (hitnormal * 0.001f),Quaternion.LookRotation(hitnormal)) as GameObject;
//				BulletHole.transform.GetChild(0).particleEmitter.Emit();
//			}
//		}
//		
//		else if(TypeParticles == "Wood")
//		{
//			if(WeaponIG.WeaponTransform.m_pWoodParticleEmit)
//			{
//				GameObject BulletHole = Instantiate(WeaponIG.bulletHole,hitpoint + (hitnormal * 0.001f),Quaternion.LookRotation(hitnormal)) as GameObject;
//				BulletHole.transform.GetChild(1).particleEmitter.Emit();
//			}
//		}
//		
//		else if(TypeParticles == "Metal")
//		{
//			if(WeaponIG.WeaponTransform.m_pMetalParticleEmit)
//			{
//	
//				//WeaponIG.WeaponTransform.m_pMetalParticleEmit.transform.position = hit.point;
//				GameObject BulletHole = Instantiate(WeaponIG.bulletHole,hitpoint + (hitnormal * 0.001f),Quaternion.LookRotation(hitnormal)) as GameObject;
//				BulletHole.transform.GetChild(1).particleEmitter.Emit();
//			}
//		}
//		else if(TypeParticles == "WeaponsParry")
//		{
//			if(WeaponIG.WeaponTransform.m_pMetalParticleEmit)
//			{
//				
//				//WeaponIG.WeaponTransform.m_pMetalParticleEmit.transform.position = hit.point;
//				GameObject BulletHole = Instantiate(ParryFlash,hitpoint + (hitnormal * 0.001f),Quaternion.LookRotation(hitnormal)) as GameObject;
//				BulletHole.transform.GetChild(0).particleEmitter.Emit();
//			}
//		}
//	}
}
