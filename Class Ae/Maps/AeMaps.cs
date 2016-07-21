using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeMaps : MonoBehaviour 
{
	public List<Map> MapList = new List<Map>();
	public static AeMaps m_pAeMaps;
	void Awake () 
	{
		m_pAeMaps = this;
		FillGamesModes ();
	}

	void FillGamesModes ()
	{
		for(int i = 0; i < MapList.Count; i++)
		{
			MapList[i].ModesSupported.Add(new FreeForAll(100, 10, 100, 2));
			MapList[i].ModesSupported.Add(new TeamDeathMatch(100, 20, 200, 5));
			MapList[i].ModesSupported.Add(new Hack(500,8,15,2));
		}
	}
	
	void Update () 
	{
	
	}
}


[System.Serializable]
public class Map 
{
	public string m_sName;
	public List<GamesModes> ModesSupported = new List<GamesModes>();
	public int ChosenGameMode;
	
	public int m_iMinimumNbPlayers;
	public int m_iMaximumNbPlayers;
	public int m_iPlayersNb;
	public int m_iLevelID;
	public Sprite MapIcon;
	public bool Selected = false;
	
	
	public void Reset ()
	{
		m_sName = "";
		m_iPlayersNb = 0;
		ChosenGameMode = 0;
		ModesSupported.Clear();
		m_iLevelID = 0;
	}
	
	public GamesModes GetCurrentMode ()
	{
		return ModesSupported[ChosenGameMode];
	}
}
