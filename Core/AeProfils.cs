using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeProfils : MonoBehaviour 
{
	public static AeProfils m_pAeProfils;
	public List<ProfilQuality> QualityProfiles = new List<ProfilQuality>();

	public ProfilPrefs DefaultProfil;

	public ProfilPrefs CurrentProfil;

	void Awake() 
	{
		m_pAeProfils = this;

		if(PlayerPrefs.HasKey("FirstTime")) GetSettings();
		else SetBasicPrefs();
	}
	void SetBasicPrefs ()
	{
		Debug.Log("Set first time preferences");
		//store that the player made it once
		PlayerPrefs.SetInt("FirstTime",0);
		PlayerPrefs.SetString("Username","Welcome");
		//quality settings default
		PlayerPrefs.SetInt("QualityInt",DefaultProfil.quality.m_iID);
		PlayerPrefs.SetInt("Resolution",DefaultProfil.ResoID);
		PlayerPrefs.SetInt("Borderless",DefaultProfil.Borderless == true ? 1 : 0);

		//defaultsounds
		PlayerPrefs.SetFloat("MasterVolume",DefaultProfil.sound.MasterVolume);
		PlayerPrefs.SetFloat("MusicVolume",DefaultProfil.sound.MusicVolume);
		PlayerPrefs.SetFloat("SoundsEffect",DefaultProfil.sound.SoundsEffectVolume);
		PlayerPrefs.SetFloat("MenuEffect",DefaultProfil.sound.MenuEffectsVolume);

		//Controles

		PlayerPrefs.SetInt("Qwerty",DefaultProfil.control.QwertyKeyboard == true ? 1 : 0);
		PlayerPrefs.SetString("Shoot",DefaultProfil.control.Shoot.ToString());
		PlayerPrefs.SetString("Reload",DefaultProfil.control.Reload.ToString());
		PlayerPrefs.SetString("PickRifle",DefaultProfil.control.PickRifle.ToString());
		PlayerPrefs.SetString("PickSword",DefaultProfil.control.PickSword.ToString());
		PlayerPrefs.SetString("Parry",DefaultProfil.control.Parry.ToString());
		PlayerPrefs.SetString("GauntletAttack",DefaultProfil.control.GauntletAttack.ToString());
		PlayerPrefs.SetString("GauntletUse",DefaultProfil.control.GauntletUse.ToString());
		PlayerPrefs.SetString("Jump",DefaultProfil.control.Jump.ToString());
		PlayerPrefs.SetString("Slide",DefaultProfil.control.Slide.ToString());
		PlayerPrefs.SetString("Run",DefaultProfil.control.Run.ToString());
		PlayerPrefs.SetString("Crouch",DefaultProfil.control.Crouch.ToString());
		PlayerPrefs.SetFloat("Sensitivity",DefaultProfil.control.Sensitivity);


		//Custom Quality
		PlayerPrefs.SetInt("Shadows",(int)DefaultProfil.quality.m_eShadows);
		PlayerPrefs.SetInt("Texture",(int)DefaultProfil.quality.m_eTexture);
		PlayerPrefs.SetInt("AntiAliasing",(int)DefaultProfil.quality.m_eAntiAliasing);
		PlayerPrefs.SetInt("Particles",(int)DefaultProfil.quality.m_eParticles);
		PlayerPrefs.SetInt("PixelCount",(int)DefaultProfil.quality.m_ePixelCount);
		PlayerPrefs.SetInt("LOD",(int)DefaultProfil.quality.m_eLODLevel);

		GetSettings();

	}
	void GetSettings ()
	{
		//Load Pseudo
		GameObject.Find("ConnectionScreen").GetComponent<AeConScreen>().m_sUsernameplace = PlayerPrefs.GetString("Username");
		//quality settings default
		switch(PlayerPrefs.GetInt("QualityInt"))
		{
			case 0 : CurrentProfil.quality = QualityProfiles[0]; break;
			case 1 : CurrentProfil.quality = QualityProfiles[1]; break;
			case 2 : CurrentProfil.quality = QualityProfiles[2]; break;
			case 3 : CurrentProfil.quality = QualityProfiles[3]; break;
			case 4 : CurrentProfil.quality = QualityProfiles[4]; break;
			case 5 :	
				QualityProfiles[5].m_eShadows = (SettingEnum)PlayerPrefs.GetInt("Shadows");
				QualityProfiles[5].m_eTexture = (SettingEnum)PlayerPrefs.GetInt("Texture");
				QualityProfiles[5].m_eAntiAliasing = (SettingEnum)PlayerPrefs.GetInt("AntiAliasing");
				QualityProfiles[5].m_eParticles = (SettingEnum)PlayerPrefs.GetInt("Particles");
				QualityProfiles[5].m_ePixelCount = (SettingEnum)PlayerPrefs.GetInt("PixelCount");
				QualityProfiles[5].m_eLODLevel = (SettingEnum)PlayerPrefs.GetInt("LOD");
				CurrentProfil.quality = QualityProfiles[5];
			break;
			default : CurrentProfil.quality = QualityProfiles[3]; break;
		}
		CurrentProfil.ResoID = PlayerPrefs.GetInt("Resolution");
		CurrentProfil.Borderless = PlayerPrefs.GetInt("Borderless") == 1 ? true : false;
		switch(CurrentProfil.ResoID)
		{
			case 4  :  Screen.SetResolution(2560,1440,CurrentProfil.Borderless); break;
			case 3  :  Screen.SetResolution(1920,1080,CurrentProfil.Borderless); break;
			case 2  :  Screen.SetResolution(1600,900,CurrentProfil.Borderless);  break;
			case 1  :  Screen.SetResolution(1366,768,CurrentProfil.Borderless);  break;
			case 0  :  Screen.SetResolution(960,540,CurrentProfil.Borderless);   break;
			default :  Screen.SetResolution(1920,1080,CurrentProfil.Borderless); break;
		}
		
		//defaultsounds
		CurrentProfil.sound.MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
		CurrentProfil.sound.MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
		CurrentProfil.sound.SoundsEffectVolume = PlayerPrefs.GetFloat("SoundsEffect");
		CurrentProfil.sound.MenuEffectsVolume = PlayerPrefs.GetFloat("MenuEffect");
		
		//Controles
		CurrentProfil.control.QwertyKeyboard = PlayerPrefs.GetInt("Qwerty") == 1 ? true : false;
		CurrentProfil.control.Shoot = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("Shoot"));
		CurrentProfil.control.Reload = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("Reload"));
		CurrentProfil.control.PickRifle = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("PickRifle"));
		CurrentProfil.control.PickSword = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("PickSword"));
		CurrentProfil.control.Parry = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("Parry"));
		CurrentProfil.control.GauntletAttack = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("GauntletAttack"));
		CurrentProfil.control.GauntletUse = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("GauntletUse"));
		CurrentProfil.control.Jump = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump"));
		CurrentProfil.control.Slide = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("Slide"));
		CurrentProfil.control.Run = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("Run"));
		CurrentProfil.control.Crouch = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("Crouch"));
		CurrentProfil.control.Sensitivity = PlayerPrefs.GetFloat("Sensitivity");
		
		
		//Custom Quality
		QualityProfiles[5].m_eShadows = (SettingEnum)PlayerPrefs.GetInt("Shadows");
		QualityProfiles[5].m_eTexture = (SettingEnum)PlayerPrefs.GetInt("Texture");
		QualityProfiles[5].m_eAntiAliasing = (SettingEnum)PlayerPrefs.GetInt("AntiAliasing");
		QualityProfiles[5].m_eParticles = (SettingEnum)PlayerPrefs.GetInt("Particles");
		QualityProfiles[5].m_ePixelCount = (SettingEnum)PlayerPrefs.GetInt("PixelCount");
		QualityProfiles[5].m_eLODLevel = (SettingEnum)PlayerPrefs.GetInt("LOD");
	}

	public void UpdatePrefs ()
	{
		Debug.Log("UpdatePreferences");
		
		//quality settings default
		PlayerPrefs.SetInt("QualityInt",CurrentProfil.quality.m_iID);
		PlayerPrefs.SetInt("Resolution",CurrentProfil.ResoID);
		PlayerPrefs.SetInt("Borderless",CurrentProfil.Borderless == true ? 1 : 0);
		
		//defaultsounds
		PlayerPrefs.SetFloat("MasterVolume",CurrentProfil.sound.MasterVolume);
		PlayerPrefs.SetFloat("MusicVolume",CurrentProfil.sound.MusicVolume);
		PlayerPrefs.SetFloat("SoundsEffect",CurrentProfil.sound.SoundsEffectVolume);
		PlayerPrefs.SetFloat("MenuEffect",CurrentProfil.sound.MenuEffectsVolume);
		
		//Controles
		PlayerPrefs.SetInt("Qwerty",CurrentProfil.control.QwertyKeyboard == true ? 1 : 0);
		PlayerPrefs.SetString("Shoot",CurrentProfil.control.Shoot.ToString());
		PlayerPrefs.SetString("Reload",CurrentProfil.control.Reload.ToString());
		PlayerPrefs.SetString("PickRifle",CurrentProfil.control.PickRifle.ToString());
		PlayerPrefs.SetString("PickSword",CurrentProfil.control.PickSword.ToString());
		PlayerPrefs.SetString("Parry",CurrentProfil.control.Parry.ToString());
		PlayerPrefs.SetString("GauntletAttack",CurrentProfil.control.GauntletAttack.ToString());
		PlayerPrefs.SetString("GauntletUse",CurrentProfil.control.GauntletUse.ToString());
		PlayerPrefs.SetString("Jump",CurrentProfil.control.Jump.ToString());
		PlayerPrefs.SetString("Slide",CurrentProfil.control.Slide.ToString());
		PlayerPrefs.SetString("Run",CurrentProfil.control.Run.ToString());
		PlayerPrefs.SetString("Crouch",CurrentProfil.control.Crouch.ToString());
		PlayerPrefs.SetFloat("Sensitivity",CurrentProfil.control.Sensitivity);
		
		
		//Custom Quality
		if(CurrentProfil.quality.m_iID == 5)
		{
			PlayerPrefs.SetInt("Shadows",(int)CurrentProfil.quality.m_eShadows);
			PlayerPrefs.SetInt("Texture",(int)CurrentProfil.quality.m_eTexture);
			PlayerPrefs.SetInt("AntiAliasing",(int)CurrentProfil.quality.m_eAntiAliasing);
			PlayerPrefs.SetInt("Particles",(int)CurrentProfil.quality.m_eParticles);
			PlayerPrefs.SetInt("PixelCount",(int)CurrentProfil.quality.m_ePixelCount);
			PlayerPrefs.SetInt("LOD",(int)CurrentProfil.quality.m_eLODLevel);
		}
	}
	public void ResetSounds ()
	{
		CurrentProfil.sound.MasterVolume = DefaultProfil.sound.MasterVolume;
		CurrentProfil.sound.MusicVolume = DefaultProfil.sound.MusicVolume;
		CurrentProfil.sound.SoundsEffectVolume = DefaultProfil.sound.SoundsEffectVolume;
		CurrentProfil.sound.MenuEffectsVolume = DefaultProfil.sound.MenuEffectsVolume;
	}
	public void ResetControls ()
	{
		CurrentProfil.control.QwertyKeyboard = DefaultProfil.control.QwertyKeyboard;
		CurrentProfil.control.Shoot = DefaultProfil.control.Shoot;
		CurrentProfil.control.Reload = DefaultProfil.control.Reload;
		CurrentProfil.control.PickRifle = DefaultProfil.control.PickRifle;
		CurrentProfil.control.PickSword = DefaultProfil.control.PickSword;
		CurrentProfil.control.Parry = DefaultProfil.control.Parry;
		CurrentProfil.control.GauntletAttack = DefaultProfil.control.GauntletAttack;
		CurrentProfil.control.GauntletUse = DefaultProfil.control.GauntletUse;
		CurrentProfil.control.Jump = DefaultProfil.control.Jump;
		CurrentProfil.control.Run = DefaultProfil.control.Run;
		CurrentProfil.control.Crouch = DefaultProfil.control.Crouch;
		CurrentProfil.control.Sensitivity = DefaultProfil.control.Sensitivity;
	}
	public void ResetVideo ()
	{
		CurrentProfil.quality = DefaultProfil.quality;
		CurrentProfil.ResoID = DefaultProfil.ResoID;
		CurrentProfil.Borderless = DefaultProfil.Borderless;
	}


	public void CalculateQualitySettings ()
	{
		if(CurrentProfil.quality.m_eAntiAliasing == SettingEnum.High) QualitySettings.antiAliasing = 8;
		else if(CurrentProfil.quality.m_eAntiAliasing == SettingEnum.Medium) QualitySettings.antiAliasing = 4;
		else QualitySettings.antiAliasing = 2;


		if(CurrentProfil.quality.m_eTexture == SettingEnum.High) QualitySettings.masterTextureLimit = 0;
		else if(CurrentProfil.quality.m_eTexture == SettingEnum.Medium) QualitySettings.masterTextureLimit = 1;
		else QualitySettings.masterTextureLimit = 2;


		if(CurrentProfil.quality.m_ePixelCount == SettingEnum.High) QualitySettings.pixelLightCount = 4;
		else if(CurrentProfil.quality.m_eAntiAliasing == SettingEnum.Medium) QualitySettings.pixelLightCount = 2;
		else QualitySettings.pixelLightCount = 1;


		if(CurrentProfil.quality.m_eShadows == SettingEnum.High) { QualitySettings.shadowCascades = 4; QualitySettings.shadowDistance = 70; QualitySettings.shadowProjection = ShadowProjection.StableFit; }
		else if(CurrentProfil.quality.m_eShadows == SettingEnum.Medium) { QualitySettings.shadowCascades = 2; QualitySettings.shadowDistance = 50; QualitySettings.shadowProjection = ShadowProjection.CloseFit; }
		else { QualitySettings.shadowCascades = 0; QualitySettings.shadowDistance = 25; QualitySettings.shadowProjection = 0; }

		if(CurrentProfil.quality.m_eLODLevel == SettingEnum.High) { QualitySettings.lodBias = 1; QualitySettings.maximumLODLevel = 3; }
		else if(CurrentProfil.quality.m_eLODLevel == SettingEnum.Medium) { QualitySettings.lodBias = 1; QualitySettings.maximumLODLevel = 2; }
		else { QualitySettings.lodBias = 1; QualitySettings.maximumLODLevel = 1; }

		if(CurrentProfil.quality.m_eParticles == SettingEnum.High) QualitySettings.particleRaycastBudget = 1024;
		else if(CurrentProfil.quality.m_eParticles == SettingEnum.Medium) QualitySettings.particleRaycastBudget = 512;
		else QualitySettings.particleRaycastBudget = 256;
	}
}

[System.Serializable]
public class ProfilQuality
{
	public string m_sName;
	public int m_iID;
	public bool m_bInteractable;
	public SettingEnum m_eShadows;
	public SettingEnum m_eTexture;
	public SettingEnum m_eAntiAliasing;
	public SettingEnum m_eParticles;
	public SettingEnum m_ePixelCount;
	public SettingEnum m_eLODLevel;
}

[System.Serializable]
public class ProfilControles
{
	public bool QwertyKeyboard;
	public KeyCode Shoot;
	public KeyCode Reload;
	public KeyCode PickRifle;
	public KeyCode PickSword;
	public KeyCode Parry;
	public KeyCode GauntletAttack;
	public KeyCode GauntletUse;
	public KeyCode Jump;
	public KeyCode Slide;
	public KeyCode Run;
	public KeyCode Crouch;
	
	public float Sensitivity;
}

[System.Serializable]
public class ProfilSounds
{
	public float MasterVolume;
	public float MusicVolume;
	public float SoundsEffectVolume;
	public float MenuEffectsVolume;
}

[System.Serializable]
public class ProfilPrefs
{
	public ProfilControles control;
	public ProfilQuality quality;
	public ProfilSounds sound;
	public int ResoID;
	public bool Borderless;
}

