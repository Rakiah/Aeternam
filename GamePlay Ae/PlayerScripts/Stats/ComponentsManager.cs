using UnityEngine;
using System.Collections;

public class ComponentsManager : MonoBehaviour 
{
	[HideInInspector]public AePlayerMovements m_pMovements;
	[HideInInspector]public AeMouseLookX m_pMouseX;
	[HideInInspector]public AeMouseLookY m_pMouseY;
	[HideInInspector]public AeBones m_pBones;
	[HideInInspector]public AeStats m_pStats;
	[HideInInspector]public AeNetworkPlayerController m_pNetworkCaller;
	[HideInInspector]public AeAnimSynchronizer m_pAnimator;
	[HideInInspector]public AeInputController m_pInputs;
	[HideInInspector]public AeNetShoot m_pWeaponHandler;
	[HideInInspector]public AeHeadName m_pNickNameHandler;

	[HideInInspector]public AePlayerTrigger m_pTrigger;

	[HideInInspector]public AeControllerStats m_pStatsSynchronizer;

	[HideInInspector]public AeHUD m_pHud;

	[HideInInspector]public Camera NormalCam;
	[HideInInspector]public Camera GunCam;

	[HideInInspector]public AudioListener Listener;
	[HideInInspector]public CharacterController controller;

	void Awake () 
	{
		m_pMovements = GetComponent<AePlayerMovements>();
		m_pMouseX = GetComponent<AeMouseLookX>();
		m_pMouseY = GetComponentInChildren<AeMouseLookY>();

		m_pBones = GetComponent<AeBones>();
		m_pStats = GetComponent<AeStats>();
		m_pNetworkCaller = GetComponent<AeNetworkPlayerController>();
		m_pAnimator = GetComponent<AeAnimSynchronizer>();
		m_pInputs = GetComponent<AeInputController>();
		m_pWeaponHandler = GetComponentInChildren<AeNetShoot>();
		m_pNickNameHandler = GetComponentInChildren<AeHeadName>();

		m_pTrigger = GetComponent<AePlayerTrigger>();

		m_pStatsSynchronizer = GetComponentInChildren<AeControllerStats>();

		controller = GetComponent<CharacterController>();

		Component [] Cameras = gameObject.GetComponentsInChildren<Camera>();
		NormalCam =  Cameras[0].GetComponent<Camera>();
		GunCam = Cameras[1].GetComponent<Camera>();
		Listener = GetComponentInChildren<AudioListener>();

		if(networkView.isMine)
		{
			m_pMouseX.enabled = true;
			m_pMouseY.enabled = true;
			NormalCam.enabled = true;
			GunCam.enabled = true;
			Listener.enabled = true;
			m_pTrigger.enabled = true;
		}
	}
}
