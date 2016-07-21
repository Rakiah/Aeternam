using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeMainMenu : MonoBehaviour 
{
	public enum GuiState
	{
		OptionsMenu,
		ShopMenu,
		ProfilMenu,
		DisconnectMenu,
		NormalGames,
		RankedGames,
		PrivateGames,
		HostGames,
		None
	};
	public GuiState MyMenuState = GuiState.None;

	public GameObject EventSystem;
	public AeFirstTimeMenu firsttime;

	public List<AeGamesMenu> GamePanel = new List<AeGamesMenu>();
	
	public float Sensibility;


	public bool UseUnityMasterServer = true;


	public GameObject PlayerMenu;

	void Start ()
	{
		EventSystem = GameObject.Find("EventSystem");

		EventSystem.SetActive(false);
		firsttime = this.GetComponent<AeFirstTimeMenu>();
		if(AeCore.m_pCoreGame.MyStats.m_sPseudo.Length < 4)
		{
			this.camera.enabled = true;
			firsttime.State = AeFirstTimeMenu.EnumFirstTime.PseudoChoice;
		}
		else if(AeCore.m_pCoreGame.MyStats.Character.Count <= 0)
		{
			this.camera.enabled = true;
			firsttime.State = AeFirstTimeMenu.EnumFirstTime.FirstTime;
		}
		else if(AeCore.m_pCoreGame.MyStats.Character.Count > 0 && AeCore.m_pCoreGame.MyStats.m_sPseudo.Length > 3)
		{
			this.camera.enabled = false;
			InstantiateMenuPlayer();
		}


	}
	public void InstantiateMenuPlayer ()
	{
		GetComponent<Camera>().enabled = false;
		GameObject SpawnPoint = GameObject.Find("SpawnPoint");
		PlayerMenu = Instantiate(AeCore.m_pCoreGame.MyStats.Character[0].m_gMenuPrefab,SpawnPoint.transform.position,SpawnPoint.transform.rotation) as GameObject;
		if(!AeCore.m_pCoreGame.MyStats.Character[0].m_gInstantiatedPrefab)
		{
			AeCore.m_pCoreGame.MyStats.Character[0].m_gInstantiatedPrefab = Instantiate(AeCore.m_pCoreGame.MyStats.Character[0].m_gCharacterPrefab,transform.position,AeCore.m_pCoreGame.MyStats.Character[0].m_gCharacterPrefab.transform.rotation) as GameObject;
			DontDestroyOnLoad(AeCore.m_pCoreGame.MyStats.Character[0].m_gInstantiatedPrefab);
		}
		AeCore.m_pCoreGame.MyStats.Character[0].m_gInstantiatedPrefab.SetActive(false);
	}
	
	public void EnableButtons ()
	{
		EventSystem.SetActive(true);
	}
	public void GetBackToPlayMode ()
	{
		EventSystem.SetActive(false);
		MyMenuState = GuiState.None;
	}
	public void Connected ()
	{
		foreach(AeGamesMenu panels in GamePanel)
		{
			panels.Connected();
		}
	}
	public void RefreshHosts ()
	{
		foreach(AeGamesMenu panels in GamePanel)
		{
			panels.hostgame.SettingsModified();
		}
	}
	public void AddPlayer (int PlayerID)
	{
		foreach(AeGamesMenu panels in GamePanel)
		{
			panels.AddPlayerLobby(PlayerID);
		}
	}
	public void RemovePlayer (int PlayerID)
	{
		foreach(AeGamesMenu panels in GamePanel)
		{
			panels.RemovePlayerLobby(PlayerID);
		}
	}
	public void Disconnected ()
	{
		foreach(AeGamesMenu panels in GamePanel)
		{
			panels.Disconnected();
		}
	}
}


[System.Serializable]
public class GroupButton
{
	public State StateButton = State.Zero;
	public GameObject Container;
}

public enum State { Zero, One, Two, Three, Four, Five, Six }
