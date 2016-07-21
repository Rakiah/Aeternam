using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AeLobby : MonoBehaviour 
{
	public AudioSource AudioComp;
	public AudioClip HighLightSound;
	public AudioClip PressedSound;

	public List<PlayerPrefabScoreBoard> PlayerListImage = new List<PlayerPrefabScoreBoard>();
	public List<Vector2> Positions = new List<Vector2>();

	public GameObject PlayerPrefab;
	public GameObject PlayerHolder;
	public AeGamesMenu MenuGame;
	void Start () 
	{
		MenuGame = GetComponent<AeGamesMenu>();
	}

	void Update () 
	{
		for(int i = 0; i < PlayerListImage.Count;i++)
		{
			PlayerListImage[i].Img.GetComponent<RectTransform>().localPosition = 
				Vector2.Lerp(PlayerListImage[i].Img.GetComponent<RectTransform>().localPosition, 
				new Vector2(PlayerListImage[i].Img.GetComponent<RectTransform>().localPosition.x,Positions[i].y),
				Time.deltaTime * 7.0F);
		}
	}

	public void Show   () { PlayerHolder.GetComponent<Animator>().SetBool("Show", true); 			   }
	public void Unshow () { PlayerHolder.GetComponent<Animator>().SetBool("Show", false); DeleteAll(); }
	
	public void StartGame ()
	{
		Network.maxConnections = -1;
		MasterServer.RegisterHost("RPBGame",AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.m_sName,
		                          AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().m_sName+";"+
		                          AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().m_sObjectivName+";In Game");
		
		AeCore.m_pCoreGame.m_pNetworkHandler.networkView.RPC("LoadGameNetwork",RPCMode.All);
	}
	public void Disconnect () { Network.Disconnect(); }

	public void AddAPlayer (int i)
	{
		PlayerPrefabScoreBoard PrefabImage = new PlayerPrefabScoreBoard();
		GameObject NewPlayerInst = Instantiate(PlayerPrefab) as GameObject;
		PrefabImage.Img = NewPlayerInst.GetComponent<Image>();
		PrefabImage.Img.GetComponent<RectTransform>().SetParent(PlayerHolder.GetComponent<RectTransform>());
		PrefabImage.Img.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
		PrefabImage.Img.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
		PrefabImage.Img.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0,0);
		PlayerListImage.Add(PrefabImage);

		if(Positions.Count <= 0) Positions.Add(new Vector2(0,320));
		
		else if(Positions.Count > 0)
		{
			if(i > 0) Positions.Add(new Vector2(Positions[i-1].x,Positions[i-1].y-50)); 
			else Positions.Add(new Vector2(Positions[i].x,Positions[i].y-50));
		}
		
		OrganizeTextAtInstantiation(i);
	}
	void DeleteAll ()
	{
		for (int i = PlayerListImage.Count - 1; i >= 0; i--) DeleteAPlayer(i);
	}
	public void DeleteAPlayer (int i)
	{
		PlayerListImage[i].Destroyed = true;
		PlayerListImage[i].Img.GetComponent<Animator>().SetBool("Destroyed",true);
		StartCoroutine(DestroyAfterAnimation(PlayerListImage[i].Img));
		PlayerListImage.RemoveAt(i);
		Positions.RemoveAt(i);
	}

	IEnumerator DestroyAfterAnimation (Image img)
	{
		yield return new WaitForSeconds(0.2f);
		Destroy(img.gameObject);
		ReorganizeList();
		ReOrganizePositions();
	}

	public void ReorganizeList ()
	{
		for(int i = 0; i < PlayerListImage.Count;i++)
		{
			if (i < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count)
				PlayerListImage[i].Img.GetComponent<RectTransform>().FindChild("PlayerID").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iPlayerID.ToString();
		}
	}
	void ReOrganizePositions ()
	{
		int basePos = 320;
		for(int i = 0;i < Positions.Count; i++)
		{		
			Positions[i] = new Vector2(Positions[i].x,basePos);
			basePos -= 50;
		}
	}


	void OrganizeTextAtInstantiation(int i)
	{
		if(PlayerListImage[i].Img)
		{
			PlayerListImage[i].Img.GetComponent<RectTransform>().FindChild("Name").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_sPseudo;
			PlayerListImage[i].Img.GetComponent<RectTransform>().FindChild("Kills").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iNbKills.ToString();
			PlayerListImage[i].Img.GetComponent<RectTransform>().FindChild("Deaths").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iNbDeaths.ToString();
			PlayerListImage[i].Img.GetComponent<RectTransform>().FindChild("Score").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iScore.ToString();
			PlayerListImage[i].Img.GetComponent<RectTransform>().FindChild("Ping").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iPing.ToString();
			PlayerListImage[i].Img.GetComponent<RectTransform>().FindChild("PlayerID").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iPlayerID.ToString();
		}
	}

	public void RemakeThePlayerList ()
	{
		DeleteAll();

		for(int i = 0;i < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count;i++) AddAPlayer(i);
	}



	public void PlayHighLightSound (Object butObj) { AudioComp.PlayOneShot(HighLightSound); }
	public void PlayPressedSound   (Object butObj) { AudioComp.PlayOneShot(PressedSound);   }
}
