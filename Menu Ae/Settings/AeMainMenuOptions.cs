using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AeMainMenuOptions : MonoBehaviour
{
	public bool InGameOptionsMenu;
	public bool showbox = false;
	public List<GroupButton> ButtonGroup = new List<GroupButton>();

	public List<GroupButtonSelect> GroupButtonSelectionList = new List<GroupButtonSelect>();
	

	public List<Button> VideoButtons = new List<Button>();

	public List<Button> ControlsButtons = new List<Button>();

	public List<Button> SoundsButtons = new List<Button>();
	public List<Slider> SoundsSliders = new List<Slider>();
	public List<Slider> ControlsSliders = new List<Slider>();

	bool CanBeModified = false;

	public Text ProfilHeader;
	public Toggle ToggleBorderless;
	public Animator WindowControl;

	public Animator AllPanelControl;
	
	public State myState = State.Zero;
	public AudioSource AudioComp;
	public AudioClip HighLightSound;
	public AudioClip PressedSound;
	
	public int CurrentInputModifying;

	void Awake () 
	{
		AudioComp = GetComponent<AudioSource>();
		if(InGameOptionsMenu) {	DontDestroyOnLoad(this); AudioComp.panLevel = 0.0f; AllPanelControl = this.GetComponent<Animator>(); }

		AssignBaseButtons ();

		foreach(GroupButton button in ButtonGroup) if(myState != button.StateButton) button.Container.GetComponent<Animator>().SetTrigger("Disable");
		Invoke("Modify", 0.5f);
	}
	void Update ()
	{
		if(InGameOptionsMenu)
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				if(showbox) MoveToGroup(0);
				
				Screen.lockCursor = !Screen.lockCursor;
				Screen.showCursor = !Screen.showCursor;
				showbox = !showbox;
				AllPanelControl.SetBool("Show",showbox);
				if(AeCore.m_pCoreGame.MyStats.InstantiatedPlayer) AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pInputs.Paused = showbox;
			}

		}
		AudioComp.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeMenuEffects;

		if(WindowControl.GetBool("Show"))
		{
			if(Input.anyKeyDown)
			{
				switch(CurrentInputModifying)
				{
					case 0 :
					AeProfils.m_pAeProfils.CurrentProfil.control.Shoot = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 1 : 
					AeProfils.m_pAeProfils.CurrentProfil.control.Reload = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 2 :
					AeProfils.m_pAeProfils.CurrentProfil.control.PickRifle = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 3 : 
					AeProfils.m_pAeProfils.CurrentProfil.control.PickSword = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 4 : 
					AeProfils.m_pAeProfils.CurrentProfil.control.Parry = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 5 : 
					AeProfils.m_pAeProfils.CurrentProfil.control.GauntletAttack = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 6 :
					AeProfils.m_pAeProfils.CurrentProfil.control.GauntletUse = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 9 :
					AeProfils.m_pAeProfils.CurrentProfil.control.Jump = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 10 :
					AeProfils.m_pAeProfils.CurrentProfil.control.Slide = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 11 :
					AeProfils.m_pAeProfils.CurrentProfil.control.Run = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					case 12 :
					AeProfils.m_pAeProfils.CurrentProfil.control.Crouch = (KeyCode)KeyCode.Parse(typeof(KeyCode), AeInputUtility.InputAction());
					UnShowInputWindow();
					break;
					default :
					UnShowInputWindow();
					break;
				}
			}
		}
	}
	void Modify () { CanBeModified = true; }

	void AssignBaseButtons ()
	{
		SetBackSounds ();
		SetBackQualityPrefab();
		SetBackResolution();
		SetBackSelectQuality();
		SetBackControls ();
	}

	void SetBackSounds ()
	{
		SoundsSliders[0].value = AeProfils.m_pAeProfils.CurrentProfil.sound.MasterVolume;
		SoundsSliders[1].value = AeProfils.m_pAeProfils.CurrentProfil.sound.MusicVolume;
		SoundsSliders[2].value = AeProfils.m_pAeProfils.CurrentProfil.sound.SoundsEffectVolume;
		SoundsSliders[3].value = AeProfils.m_pAeProfils.CurrentProfil.sound.MenuEffectsVolume;
	}

	void SetBackControls ()
	{
		ControlsButtons[0].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.Shoot.ToString();
		ControlsButtons[1].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.Reload.ToString();
		ControlsButtons[2].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.PickRifle.ToString();
		ControlsButtons[3].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.PickSword.ToString();
		ControlsButtons[4].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.Parry.ToString();
		ControlsButtons[5].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.GauntletAttack.ToString();
		ControlsButtons[6].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.GauntletUse.ToString();
		ControlsButtons[9].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.Jump.ToString();
		ControlsButtons[10].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.Slide.ToString();
		ControlsButtons[11].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.Run.ToString();
		ControlsButtons[12].GetComponentInChildren<Text>().text = AeProfils.m_pAeProfils.CurrentProfil.control.Crouch.ToString();
		ControlsSliders[0].value = AeProfils.m_pAeProfils.CurrentProfil.control.Sensitivity;



		if(AeProfils.m_pAeProfils.CurrentProfil.control.QwertyKeyboard)
		{
			GroupButtonSelectionList[8].ButtonInList[0].animator.SetBool("Selected",true);
			GroupButtonSelectionList[8].ButtonInList[1].animator.SetBool("Selected",false);
		}
		else
		{
			GroupButtonSelectionList[8].ButtonInList[1].animator.SetBool("Selected",true);
			GroupButtonSelectionList[8].ButtonInList[0].animator.SetBool("Selected",false);
		}
	}
	void SetBackQualityPrefab ()
	{
		foreach(Button but in GroupButtonSelectionList[0].ButtonInList) but.animator.SetBool("Selected",false);
		
		switch(AeProfils.m_pAeProfils.CurrentProfil.quality.m_iID)
		{
			case 0 :	GroupButtonSelectionList[0].ButtonInList[0].animator.SetBool("Selected",true);	break;
			case 1 :	GroupButtonSelectionList[0].ButtonInList[1].animator.SetBool("Selected",true);	break;
			case 2 :	GroupButtonSelectionList[0].ButtonInList[2].animator.SetBool("Selected",true);	break;
			case 3 :	GroupButtonSelectionList[0].ButtonInList[3].animator.SetBool("Selected",true);	break;
			case 4 :	GroupButtonSelectionList[0].ButtonInList[4].animator.SetBool("Selected",true);	break;
			case 5 :	GroupButtonSelectionList[0].ButtonInList[5].animator.SetBool("Selected",true);	break;
			default :	GroupButtonSelectionList[0].ButtonInList[3].animator.SetBool("Selected",true);	break;
		}
	}
	void SetBackSelectQuality ()
	{
		ProfilHeader.text = AeProfils.m_pAeProfils.CurrentProfil.quality.m_sName;
		for(int i = 2;i < 8;i++)
		{
			foreach(Button but in GroupButtonSelectionList[i].ButtonInList)
			{
				if(!AeProfils.m_pAeProfils.CurrentProfil.quality.m_bInteractable) { but.interactable = false; but.animator.SetTrigger("Disabled"); }
				else { but.interactable = true; }
				but.animator.SetBool("Selected",false);
			}
		}
		switch(AeProfils.m_pAeProfils.CurrentProfil.quality.m_eTexture)
		{
			case SettingEnum.Low : GroupButtonSelectionList[2].ButtonInList[0].animator.SetBool("Selected",true);	    break;
			case SettingEnum.Medium : GroupButtonSelectionList[2].ButtonInList[1].animator.SetBool("Selected",true);	break;
			case SettingEnum.High : GroupButtonSelectionList[2].ButtonInList[2].animator.SetBool("Selected",true);  	break;
		}
		switch(AeProfils.m_pAeProfils.CurrentProfil.quality.m_ePixelCount)
		{
			case SettingEnum.Low : GroupButtonSelectionList[3].ButtonInList[0].animator.SetBool("Selected",true);		break;
			case SettingEnum.Medium : GroupButtonSelectionList[3].ButtonInList[1].animator.SetBool("Selected",true);	break;
			case SettingEnum.High : GroupButtonSelectionList[3].ButtonInList[2].animator.SetBool("Selected",true);		break;
		}
		switch(AeProfils.m_pAeProfils.CurrentProfil.quality.m_eParticles)
		{
			case SettingEnum.Low : GroupButtonSelectionList[4].ButtonInList[0].animator.SetBool("Selected",true);		break;
			case SettingEnum.Medium : GroupButtonSelectionList[4].ButtonInList[1].animator.SetBool("Selected",true);	break;
			case SettingEnum.High : GroupButtonSelectionList[4].ButtonInList[2].animator.SetBool("Selected",true);		break;
		}
		switch(AeProfils.m_pAeProfils.CurrentProfil.quality.m_eShadows)
		{
			case SettingEnum.Low : GroupButtonSelectionList[5].ButtonInList[0].animator.SetBool("Selected",true);		break;
			case SettingEnum.Medium : GroupButtonSelectionList[5].ButtonInList[1].animator.SetBool("Selected",true);	break;
			case SettingEnum.High : GroupButtonSelectionList[5].ButtonInList[2].animator.SetBool("Selected",true);		break;
		}
		switch(AeProfils.m_pAeProfils.CurrentProfil.quality.m_eAntiAliasing)
		{
			case SettingEnum.Low : GroupButtonSelectionList[6].ButtonInList[0].animator.SetBool("Selected",true);		break;
			case SettingEnum.Medium : GroupButtonSelectionList[6].ButtonInList[1].animator.SetBool("Selected",true);	break;
			case SettingEnum.High : GroupButtonSelectionList[6].ButtonInList[2].animator.SetBool("Selected",true);		break;
		}

		switch(AeProfils.m_pAeProfils.CurrentProfil.quality.m_eLODLevel)
		{
			case SettingEnum.Low : GroupButtonSelectionList[7].ButtonInList[0].animator.SetBool("Selected",true);		break;
			case SettingEnum.Medium : GroupButtonSelectionList[7].ButtonInList[1].animator.SetBool("Selected",true);	break;
			case SettingEnum.High : GroupButtonSelectionList[7].ButtonInList[2].animator.SetBool("Selected",true);		break;
		}
	}
	void SetBackResolution ()
	{
		foreach(Button but in GroupButtonSelectionList[1].ButtonInList) but.animator.SetBool("Selected",false);
		
		for(int i = GroupButtonSelectionList[1].ButtonInList.Count;i >= 0;i--) 
			if(i == AeProfils.m_pAeProfils.CurrentProfil.ResoID) GroupButtonSelectionList[1].ButtonInList[i].animator.SetBool("Selected", true);	
		
		switch(AeProfils.m_pAeProfils.CurrentProfil.ResoID)
		{
			case 4 : Screen.SetResolution(2560,1440,AeProfils.m_pAeProfils.CurrentProfil.Borderless);	break;
			case 3 : Screen.SetResolution(1920,1080,AeProfils.m_pAeProfils.CurrentProfil.Borderless);	break;
			case 2 : Screen.SetResolution(1600,900,AeProfils.m_pAeProfils.CurrentProfil.Borderless);	break;
			case 1 : Screen.SetResolution(1366,768,AeProfils.m_pAeProfils.CurrentProfil.Borderless);	break;
			case 0 : Screen.SetResolution(960,540,AeProfils.m_pAeProfils.CurrentProfil.Borderless);		break;
		}
		ToggleBorderless.isOn = AeProfils.m_pAeProfils.CurrentProfil.Borderless;
	}


	public void MoveToGroup (int StateID)
	{
		RandomizeButtonsAnimations ();
		foreach(GroupButton button in ButtonGroup)
		{
			if(button.StateButton == myState) button.Container.GetComponent<Animator>().SetTrigger("Disable");
			if((int)button.StateButton == StateID) button.Container.GetComponent<Animator>().SetTrigger("Enable");
		}
		myState = (State)StateID;
	}

	void RandomizeButtonsAnimations ()
	{
		foreach(GroupButton grpbut in ButtonGroup)
		{
			grpbut.Container.GetComponent<Animator>().SetInteger("IdDisable",Random.Range(0,4));
			grpbut.Container.GetComponent<Animator>().SetInteger("IdEnable",Random.Range(0,4));
		}
	}

	public void PlayHighLightSound (Object butObj)
	{
		GameObject convertBut = (GameObject)butObj;
		if(convertBut.GetComponent<Button>().interactable) AudioComp.PlayOneShot(HighLightSound);

	}
	public void PlayPressedSound (Object butObj)
	{
		GameObject convertBut = (GameObject)butObj;
		if(convertBut.GetComponent<Button>().interactable) { AudioComp.PlayOneShot(PressedSound); CheckButton(convertBut.GetComponent<Button>()); }
	}
	public void ModifySliders(Object SlidObj)
	{
		if(CanBeModified)
		{
			GameObject slider = (GameObject)SlidObj;
			Slider slid = slider.GetComponent<Slider>();
			for(int i = 0; i < SoundsSliders.Count;i++)
			{
				if(SoundsSliders[i] == slid)
				{
					switch(i)
					{
						case 0 : AeProfils.m_pAeProfils.CurrentProfil.sound.MasterVolume = SoundsSliders[0].value;       break;
						case 1 : AeProfils.m_pAeProfils.CurrentProfil.sound.MusicVolume = SoundsSliders[1].value;        break;
						case 2 : AeProfils.m_pAeProfils.CurrentProfil.sound.SoundsEffectVolume = SoundsSliders[2].value; break;
						case 3 : AeProfils.m_pAeProfils.CurrentProfil.sound.MenuEffectsVolume = SoundsSliders[3].value;  break;
					}
				}
			}
			for(int i = 0; i < ControlsSliders.Count;i++)
			{
				if(ControlsSliders[i] == slid)
				{
					switch(i)
					{
						case 0 : AeProfils.m_pAeProfils.CurrentProfil.control.Sensitivity = ControlsSliders[0].value; break;
					}
				}
			}
		}
	}
	public void ModifyTogglers(Object TogObj)
	{
		if(CanBeModified) AeProfils.m_pAeProfils.CurrentProfil.Borderless = ToggleBorderless.isOn;
		
	}

	public void SelectMe(Object ObjToSelect)
	{
		GameObject ObjectConvert = (GameObject)ObjToSelect;
		foreach(GroupButtonSelect grpBS in GroupButtonSelectionList)
			if(grpBS.ButtonInList.Contains(ObjectConvert.GetComponent<Button>())) foreach(Button but in grpBS.ButtonInList) but.animator.SetBool("Selected",false);

		ObjectConvert.GetComponent<Animator>().SetBool("Selected",true);
	}

	void CheckButton (Button ButConvert)
	{
		if(VideoButtons.Contains(ButConvert))         { for(int i = 0; i < VideoButtons.Count;    i++) if(VideoButtons[i]    == ButConvert) DoSomething(i,0); }
		else if(ControlsButtons.Contains(ButConvert)) { for(int i = 0; i < ControlsButtons.Count; i++) if(ControlsButtons[i] == ButConvert) DoSomething(i,1); }
		else if(SoundsButtons.Contains(ButConvert))   { for(int i = 0; i < SoundsButtons.Count;   i++) if(SoundsButtons[i]   == ButConvert) DoSomething(i,2); }
	}

	void DoSomething (int i, int x)
	{
		if(x == 0)
		{
			switch(i)
			{
				case 0 : 
				AeProfils.m_pAeProfils.CurrentProfil.quality = AeProfils.m_pAeProfils.QualityProfiles[0];
				SetBackSelectQuality();
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 1 : 
				AeProfils.m_pAeProfils.CurrentProfil.quality = AeProfils.m_pAeProfils.QualityProfiles[1];
				SetBackSelectQuality();
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 2 :
				AeProfils.m_pAeProfils.CurrentProfil.quality = AeProfils.m_pAeProfils.QualityProfiles[2];
				SetBackSelectQuality();
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 3 : 
				AeProfils.m_pAeProfils.CurrentProfil.quality = AeProfils.m_pAeProfils.QualityProfiles[3];
				SetBackSelectQuality();
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 4 : 
				AeProfils.m_pAeProfils.CurrentProfil.quality = AeProfils.m_pAeProfils.QualityProfiles[4];
				SetBackSelectQuality();
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 5 : 
				AeProfils.m_pAeProfils.CurrentProfil.quality = AeProfils.m_pAeProfils.QualityProfiles[5];
				SetBackSelectQuality();
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 6 :
				AeProfils.m_pAeProfils.CurrentProfil.ResoID = 0;
				SetBackResolution();
				break;
				case 7 :
				AeProfils.m_pAeProfils.CurrentProfil.ResoID = 1;
				SetBackResolution();
				break;
				case 8 :
				AeProfils.m_pAeProfils.CurrentProfil.ResoID = 2;
				SetBackResolution();
				break;
				case 9 :
				AeProfils.m_pAeProfils.CurrentProfil.ResoID = 3;
				SetBackResolution();
				break;
				case 10 :
				AeProfils.m_pAeProfils.CurrentProfil.ResoID = 4;
				SetBackResolution();
				break;
				case 11 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eTexture = SettingEnum.Low;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 12 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eTexture = SettingEnum.Medium;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 13 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eTexture = SettingEnum.High;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 14 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_ePixelCount = SettingEnum.Low;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 15 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_ePixelCount = SettingEnum.Medium;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 16 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_ePixelCount = SettingEnum.High;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 17 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eParticles = SettingEnum.Low;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 18 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eParticles = SettingEnum.Medium;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 19 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eParticles = SettingEnum.High;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 20 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eShadows = SettingEnum.Low;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 21 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eShadows = SettingEnum.Medium;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 22 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eShadows = SettingEnum.High;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 23 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eAntiAliasing = SettingEnum.Low;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 24 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eAntiAliasing = SettingEnum.Medium;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 25 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eAntiAliasing = SettingEnum.High;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 26 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eLODLevel = SettingEnum.Low;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 27 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eLODLevel = SettingEnum.Medium;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 28 :
				AeProfils.m_pAeProfils.CurrentProfil.quality.m_eLODLevel = SettingEnum.High;
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
				case 29 :
				AeProfils.m_pAeProfils.ResetVideo();
				SetBackQualityPrefab();
				SetBackSelectQuality();
				SetBackResolution();
				AeProfils.m_pAeProfils.CalculateQualitySettings();
				break;
			}
		}
		else if(x == 1)
		{
			if(i != 13 && i != 7 && i != 8)
			{
				CurrentInputModifying = i;
				ShowInputWindow ();
			}
			else if(i == 7)
			{
				AeProfils.m_pAeProfils.CurrentProfil.control.QwertyKeyboard = true;
			}
			else if(i == 8)
			{
				AeProfils.m_pAeProfils.CurrentProfil.control.QwertyKeyboard = false;
			}
			else
			{
				AeProfils.m_pAeProfils.ResetControls();
				SetBackControls();
			}
		}
		else if(x == 2)
		{
			switch(i)
			{
				case 0 : AeProfils.m_pAeProfils.ResetSounds(); SetBackSounds(); break;
			}
		}
	}

	public void ShowInputWindow ()
	{
		foreach(Button but in ControlsButtons) but.interactable = false;
		WindowControl.SetBool("Show",true);
	}
	public void UnShowInputWindow ()
	{
		SetBackControls();
		foreach(Button but in ControlsButtons) but.interactable = true;
		WindowControl.SetBool("Show",false);
	}

	public void SaveSettings () { AeProfils.m_pAeProfils.UpdatePrefs(); }
	public void Quit() { if(InGameOptionsMenu) Application.Quit(); }
	public void GoMainMenu () { if(InGameOptionsMenu) Network.Disconnect(); }
}

[System.Serializable]
public class GroupButtonSelect
{
	public string Name;
	public List<Button> ButtonInList = new List<Button>();
}

public enum SettingEnum { Low, Medium, High }
