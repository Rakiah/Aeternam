using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeCharacters : MonoBehaviour 
{
	public static AeCharacters m_pCharacter;
	public List<Character> CharacterList = new List<Character>();

	void Awake () 
	{
		m_pCharacter = this;
	}
	

	void Update () 
	{
	}
}


[System.Serializable]
public class Character
{
	public string m_sName;
	public int m_iCharID;
	public Texture2D m_tCharIcon;
	public int m_iHealthStat;
	public int m_iEnergyStat;
	public int m_iSpeedStat;
	
	public GameObject m_gCharacterPrefab;
	public GameObject m_gInstantiatedPrefab;
	
	public GameObject m_gMenuPrefab;
	public GameObject m_gInGamePrefab;
	
}


[System.Serializable]
public class Caracteristic
{
	public string Name;
	public Carac Caracte;
	public int Points;
}
[System.Serializable]
public class CaracAll
{
	public Caracteristic Ninja;
	public Caracteristic Tank;
	public Caracteristic Magician;
	public Caracteristic Gunslinger;
	public Caracteristic Swordsman;
}

public enum Carac { Ninja, Tank, Magician, Gunslinger, Swordsman }
