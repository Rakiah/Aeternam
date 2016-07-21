using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeInputController : MonoBehaviour 
{
	ComponentsManager manager;
	bool RunInput, CrouchInput, JumpInput, SlideInput, ShootInput, MagicShotInput, ReloadInput, Action3Input, PrimaryInput, SecondaryInput;

	List<DoubleHitClass> dashDoubleHits = new List<DoubleHitClass>();

	float HorizontalInput,VerticalInput,MouseX,MouseY,ScrollWheel;

	public bool Paused;

	void Awake () 
	{
		manager = GetComponent<ComponentsManager>();
		for(int i = 0; i < 4; i++)
		{
			dashDoubleHits.Add(new DoubleHitClass());
		}
	}

	void Start ()
	{
		if(networkView.isMine)
		{
			networkView.RPC("ShowPseudoHead",RPCMode.All,AeCore.m_pCoreGame.MyStats.m_sPseudo);

			manager.m_pBones.Initialize();
		}
	}

	void Update () 
	{
		if(!AeCore.m_pCoreGame.MyStats.m_bDied)
		{
			if(Paused || !networkView.isMine) SetZeroInput();
			else GetInputs ();


			if(networkView.isMine)
			{
				
				manager.m_pMouseX.Move (MouseX);
				manager.m_pMouseY.Move (MouseY);
				
				manager.m_pMovements.Move (RunInput, CrouchInput, JumpInput, SlideInput,HorizontalInput, VerticalInput, dashDoubleHits);

				manager.m_pWeaponHandler.SwitchWeaponController(PrimaryInput, SecondaryInput, ScrollWheel);
				manager.m_pWeaponHandler.ActionsController(ShootInput, ReloadInput, Action3Input, MagicShotInput);
			}

			else
			{
				SetZeroInput();

				manager.m_pMovements.MoveNetwork();
			}
			manager.m_pWeaponHandler.RealisticMovements(MouseX,MouseY,HorizontalInput,VerticalInput,JumpInput);
		}
	}


	void GetInputs ()
	{
		manager.m_pNickNameHandler.CheckHeadInput();

		Screen.lockCursor = true;
		Screen.showCursor = false;
		SetDoubleHit();
		CrouchInput = Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Crouch);
		RunInput    = Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Run);
		JumpInput   = Input.GetKeyDown(AeProfils.m_pAeProfils.CurrentProfil.control.Jump);
		SlideInput  = Input.GetKey(AeProfils.m_pAeProfils.CurrentProfil.control.Slide);

		dashDoubleHits[0].direction = Vector3.forward;
		dashDoubleHits[1].direction = Vector3.back;
		dashDoubleHits[2].direction = Vector3.left;
		dashDoubleHits[3].direction = Vector3.right;
		
		if(AeProfils.m_pAeProfils.CurrentProfil.control.QwertyKeyboard)
		{
			dashDoubleHits[0].keycode = KeyCode.W;
			dashDoubleHits[2].keycode = KeyCode.A;
			dashDoubleHits[0].singleHit = Input.GetKeyDown(KeyCode.W);
			dashDoubleHits[2].singleHit = Input.GetKeyDown(KeyCode.A);
			HorizontalInput = Input.GetAxis("HorizontalQwerty");
			VerticalInput   = Input.GetAxis("VerticalQwerty");
		}
		else 
		{
			dashDoubleHits[0].keycode = KeyCode.Z;
			dashDoubleHits[2].keycode = KeyCode.Q;
			dashDoubleHits[0].singleHit = Input.GetKeyDown(KeyCode.Z);
			dashDoubleHits[2].singleHit = Input.GetKeyDown(KeyCode.Q);
			HorizontalInput = Input.GetAxis("Horizontal");
			VerticalInput   = Input.GetAxis("Vertical");
		}
		dashDoubleHits[1].keycode = KeyCode.S;
		dashDoubleHits[3].keycode = KeyCode.D;
		dashDoubleHits[1].singleHit = Input.GetKeyDown(KeyCode.S);
		dashDoubleHits[3].singleHit = Input.GetKeyDown(KeyCode.D);

		
		MouseX = Input.GetAxis("Mouse X") * AeProfils.m_pAeProfils.CurrentProfil.control.Sensitivity;
		MouseY = Input.GetAxis("Mouse Y") * AeProfils.m_pAeProfils.CurrentProfil.control.Sensitivity;

		if(manager.m_pWeaponHandler.WeaponInventory.Count > 0)
		{
			ShootInput = manager.m_pWeaponHandler.GetCurrentWeapon().CanAction1();
			ReloadInput = manager.m_pWeaponHandler.GetCurrentWeapon().CanAction2();
			Action3Input = manager.m_pWeaponHandler.GetCurrentWeapon().StatusAction3();
		}

		if(manager.m_pWeaponHandler.gauntlet != null) MagicShotInput = manager.m_pWeaponHandler.gauntlet.CanAction();

		
		ScrollWheel = Input.GetAxisRaw("Mouse ScrollWheel");

		PrimaryInput = Input.GetKeyDown(AeProfils.m_pAeProfils.CurrentProfil.control.PickRifle);
		SecondaryInput = Input.GetKeyDown(AeProfils.m_pAeProfils.CurrentProfil.control.PickSword);
	}

	void SetZeroInput ()
	{
		CrouchInput = false;
		RunInput    = false;
		JumpInput   = false;
		SlideInput  = false;
		

		HorizontalInput = 0.0f;
		VerticalInput = 0.0f;

		
		MouseX = 0.0f;
		MouseY = 0.0f;
		
		ShootInput = false;
		MagicShotInput = false;
		ReloadInput = false;
		
		ScrollWheel = 0.0f;
	}

	void SetDoubleHit ()
	{
		for(int i = 0; i < dashDoubleHits.Count; i++)
		{
			if(dashDoubleHits[i].timing <= 0.0f) dashDoubleHits[i].doubleHit = false;
			if(Input.GetKeyDown(dashDoubleHits[i].keycode) && dashDoubleHits[i].timing > 0.0f)
			{
				dashDoubleHits[i].doubleHit = true;
				dashDoubleHits[i].timing = 0.0f;
			}

			if(dashDoubleHits[i].singleHit) dashDoubleHits[i].timing = 0.15f;
		
			else dashDoubleHits[i].timing -= Time.deltaTime;
		}
	}
}

public class DoubleHitClass
{
	public KeyCode keycode { get; set; }
	public Vector3 direction;
	public bool singleHit { get; set; }
	public bool doubleHit { get; set; }
	public float timing { get; set; }
}
