using UnityEngine;
using System.Collections;

public class AeTools
{
	public static float ClampAngle (float angle, float min, float max)
	{
		angle = angle % 360;
		
		if((angle >= -360F) && (angle <= 360F))
		{
			
			if(angle < -360F) angle += 360F;
			if(angle > 360F) angle -= 360F;    
		}
		return Mathf.Clamp (angle, min, max);
	}

	public static bool isEnnemy (string t)
	{
		if(t == "PlayerHead" | t == "PlayerMid" | t == "PlayerMidTop" | t == "PlayerLeg" | t == "PlayerArm" | t == "Parryweapons") return true;

		return false;
	}

	public static bool canDamage(Gauntlet g, AeStats h, bool isMine)
	{
		if(!h.AlreadyTouchedByMagic) 
		{
			if(!isMine)
			{
				return true;
			}
			else if(g.SelfDamage)
			{
				return true;
			}
		}
		else if(g.MultipleTimeDamage)
		{
			if(!isMine)
			{
				return true;
			}
			else if(g.SelfDamage)
			{
				return true;
			}
		}

		return false;
	}
	
	public static int damageLocationToInt(string t)
	{
		switch(t)
		{
			case "PlayerHead"   : return  1;
			case "PlayerMid"    : return  2;
			case "PlayerMidTop" : return  3;
			case "PlayerLeg"    : return  4;
			case "PlayerArm"    : return  5;
			case "Parryweapons" : return  6;
			default             : return -1;
		}
	}
	
	public static Weapon GetWeaponById (int WeaponID)
	{
		foreach(MachineGun tempWeap in AeWeapons.m_pAeWeapons.AllMachinesGuns)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return tempWeap;
			}
		}
		
		foreach(ShotGun tempWeap in AeWeapons.m_pAeWeapons.AllShotsGuns)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return tempWeap;
			}
		}
		
		foreach(SniperGun tempWeap in AeWeapons.m_pAeWeapons.AllSnipers)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return tempWeap;
			}
		}
		
		foreach(MeleeGun tempWeap in AeWeapons.m_pAeWeapons.AllMeleeWeapons)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return tempWeap;
			}
		}


		return null;
	}

	public static Weapon CopyWeaponById (int WeaponID)
	{
		foreach(MachineGun tempWeap in AeWeapons.m_pAeWeapons.AllMachinesGuns)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return new MachineGun(tempWeap);
			}
		}

		foreach(ShotGun tempWeap in AeWeapons.m_pAeWeapons.AllShotsGuns)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return new ShotGun(tempWeap);
			}
		}
	
		foreach(SniperGun tempWeap in AeWeapons.m_pAeWeapons.AllSnipers)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return new SniperGun(tempWeap);
			}
		}

		foreach(MeleeGun tempWeap in AeWeapons.m_pAeWeapons.AllMeleeWeapons)
		{
			if(tempWeap.WeaponID == WeaponID)
			{
				return new MeleeGun(tempWeap);
			}
		}

		return null;
	}

	public static GameObject CreateParticle (GameObject particle, HitDamageInfo info, bool hole)
	{
		GameObject BulletHole = MonoBehaviour.Instantiate(particle, info.hit.point + (info.hit.normal * 0.01f),Quaternion.LookRotation(info.hit.normal)) as GameObject;

		if(!hole) BulletHole.transform.GetChild(1).gameObject.SetActive(false);
		MonoBehaviour.Destroy(BulletHole,10.0f);

		return BulletHole;
	}

	public static GameObject CreateParticle (GameObject particle, HitDamageInfo info, bool hole, float offset)
	{
		GameObject BulletHole = MonoBehaviour.Instantiate(particle, info.hit.point + (info.hit.normal * offset),Quaternion.LookRotation(info.hit.normal)) as GameObject;
		
		if(!hole) BulletHole.transform.GetChild(1).gameObject.SetActive(false);
		MonoBehaviour.Destroy(BulletHole,10.0f);

		return BulletHole;
	}

	public static void SimulateAConnection()
	{
		int NewIDPlayerStats;
		PlayerStats newplayerstats = new PlayerStats();
		newplayerstats.m_pNetworkPlayer = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[0].m_pNetworkPlayer;
		AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Add(newplayerstats);
		NewIDPlayerStats = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.LastIndexOf(newplayerstats);
		AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[NewIDPlayerStats].m_iPlayerID = NewIDPlayerStats;
		GameObject.Find("HostGameRemade").GetComponent<AeLobby>().AddAPlayer(NewIDPlayerStats);
		//AeCore.m_pCoreGame.m_pNetworkHandler.ShareStats();
	}
	public static void SimulateADisconnection (int ToDisconnect)
	{
//		AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.RemoveAt(ToDisconnect);
//		AeCore.m_pCoreGame.m_pNetworkHandler.RemoveAt(ToDisconnect);
		for (int i = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count - 1; i >= 0; i--)
		{
			AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iPlayerID = i;
		}
		GameObject.Find("HostGameRemade").GetComponent<AeLobby>().DeleteAPlayer(ToDisconnect);
	}
}
