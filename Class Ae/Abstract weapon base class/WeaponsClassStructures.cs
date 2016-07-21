using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CrossHair
{
	public float CrossHairBasicOffSet;
	public float CrossHairOffSetModifier;
	public float CrossHairStability;
	public float CrossHairMaxOffSet;

	public void Initalize (float basic, float offset, float stability, float maxoffset)
	{
		CrossHairBasicOffSet = basic;
		CrossHairOffSetModifier = offset;
		CrossHairStability = stability;
		CrossHairMaxOffSet = maxoffset;
	}
}

public struct WeaponSounds
{
	public AudioSource SoundAction1;
	public AudioSource SoundAction2;
	
	public AudioSource ShowIn;
	public AudioSource ShowOut;
	
	
	public bool Initialize (Transform weap)
	{
		try
		{
			SoundAction1 = weap.Find("Sounds/Action1").audio;
			SoundAction2 = weap.Find("Sounds/Action2").audio;
			ShowIn = weap.Find("Sounds/ShowIn").audio;
			ShowOut = weap.Find("Sounds/ShowOut").audio;
			return true;
		}
		catch
		{
			Debug.Log("failed while adding sounds");
			return false;
		}
	}
}

public struct WeaponStats
{
	public float FireRate;
	public float WeaponDamage;
	public float Force;
	public float Range;

	public void Initialize (float rate, float damage, float force, float range)
	{
		FireRate = rate;
		WeaponDamage = damage;
		Force = force;
		Range = range;
	}
}

public struct WeaponParticles
{
	public GameObject Concrete;
	public GameObject Metal;
	public GameObject Blood;
	public GameObject Wood;
	
	public bool Initialize (Transform weap)
	{
		try
		{
			Blood = Resources.Load("Particles/BloodHole") as GameObject;
			Concrete = Resources.Load("Particles/ConcreteHole") as GameObject;
			Wood = Resources.Load("Particles/WoodHole") as GameObject;
			Metal = Resources.Load("Particles/MetalHole") as GameObject;
			return true;
		}
		catch
		{
			Debug.Log("failed while adding particles");
			return false;
		}
	}
}

public struct WeaponTransforms
{
	public ComponentsManager componentsManager;
	public Animator weaponAnimator;
	public Transform Weapon;
	
	public Transform WeaponSpawnPoint;
	
	public WeaponParticles Particles;
	public WeaponSounds Sounds;
	
	
	public List<Transform> Lights;
	
	public bool Initialize (Transform weap)
	{
		try
		{
			Weapon = weap;
			
			Lights = new List<Transform>();
			
			Transform LightHolder = weap.Find("Lights");
			for(int i = 0; i < LightHolder.childCount; i++)
			{
				Lights.Add(LightHolder.GetChild(i));
				Lights[i].gameObject.SetActive(false);
			}

			componentsManager = Weapon.root.GetComponent<ComponentsManager>();
			weaponAnimator = weap.GetComponent<Animator>();
			
			return true;
		}
		catch
		{
			Debug.Log("failed while adding transforms");
			return false;
		}
	}
}

public struct HitDamageInfo
{
	public bool isEnnemy;
	public NetworkView player;
	public AeStats stat;
	public string damageLocation;
	public int weaponDamage;
	public int weaponID;
	public bool physicsFailed;

	public RaycastHit hit;
}

public class RecoilPart
{
	public int startingPart;
	public int endingPart;

	public float x;
	public float y;

	public RecoilPart (string [] parsedInfo)
	{
		try
		{
			startingPart = int.Parse(parsedInfo[0]);
			endingPart = int.Parse(parsedInfo[1]);

			x = float.Parse(parsedInfo[2]);
			y = float.Parse(parsedInfo[3]);

		}
		catch
		{
//			Debug.Log("error while parsing recoil");
		}
	}
}

