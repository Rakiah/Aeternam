using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeNetworkHandler : MonoBehaviour 
{
	public List<PlayerStats> LPlayerStats = new List<PlayerStats>();
	public string EndGameText;
	public bool GameLaunched = false;
	[HideInInspector] public AeChat m_pChat;
	[HideInInspector] public AeMainMenuOptions m_pOptionsInGameMenu;
	[HideInInspector] public AeScoreBoard m_pScoreBoard;
	[HideInInspector] public AeMainMenu m_pMenu;
	[HideInInspector] public Map ServerInformations;


	void Awake () 
	{
		NetworkView view = gameObject.AddComponent<NetworkView>();
		view.stateSynchronization = NetworkStateSynchronization.Off;
		view.observed = null;

		m_pChat = GetComponent<AeChat>();
	}

	void OnServerInitialized ()
	{
		m_pMenu.Connected();
		PlayerStats ServerStats = new PlayerStats();
		ServerStats.m_sPseudo = AeCore.m_pCoreGame.MyStats.m_sPseudo;
		ServerStats.m_pNetworkPlayer = Network.player;
		LPlayerStats.Add(ServerStats);
		m_pMenu.AddPlayer(0);
		ShareStats();
		InvokeRepeating("UpdatePing",4.0f,4.0f);
	}

	void OnConnectedToServer ()
	{
		m_pMenu.Connected();
	}

	void OnPlayerConnected (NetworkPlayer playerconnected)
	{
		int NewIDPlayerStats;
		PlayerStats newplayerstats = new PlayerStats();
		newplayerstats.m_pNetworkPlayer = playerconnected;
		LPlayerStats.Add(newplayerstats);
		NewIDPlayerStats = LPlayerStats.LastIndexOf(newplayerstats);
		LPlayerStats[NewIDPlayerStats].m_iPlayerID = NewIDPlayerStats;

		networkView.RPC("GiveDefaultInformations",LPlayerStats[NewIDPlayerStats].m_pNetworkPlayer,
		                NewIDPlayerStats,LPlayerStats[0].m_iPlayerID,
		                LPlayerStats[0].m_sPseudo,/* end server infos */
		                ServerInformations.m_iLevelID,
		                ServerInformations.m_iPlayersNb,
		                ServerInformations.GetCurrentMode().m_iObjectives,
		                ServerInformations.GetCurrentMode().m_sName);
	}

	void OnPlayerDisconnected (NetworkPlayer playerdisconnected)
	{
		Network.DestroyPlayerObjects(playerdisconnected);
		Network.RemoveRPCs(playerdisconnected);
		networkView.RPC("RemovePlayer", RPCMode.All, playerdisconnected);
	}

	void OnDisconnectedFromServer ()
	{
		LPlayerStats.Clear();
		if(!GameLaunched) m_pMenu.Disconnected();
		else GoToLobby();

		ServerInformations.Reset();
		m_pChat.listmess.Clear();
		m_pChat.y = 0;
	}

	void UpdatePing ()
	{
		for (int i = 0; i < LPlayerStats.Count; i++)
		{
			int iPing = Network.GetAveragePing(LPlayerStats[i].m_pNetworkPlayer);
			networkView.RPC("SetPingScoreBoard",RPCMode.All,i,iPing);
		}
	}
	
	void ShareStats ()
	{
		for (int i = 0; i < LPlayerStats.Count; i++)
		{
			networkView.RPC("UpdatePlayerStats",RPCMode.All,LPlayerStats[i].m_sPseudo,LPlayerStats[i].m_iPlayerID,LPlayerStats[i].m_pNetworkPlayer);
		}
	}

	[RPC]
	void GiveDefaultInformations (int MyID,int ServerID,string ServerPseudo, int levelID, int nbPlayers, int nbObjectives, string ModeName)
	{
		PlayerStats tempstat = new PlayerStats();
		tempstat.m_iPlayerID = ServerID;
		tempstat.m_sPseudo = ServerPseudo;
		LPlayerStats.Add(tempstat);
		AeCore.m_pCoreGame.MyStats.m_iPlayerID = MyID;
		m_pMenu.AddPlayer(ServerID);


		ServerInformations.m_iLevelID = levelID;
		ServerInformations.m_iPlayersNb = nbPlayers;
		foreach(Map tempMap in AeMaps.m_pAeMaps.MapList)
		{
			if(tempMap.m_iLevelID == levelID)
			{
				ServerInformations.m_iMaximumNbPlayers = tempMap.m_iMaximumNbPlayers;
				ServerInformations.m_iMinimumNbPlayers = tempMap.m_iMinimumNbPlayers;
				ServerInformations.m_sName = tempMap.m_sName;
				ServerInformations.MapIcon = tempMap.MapIcon;

				foreach(GamesModes mode in tempMap.ModesSupported)
				{
					if(ModeName == mode.m_sName)
					{
						ServerInformations.ModesSupported.Add(mode);
						ServerInformations.ModesSupported[0].m_iObjectives = nbObjectives;
					}
				}
			}
		}
		m_pMenu.RefreshHosts();
		networkView.RPC("GiveServerInformations",RPCMode.Server,AeCore.m_pCoreGame.MyStats.m_sPseudo,AeCore.m_pCoreGame.MyStats.m_iPlayerID);
	}

	[RPC]
	void GiveServerInformations (string Pseudo, int ID)
	{
		LPlayerStats[ID].m_sPseudo = Pseudo;
		m_pMenu.AddPlayer(ID);
		ShareStats();
	}

	[RPC]
	void LoadGameNetwork ()
	{
		foreach(PlayerStats tempStat in LPlayerStats) tempStat.Reset();

		AeCore.m_pCoreGame.MyStats.m_iTeamID = -1;
		m_pChat.InGame = true;
		AeGameLoader.m_pAeGameLoader.LoadAGame(ServerInformations.m_iLevelID);
		GameLaunched = true;
	}

	[RPC] 
	void RemovePlayer(NetworkPlayer playerdisconnected)
	{
		for (int i = LPlayerStats.Count - 1; i >= 0; i--)
		{
			if(LPlayerStats[i].m_pNetworkPlayer == playerdisconnected)
			{
				LPlayerStats.RemoveAt(i);
				if(!GameLaunched) m_pMenu.RemovePlayer(i);
				if(m_pScoreBoard) m_pScoreBoard.DeleteAPlayer(i);
			}
		}

		for (int i = 0; i < LPlayerStats.Count;i++)
		{
			LPlayerStats[i].m_iPlayerID = i;
		}
	}

	[RPC]
	void GoToLobby ()
	{
		AeGameLoader.m_pAeGameLoader.LoadAGame(2);
		GameLaunched = false;
		ServerInformations.GetCurrentMode().gameEnded = false;
		m_pChat.KillLines.Clear();
		Destroy(m_pOptionsInGameMenu.gameObject);
		Screen.lockCursor = false;
		Screen.showCursor = true;
		m_pChat.InGame = false;
		m_pChat.isOnMenu = false;
		AeDataRequest.m_pAeDataRequest.RefreshDatas();
		EndGameText = "";
		m_pOptionsInGameMenu = null;
	}

	[RPC]
	void SetPingScoreBoard (int ID,int iPing)
	{
		LPlayerStats[ID].m_iPing = iPing;
		if(m_pScoreBoard) m_pScoreBoard.ReMakePing(ID);
	}

	[RPC]
	void UpdatePlayerStats (string Pseudo, int ID, NetworkPlayer Player)
	{
		if(ID > LPlayerStats.Count - 1)
		{
			PlayerStats TempStats = new PlayerStats();
			TempStats.m_iPlayerID = ID;
			TempStats.m_sPseudo = Pseudo;
			TempStats.m_pNetworkPlayer = Player;
			LPlayerStats.Add(TempStats);
			m_pMenu.AddPlayer(ID);
			if(m_pScoreBoard) m_pScoreBoard.AddAPlayer(ID);
		}
		else
		{
			LPlayerStats[ID].m_iPlayerID = ID;
			LPlayerStats[ID].m_sPseudo = Pseudo;
			LPlayerStats[ID].m_pNetworkPlayer = Player;
		}

		if(Player == Network.player)
		{
			AeCore.m_pCoreGame.MyStats.m_iPlayerID = ID;
		}
	}

	[RPC]
	void UpdatePlayerTeam (int TeamID, int PlayerID)
	{
		LPlayerStats[PlayerID].m_iTeamID = TeamID;
		LPlayerStats[PlayerID].PlayerComponents.m_pStats.m_iTeamID = TeamID;
		m_pScoreBoard.AssignTeam();

		if(Network.isClient) ServerInformations.GetCurrentMode().UpdateTeams(TeamID, PlayerID);

		if(PlayerID == AeCore.m_pCoreGame.MyStats.m_iPlayerID) 
		{
			AeCore.m_pCoreGame.MyStats.m_iTeamID = TeamID;
			ServerInformations.GetCurrentMode().FillSpawnPoints();
			LPlayerStats[PlayerID].InstantiatedPlayer.transform.position = ServerInformations.GetCurrentMode().GetSpawnPoint();
		}
	}

	[RPC]
	void SpawnCall ()
	{
		ServerInformations.GetCurrentMode().Spawn();
	}

	[RPC]
	void GameOverNetwork (string EndGame)
	{
		ServerInformations.GetCurrentMode().gameEnded = true;
		EndGameText = EndGame;
		Invoke("GoToLobby", 3.0f);
	}

	[RPC]
	public void Spawned (int PlayerID, NetworkViewID viewID)
	{
		PlayerStats spawnedPlayer = LPlayerStats[PlayerID];

		spawnedPlayer.m_bDied = false;
		spawnedPlayer.m_iNetworkViewID = viewID;
		spawnedPlayer.InstantiatedPlayer = NetworkView.Find(viewID).gameObject;
		spawnedPlayer.PlayerComponents = spawnedPlayer.InstantiatedPlayer.GetComponent<ComponentsManager>();
		spawnedPlayer.PlayerComponents.m_pStats.m_iTeamID = spawnedPlayer.m_iTeamID;

		if(m_pScoreBoard) m_pScoreBoard.ReMakeAlive(PlayerID);
	}

	[RPC]
	public void RegisterKillRPC (int Killer, int Killed, int WeaponUsed, int BodyPart)
	{
		ServerInformations.GetCurrentMode().RegisterKill(Killer, Killed, WeaponUsed, BodyPart);
	}

	[RPC]
	public void ChooseTeam (int TeamID, int PlayerID)
	{
		ServerInformations.GetCurrentMode().UpdateTeams(TeamID, PlayerID);
	}

}