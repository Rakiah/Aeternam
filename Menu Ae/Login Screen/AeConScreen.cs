using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AeConScreen : MonoBehaviour 
{
	
	public Rect windowRect = new Rect(20, 20, 120, 50);
	public string message;
	public string m_sUsername = "";
	public string m_sPassword = "";
	public string m_sUsernameplace = "";
	public string m_sPasswordplace = "";
	public bool m_bUserorPasswordNotFound = false;
	public bool EnclenchedConnection = false;
	public Texture2D BackGround;
	public bool FirstTime = true;
	
	public void DataReceived () { AeClientServer.m_pClientServer.ConnectToMasterServer(); }

	void Start () { Application.runInBackground = true; }

	void OnGUI ()
	{
		float width = 1920f;
		float height = 1080f;
		float rx = Screen.width / width;
		float ry = Screen.height / height;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));
		GUI.DrawTexture(new Rect(0,0,1920,1080),BackGround);
		if(EnclenchedConnection && !m_bUserorPasswordNotFound) GUI.Label(new Rect(920,420,80,80),"Connecting");
		
		if(m_bUserorPasswordNotFound)
		{
			windowRect = GUI.Window(0, windowRect, CantConnectWindow,"");
			EnclenchedConnection = false;
		}
		else
		{
			if(GUI.Button(new Rect(860,450,200,50),"Connection au serveur") || Event.current.keyCode == KeyCode.Return && !EnclenchedConnection)
			{
				if(m_sUsernameplace != "" || m_sPasswordplace != "")
				{
					PlayerPrefs.SetString("Username",m_sUsernameplace);
					AeDataRequest.m_pAeDataRequest.RequestLogin(m_sUsernameplace,m_sPasswordplace);
					EnclenchedConnection = true;
				}
			}
			m_sUsernameplace = GUI.TextField(new Rect(860,350,200,30),m_sUsernameplace,12);
			m_sPasswordplace = GUI.PasswordField(new Rect(860,388,200,30),m_sPasswordplace,"*"[0],12);
			if(GUI.Button(new Rect(860,500,200,50),"Creer un compte"))
				Application.OpenURL("http://rakiah.com/aeternam/signup.php");
		}
	}

	void CantConnectWindow(int windowID) 
	{
		GUI.Label(new Rect(150,50,200,200),"Can't connect to the server : "+ message);
		if(GUI.Button(new Rect(200,250,100,25),"OK"))
		{
			m_bUserorPasswordNotFound = false;
			message = "";
		}
	}
}
