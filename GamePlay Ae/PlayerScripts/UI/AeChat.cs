using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeChat : MonoBehaviour 
{
	public List<string> listmess = new List<string>();
	
	public List<ChatKillerLine> KillLines = new List<ChatKillerLine>();
	public int ScreenShot;
	
	public float InactivityTimer;
	public bool ChatInactived = false;
	
	public Rect LobbyRect;

	public Rect InGameRect;

	public bool InGame;
	public bool isOnMenu;
	private Vector2 scrollView = Vector2.up;
	public int y = 200;
	public GUISkin myskin;
	string addtolist = "";
	Texture2D headshot;
	
	void Start ()
	{
		ScreenShot = PlayerPrefs.GetInt("Screenshot");
		headshot = Resources.Load("HeadShotTexture") as Texture2D;
	}
	void OnGUI ()
	{
		float width = 1920f;
		float height = 1080f;
		float rx = Screen.width / width;
		float ry = Screen.height / height;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));
		if(Network.peerType != NetworkPeerType.Disconnected)
		{
			if(!InGame && isOnMenu)
			{
				ShowChat();
				ShowKillsLines();
			}
			if(InGame)
			{
				if(!ChatInactived) ShowChat();
				ShowKillsLines();
			}
		}
		if(Input.GetKeyDown(KeyCode.F10))
		{
			Application.CaptureScreenshot(Application.dataPath+"/ScreenShotAeternam"+ ScreenShot.ToString());
			ScreenShot++;
			PlayerPrefs.SetInt("Screenshot",ScreenShot);
		}
	}
	void Update ()
	{
		if(InactivityTimer > 0)
		{
			InactivityTimer -= Time.deltaTime;
			ChatInactived = false;
		}
		else if(InactivityTimer <= 0)
		{
			InactivityTimer = 0;
			ChatInactived = true;
		}
		
		if(Input.GetKeyDown(KeyCode.Return))
		{
			InactivityTimer = 10.0f;
		}
	}
	void ShowChat ()
	{
		if(!InGame)
		{
			GUI.Window(1, LobbyRect, window, "Chat",myskin.window);
		}
		else
		{
			GUI.Window(1, InGameRect, window, "Chat",myskin.window);
		}
	}
	void ShowKillsLines ()	
	{

		int offsetbetwin = 20;
		for(int i = 0; i <= KillLines.Count-1;i++)
		{
			GUI.Label(new Rect(20,offsetbetwin,100,100),KillLines[i].Killer);
			if(KillLines[i].headshot)
			{
				if(KillLines[i].WeaponUsedKill)
				{
					GUI.DrawTexture(new Rect(170,offsetbetwin-25,-70, 70),KillLines[i].WeaponUsedKill);
				}
				GUI.DrawTexture(new Rect(185,offsetbetwin-12.5f, 40,40),headshot);
				GUI.Label(new Rect(240,offsetbetwin,100,100),KillLines[i].Killed);
			}
			else
			{
				if(KillLines[i].WeaponUsedKill)
				{
					GUI.DrawTexture(new Rect(170,offsetbetwin-25,-70, 70),KillLines[i].WeaponUsedKill);
				}
				GUI.Label(new Rect(190,offsetbetwin,100,100),KillLines[i].Killed);
			}
			offsetbetwin += 40;
		}
		offsetbetwin = 20;
	}
	void window (int id)
	{
		float x = 0.0f;
		addtolist = GUI.TextField(new Rect(10,178,420,20),addtolist,125 - AeCore.m_pCoreGame.MyStats.m_sPseudo.Length - 3);
		scrollView = GUI.BeginScrollView(new Rect(40,25,440,130),scrollView,new Rect(0,0,0,y));
		foreach(string c in listmess)
        {
			GUI.Label(new Rect(0,x,400,35),c);
			x += 30;
		}
		GUI.EndScrollView();
		if (GUI.Button(new Rect(440,178,50,20),"post")&& addtolist != "" || Event.current.keyCode == KeyCode.Return && addtolist != "")
        {
			GetComponent<NetworkView>().networkView.RPC("postmsg",RPCMode.All,AeCore.m_pCoreGame.MyStats.m_sPseudo + " : " + addtolist,30);
            addtolist = "";
		}
	}
	
	
	[RPC]
	void postmsg (string MsgToPoste,int movingv2)
	{
		listmess.Add(MsgToPoste);
		InactivityTimer = 10.0f;
		y += movingv2;
		scrollView.y = y;
	}
	public void AddKillBar (PlayerStats Killer,PlayerStats Killed,Weapon WeaponUsed, bool headshot)
	{
		KillLines.Add(new ChatKillerLine(Killer.m_sPseudo, Killed.m_sPseudo, WeaponUsed, headshot));
		
		if(KillLines.Count > 4)
		{
			KillLines.RemoveAt(0);
		}
	}
}

[System.Serializable]
public class ChatKillerLine
{
	public string Killer;
	public string Killed;
	public bool headshot;
	public Texture2D WeaponUsedKill;
	
	
	public ChatKillerLine (string kill,string killd,Weapon WeaponUsed, bool head)
	{
		Killer = kill;
		Killed = killd;
		if(WeaponUsed != null) WeaponUsedKill = WeaponUsed.m_tIconTexture;
		else WeaponUsedKill = AeTools.GetWeaponById(1).m_tIconTexture;
		headshot = head;
	}
}

