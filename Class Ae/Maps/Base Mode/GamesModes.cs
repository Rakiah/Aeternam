using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public abstract class GamesModes
{
	public string m_sName;

	public string m_sObjectivName;
	public int m_iMaxObjectives;
	public int m_iMinObjectives;
	public int m_iObjectives;
	public int m_iPoints;
	public int m_iPointsPerVictory;

	public float m_fRespawnTime;

	public bool m_bTeamGame;

	public bool gameEnded;
	
	public Color PlayerColor;

	public GameObject Player;
	public GameObject PlayerHud;
	public GameObject SpawnCamera;

	public List<GameObject> SpawnPoints = new List<GameObject>();

	public abstract void FillSpawnPoints ();

	public abstract void SetGameUI(Image Left,Image Right);
	
	public abstract int PointsPerKill (bool headshot);
	public abstract int PointsPerMatch (bool victory);

	public abstract bool IsGameOver ();
	public abstract void Initialize ();
	public abstract void UpdateTeams (int TeamID, int PlayerID);
	public abstract void SendAttack (HitDamageInfo info);
	public abstract void RegisterKill (int Killer, int Killed, int weapon, int BodyPart);
	public abstract void RegisterObjective (int ID);
	
	public abstract IEnumerator DelaySpawn (float timing, NetworkPlayer player);
	public abstract IEnumerator DelaySpawnAll(float timing);

	public abstract void Spawn ();

	public virtual GameObject GetPlayer () { if(Player == null) Player = (GameObject)Resources.Load("PlayerInGameNurani"); return Player; }
	
	public virtual GameObject GetPlayerHud () { if(PlayerHud == null) PlayerHud = (GameObject)Resources.Load("HUDObject"); return PlayerHud; }
	
	public virtual GameObject GetSpawnCamera () { if(SpawnCamera == null) SpawnCamera = GameObject.Find("SpawnCamera"); return SpawnCamera; }
	
	public abstract Vector3 GetSpawnPoint ();
}


public class Team
{
	public int players;
	public int objectives;
	public int deathsPlayers;
	public bool hasLostRound;
	public Color color;
	
	public bool available;
	public GameObject objectCollider;
	
	public Team(int pl, int obj, Color co, GameObject objectCol)
	{
		players = pl;
		objectives = obj;
		color = co;
		objectCollider = objectCol;
		
		available = true;
	}
	
	public void SetTeamStatus (bool status)
	{
		objectCollider.collider.isTrigger = status;
		available = status;
	}
}