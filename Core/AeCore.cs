using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeCore : MonoBehaviour 
{
	public static AeCore m_pCoreGame;
	[HideInInspector] public AeSoundManager m_pSoundManager;
	[HideInInspector] public AeNetworkHandler m_pNetworkHandler;
	[HideInInspector] public PlayerStats MyStats = new PlayerStats();
	
	void Awake () 
	{
		m_pCoreGame = this;
		m_pSoundManager = this.GetComponent<AeSoundManager>();
		m_pNetworkHandler = GetComponent<AeNetworkHandler>();
		DontDestroyOnLoad(this.gameObject);
		Application.targetFrameRate = 60;
	}

	void Update () 
	{
	
	}

	
	public void GiveDataToCore (string Data)
	{
		string [] DataRequestedstr;
		DataRequestedstr = Data.Split('!');
		int.TryParse(DataRequestedstr[0],out MyStats.m_iNbKills);
		int.TryParse(DataRequestedstr[1],out MyStats.m_iNbDeaths);
		int.TryParse(DataRequestedstr[2],out MyStats.m_iNbAssists);
		int.TryParse(DataRequestedstr[3],out MyStats.m_iMonneyRecolted);
		int.TryParse(DataRequestedstr[4],out MyStats.m_iNbVictory);
		int.TryParse(DataRequestedstr[5],out MyStats.m_iNbDefeat);
		MyStats.ParseWeapons(DataRequestedstr[6]);
		MyStats.ParseCharacters(DataRequestedstr[7]);
		int.TryParse(DataRequestedstr[8],out MyStats.m_iDatabaseID);
		MyStats.m_sPseudo = DataRequestedstr[9];
	}
}

[System.Serializable]
public class PlayerStats
{
	public int m_iDatabaseID;
	public int m_iPlayerID;
	public int m_iNbDeaths = 0;
	public int m_iNbKills = 0;
	public int m_iNbAssists = 0;
	public int m_iNbVictory = 0;
	public int m_iNbDefeat = 0;
	public int m_iMonneyRecolted = 0;
	public int m_iTeamID = -1;
	public int m_iScore = 0;
	public CaracAll PlayerCaracteristics;
	public List<Character> Character = new List<Character>();
	
	public string username;
	public string password;
	
	public List<Item> Inventory = new List<Item>();
	
	public int m_iPing = -1;
	
	public NetworkPlayer m_pNetworkPlayer;
	public NetworkViewID m_iNetworkViewID;
	
	public GameObject InstantiatedPlayer;
	
	public ComponentsManager PlayerComponents;
	
	public bool m_bDied = true;
	
	public List<string> OwnedWeapons = new List<string>();
	public List<Weapon> m_wCurrentWeapons = new List<Weapon>();
	public string m_sPseudo = string.Empty;
	
	public bool FirstTime;
	
	public PlayerStats()
	{
		m_iTeamID = -1;
	}
	
	public void Reset ()
	{
		m_iNbDeaths = 0;
		m_iNbKills = 0;
		m_iNbAssists = 0;
		m_iMonneyRecolted = 0;
		m_iTeamID = -1;
		m_iScore = 0;
	}
	
	public void ResetAll ()
	{
		m_iNbDeaths = 0;
		m_iNbKills = 0;
		m_iNbAssists = 0;
		m_iMonneyRecolted = 0;
		m_iTeamID = -1;
		m_iScore = 0;
		Character.Clear();
		m_iPing = 0;
		m_bDied = false;
		PlayerCaracteristics = null;
		OwnedWeapons.Clear();
		m_wCurrentWeapons.Clear();
		m_sPseudo = string.Empty;
		FirstTime = true;
	}
	
	public void ParseWeapons (string weaptoparse)
	{
		string [] TempOwnedWep;
		TempOwnedWep = weaptoparse.Split(';');
		foreach(string tempWep in TempOwnedWep)
		{
			OwnedWeapons.Add(tempWep);
		}
	}
	
	public void ParseCharacters(string CharToParse)
	{
		if(CharToParse != "")
		{
			CharToParse = CharToParse.Remove(CharToParse.Length - 1);
			string [] TempOwnChar;
			TempOwnChar = CharToParse.Split(';');
			foreach(string Char in TempOwnChar)
			{
				int TempChar = 0;
				int.TryParse(Char,out TempChar);
				foreach(Character tempChar in AeCharacters.m_pCharacter.CharacterList)
				{
					if(tempChar.m_iCharID == TempChar)
					{
						Character charact = new Character();
						charact.m_sName = tempChar.m_sName;
						charact.m_iEnergyStat = tempChar.m_iEnergyStat;
						charact.m_iHealthStat = tempChar.m_iHealthStat;
						charact.m_iSpeedStat = tempChar.m_iSpeedStat;
						charact.m_iCharID = tempChar.m_iCharID;
						charact.m_gCharacterPrefab = tempChar.m_gCharacterPrefab;
						charact.m_gMenuPrefab = Resources.Load("PlayerMenu"+tempChar.m_sName) as GameObject;
						charact.m_gInGamePrefab = Resources.Load("PlayerInGame"+tempChar.m_sName) as GameObject;
						charact.m_tCharIcon = Resources.Load(tempChar.m_sName+"Bar") as Texture2D;
						Character.Add(charact);
					}
				}
			}
		}
	}

	public void ParseInventory ()
	{
		//		Inventory.Clear();
		//		for(int i = 0;i < OwnedWeapons.Count;i++)
		//		{
		//			for(int j = 0; j < AeWeapons.m_pAeWeapons.WeaponAvailableInTheGame.Count;j++)
		//			{
		//				if(OwnedWeapons[i] == AeWeapons.m_pAeWeapons.WeaponAvailableInTheGame[j].WeaponID)
		//				{
		//					Item tempItem = new Item();
		//					tempItem.Name = AeWeapons.m_pAeWeapons.WeaponAvailableInTheGame[j].m_sWeaponName;
		//					tempItem.TypeItem = 0;
		//					tempItem.WeaponSlot = AeWeapons.m_pAeWeapons.WeaponAvailableInTheGame[j];
		//					
		//					Inventory.Add(tempItem);
		//				}
		//			}
		//		}
	}
}
