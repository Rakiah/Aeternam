
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AePlayerMovements : MonoBehaviour
{
	public List<Stance.PlayerStance> AllStances = new List<Stance.PlayerStance>();
	public Stance.PlayerStance currentStance;
    
	[HideInInspector] public int iStanceID = 0;

	[HideInInspector] public ComponentsManager manager;

	private Vector3 lastPosition;
	
	public float gravity = 20.0f;
	public float RangeBlocker = 0.5f;
	
	// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	public float fallingDamageThreshold = 10.0f;
		
		
	private Vector3 moveDirection = Vector3.zero;
	[HideInInspector] public bool grounded = false;
	[HideInInspector] public bool isReallyGrounded = false;
	private float fallStartLevel;
	private bool falling;
	private float AirTimeDown = 0.0f;
	[HideInInspector] public float inputX = 0.0f;
	[HideInInspector] public float inputY = 0.0f;

	public Stance.EnumStance stance;
	

	[HideInInspector] public bool RunInput, CrouchInput, JumpInput, SlideInput, isDashing, isWRJump;

	bool canDash = true;
	bool didDoubleJump;
	bool wallRunJump = false;
	float timingDoubleJump;


	int FrameStanceOverWrited = 0;


	/*start idle zone*/
	public float idleSpeed = 0.0f;
	public float idleRecoilFactor = 0.7f;
	public float idleEnergyChanger = 5.0f;
	public int idleFieldOfView = 70;
	/*end idle zone  */

	/*start walk zone */
	public float walkSpeed = 7.0f;
	public float walkRecoilFactor = 1.0f;
	public float walkEnergyChanger = 2.0f;
	public int walkFieldOfView = 70;
	/*end walk zone */

	/*start run zone */
	public float runSpeed = 12.0f;
	public float runRecoilFactor = 1.5f;
	public float runEnergyChanger = -3.0f;
	public int runFieldOfView = 90;
	/*end run zone */

	/*start crouch zone */
	public float crouchSpeed = 4.0f;
	public float crouchRecoilFactor = 0.5f;
	public float crouchEnergyChanger = 7.0f;
	public int crouchFieldOfView = 65;
	/*end crouch zone */

	/*start slide zone */
	public float slideSpeed = 18.0f;
	public float slideRecoilFactor = 1.7f;
	public float slideEnergyChanger = -13.0f;
	public int slideFieldOfView = 100;
	/*end slide zone */

	/*start falling zone */
	public float timingConsiderReallyFall = 0.05f;
	public float fallingSpeed = 5.0f;
	public float fallingRecoilFactor = 1.2f;
	public float fallingEnergyChanger = 0.0f;
	public int fallingFieldOfView = -1;
	/*end falling zone */

	/*start wrLeft zone */
	public float wrLeftSpeed = 13.0f;
	public float wrLeftRecoilFactor = 1.3f;
	public float wrLeftEnergyChanger = -7.0f;
	public int wrLeftFieldOfView = 85;
	/*end wrLeft zone */

	/*start wrRight zone */
	public float wrRightSpeed = 13.0f;
	public float wrRightRecoilFactor = 1.3f;
	public float wrRightEnergyChanger = -7.0f;
	public int wrRightFieldOfView = 85;
	/*end wrRight zone */

	/*start jump zone */
	public float jumpSpeed = 8.0f;
	public float jumpEnergyChanger = -2.0f;
	/*end jump zone */
	/*start jump zone */
	public float doubleJumpSpeed = 10.0f;
	public float doubleJumpEnergyChanger = -5.0f;
	public float doubleJumpTimingCan = 1.0f;
	/*end doubleJump zone */

	/*start dash zone */
	public float speedDashBase = 7.0f;
	public float speedDashByFrame = 100.0f;
	public float dashDuration = 3.0f;
	public float dashEnergyCost = 20.0f;
	public float dashBetweenTiming = 1.5f;

	public GameObject trail;
	//public float
	/*end dash zone */



	void Awake()
	{
		manager = GetComponent<ComponentsManager>();

		AllStances.Add(new Stance.Idle(idleSpeed,this,Stance.EnumStance.Idle, idleRecoilFactor, idleEnergyChanger, idleFieldOfView));

		AllStances.Add(new Stance.Walk(walkSpeed,this,Stance.EnumStance.Walk,walkRecoilFactor, walkEnergyChanger, walkFieldOfView));

		AllStances.Add(new Stance.Run(runSpeed,this,Stance.EnumStance.Run, runRecoilFactor, runEnergyChanger, runFieldOfView));

		AllStances.Add(new Stance.Crouch(crouchSpeed,this,Stance.EnumStance.Crouch, crouchRecoilFactor, crouchEnergyChanger, crouchFieldOfView));

		AllStances.Add(new Stance.Slide(slideSpeed,this, Stance.EnumStance.Slide, slideRecoilFactor, slideEnergyChanger, slideFieldOfView));
		
		AllStances.Add(new Stance.Falling(fallingSpeed,this,Stance.EnumStance.Falling, fallingRecoilFactor, fallingEnergyChanger, fallingFieldOfView));

		AllStances.Add(new Stance.WallRunLeft(wrLeftSpeed,this,Stance.EnumStance.WallRunL, wrLeftRecoilFactor, wrLeftEnergyChanger, wrLeftFieldOfView));
		AllStances.Add(new Stance.WallRunRight(wrRightSpeed,this,Stance.EnumStance.WallRunR, wrRightRecoilFactor, wrRightEnergyChanger, wrRightFieldOfView));
		
		
		SwapTo(0);
	}
	
	public void Move (bool Run, bool Crouch, bool Jump, bool Slide, float Horizontal, float Vertical, List<DoubleHitClass> doublehit)
	{
		this.RunInput = Run;
		this.CrouchInput = Crouch;
		this.JumpInput = Jump;
		this.SlideInput = Slide;


		if(currentStance.m_eStance == Stance.EnumStance.Slide) inputX = 0.0f;
		else if(currentStance.m_eStance == Stance.EnumStance.WallRunL && inputX < 0.0f) inputX = 0.0f;
		else if(currentStance.m_eStance == Stance.EnumStance.WallRunR && inputX > 0.0f) inputX = 0.0f;
		else inputX = Horizontal;

		inputY = Vertical;

		if(!IsWallRunning())
		{
			if(inputX > 0.0f) 
			{
				if(AeRaycasts.IsSomethingThere(this.gameObject,Vector3.right,RangeBlocker, (2.2f/5.0f))) inputX = 0.0f;
			}
			if(inputX < 0.0f) 
			{
				if(AeRaycasts.IsSomethingThere(this.gameObject,Vector3.left,RangeBlocker, (2.2f/5.0f))) inputX = 0.0f;
			}
		}
		if(inputY > 0.0f) 
		{
			if(AeRaycasts.IsSomethingThere(this.gameObject,Vector3.forward,RangeBlocker, (2.2f/5.0f))) inputY = 0.0f;
		}
		if(inputY < 0.0f) 
		{
			if(AeRaycasts.IsSomethingThere(this.gameObject,Vector3.back,RangeBlocker, (2.2f/5.0f))) inputY = 0.0f;
		}

		if(!isDashing)
		{
			if(canDash && manager.m_pStats.Energy > dashEnergyCost)
			{
				for(int i = 0; i < doublehit.Count; i++)
				{
					if(doublehit[i].doubleHit)
					{
						StartCoroutine(DoubleHitDash(doublehit[i].direction));
					}
				}
			}
		}
		else
		{
			inputX = 0.0f;
			inputY = 0.0f;
		}

		
		FrameStanceOverWrited = 0;
		
		for(int i = 0; i < AllStances.Count; i++)
		{
			if(AllStances[i].CanSwitchState()) FrameStanceOverWrited = i;
			
			AllStances[i].Update();
		}
		
		SwapTo(FrameStanceOverWrited);
		
		DoPhysics();

		stance = currentStance.m_eStance;

		iStanceID = (int)currentStance.m_eStance;

		if(currentStance.fieldOfView != -1 && !Mathf.Approximately(manager.NormalCam.fieldOfView, currentStance.fieldOfView)) manager.NormalCam.fieldOfView = Mathf.Lerp(manager.NormalCam.fieldOfView, currentStance.fieldOfView, 2.0f * Time.deltaTime);
	}

	public void MoveNetwork ()
	{
		SwapTo(iStanceID);
		for(int i = 0; i < AllStances.Count; i++)
		{
			AllStances[i].Update();
		}
	}

	void DoPhysics ()
	{
		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f) ? 0.600f : 1.0f;
		if(grounded || IsWallRunning())
		{
			//and if he's not wall running
			if(!IsWallRunning())
			{
				moveDirection = new Vector3((inputX * inputModifyFactor) * currentStance.m_fSpeedWhile, 0, (inputY * inputModifyFactor) * currentStance.m_fSpeedWhile);
				moveDirection = this.transform.TransformDirection(moveDirection);
				if (falling) 
				{
					falling = false;
					if(this.transform.position.y < fallStartLevel - 0.10f) manager.m_pAnimator.PlayFall();
					if(this.transform.position.y < fallStartLevel - fallingDamageThreshold) FallingDamageAlert (fallStartLevel - this.transform.position.y);
				}
				if(JumpInput && currentStance.m_eStance != Stance.EnumStance.Slide) { moveDirection.y = jumpSpeed; manager.m_pStats.Energy -= jumpEnergyChanger;}

				
				AirTimeDown = 0.0F;
			}
			else
			{
				moveDirection = new Vector3(inputX * currentStance.m_fSpeedWhile, 0, currentStance.m_fSpeedWhile);

				if(currentStance.m_eStance == Stance.EnumStance.WallRunL)
				{
					if(JumpInput) StartCoroutine(WallRunJump(Vector3.right));

					if(inputX < 0.0f) moveDirection.y = jumpSpeed * 0.8f;
					
				}
				else if(currentStance.m_eStance == Stance.EnumStance.WallRunR)
				{
					if(JumpInput) StartCoroutine(WallRunJump(Vector3.left));
					
					if(inputX > 0.0f) moveDirection.y = jumpSpeed * 0.8f;
				}

				moveDirection = this.transform.TransformDirection(moveDirection);
			}

			timingDoubleJump = 0.0f;
			didDoubleJump = false;
		}
		else
		{
			if (!falling)
			{
				falling = true;
				fallStartLevel = this.transform.position.y;
			}
			else
			{
				AirTimeDown += Time.deltaTime;
				timingDoubleJump += Time.deltaTime;
			}

			if(JumpInput && !didDoubleJump && timingDoubleJump > doubleJumpTimingCan)
			{
				didDoubleJump = true;
				moveDirection.y = doubleJumpSpeed;
				manager.m_pStats.Energy -= doubleJumpEnergyChanger;
			}
			
			moveDirection.x = ((inputX * currentStance.m_fSpeedWhile) * inputModifyFactor) / 1.5f;
			moveDirection.z = ((inputY * currentStance.m_fSpeedWhile) * inputModifyFactor) / 1.5f;
			moveDirection = this.transform.TransformDirection(moveDirection);
		}


		moveDirection.y -= gravity * Time.deltaTime;

		if(wallRunJump) moveDirection.x = moveDirection.x /1.5f;

		grounded = (manager.controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

		
		if(AirTimeDown > timingConsiderReallyFall) isReallyGrounded = false;
		else isReallyGrounded = true;

	}

	IEnumerator DoubleHitDash (Vector3 direction)
	{
		isDashing = true;
		canDash = false;
	
		float time = 0.0f;
		float sp = speedDashBase;
		trail.SetActive(true);
		while (time < dashDuration)
		{
			Vector3 forward = this.transform.TransformDirection(direction);
			grounded = (manager.controller.Move(forward * sp * Time.deltaTime) & CollisionFlags.Below) != 0;

			if(time < (dashDuration/2)) sp += (speedDashByFrame * Time.deltaTime);
			else sp -= (speedDashByFrame * Time.deltaTime);
			
			time += Time.deltaTime;

			yield return null;
		}

		trail.SetActive(false);

		manager.m_pStats.Energy -= dashEnergyCost;

		isDashing = false;

		yield return new WaitForSeconds(dashBetweenTiming);
		canDash = true;
	}

	IEnumerator WallRunJump (Vector3 direction)
	{
		wallRunJump = true;
		manager.m_pStats.Energy -= jumpEnergyChanger;
		float t = 0.0f;
		while(t < 0.35f)
		{
			direction.y = 0.65f;

			Vector3 dire = this.transform.TransformDirection(direction);
			grounded = (manager.controller.Move(dire * 10.0f * Time.deltaTime) & CollisionFlags.Below) != 0;
			t += Time.deltaTime;

			yield return null;
		}

		wallRunJump = false;
	}


	void FallingDamageAlert (float fallDistance) 
	{
		if(fallDistance > 15.0f)
		{
			manager.m_pStats.CheckDamageAndHealth(-1,15,3,-1);
		}
		else if(fallDistance > 20.0f)
		{
			manager.m_pStats.CheckDamageAndHealth(-1,35,3,-1);
		}
		else if(fallDistance > 25.0f)
		{
			manager.m_pStats.CheckDamageAndHealth(-1,50,3,-1);
		}
		print ("Ouch! Fell " + fallDistance + " units!");   
	}

	void SwapTo(int i)
	{
		if(currentStance != AllStances[i])
		{
			for(int j = 0; j < AllStances.Count; j++)
			{
				if(i == j) { AllStances[j].m_bIsCurrent = true; currentStance = AllStances[j]; }
				else AllStances[j].m_bIsCurrent = false;
				
			}
		}
	}



	//raccourci functions
	public bool IsMoving ()
	{
		if(Mathf.Approximately(inputX,0.0f) && Mathf.Approximately(inputY,0.0f)) return false;
		else return true;
	}
	public bool IsWallRunning ()
	{
		if(currentStance.m_eStance == Stance.EnumStance.WallRunL || currentStance.m_eStance == Stance.EnumStance.WallRunR) return true;
		else return false;
	}
	public bool isSlidingOrRunning ()
	{
		if(currentStance.m_eStance == Stance.EnumStance.Slide || currentStance.m_eStance == Stance.EnumStance.Run) return true;
		else return false;
	}

	public Transform GetGrandChild(int underChild)
	{
		Transform tr = this.transform;
		for(int i = 0; i < underChild; i++)
		{
			tr = tr.GetChild(0);
		}

		return tr;
	}
}
