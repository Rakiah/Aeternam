using UnityEngine;
using System.Collections;

public class AeGameLoader : MonoBehaviour 
{

	public static AeGameLoader m_pAeGameLoader;
	void Awake () 
	{
		m_pAeGameLoader = this;
	}
	
	public void LoadAGame (int LevelToLoad)
	{
		Application.LoadLevel(LevelToLoad);
	}
	public void OnLevelWasLoaded (int LevelLoaded)
	{
		if(LevelLoaded == 2)
		{
			Screen.lockCursor = false;
			Screen.showCursor = true;
			AeCore.m_pCoreGame.m_pNetworkHandler.m_pMenu = GameObject.Find("MainMenu").GetComponent<AeMainMenu>();
		}
		if(AeMaps.m_pAeMaps)
		{
			foreach(Map map in AeMaps.m_pAeMaps.MapList)
			{
				if(map.m_iLevelID == LevelLoaded)
				{
					GameObject InstantiatedOptions = Instantiate(Resources.Load("OptionsInGameObject") as GameObject) as GameObject;
					AeCore.m_pCoreGame.m_pNetworkHandler.m_pOptionsInGameMenu = InstantiatedOptions.GetComponentInChildren<AeMainMenuOptions>();
					AeCore.m_pCoreGame.m_pNetworkHandler.m_pScoreBoard = InstantiatedOptions.GetComponentInChildren<AeScoreBoard>();
					AeCore.m_pCoreGame.m_pNetworkHandler.m_pScoreBoard.RemakeThePlayerList();

					AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().Initialize();
				}
			}
		}
		AeCore.m_pCoreGame.m_pSoundManager.PlayMusic(LevelLoaded);
	}
}
