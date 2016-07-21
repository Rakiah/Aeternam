using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AeGamesMenu : MonoBehaviour 
{
	public AeLobby lobby;
	public AeHostGame hostgame;
	public AeNormalGames normalgame;
	public Animator Header;
	public PanelGameType PanelType = PanelGameType.NormalGame;
	public string CurrentHeaderText;

	void Start ()
	{
		lobby = GetComponent<AeLobby>();
		hostgame = GetComponent<AeHostGame>();
		normalgame = GetComponent<AeNormalGames>();
		hostgame.InstantiateImage();
		lobby.Unshow();
		hostgame.DontShow();
		normalgame.UnShow();

		if(PanelType == PanelGameType.NormalGame)
		{
			CurrentHeaderText = "Normal Games";
			normalgame.Show();
			StartCoroutine(MoveHeader());
		}
		else
		{		
			CurrentHeaderText = "Host Games";
			hostgame.Show();
			StartCoroutine(MoveHeader());
		}

		if(Network.isServer || Network.isClient)
		{
			Connected();
			lobby.RemakeThePlayerList();
		}
	}

	public void Connected         (            ) { StartCoroutine(GoToLobby());      }
	public void Disconnected      (            ) { StartCoroutine(GoToDisconnect()); }
	public void AddPlayerLobby    (int PlayerID) { lobby.AddAPlayer(PlayerID);	     }
	public void RemovePlayerLobby (int PlayerID) { lobby.DeleteAPlayer(PlayerID);    }

	IEnumerator GoToDisconnect ()
	{
		if(PanelType == PanelGameType.NormalGame)
		{
			lobby.Unshow();
			hostgame.DontShow();
			CurrentHeaderText = "Normal Games";
			StartCoroutine(MoveHeader());
			yield return new WaitForSeconds(1.0f);
			normalgame.Show();
		}
		else
		{
			lobby.Unshow();
			hostgame.DontShow();
			CurrentHeaderText = "Host Games";
			StartCoroutine(MoveHeader());
			yield return new WaitForSeconds(1.0f);
			hostgame.ContainerSettingsButtons[hostgame.ContainerSettingsButtons.Count-1].gameObject.SetActive(true);
			foreach(Button but in hostgame.ContainerSettingsButtons) but.gameObject.SetActive(true);
			foreach(Button but in hostgame.LobbyButtons) but.gameObject.SetActive(false);
			hostgame.InstantiateImage();
			hostgame.Show();
		}
	}
	IEnumerator GoToLobby()
	{
		CurrentHeaderText = "Lobby";
		StartCoroutine(MoveHeader());
		if(PanelType == PanelGameType.NormalGame)
		{
			normalgame.UnShow();
			yield return new WaitForSeconds(0.6f);
			lobby.Show();
		}
		else
		{
			hostgame.DontShow();
			yield return new WaitForSeconds(0.6f);
			lobby.Show();
		}

		if(Network.isClient)
		{
			foreach(Button but in hostgame.ContainerSettingsButtons) but.gameObject.SetActive(false);
			hostgame.LobbyButtons[2].gameObject.SetActive(true);
		}
		else if(Network.isServer)
		{
			
			hostgame.ContainerSettingsButtons[hostgame.ContainerSettingsButtons.Count-1].gameObject.SetActive(false);
			foreach(Button but in hostgame.LobbyButtons) but.gameObject.SetActive(true);
		}

		foreach(Image mapdis in hostgame.MapsUI) Destroy(mapdis.gameObject);
		hostgame.MapsUI.Clear();
		hostgame.Show();
	}
	IEnumerator MoveHeader ()
	{
		Header.SetBool("UnShow",true);
		yield return new WaitForSeconds(0.8f);
		Header.gameObject.GetComponent<Text>().text = CurrentHeaderText;
		Header.SetBool("UnShow",false);
	}
}

public enum PanelGameType {HostGame, NormalGame, Lobby}
