using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hack : GamesModes
{
	public List<Team> Teams = new List<Team>();

	public Hack (int pointsPerKill, int defaultObjective, int maxObjective, int minObjective)
	{
		m_iObjectives = defaultObjective;
		m_iMaxObjectives = maxObjective;
		m_iMinObjectives = minObjective;
		
		m_sName = "Hack";
		m_sObjectivName = "Rounds";
		
		m_iPoints = pointsPerKill;
		m_iPointsPerVictory = 500;
		m_fRespawnTime = 3.0f;
		
		m_bTeamGame = true;
		
		Teams.Add(new Team(0, 0, Color.red, null));
		Teams.Add(new Team(0, 0, Color.blue, null));
		
		PlayerColor = Color.yellow;
	}
	
	public override void SetGameUI(Image left, Image right)
	{
		left.color = Teams[0].color;
		right.color = Teams[1].color;
		
		left.transform.FindChild("Header").GetComponent<Image>().color = Teams[0].color;
		right.transform.FindChild("Header").GetComponent<Image>().color = Teams[1].color;
	}
	
	public override void FillSpawnPoints ()
	{
		SpawnPoints.Clear();
		
		SpawnPoints.Add(GameObject.Find("SpawnPoints/Team" + AeCore.m_pCoreGame.MyStats.m_iTeamID.ToString()+"/RightDown"));
		SpawnPoints.Add(GameObject.Find("SpawnPoints/Team" + AeCore.m_pCoreGame.MyStats.m_iTeamID.ToString()+"/LeftUp"));
		SpawnPoints.Add(GameObject.Find("SpawnPoints/Team" + AeCore.m_pCoreGame.MyStats.m_iTeamID.ToString()+"/LeftDown"));
	}
	
	public override Vector3 GetSpawnPoint ()
	{
		Vector3 position = Vector3.zero;
		float PossibleX = SpawnPoints[2].transform.position.x - SpawnPoints[0].transform.position.x;
		float PossibleZ = SpawnPoints[2].transform.position.z - SpawnPoints[1].transform.position.z;
		
		position = new Vector3( SpawnPoints[2].transform.position.x - Random.Range(0,PossibleX),
		                       SpawnPoints[2].transform.position.y ,
		                       SpawnPoints[2].transform.position.z - Random.Range(0,PossibleZ));
		
		return position;
	}
	
	public override bool IsGameOver ()
	{
		for(int i = 0; i < Teams.Count; i ++)
		{
			if(Teams[i].objectives >= m_iObjectives)
			{
				foreach(PlayerStats s in AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats)
				{
					bool MatchResult = s.m_iTeamID == i ? true : false;
					
					AeDataRequest.m_pAeDataRequest.RegisterMatch(s.m_sPseudo, MatchResult, PointsPerMatch(MatchResult));
				}
				
				AeCore.m_pCoreGame.networkView.RPC("GameOverNetwork", RPCMode.All, Teams[i].color.ToString() + " team has won the game");
				return true;
			}
		}

		return false;
	}
	
	public override void UpdateTeams (int TeamID, int PlayerID)
	{
		if(Network.isServer) 
		{ 
			if(!Teams[TeamID].available || AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[PlayerID].m_iTeamID > -1) return; 
			AeCore.m_pCoreGame.m_pNetworkHandler.networkView.RPC("UpdatePlayerTeam", RPCMode.All, TeamID, PlayerID);    
		}
				
		Teams[TeamID].players ++;                                                                                                                    
		
		int LTP = 0;
		
		//iterate twice to get the lower point and a third time to disable colliders on non authorized choice because there is too much players already
		foreach(Team t in Teams) if(t.players >= LTP) LTP = t.players;
		foreach(Team t in Teams) if(t.players <= LTP) LTP = t.players;
		
		foreach(Team t in Teams) t.SetTeamStatus(t.players > LTP ? false : true);
		
	}
	
	public override void Initialize ()
	{
		CreateLobby();
		FillSpawnPoints();

		if(Network.isServer)
		{
			Spawn();
			for(int i = 1; i < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count; i++) 
				AeCore.m_pCoreGame.StartCoroutine(DelaySpawn(0.5f, AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_pNetworkPlayer));
		}
	}
	
	public override int PointsPerKill (bool headshot) { return m_iPoints * (headshot == true ? 2 : 1); }
	
	public override int PointsPerMatch (bool victory) { return m_iPointsPerVictory / (victory == true ? 1 : 4); }
	
	public override void RegisterKill (int Killer, int Killed, int weapon, int BodyPart)
	{
		if(Killer < 0) Killer = Killed;
		
		PlayerStats KillerStats = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[Killer];
		PlayerStats KilledStats = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[Killed];
		
		Weapon weap = AeTools.GetWeaponById(weapon);
		
		bool headshot = BodyPart == 1 ? true : false;
		
		int Money = PointsPerKill(headshot);
		
		AeScoreBoard scoreboard = AeCore.m_pCoreGame.m_pNetworkHandler.m_pScoreBoard;
		
		if(Killer != Killed)
		{ 
			KillerStats.m_iNbKills ++; 
			KillerStats.m_iMonneyRecolted += Money;
			if(Killer == AeCore.m_pCoreGame.MyStats.m_iPlayerID) 
				if(AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud) 
					AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud.MadeAKill(headshot, Money, KilledStats.m_sPseudo, KillerStats.m_iMonneyRecolted - Money);
			
			if(scoreboard) scoreboard.ReOrganizeText(Killer);
		}


		KilledStats.m_iNbDeaths ++;
		KilledStats.m_bDied = true;
		
		
		if(scoreboard){ scoreboard.ReOrganizeText(Killed); scoreboard.ReMakeAlive(Killed); }
		
		AeCore.m_pCoreGame.m_pNetworkHandler.m_pChat.AddKillBar(KillerStats,KilledStats, weap, headshot);
	
		if(Network.isServer)
		{
			AeCore.m_pCoreGame.networkView.RPC("RegisterKillRPC", RPCMode.Others, Killer, Killed, weapon, BodyPart);

			if(Killer != Killed) AeDataRequest.m_pAeDataRequest.RegisterKill(KillerStats.m_sPseudo, true, PointsPerKill(headshot));
			AeDataRequest.m_pAeDataRequest.RegisterKill(KilledStats.m_sPseudo, false, 0);
			RegisterObjective(Killed);

		}
	}

	public override void RegisterObjective (int ID)
	{
		PlayerStats KilledStats = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[ID];

		Teams[KilledStats.m_iTeamID].deathsPlayers ++;
		if(Teams[KilledStats.m_iTeamID].deathsPlayers >= Teams[KilledStats.m_iTeamID].players)
		{
			Debug.Log("team : " + KilledStats.m_iTeamID + " is out");
			Teams[KilledStats.m_iTeamID].hasLostRound = true;

			int lostTeams = 0;
			int winningTeam = -1;
			for(int i = 0; i < Teams.Count; i++)
			{
				if(Teams[i].hasLostRound) lostTeams++;
				else winningTeam = i;
			}
			
			//if there is only one team left, grant them a point and start up the new round
			if(winningTeam != -1 && lostTeams >= Teams.Count - 1)
			{
				Debug.Log("team " + winningTeam + " won the round");
				Teams[winningTeam].objectives++;

				gameEnded = IsGameOver();

				if(!gameEnded)
				{
					foreach(Team t in Teams)
					{
						t.deathsPlayers = 0;
						t.hasLostRound = false;
					}

					AeCore.m_pCoreGame.StartCoroutine(DelaySpawnAll(m_fRespawnTime));
				}		
			}
		}
	}
	public override void SendAttack (HitDamageInfo info)
	{
		if(info.stat.m_iTeamID != AeCore.m_pCoreGame.MyStats.m_iTeamID)
		{
			Debug.Log("Attack from " +AeCore.m_pCoreGame.MyStats.m_iTeamID + " to " + info.stat.m_iTeamID);
			if(info.stat.Health > 0 && !info.stat.SpawnProtected)
			{
				info.player.RPC("TakeDamage",RPCMode.All, AeCore.m_pCoreGame.MyStats.m_iPlayerID, info.weaponDamage, info.weaponID, AeTools.damageLocationToInt(info.damageLocation));
				
				AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud.StartCoroutine(AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud.Hitmark
				                                                                  (AeTools.damageLocationToInt(info.damageLocation) == 1 ? true : false));
			}
		}
	}
	
	public override void Spawn ()
	{
		if(AeCore.m_pCoreGame.MyStats.InstantiatedPlayer) AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pStats.UnableComponents(0.0001f);

		GetSpawnCamera().GetComponent<AudioListener>().enabled = false;
		GameObject player = Network.Instantiate(GetPlayer(), GetSpawnPoint(), Quaternion.identity, 0) as GameObject;
		AeCore.m_pCoreGame.MyStats.InstantiatedPlayer = player;
		AeCore.m_pCoreGame.MyStats.PlayerComponents = player.GetComponent<ComponentsManager>();
		AeCore.m_pCoreGame.MyStats.m_bDied = false;
		AeCore.m_pCoreGame.m_pNetworkHandler.networkView.RPC("Spawned",RPCMode.All,AeCore.m_pCoreGame.MyStats.m_iPlayerID, player.networkView.viewID);
		
		GameObject HUD = MonoBehaviour.Instantiate(GetPlayerHud()) as GameObject;
		AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud = HUD.GetComponentInChildren<AeHUD>();
		HUD.transform.localPosition = new Vector3(0,0,5);
		
		player.transform.GetComponentInChildren<AeNetShoot>().gameObject.tag = "MainCamera";
		player.transform.FindChild("PublicGraphics").GetChild(0).GetComponent<Animator>().SetBool("LocalPlayer",true);
		
		player.networkView.RPC("SetWeapon", RPCMode.All, 1);
		player.networkView.RPC("SetWeapon", RPCMode.All, 6);
		
		player.networkView.RPC("SetGauntlet", RPCMode.All);
		
		player.networkView.RPC("SwitchGuns",RPCMode.All, 0);
		
		AeCore.m_pCoreGame.m_pSoundManager.PlayerInitalized(player);	
	}
	public override IEnumerator DelaySpawn (float timing, NetworkPlayer player)
	{
		yield return new WaitForSeconds(timing);
		
		if(player == AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[0].m_pNetworkPlayer) Spawn();
		else AeCore.m_pCoreGame.m_pNetworkHandler.networkView.RPC("SpawnCall", player);
	}

	public override IEnumerator DelaySpawnAll(float timing)
	{
		yield return new WaitForSeconds(timing);
		
		AeCore.m_pCoreGame.m_pNetworkHandler.networkView.RPC("SpawnCall", RPCMode.All);
	}
	
	void CreateLobby ()
	{
		GameObject lobby = MonoBehaviour.Instantiate(Resources.Load("TeamSelect") as GameObject, new Vector3(500,500,500), Quaternion.identity) as GameObject;
		
		lobby.transform.FindChild("Team-1").SetParent(GameObject.Find("SpawnPoints").transform);
		
		for(int i = 0; i < Teams.Count; i++)
		{
			Team t = Teams[i];
			t.objectCollider = lobby.transform.GetChild(i).gameObject;
			t.objectCollider.name = i.ToString();
			t.objectCollider.renderer.material.color = t.color;
		}
	}
}
