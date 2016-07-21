using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AeHostGame : MonoBehaviour 
{
	public Map CurrentMap;
	public GameObject MapHolder;
	public GameObject ImagePrefab;
	public List<Image> MapsUI = new List<Image>();

	public int IndexSelected;
	public int IndexRight;
	public int IndexLeft;

	public int ModeID;
	public AudioSource AudioComp;
	public AudioClip HighLightSound;
	public AudioClip PressedSound;

	public Text MapName;
	public Text ModeName;
	public Text ObjectifName;
	public Text NbObjectivs;
	public Text Players;
	public Animator ContainerSettings;

	public List<Button> ContainerSettingsButtons = new List<Button>();
	public List<Button> LobbyButtons = new List<Button>();


	public AeGamesMenu MenuGame;

	void Awake () 
	{
		MenuGame = GetComponent<AeGamesMenu>();
		AudioComp = GetComponent<AudioSource>();

		IndexSelected = 0;
	}
	void Update ()
	{
		AudioComp.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeMenuEffects;
	}

	public void InstantiateImage()
	{
		for(int i = 0; i < AeMaps.m_pAeMaps.MapList.Count;i++)
		{
			GameObject InstantiatedImage = Instantiate(ImagePrefab) as GameObject;
			InstantiatedImage.GetComponent<RectTransform>().SetParent(MapHolder.GetComponent<RectTransform>());
			InstantiatedImage.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			InstantiatedImage.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
			InstantiatedImage.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0,0);
			InstantiatedImage.GetComponent<Image>().sprite = AeMaps.m_pAeMaps.MapList[i].MapIcon;
			InstantiatedImage.name = AeMaps.m_pAeMaps.MapList[i].m_sName;
			MapsUI.Add(InstantiatedImage.GetComponent<Image>());
			InstantiatedImage.GetComponent<Button>().onClick.AddListener(() => { MoveWithImage(InstantiatedImage.GetComponent<Image>());}); 
		}
		MoveRight();
		CheckTheOneToSelect ();
	}
	public void HostGame ()
	{
		Network.InitializeServer(CurrentMap.m_iPlayersNb - 1,5678,false);
		MasterServer.RegisterHost("Aeternam",CurrentMap.m_sName,CurrentMap.GetCurrentMode().m_sName +";"
		                          + CurrentMap.GetCurrentMode().m_iObjectives + ";Waiting");

		AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.m_iLevelID = CurrentMap.m_iLevelID;
		AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.m_sName = CurrentMap.m_sName;
		AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.ModesSupported.Add(CurrentMap.GetCurrentMode());
		AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.m_iPlayersNb = CurrentMap.m_iPlayersNb;
		AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.ChosenGameMode = 0;
	}

	public void DontShow ()
	{
		ContainerSettings.GetComponent<CanvasGroup>().interactable = false;
		ContainerSettings.SetBool("UnShow",true);
		for(int i = 0;i < MapsUI.Count;i ++) MapsUI[i].GetComponent<Animator>().SetBool("UnShow",true);
		
	}
	public void Show ()
	{
		ContainerSettings.GetComponent<CanvasGroup>().interactable = true;
		ContainerSettings.SetBool("UnShow",false);
		for(int i = 0;i < MapsUI.Count;i ++) MapsUI[i].GetComponent<Animator>().SetBool("UnShow",false);
		
	}

	public void AddKills (bool AddkillOrMinus)
	{
		if(AddkillOrMinus)
		{
			CurrentMap.GetCurrentMode().m_iObjectives++;
			if (CurrentMap.GetCurrentMode().m_iObjectives > AeMaps.m_pAeMaps.MapList[IndexSelected].GetCurrentMode().m_iMaxObjectives)
				CurrentMap.GetCurrentMode().m_iObjectives = AeMaps.m_pAeMaps.MapList[IndexSelected].GetCurrentMode().m_iMinObjectives;

		}
		else
		{
			CurrentMap.GetCurrentMode().m_iObjectives--;
			if (CurrentMap.GetCurrentMode().m_iObjectives < AeMaps.m_pAeMaps.MapList[IndexSelected].GetCurrentMode().m_iMinObjectives)
				CurrentMap.GetCurrentMode().m_iObjectives = AeMaps.m_pAeMaps.MapList[IndexSelected].GetCurrentMode().m_iMaxObjectives;
		}

		SettingsModified();
	}

	public void MoveModeRight(bool MoveLeftOrRight)
	{
		if(MoveLeftOrRight)
		{
			CurrentMap.ChosenGameMode++;
			if (CurrentMap.ChosenGameMode > AeMaps.m_pAeMaps.MapList[IndexSelected].ModesSupported.Count - 1)
				CurrentMap.ChosenGameMode = 0;


		}
		else
		{
			CurrentMap.ChosenGameMode--;
			if (CurrentMap.ChosenGameMode < 0)
				CurrentMap.ChosenGameMode = AeMaps.m_pAeMaps.MapList[IndexSelected].ModesSupported.Count - 1;

		}
		SettingsModified();
	}

	public void AddPlayers (bool AddPlayerOrMinus)
	{
		if(AddPlayerOrMinus)
		{
			CurrentMap.m_iPlayersNb++;
			if (CurrentMap.m_iPlayersNb > AeMaps.m_pAeMaps.MapList[IndexSelected].m_iMaximumNbPlayers)
				CurrentMap.m_iPlayersNb = AeMaps.m_pAeMaps.MapList[IndexSelected].m_iMinimumNbPlayers;
			
		}
		else
		{
			CurrentMap.m_iPlayersNb--;
			if (CurrentMap.m_iPlayersNb < AeMaps.m_pAeMaps.MapList[IndexSelected].m_iMinimumNbPlayers)
				CurrentMap.m_iPlayersNb = AeMaps.m_pAeMaps.MapList[IndexSelected].m_iMaximumNbPlayers;

		}
		SettingsModified();
	}
	public void SettingsModified ()
	{
		if(Network.isClient)
		{
			Map map = AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations;

			ModeName.text = map.GetCurrentMode().m_sName;
			MapName.text = map.m_sName;
			ObjectifName.text = map.GetCurrentMode().m_sObjectivName;
			NbObjectivs.text = map.GetCurrentMode().m_iObjectives.ToString();
			Players.text = map.m_iPlayersNb.ToString();
		}
		else
		{
			ModeName.text = CurrentMap.GetCurrentMode().m_sName;
			MapName.text = CurrentMap.m_sName;
			ObjectifName.text = CurrentMap.GetCurrentMode().m_sObjectivName;
			NbObjectivs.text = CurrentMap.GetCurrentMode().m_iObjectives.ToString();
			Players.text = CurrentMap.m_iPlayersNb.ToString();
		}
	}
	void CheckTheOneToSelect ()
	{
		for(int i = 0; i < AeMaps.m_pAeMaps.MapList.Count; i++)
		{
			if(i != IndexRight && i != IndexLeft && i != IndexSelected)
			{
				MapsUI[i].GetComponent<Button>().interactable = false;
				MapsUI[i].GetComponent<Animator>().SetBool("Selected",false);
				MapsUI[i].GetComponent<Animator>().SetBool("Left",false);
				MapsUI[i].GetComponent<Animator>().SetBool("Right",false);
				MapsUI[i].GetComponent<Animator>().SetBool("Disabled",true);
			}
		}
		MapsUI[IndexSelected].GetComponent<Animator>().SetBool("Disabled",false);
		MapsUI[IndexLeft].GetComponent<Animator>().SetBool("Disabled",false);
		MapsUI[IndexRight].GetComponent<Animator>().SetBool("Disabled",false);
		MapsUI[IndexSelected].GetComponent<Button>().interactable = false;
		MapsUI[IndexLeft].GetComponent<Button>().interactable = true;
		MapsUI[IndexRight].GetComponent<Button>().interactable = true;


		MapsUI[IndexSelected].GetComponent<Animator>().SetBool("Selected",true);
		MapsUI[IndexSelected].GetComponent<Animator>().SetBool("Left",false);
		MapsUI[IndexSelected].GetComponent<Animator>().SetBool("Right",false);

		MapsUI[IndexLeft].GetComponent<Animator>().SetBool("Left",true);
		MapsUI[IndexLeft].GetComponent<Animator>().SetBool("Right",false);
		MapsUI[IndexLeft].GetComponent<Animator>().SetBool("Selected",false);

		MapsUI[IndexRight].GetComponent<Animator>().SetBool("Right",true);
		MapsUI[IndexRight].GetComponent<Animator>().SetBool("Left",false);
		MapsUI[IndexRight].GetComponent<Animator>().SetBool("Selected",false);

		MapsUI[IndexLeft].GetComponent<RectTransform>().SetParent(null);
		MapsUI[IndexLeft].GetComponent<RectTransform>().SetParent(MapHolder.GetComponent<RectTransform>());
		MapsUI[IndexLeft].GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);

		MapsUI[IndexRight].GetComponent<RectTransform>().SetParent(null);
		MapsUI[IndexRight].GetComponent<RectTransform>().SetParent(MapHolder.GetComponent<RectTransform>());
		MapsUI[IndexRight].GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);

		MapsUI[IndexSelected].GetComponent<RectTransform>().SetParent(null);
		MapsUI[IndexSelected].GetComponent<RectTransform>().SetParent(MapHolder.GetComponent<RectTransform>());
		MapsUI[IndexSelected].GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
		
		AeMaps.m_pAeMaps.MapList[IndexSelected].Selected = true;
		LoadTextAfterMap();
	}
	void LoadTextAfterMap()
	{
		CurrentMap.ModesSupported.Clear();
		for(int i = 0;i < AeMaps.m_pAeMaps.MapList[IndexSelected].ModesSupported.Count;i++) CurrentMap.ModesSupported.Add(AeMaps.m_pAeMaps.MapList[IndexSelected].ModesSupported[i]);
		

		CurrentMap.m_sName = AeMaps.m_pAeMaps.MapList[IndexSelected].m_sName;
		CurrentMap.GetCurrentMode().m_iObjectives = AeMaps.m_pAeMaps.MapList[IndexSelected].GetCurrentMode().m_iObjectives;
		CurrentMap.m_iPlayersNb = AeMaps.m_pAeMaps.MapList[IndexSelected].m_iPlayersNb;
		CurrentMap.GetCurrentMode().m_sObjectivName = AeMaps.m_pAeMaps.MapList[IndexSelected].GetCurrentMode().m_sObjectivName;
		CurrentMap.m_iLevelID = AeMaps.m_pAeMaps.MapList[IndexSelected].m_iLevelID;

		ModeName.text = CurrentMap.GetCurrentMode().m_sName;
		MapName.text = CurrentMap.m_sName;
		ObjectifName.text = CurrentMap.GetCurrentMode().m_sObjectivName;
		NbObjectivs.text = CurrentMap.GetCurrentMode().m_iObjectives.ToString();
		Players.text = CurrentMap.m_iPlayersNb.ToString();
	}

	public void MoveLeft ()
	{
		IndexSelected --;

		if(IndexSelected < 0) IndexSelected = MapsUI.Count - 1;

		if(IndexSelected - 1 < 0) IndexLeft = MapsUI.Count -1;
		else IndexLeft = IndexSelected - 1;

		
		if(IndexSelected + 1 > MapsUI.Count - 1) IndexRight = 0;
		else IndexRight = IndexSelected + 1;

		CheckTheOneToSelect();
	}
	public void MoveRight ()
	{
		IndexSelected ++;
		if(IndexSelected > MapsUI.Count -1) IndexSelected = 0;

		if(IndexSelected - 1 < 0) IndexLeft = MapsUI.Count -1;
		else IndexLeft = IndexSelected - 1;
		
		if(IndexSelected + 1 > MapsUI.Count - 1) IndexRight = 0;
		else IndexRight = IndexSelected + 1;
	
		CheckTheOneToSelect();
	}
	public void MoveWithImage (Image obj)
	{
		if(obj == MapsUI[IndexLeft]) 
		{
			MoveLeft();
			PlayPressedSound((Object)obj);
		}
		else if(obj == MapsUI[IndexRight])
		{
			MoveRight();
			PlayPressedSound((Object)obj);
		}
	}

	public void PlayHighLightSound (Object butObj)
	{
		GameObject but = (GameObject)butObj;
		if(but.GetComponent<Button>().interactable) AudioComp.PlayOneShot(HighLightSound);

	}
	public void PlayPressedSound (Object butObj) { AudioComp.PlayOneShot(PressedSound); }
}
