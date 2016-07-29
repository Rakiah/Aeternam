using UnityEngine;
using System.Collections;

public class AeDataRequest : MonoBehaviour
{
	public static AeDataRequest m_pAeDataRequest;

	public string URL = "http://rakiah.com/MY_SECRET_PATH";

	public bool RequestingAttempt = false;
	void Awake () 
	{
		m_pAeDataRequest = this;
		DontDestroyOnLoad(this);
	}

	void Update () 
	{

	}

	public void RequestGuns()
	{
		WWWForm m_fGunsRequest = new WWWForm();
		WWW GunsRequest = new WWW(URL + "MY_SECRET_PATH_TO_GET_GUNS", m_fGunsRequest);
		StartCoroutine(AnswerGunRequest(GunsRequest));
	}

	public void RequestUnlockWeapons(Weapon WeaponToUnlock)
	{
		if(AeCore.m_pCoreGame.MyStats.m_iMonneyRecolted >= WeaponToUnlock.m_iPrice)
		{
			WWWForm m_fBuyRequest = new WWWForm();
			m_fBuyRequest.AddField("user", AeCore.m_pCoreGame.MyStats.m_sPseudo);
			m_fBuyRequest.AddField("price", WeaponToUnlock.m_iPrice);
			m_fBuyRequest.AddField("weapID", WeaponToUnlock.WeaponID);
			m_fBuyRequest.AddField("security", MY_SECRET_CODE);

			WWW BuyRequest = new WWW(URL + "MY_SECRET_PATH_TO_BUY_GUNS", m_fBuyRequest);
			StartCoroutine(AnswerBuy(BuyRequest));
		}
	}

	public void RequestLogin (string m_sUserName, string m_sPassword)
	{
		AeCore.m_pCoreGame.MyStats.username = m_sUserName;
		AeCore.m_pCoreGame.MyStats.password = m_sPassword;
		WWWForm m_fLoginRequest = new WWWForm();
		m_fLoginRequest.AddField("user", m_sUserName);
		m_fLoginRequest.AddField("password", m_sPassword);
		m_fLoginRequest.AddField("security", MY_SECRET_CODE);

		WWW logincheck = new WWW(URL + "MY_SECRET_PATH_TO_LOGIN", m_fLoginRequest);
		StartCoroutine(AnswerLogin(logincheck,false));
	}

	public void RequestPseudo (string m_sPseudo)
	{
		WWWForm m_fPseudoRequest = new WWWForm();
		m_fPseudoRequest.AddField("userid",AeCore.m_pCoreGame.MyStats.m_iDatabaseID);
		m_fPseudoRequest.AddField("security", MY_SECRET_CODE);
		m_fPseudoRequest.AddField("pseudo", m_sPseudo);
		WWW AskingPseudo = new WWW(URL + "MY_SECRET_PATH", m_fPseudoRequest);
		RequestingAttempt = true;
		StartCoroutine(AnswerPseudo(AskingPseudo));
	}

	public void RegisterKill(string pseudo, bool kill, int money)
	{
		WWWForm m_fSendingInfoRequest = new WWWForm();
		m_fSendingInfoRequest.AddField("user", pseudo);
		m_fSendingInfoRequest.AddField("kill", kill == true ? 1 : 0);
		m_fSendingInfoRequest.AddField("money", money);
		m_fSendingInfoRequest.AddField("security", MY_SECRET_CODE);

		WWW SendingInfo = new WWW(URL + "MY_SECRET_PATH", m_fSendingInfoRequest);
		StartCoroutine(RegisterKillAnswer(SendingInfo));
	}

	public void RegisterMatch(string pseudo, bool victory, int money)
	{
		WWWForm m_fSendingInfoRequest = new WWWForm();
		m_fSendingInfoRequest.AddField("user", pseudo);
		m_fSendingInfoRequest.AddField("match", victory == true ? 1 : 0);
		m_fSendingInfoRequest.AddField("money", money);
		m_fSendingInfoRequest.AddField("security", MY_SECRET_CODE);

		WWW SendingInfo = new WWW(URL + "MY_SECRET_PATH", m_fSendingInfoRequest);
		StartCoroutine(RegisterMatchAnswer(SendingInfo));
	}

	public void RefreshDatas ()
	{
		WWWForm m_fDataRequest = new WWWForm();
		m_fDataRequest.AddField("user", AeCore.m_pCoreGame.MyStats.username);
		m_fDataRequest.AddField("password", AeCore.m_pCoreGame.MyStats.password);
		m_fDataRequest.AddField("security", MY_SECRET_CODE);

		WWW DataRequest = new WWW(URL + "MY_SECRET_PATH", m_fDataRequest);
		StartCoroutine(AnswerLogin(DataRequest,true));
	}


	public void RequestCharacter (int CharacterID)
	{
		WWWForm m_fSendingCharRequest = new WWWForm();
		m_fSendingCharRequest.AddField("userid", AeCore.m_pCoreGame.MyStats.m_iDatabaseID);
		m_fSendingCharRequest.AddField("Character", CharacterID);
		m_fSendingCharRequest.AddField("security", MY_SECRET_CODE);

		WWW SendingInfo = new WWW(URL + "MY_SECRET_PATH", m_fSendingCharRequest);
		StartCoroutine(AddCharacter(SendingInfo));
	}






	IEnumerator AddCharacter(WWW CharacterRequest)
	{
		yield return CharacterRequest;
		if (CharacterRequest.error == null)
		{
			if (CharacterRequest.text == "Succesfully added Character")
			{
				foreach(Character CharToDestroy in AeCharacters.m_pCharacter.CharacterList)
					Destroy(CharToDestroy.m_gInstantiatedPrefab);
				RefreshDatas();
			}
			else if (CharacterRequest.text == "Nope") Debug.Log("Error");
			else Debug.Log("Problem with answer of the database : " + CharacterRequest.text);
		}
		else Debug.Log("Error, the packet didn't even acceed to the database (probably script problem?");
		RequestingAttempt = false;
	}

	IEnumerator AnswerLogin(WWW logincheck, bool Refresh)
	{
		yield return logincheck;
		if (!Refresh)
		{
			AeConScreen conn = GameObject.Find("ConnectionScreen").GetComponent<AeConScreen>();
			if (logincheck.error == null)
			{
				//password wrong;
				if(logincheck.text == "Password does not match")
				{
					conn.m_bUserorPasswordNotFound = true;
					conn.message += logincheck.text;
				}
				//username wrong;
				else if(logincheck.text == "Username does not exist")
				{
					conn.m_bUserorPasswordNotFound = true;
					conn.message += logincheck.text;
				}
				//Security wrong;
				else if(logincheck.text == "Nope")
				{
					conn.m_bUserorPasswordNotFound = true;
					conn.message += logincheck.text;
				}
				//Login succes here !
				else
				{
					Debug.Log("Welcome player, here is you're parsed data !" + logincheck.text);
					AeCore.m_pCoreGame.MyStats.ResetAll();
					AeCore.m_pCoreGame.GiveDataToCore(logincheck.text);
					AeCore.m_pCoreGame.MyStats.ParseInventory();
					RequestGuns();
				}
			}
			else
			{
				conn.m_bUserorPasswordNotFound = true;
				conn.message += "Error"+logincheck.text+"\n";
			}
		}
		else
		{
			if (logincheck.error == null)
			{
				//password wrong;
				if(logincheck.text == "Password does not match")
				{
					Debug.Log("error pwd");
				}
				//username wrong;
				else if(logincheck.text == "Username does not exist")
				{
					Debug.Log("error username");
				}
				//Security wrong;
				else if(logincheck.text == "Nope")
				{
					Debug.Log("Error security");
				}
				//Login succes here !
				else
				{
					AeCore.m_pCoreGame.MyStats.ResetAll();
					AeCore.m_pCoreGame.GiveDataToCore(logincheck.text);
					AeCore.m_pCoreGame.MyStats.ParseInventory();

					if(GameObject.Find("MainMenu"))
					{
						if (!GameObject.Find("MainMenu").GetComponent<AeMainMenu>().PlayerMenu)
						{
							GameObject.Find("MainMenu").GetComponent<AeMainMenu>().InstantiateMenuPlayer();
							GameObject.Find("MainMenu").GetComponent<AeMainMenu>().firsttime.State = AeFirstTimeMenu.EnumFirstTime.None;
						}
					}
				}
			}
			else
			{
				Debug.Log("error db"+logincheck.error);
			}
		}
		RequestingAttempt = false;
	}

	IEnumerator AnswerPseudo (WWW PseudoCheck) 
	{
		yield return PseudoCheck;
		if(PseudoCheck.error == null)
		{
			if(PseudoCheck.text == "Succesfully Added")
			{
				Debug.Log("Pseudo accepted new table created !");
				GameObject.Find("MainMenu").GetComponent<AeMainMenu>().firsttime.State = AeFirstTimeMenu.EnumFirstTime.FirstTime;
			}
			else if(PseudoCheck.text == "Pseudo already taken")
			{
				Debug.Log("Already taken");
				GameObject.Find("MainMenu").GetComponent<AeMainMenu>().firsttime.messageerrorpseudo = "Already taken";
			}
			else if(PseudoCheck.text == "Error while adding")
			{
				Debug.Log("error with the insert sql");
				GameObject.Find("MainMenu").GetComponent<AeMainMenu>().firsttime.messageerrorpseudo = "Error database try again please or share us this bug on our website";
			}
			else
			{
				GameObject.Find("MainMenu").GetComponent<AeMainMenu>().firsttime.messageerrorpseudo = "Error database try again please or share us this bug on our website";
				Debug.Log("error");
			}
		}
		else
		{
			Debug.Log("error during the request");
			GameObject.Find("MainMenu").GetComponent<AeMainMenu>().firsttime.messageerrorpseudo = "Error database try again please or share us this bug on our website";
		}
		RequestingAttempt = false;
	}

	IEnumerator AnswerBuy (WWW BuyRequest)
	{
		yield return BuyRequest;
		if(BuyRequest.error == null)
		{
			Debug.Log(BuyRequest.text);
			RefreshDatas ();
		}
		else
		{
			Debug.Log("Problem during buying gun");
		}
		RequestingAttempt = false;
	}

	IEnumerator AnswerGunRequest(WWW GunRequest)
	{
		yield return GunRequest;
		if(GunRequest.error == null)
		{
			string Gunrequeststr = GunRequest.text;
			Gunrequeststr = Gunrequeststr.Remove(Gunrequeststr.Length - 1);
			AeWeapons.m_pAeWeapons.ParseGuns(Gunrequeststr);
		}
		else
		{
			GameObject.Find("ConnectionScreen").GetComponent<AeConScreen>().m_bUserorPasswordNotFound = true;
			GameObject.Find("ConnectionScreen").GetComponent<AeConScreen>().message += "Error"+GunRequest.error+"\n";
			Debug.Log("Problem during request guns : "+GunRequest.error);
		}
		RequestingAttempt = false;
	}





	IEnumerator RegisterKillAnswer (WWW SendingInformations)
	{
		yield return SendingInformations;
		if (SendingInformations.error == null)
		{
			if (SendingInformations.text != "Succesfully added informations")
			{
				Debug.LogError("Failed when we gave the kill informations, its an important informations, take caution");
			}
		}
		else
		{
			Debug.LogError("Failed when we gave the kill informations, its an important informations, take caution");
		}
		RequestingAttempt = false;
	}

	IEnumerator RegisterMatchAnswer (WWW SendingInformations)
	{
		yield return SendingInformations;
		if (SendingInformations.error == null)
		{
			if (SendingInformations.text != "Succesfully added informations")
			{
				Debug.LogError("Failed when we gave the match informations, its an important informations, take caution");
			}
		}
		else
		{
			Debug.LogError("Failed when we gave the match informations, its an important informations, take caution");
		}
		RequestingAttempt = false;
	}
}
