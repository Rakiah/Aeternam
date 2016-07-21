using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AeScoreBoard : MonoBehaviour 
{
	public List<PlayerPrefabScoreBoard> PlayerListImage = new List<PlayerPrefabScoreBoard>();
	public int PlayerInContainer1;
	public int PlayerInCOntainer2;
	public List<Vector2> Positions = new List<Vector2>();

	public GameObject PlayerPrefab;

	public CanvasGroup Group;
	

	public Image Container1;
	public Image Container2;

	public List<Text> ContainerHeader1 = new List<Text>();
	public List<Text> ContainerHeader2 = new List<Text>();
	
	void Start () 
	{
		AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().SetGameUI(Container1, Container2);
	}
	

	void Update () 
	{
		if(Input.GetKey(KeyCode.Tab))
		{
			Group.alpha = 1;
		}
		else if(!Input.GetKey(KeyCode.Tab))
		{
			Group.alpha = 0;
		}
		for(int i = 0; i < PlayerListImage.Count;i++)
		{
			PlayerListImage[i].Img.GetComponent<RectTransform>().localPosition = 
						Vector2.Lerp(PlayerListImage[i].Img.GetComponent<RectTransform>().localPosition, 
			            new Vector2(PlayerListImage[i].Img.GetComponent<RectTransform>().localPosition.x,Positions[i].y),
			            Time.deltaTime * 7.0F);
		}
	}
	public void AssignTeam ()
	{
		for(int i = 0; i < PlayerListImage.Count; i++)
		{
			if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iTeamID == 0)
			{
				PlayerListImage[i].Img.color = Color.red;
				PlayerListImage[i].Img.rectTransform.FindChild("Team").GetComponent<Text>().text = "Red";
				PlayerListImage[i].Img.rectTransform.SetParent(Container1.rectTransform);
				PlayerListImage[i].Img.rectTransform.localPosition = new Vector3(0,0,0);
			}
			else if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iTeamID == 1)
			{
				PlayerListImage[i].Img.color = Color.blue;
				PlayerListImage[i].Img.rectTransform.FindChild("Team").GetComponent<Text>().text = "Blue";
				PlayerListImage[i].Img.rectTransform.SetParent(Container2.rectTransform);
				PlayerListImage[i].Img.rectTransform.localPosition = new Vector3(0,0,0);
			}
			else
			{
				PlayerListImage[i].Img.color = Color.black;
				PlayerListImage[i].Img.rectTransform.FindChild("Team").GetComponent<Text>().text = "Black";
			}


			if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iPlayerID == AeCore.m_pCoreGame.MyStats.m_iPlayerID)
			{
				PlayerListImage[i].Img.color = AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().PlayerColor;
			}

		}
	}
	public void AddAPlayer (int i)
	{
		PlayerPrefabScoreBoard PrefabImage = new PlayerPrefabScoreBoard();
		GameObject NewPlayerInst = Instantiate(PlayerPrefab) as GameObject;
		PrefabImage.Img = NewPlayerInst.GetComponent<Image>();
		if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iPlayerID == AeCore.m_pCoreGame.MyStats.m_iPlayerID)
		{
			PrefabImage.Img.color = AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().PlayerColor;
		}
		if(PlayerInContainer1 > PlayerInCOntainer2)
		{
			PlayerInCOntainer2++;
			PrefabImage.Img.rectTransform.SetParent(Container2.rectTransform);
		}
		else
		{
			PlayerInContainer1++;
			PrefabImage.Img.rectTransform.SetParent(Container1.rectTransform);
		}
		PrefabImage.Img.rectTransform.localPosition = new Vector3(0,0,0);
		PrefabImage.Img.rectTransform.localScale = new Vector3(1,1,1);
		PrefabImage.Img.rectTransform.localEulerAngles = new Vector3(0,0,0);
		PlayerListImage.Add(PrefabImage);
		
		if(Positions.Count <= 0)
		{
			Positions.Add(new Vector2(0,220));
			Positions.Add(new Vector2(0,220));
		}
		else if(Positions.Count > 0)
		{
			if(i > 0)
			{
				Positions.Add(new Vector2(Positions[i-1].x,Positions[i-1].y-25));
				Positions.Add(new Vector2(Positions[i-1].x,Positions[i-1].y-25));
			}
			else
			{
				Positions.Add(new Vector2(Positions[i].x,Positions[i].y-25));
				Positions.Add(new Vector2(Positions[i].x,Positions[i].y-25));
			}
		}
		ReOrganizeText(i);
	}
	void DeleteAll ()
	{
		for (int i = PlayerListImage.Count - 1; i >= 0; i--)
		{
			DeleteAPlayer(i);
		}
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
	}
	void ReOrganizePositions ()
	{
		int basePos = 220;
		for(int i = 0;i < Positions.Count; i++)
		{		
			Positions[i] = new Vector2(Positions[i].x,basePos);
			Positions[i+1] = new Vector2(Positions[i].x,basePos);
			basePos -= 25;
		}
	}
	
	
	public void ReOrganizeText(int i)
	{
		if(PlayerListImage[i].Img)
		{
			PlayerStats stat = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i];
			PlayerListImage[i].Img.rectTransform.FindChild("Name").GetComponent<Text>().text = stat.m_sPseudo;
			PlayerListImage[i].Img.rectTransform.FindChild("Kills").GetComponent<Text>().text = stat.m_iNbKills.ToString();
			PlayerListImage[i].Img.rectTransform.FindChild("Deaths").GetComponent<Text>().text = stat.m_iNbDeaths.ToString();
			PlayerListImage[i].Img.rectTransform.FindChild("Score").GetComponent<Text>().text = stat.m_iMonneyRecolted.ToString();
		}

		if(AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().m_bTeamGame)
		{
			int KillsTeam1 = 0;
			int DeathTeam1 = 0;
			int ScoreTeam1 = 0;
			int KillsTeam2 = 0;
			int DeathTeam2 = 0;
			int ScoreTeam2 = 0;

			for(int x = 0; x < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count; x++)
			{
				if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iTeamID == 0)
				{
					KillsTeam1 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iNbKills;
					DeathTeam1 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iNbDeaths;
					ScoreTeam1 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iMonneyRecolted;
				}
				else if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iTeamID == 1)
				{
					KillsTeam2 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iNbKills;
					DeathTeam2 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iNbDeaths;
					ScoreTeam2 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iMonneyRecolted;
				}
			}
			ContainerHeader1[1].text = "Kills \n" + KillsTeam1;
			ContainerHeader1[2].text = "Deaths \n" + DeathTeam1;
			ContainerHeader1[3].text = "Score \n" + ScoreTeam1;

			ContainerHeader2[1].text = "Kills \n" + KillsTeam2;
			ContainerHeader2[2].text = "Deaths \n" + DeathTeam2;
			ContainerHeader2[3].text = "Score \n" + ScoreTeam2;
		}
	}

	public void ReMakePing (int i)
	{
		if(PlayerListImage[i].Img)
		{
			PlayerListImage[i].Img.rectTransform.FindChild("Ping").GetComponent<Text>().text = AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_iPing.ToString();
		}

		if(AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().m_bTeamGame)
		{
			int MoyPing1 = 0;
			int MoyPing2 = 0;
			int NumberPlayers1 = 0;
			int NumberPlayers2 = 0;

			for(int x = 0; x < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count; x++)
			{
				if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iTeamID == 0)
				{
					MoyPing1 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iPing;
					NumberPlayers1++;
				}
				else if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iTeamID == 1)
				{
					MoyPing2 += AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iPing;
					NumberPlayers2++;
				}
			}
			if(NumberPlayers1 > 0)
			{
				MoyPing1 = MoyPing1 / NumberPlayers1;
			}
			if(NumberPlayers2 > 0)
			{
				MoyPing2 = MoyPing2 / NumberPlayers2;
			}
			ContainerHeader1[4].text = "Ping \n" + MoyPing1;
			ContainerHeader1[0].text = "Players \n" + NumberPlayers1;

			ContainerHeader2[4].text = "Ping \n" + MoyPing2;
			ContainerHeader2[0].text = "Players \n" + NumberPlayers2;
		}
	}

	public void ReMakeAlive(int i)
	{
		if(PlayerListImage[i].Img) PlayerListImage[i].Img.rectTransform.FindChild("Alive").GetComponent<Text>().text = 
			AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[i].m_bDied == true ? "Dead" : "Alive";
		

		if(AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().m_bTeamGame)
		{
			int NumberPlayers1 = 0;
			int NumberPlayers2 = 0;
			int AlivePlayers1 = 0;
			int AlivePlayers2 = 0;
			
			for(int x = 0; x < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count; x++)
			{
				if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iTeamID == 0)
				{
					NumberPlayers1++;
					if(!AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_bDied)
					{
						AlivePlayers1++;
					}
				}
				else if(AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_iTeamID == 1)
				{
					NumberPlayers2++;
					if(!AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats[x].m_bDied)
					{
						AlivePlayers2++;
					}
				}
			}
			ContainerHeader1[6].text = "Alive \n" + AlivePlayers1+ " / "+ NumberPlayers1;
			ContainerHeader1[0].text = "Players \n" + NumberPlayers1;

			ContainerHeader2[6].text = "Alive \n" + AlivePlayers2+ " / "+ NumberPlayers2;
			ContainerHeader2[0].text = "Players \n" + NumberPlayers2;
		}
	}
	public void RemakeThePlayerList ()
	{
		DeleteAll();
		
		for(int i = 0;i < AeCore.m_pCoreGame.m_pNetworkHandler.LPlayerStats.Count;i++)
		{
			AddAPlayer(i);
		}
	}		
}

[System.Serializable]
public class PlayerPrefabScoreBoard
{
	public Image Img;
	public bool Destroyed;
}