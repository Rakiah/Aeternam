using UnityEngine;
using System.Collections;

public class AeFirstTimeMenu : MonoBehaviour 
{
	public enum EnumFirstTime {PseudoChoice,FirstTime,CharacterChoice,None};
	public EnumFirstTime State = EnumFirstTime.None;

	int CharacterID = 0;

	string pseudoWished = "";
	public string messageerrorpseudo = "Choose a name";

	void OnGUI ()
	{
		float width = 1920f;
		float height = 1080f;
		float rx = Screen.width / width;
		float ry = Screen.height / height;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));

		switch(State)
		{
			case EnumFirstTime.PseudoChoice : Page_PseudoChoice();
			break;
			case EnumFirstTime.FirstTime : Page_FirstTime();
			break;
			case EnumFirstTime.CharacterChoice : Page_CharacterChoice();
			break;
			case EnumFirstTime.None :
			break;
		}
		
	}
	void Page_FirstTime ()
	{
		GUI.Box(new Rect(760,300,400,20),"Bienvenue dans Aeternam ! Voulez vous jouez le tutoriel ?");
		if(GUI.Button(new Rect(760,600,150,30),"Oui : Not Added Yet"))
		{
			
		}
		if(GUI.Button(new Rect(1160-150,600,150,30),"Non"))
		{
			if(AeCore.m_pCoreGame.MyStats.Character.Count <= 0)
			{
				State = EnumFirstTime.CharacterChoice;
				this.camera.enabled = true;
				foreach(Character CharToInstantiate in AeCharacters.m_pCharacter.CharacterList)
				{
					CharToInstantiate.m_gInstantiatedPrefab = Instantiate(CharToInstantiate.m_gCharacterPrefab,
					                                                      CharToInstantiate.m_gCharacterPrefab.transform.position,
					                                                      CharToInstantiate.m_gCharacterPrefab.transform.rotation) as GameObject;
					
					if(CharToInstantiate.m_iCharID == 0)
					{
						CharToInstantiate.m_gInstantiatedPrefab.SetActive(true);
					}
					else
					{
						CharToInstantiate.m_gInstantiatedPrefab.SetActive(false);
					}
					CharToInstantiate.m_gInstantiatedPrefab.transform.rotation = this.transform.rotation;
				}
			}
			else
			{
				AeDataRequest.m_pAeDataRequest.RefreshDatas();
			}
		}
	}
	void Page_PseudoChoice ()
	{
		GUI.Box(new Rect(760,300,400,20),"Bienvenue dans Aeternam ! Veuillez choisir un pseudo");

		pseudoWished = GUI.TextField(new Rect(860,500,200,30),pseudoWished);
		if(GUI.Button(new Rect(860,550,200,30),"Validé") && pseudoWished.Length > 3)
		{
			if(!AeDataRequest.m_pAeDataRequest.RequestingAttempt)
			{
				AeDataRequest.m_pAeDataRequest.RequestPseudo(pseudoWished);
			}
		}

		GUI.Label(new Rect(860,600,200,30),messageerrorpseudo);
	}
	void Page_CharacterChoice ()
	{
		GUI.Label(new Rect(760,300,400,20),"Choissisez votre personnage");
		
		for(int i = 0;i < AeCharacters.m_pCharacter.CharacterList.Count; i++)
		{
			Vector3 tempPos = this.camera.ScreenToWorldPoint(new Vector3(Screen.width*0.7f, Screen.height/5,1.5f));
			AeCharacters.m_pCharacter.CharacterList[i].m_gInstantiatedPrefab.transform.position = tempPos;
			if(AeCharacters.m_pCharacter.CharacterList[i].m_iCharID == CharacterID)
			{
				GUI.Label(new Rect(760,350,100,20),AeCharacters.m_pCharacter.CharacterList[i].m_sName);
				GUI.Label(new Rect(760,390,100,20),AeCharacters.m_pCharacter.CharacterList[i].m_iHealthStat.ToString());
				GUI.Label(new Rect(760,430,100,20),AeCharacters.m_pCharacter.CharacterList[i].m_iEnergyStat.ToString());
				GUI.Label(new Rect(760,470,100,20),AeCharacters.m_pCharacter.CharacterList[i].m_iSpeedStat.ToString());
				
				if(GUI.Button(new Rect(760,800,100,30),"Choisir") && !AeDataRequest.m_pAeDataRequest.RequestingAttempt)
				{
					AeDataRequest.m_pAeDataRequest.RequestingAttempt = true;
					AeDataRequest.m_pAeDataRequest.RequestCharacter(AeCharacters.m_pCharacter.CharacterList[i].m_iCharID);
				}
				if(CharacterID < AeCharacters.m_pCharacter.CharacterList.Count -1)
				{
					if(GUI.Button(new Rect(860,600,100,30),"Next"))
					{
						AeCharacters.m_pCharacter.CharacterList[i + 1].m_gInstantiatedPrefab.SetActive(true);
						AeCharacters.m_pCharacter.CharacterList[i].m_gInstantiatedPrefab.SetActive(false);
						CharacterID++;
					}
				}
				if(CharacterID > 0)
				{
					if(GUI.Button(new Rect(760,600,100,30),"Previous"))
					{
						AeCharacters.m_pCharacter.CharacterList[i - 1].m_gInstantiatedPrefab.SetActive(true);
						AeCharacters.m_pCharacter.CharacterList[i].m_gInstantiatedPrefab.SetActive(false);
						CharacterID--;
					}
				}
			}
		}
	}
}
