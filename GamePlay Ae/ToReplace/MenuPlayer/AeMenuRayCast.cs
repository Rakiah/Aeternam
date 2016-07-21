using UnityEngine;
using System.Collections;

public class AeMenuRayCast : MonoBehaviour	
{
	public RaycastHit hit;
	public AeMainMenu.GuiState GUIState;
	public Vector3 CameraLastPosition;
	public Quaternion CameraLastRotation;
	public Vector3 CurrentCameraPosition;
	public Quaternion CurrentCameraRotation;
	public Vector3 CameraNewPosition;
	public Quaternion CameraNewRotation;
	public bool IsOnMenu = false;

	public Texture2D RoundCross;
	bool Contact = false;
	public AeMainMenu MainMenu;

	public float speed = 3.0F;


	public bool LerpOverTheNewCamera = false;

	void Start () 
	{
		MainMenu = GameObject.Find("MainMenu").GetComponent<AeMainMenu>();
		Screen.lockCursor = true;
		Screen.showCursor = false;
	}

	void Update () 
	{
		if(LerpOverTheNewCamera)
		{
			CurrentCameraPosition = this.transform.position;
			CurrentCameraRotation = this.transform.rotation;

			transform.position = Vector3.Slerp(CurrentCameraPosition,CameraNewPosition,speed * Time.deltaTime);
			transform.rotation = Quaternion.Slerp(CurrentCameraRotation,CameraNewRotation,speed * Time.deltaTime);

			float dist = Vector3.Distance(CurrentCameraPosition, CameraNewPosition);
			if(dist < 0.01 && !IsOnMenu)
			{
				LerpOverTheNewCamera = false;
				IsOnMenu = true;
				MainMenu.EnableButtons();
			}
			else if(dist < 0.01 && IsOnMenu)
			{
				EnableInGamePlayer();
				LerpOverTheNewCamera = false;
				IsOnMenu = false;
			}
		}

		Vector3 forward = this.transform.TransformDirection(Vector3.forward);
		int layer1  = 2;
		int layer2  = 8;
		int layerMask = ~((1 << layer1) | (1 << layer2)); // NOT ~(1 << layer1) | ~(1 << layer2)
		if(Physics.Raycast(this.transform.position,forward,out hit,5.0f,layerMask))
		{
			switch(hit.collider.gameObject.tag)
			{
				case "OptionsScreen" :
				if(!LerpOverTheNewCamera && !IsOnMenu)
					{
						Contact = true;
					}
				break;
				case "ProfilScreen" :
				if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = true;
					}
				break;
				case "ArmurerieScreen" :
				if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = true;
					}
				break;
				case "DisconnectScreen" :
				if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = true;
					}
				break;
				case "Normal games" :
				if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = true;
					}
				break;
				case "Private games" :
				if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = true;
					}
				break;
				case "Ranked games" :
				if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = true;
					}
				break;
				case "Host games" :
				if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = true;
					}
				break;
				default :
					Contact = false;
				break;
			}
		}

		if(Input.GetMouseButtonDown(0) && Contact && !LerpOverTheNewCamera && !IsOnMenu)
		{
			if(Physics.Raycast(this.transform.position,forward,out hit,5.0f,layerMask))
			{
				switch(hit.collider.gameObject.tag)
				{
					case "OptionsScreen" :

						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.OptionsMenu;
						break;
					case "ProfilScreen" :
					if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.ProfilMenu;
					}
					break;
					case "ArmurerieScreen" :
					if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.ShopMenu;
					}
					break;
					case "DisconnectScreen" :
					if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.DisconnectMenu;
					}
					break;
					case "Normal games" :
					if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.NormalGames;
					}
					break;
					case "Private games" :
					if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.PrivateGames;

					}
					break;
					case "Ranked games" :
					if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.RankedGames;
					}
					break;
					case "Host games" :
					if(!LerpOverTheNewCamera&& !IsOnMenu)
					{
						Contact = false;
						DisableInGamePlayer();
						SmoothCameraMove(0,hit.collider.gameObject);
						MainMenu.MyMenuState = AeMainMenu.GuiState.HostGames;
					}
					break;
					default :
					Contact = false;
					break;
				}
			}
		}
		if(Input.GetKeyDown(KeyCode.Escape) && !LerpOverTheNewCamera && IsOnMenu)
		{
			SmoothCameraMove(1,null);
			if(AeCore.m_pCoreGame.MyStats.Character[0].m_gInstantiatedPrefab)
			{
				AeCore.m_pCoreGame.MyStats.Character[0].m_gInstantiatedPrefab.SetActive(false);
			}
			AeCore.m_pCoreGame.GetComponent<AeChat>().isOnMenu = false;
			MainMenu.GetBackToPlayMode();
		}
	}


	void DisableInGamePlayer ()
	{
		transform.root.GetComponent<AeMenuPlayerMovement>().enabled  = false;
		transform.root.GetComponent<AeMenuMouseLookX>().enabled = false;
		GetComponent<AeMenuMouseLookY>().enabled = false;
		transform.root.GetComponent<AeMenuFootSteps>().enabled = false;
		Screen.lockCursor = false;
		Screen.showCursor = true;
	}
	void EnableInGamePlayer ()
	{
		transform.root.GetComponent<AeMenuPlayerMovement>().enabled  = true;
		transform.root.GetComponent<AeMenuMouseLookX>().enabled = true;
		GetComponent<AeMenuMouseLookY>().enabled = true;
		transform.root.GetComponent<AeMenuFootSteps>().enabled = true;
		Screen.lockCursor = true;
		Screen.showCursor = false;
	}
	void SmoothCameraMove (int In,GameObject NewPosition)
	{
		if(In == 0)
		{

			CameraLastPosition = this.transform.position;
			CameraLastRotation = this.transform.rotation;

			CameraNewPosition = NewPosition.transform.FindChild("Camera").position;
			CameraNewRotation = NewPosition.transform.FindChild("Camera").rotation;


			LerpOverTheNewCamera = true;
		}
		else
		{
			CameraNewPosition = CameraLastPosition;
			CameraNewRotation = CameraLastRotation;


			LerpOverTheNewCamera = true;
		}
	}

	void OnGUI ()
	{
		float width = 1920f;
		float height = 1080f;
		float rx = Screen.width / width;
		float ry = Screen.height / height;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));
		if(Contact && !LerpOverTheNewCamera && !IsOnMenu)
		{
			if(hit.collider)
			{
				GUI.Label(new Rect(960,200,100,50),"Click to enter the "+ hit.collider.gameObject.name);
			}
		}
		if(!IsOnMenu)
		{
			GUI.DrawTexture(new Rect(955,535,10,10),RoundCross);
		}
	}
}
