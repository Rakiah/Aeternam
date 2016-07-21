using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public class ShotGun : Weapon
{
	public int m_iAmmoMaxInAClip;
	public int m_iMaxCartouches;
	public int m_iFragments;
	public float m_fReloadTime;
	
	public ShotGun(string [] StrToParse)
	{
		
		m_sWeaponName = StrToParse[0];
		int.TryParse(StrToParse[1],out WeaponID);
		if(WeaponID == 1 || WeaponID == 4 || WeaponID == 6) Default = true;
		else Default = false;
		
		//int
		int.TryParse(StrToParse[3],out m_iAmmoMaxInAClip);
		int.TryParse(StrToParse[4],out m_iMaxCartouches);
		int.TryParse(StrToParse[5],out m_iPrice);
		
		
		//float
		m_sStats.FireRate = float.Parse(StrToParse[6]);
		m_fReloadTime = float.Parse(StrToParse[7]);
		
		m_sCrossHair.CrossHairBasicOffSet = float.Parse(StrToParse[8]);
		m_sCrossHair.CrossHairOffSetModifier = float.Parse(StrToParse[9]);
		m_sCrossHair.CrossHairStability = float.Parse(StrToParse[10]);
		m_sCrossHair.CrossHairMaxOffSet = float.Parse(StrToParse[11]);
		
		m_sStats.Range = float.Parse(StrToParse[12]);
		m_sStats.WeaponDamage = float.Parse(StrToParse[13]);
		m_sStats.Force = float.Parse(StrToParse[14]);
		
		m_tIconTexture = Resources.Load(m_sWeaponName+"Texture2D") as Texture2D;	
	}
	
	public ShotGun(ShotGun copy)
	{
		
	}
	
	public override bool CanAction1 ()
	{
		if(Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Shoot))
		{
			return true;
		}
		return false;
	}
	
	public override IEnumerator LaunchAction1 ()
	{
		m_sTransforms.Weapon.root.networkView.RPC("ShootOverNetwork", RPCMode.All);
		yield return null;
	}
	
	public override IEnumerator DoAction1CallBack ()
	{
		return null;
	}
	
	public override bool CanAction2 ()
	{
		return true;
	}
	
	public override IEnumerator LaunchAction2 ()
	{
		return null;
	}
	
	public override IEnumerator DoAction2CallBack ()
	{
		return null;
	}
	
	public override bool StatusAction3 ()
	{
		return true;
	}
	
	public override IEnumerator ProcessAction3 (bool status)
	{
		return null;
	}
	
	public override IEnumerator DoAction3CallBack ()
	{
		return null;
	}

	public override void Update (float MouseX, float MouseY, float HorizontalInput, float VerticalInput, bool JumpInput)
	{

	}

	public override bool CanSwitch ()
	{
		return true;
	}
	
	public override float FadeAway ()
	{
		return 0.5f;
	}
	
	public override float FadeIn ()
	{
		return 0.5f;
	}
	
	public override bool Initialize (bool local, GameObject multiHolder, GameObject localHolder)
	{
		return true;
	}
}
