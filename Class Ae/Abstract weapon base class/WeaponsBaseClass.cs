using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public abstract class Weapon
{
	public string m_sWeaponName;
	public int WeaponID;
	
	public Texture2D m_tIconTexture;
	
	public bool Default;
	
	public int m_iPrice;

	public WeaponStats m_sStats;
	public CrossHair m_sCrossHair;
	public WeaponTransforms m_sTransforms;

	//Shoot Actions

	//shoot and reload for guns and big attack for melee
	public abstract bool CanAction1 ();
	public abstract IEnumerator LaunchAction1 ();
	public abstract IEnumerator DoAction1CallBack ();

	public abstract bool CanAction2 ();
	public abstract IEnumerator LaunchAction2 ();
	public abstract IEnumerator DoAction2CallBack ();

	//full press actions like scope or parry
	public abstract bool StatusAction3 ();
	public abstract IEnumerator ProcessAction3 (bool status);
	public abstract IEnumerator DoAction3CallBack ();

	public abstract void Update (float MouseX, float MouseY, float HorizontalInput, float VerticalInput, bool JumpInput);

	public abstract bool CanSwitch ();

	public abstract float FadeAway ();
	public abstract float FadeIn ();

	public abstract bool Initialize (bool local, GameObject MultiHolder, GameObject LocalHolder);
}
