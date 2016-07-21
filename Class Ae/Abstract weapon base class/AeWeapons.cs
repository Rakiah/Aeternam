using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AeWeapons : MonoBehaviour
{
	public static AeWeapons m_pAeWeapons;
	public List<MachineGun> AllMachinesGuns = new List<MachineGun>();
	public List<ShotGun> AllShotsGuns = new List<ShotGun>();
	public List<MeleeGun> AllMeleeWeapons = new List<MeleeGun>();
	public List<SniperGun> AllSnipers = new List<SniperGun>();

	
	void Awake ()
	{
		m_pAeWeapons = this;
	}
	
	void Update () 
	{
	
	}
	public void LoadGunsFromDataBase ()
	{
		AeDataRequest.m_pAeDataRequest.RequestGuns();
	}


	public void ParseGuns (string StrToParse)
	{
		string [] Datarequestedstr;
		Datarequestedstr = StrToParse.Split('!');
		for(int i = 1; i < Datarequestedstr.Length;i++)	
		{
			string [] parsedWeapon = Datarequestedstr[i].Split (';');
			switch(parsedWeapon[2])
			{
				case "1" : AllMachinesGuns.Add(new MachineGun(parsedWeapon));
				break;
				case "2" : AllShotsGuns.Add(new ShotGun(parsedWeapon));
				break;
				case "3" : AllSnipers.Add(new SniperGun(parsedWeapon));
				break;
				case "4" : AllMeleeWeapons.Add(new MeleeGun(parsedWeapon));
				break;
			}
		}
		AeCore.m_pCoreGame.MyStats.ParseInventory();
		if(GameObject.Find("ConnectionScreen")) GameObject.Find("ConnectionScreen").GetComponent<AeConScreen>().DataReceived();
	}
}
