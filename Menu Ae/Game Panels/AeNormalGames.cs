using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AeNormalGames : MonoBehaviour 
{
	public GameObject GameListHolder;
	public GameObject PrefabGame;
	public List<GameObject> GameBar = new List<GameObject>();
	public List<HostData> HostGames = new List<HostData>();
	public AeGamesMenu MenuGame;
	void Start () 
	{
		MenuGame = GetComponent<AeGamesMenu>();
		MasterServer.ClearHostList();
		InvokeRepeating("RefreshLists", 1.0f,3.0f);
	}

	void RefreshLists ()
	{
		MasterServer.RequestHostList("Aeternam");
		foreach(GameObject obj in GameBar)
		{
			Destroy(obj);
		}
		GameBar.Clear();
		HostGames.Clear();
		InstantiateEachGameBar();

	}

	public void Show   () { GameListHolder.GetComponent<Animator>().SetBool("Show",true);  }
	public void UnShow () { GameListHolder.GetComponent<Animator>().SetBool("Show",false); }

	void InstantiateEachGameBar ()
	{
		int GameID = 0;
		int YAxis = 270;
		foreach(HostData GameData in MasterServer.PollHostList())
		{
			string [] TempStrComment;
			TempStrComment = GameData.comment.Split(';');


			GameObject NewGameListObject = Instantiate(PrefabGame) as GameObject;
			NewGameListObject.GetComponent<RectTransform>().SetParent(GameListHolder.GetComponent<RectTransform>());
			NewGameListObject.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			NewGameListObject.GetComponent<RectTransform>().localEulerAngles = new Vector3(0,0,0);
			NewGameListObject.GetComponent<RectTransform>().localPosition = new Vector2(0,YAxis);
			NewGameListObject.GetComponent<RectTransform>().FindChild("GameID").GetComponent<Text>().text = GameID.ToString();
			NewGameListObject.GetComponent<RectTransform>().FindChild("Map").GetComponent<Text>().text = GameData.gameName;
			NewGameListObject.GetComponent<RectTransform>().FindChild("Mode").GetComponent<Text>().text = TempStrComment[0];
			NewGameListObject.GetComponent<RectTransform>().FindChild("Players").GetComponent<Text>().text = GameData.connectedPlayers+" / "+GameData.playerLimit;
			NewGameListObject.GetComponent<RectTransform>().FindChild("Objectives").GetComponent<Text>().text = TempStrComment[1];
			NewGameListObject.GetComponent<RectTransform>().FindChild("Status").GetComponent<Text>().text = TempStrComment[2];
			NewGameListObject.GetComponent<RectTransform>().FindChild("Join").GetComponent<Button>().onClick.AddListener(() => { ConnectTo(GameID - 1);}); 
			
			if(TempStrComment[2] == "Waiting") NewGameListObject.GetComponent<RectTransform>().FindChild("Join").GetComponent<Button>().interactable = true;
		
			HostGames.Add(GameData);
			GameBar.Add(NewGameListObject);
			YAxis -= 50;
			GameID++;
		}
	}

	public void ConnectTo(int id)
	{
		print("Connecting "+HostGames[id].ip[0] + " at Port "+HostGames[id].port);
		if(Network.peerType == NetworkPeerType.Disconnected)
		{
			if(!HostGames[id].useNat) Network.Connect(HostGames[id].ip,HostGames[id].port);
			else Network.Connect(HostGames[id].guid);
		}
	}
}
