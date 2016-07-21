using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeForAll : GamesModes
{
	PlayerStats LeaderStats;
	int m_iSpawnID;

	public FreeForAll (int pointsPerKill, int defaultObjective, int maxObjective, int minObjective)
	{
		m_iObjectives = defaultObjective;
		m_iMaxObjectives = maxObjective;
		m_iMinObjectives = minObjective;
		
		m_sName = "Free for all";
		m_sObjectivName = "Kills";
		
		m_iPoints = pointsPerKill;
		m_iPointsPerVictory = 500;
		m_fRespawnTime = 3.0f;
		m_bTeamGame = false;

		PlayerColor = Color.yellow;
	}
	
	public override void SetGameUI(Image left, Image right)
	{
		left.color = Color.black;
		right.color = Color.black;
		
		left.transform.FindChild("Header").GetComponent<Image>().color = Color.black;
		right.transform.FindChild("Header").GetComponent<Image>().color = Color.black;
	}

	public override void FillSpawnPoints ()
	{
		SpawnPoints.Add(GameObject.Find("SpawnPoints/FFA/"+AeCore.m_pCoreGame.MyStats.m_iPlayerID+"SpawnPointID0"));
		SpawnPoints.Add(GameObject.Find("SpawnPoints/FFA/"+AeCore.m_pCoreGame.MyStats.m_iPlayerID+"SpawnPointID1"));
		SpawnPoints.Add(GameObject.Find("SpawnPoints/FFA/"+AeCore.m_pCoreGame.MyStats.m_iPlayerID+"SpawnPointID2"));
		SpawnPoints.Add(GameObject.Find("SpawnPoints/FFA/"+AeCore.m_pCoreGame.MyStats.m_iPlayerID+"SpawnPointID3"));
	}
	
	public override Vector3 GetSpawnPoint ()
	{
		Vector3 position = SpawnPoints[m_iSpawnID].transform.position;
		m_iSpawnID ++;

		if(m_iSpawnID > SpawnPoints.Count - 1) m_iSpawnID = 0;

		return position;
	}
	
	public override bool IsGameOver ()
	{
		if(LeaderStats.m_iNbKills >= m_iObjectives)
		{
			foreach(PlayerStats stats in AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats)
			{
				if(stats != LeaderStats) AeDataRequest.m_pAeDataRequest.RegisterMatch(stats.m_sPseudo, false, PointsPerMatch(false));
				else AeDataRequest.m_pAeDataRequest.RegisterMatch(stats.m_sPseudo, true, PointsPerMatch(true));
			}

			AeCore.m_pCoreGame.networkView.RPC("GameOverNetwork",RPCMode.All, LeaderStats.m_sPseudo +" has won the game");

			return true;
		}

		return false;
	}

	public override void UpdateTeams (int TeamID, int PlayerID){}

	public override void Initialize ()
	{
		AeCore.m_pCoreGame.MyStats.m_iTeamID = -1;
		FillSpawnPoints ();

		if(Network.isServer)
		{
			Spawn();
			for(int i = 1; i < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count; i++) 
				AeCore.m_pCoreGame.StartCoroutine(DelaySpawn(0.5f, AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_pNetworkPlayer));
		}

	}
	
	public override int PointsPerKill (bool headShot) { return m_iPoints * (headShot == true ? 2 : 1); }

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

		if(Network.isServer)
		{
			if(!gameEnded)
			{
				AeCore.m_pCoreGame.networkView.RPC("RegisterKillRPC", RPCMode.Others, Killer, Killed, weapon, BodyPart);
				if(Killer != Killed) AeDataRequest.m_pAeDataRequest.RegisterKill(KillerStats.m_sPseudo, true, PointsPerKill(headshot));
				AeDataRequest.m_pAeDataRequest.RegisterKill(KilledStats.m_sPseudo, false, 0);

				AeCore.m_pCoreGame.StartCoroutine(DelaySpawn(m_fRespawnTime, KilledStats.m_pNetworkPlayer));
			}
		}
		
		if(Killer != Killed)
		{ 
			KillerStats.m_iNbKills ++;
			RegisterObjective(Killer);
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
	}

	public override void RegisterObjective (int ID)
	{
		PlayerStats KillerStats = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[ID];

		if(LeaderStats == null) LeaderStats = KillerStats;
		else if (KillerStats.m_iNbKills > LeaderStats.m_iNbKills) LeaderStats = KillerStats;

		if(Network.isServer) gameEnded = IsGameOver();
	}

	public override void SendAttack (HitDamageInfo info)
	{
		if(info.stat.Health > 0 && !info.stat.SpawnProtected)
		{
			info.player.RPC("TakeDamage",RPCMode.All, AeCore.m_pCoreGame.MyStats.m_iPlayerID, info.weaponDamage, info.weaponID, AeTools.damageLocationToInt(info.damageLocation));
			AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud.StartCoroutine(AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud.Hitmark(AeTools.damageLocationToInt(info.damageLocation) == 1 ? true : false));
		}
	}
	
	public override void Spawn ()
	{
		GetSpawnCamera().GetComponent<AudioListener>().enabled = false;

		GameObject player = Network.Instantiate(GetPlayer(), GetSpawnPoint(), Quaternion.identity, 0) as GameObject;
		AeCore.m_pCoreGame.MyStats.InstantiatedPlayer = player;
		AeCore.m_pCoreGame.MyStats.PlayerComponents = player.GetComponent<ComponentsManager>();
		AeCore.m_pCoreGame.MyStats.m_bDied = false;
		AeCore.m_pCoreGame.m_pNetworkHandler.networkView.RPC("Spawned",RPCMode.All, AeCore.m_pCoreGame.MyStats.m_iPlayerID, player.networkView.viewID);
		
		GameObject HUD = MonoBehaviour.Instantiate(GetPlayerHud()) as GameObject;
		AeCore.m_pCoreGame.MyStats.PlayerComponents.m_pHud = HUD.GetComponentInChildren<AeHUD>();
		HUD.transform.localPosition = new Vector3(0,0,5);

		player.transform.GetComponentInChildren<AeNetShoot>().gameObject.tag = "MainCamera";
		player.transform.FindChild("PublicGraphics").GetChild(0).GetComponent<Animator>().SetBool("LocalPlayer",true);
		
		player.networkView.RPC("SetWeapon", RPCMode.All, 2);
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
}
