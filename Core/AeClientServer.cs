using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

public class AeClientServer : MonoBehaviour 
{
	
	public static AeClientServer m_pClientServer;
	
	[HideInInspector] public AeConScreen ConnectionScreen;
	[HideInInspector] public AeMainMenu MainMenu;

	
	public Socket m_clientSocket;
	
	public bool NoMasterServer = false;
	public bool ConnectedToMasterServer = false;
	bool Loaded = false;
	
	
	void Start () 
	{
		m_pClientServer = this;
		ConnectionScreen = GameObject.Find("ConnectionScreen").GetComponent<AeConScreen>();
	}
	
	void Update () 
	{
		if(ConnectedToMasterServer && !Loaded)
		{
			AeGameLoader.m_pAeGameLoader.LoadAGame(2);
			Loaded = true;
		}
		
	}
	
	void WaitForData()
	{
		try
		{

			StateObject theSocPkt = new StateObject();
			theSocPkt.workSocket = m_clientSocket;
			m_clientSocket.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length,SocketFlags.None,new AsyncCallback(OnDataReceived),theSocPkt);
		}
		catch(SocketException se)
		{
			Debug.Log(se.Message);
		}		
	}

	void OnDataReceived(IAsyncResult AR)
	{

		try
		{
			StateObject SO = (StateObject) AR.AsyncState;
			Socket Client = SO.workSocket;
			
			int bytesRead = Client.EndReceive(AR);
			Debug.LogWarning("Message Incoming, unparse it and read it");
			if (bytesRead > 0) 
			{
				// store bytes
				SO.sb.Append(Encoding.UTF8.GetString(SO.dataBuffer,0,bytesRead));
			}
			if(bytesRead == SO.dataBuffer.Length)
			{
				//if its as big as our buffer size get some more
				Client.BeginReceive(SO.dataBuffer, 0, SO.dataBuffer.Length, 0, new AsyncCallback(OnDataReceived), SO);
			}
			else
			{
				Debug.LogWarning("Message received : " + SO.sb.ToString());
				SO.sb = new StringBuilder();
				WaitForData();
			}
		}
		catch (ObjectDisposedException)
		{
			Debug.Log("\nOnDataReceived: Socket has been closed\n");
		}
		catch (SocketException se)
		{
			Debug.Log(se.Message);
		}
	}
	
	void OnConnectReceived(IAsyncResult ar)
	{
		try
		{
			Socket client = (Socket) ar.AsyncState;
			client.EndConnect(ar);
			if (client.Connected)
			{
				ConnectedToMasterServer = true;
				ConnectionScreen.EnclenchedConnection = false;
				WaitForData();
				string converter = AeCore.m_pCoreGame.MyStats.m_sPseudo;
				byte [] Pseudo = Encoding.UTF8.GetBytes(converter.ToString());
				client.BeginSend(Pseudo, 0, Pseudo.Length, SocketFlags.None, new AsyncCallback(OnSendCallback),client);
			}
		}
		catch (SocketException se)
		{
			string str;
			str = "\nConnection failed, server isnt running or you dont have any internet connection ?\n" + se.Message;
			
			ConnectionScreen.m_bUserorPasswordNotFound = true;
			ConnectionScreen.message = str;
		}
	}

	void OnSendCallback (IAsyncResult AR)
	{
		try
		{	
			Socket soc = (Socket)AR.AsyncState;
			
			int send = soc.EndSend(AR);
			
			Debug.Log("Pseudo sent to the server as bytes :" + send.ToString());
		}
		catch(ArgumentNullException ANE)
		{
			Debug.Log(ANE.Message);
		}
		catch(InvalidOperationException IO)
		{
			Debug.Log(IO.Message);
		}
	}
	public void ConnectToMasterServer()
	{
		if(NoMasterServer)
		{
			ConnectedToMasterServer = true;
			ConnectionScreen.EnclenchedConnection = false;
		}
		else
		{
			
			try
			{
				Socket SockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				m_clientSocket = SockClient;
				
				// Cet the remote IP address
				IPAddress ip = IPAddress.Parse("127.0.0.1");
				int iPortNo = 5679;
				// Create the end point 
				IPEndPoint ipEnd = new IPEndPoint(ip, iPortNo);
				// Connect to the remote host
				m_clientSocket.BeginConnect(ipEnd,new AsyncCallback(OnConnectReceived),m_clientSocket);
			}
			catch (SocketException se)
			{
				string str;
				str = "\nConnection failed, server isnt running or you dont have any internet connection ?\n" + se.Message;
				
				ConnectionScreen.m_bUserorPasswordNotFound = true;
				ConnectionScreen.message = str;
			}
		}
	}
	public void DisconnectToMasterServer (bool Quit)
	{
		try
		{
			m_clientSocket.Shutdown(SocketShutdown.Both);
			m_clientSocket.Disconnect(true);
		}
		catch(SocketException se)
		{
			Debug.Log(se.ToString());
		}
	}

	void OnApplicationQuit ()
	{
		if(!NoMasterServer)
		DisconnectToMasterServer(true);
	}
}

public class StateObject 
{
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 512;
	public int BufferS  = 512;
	// Receive buffer.
	public byte[] dataBuffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
}


