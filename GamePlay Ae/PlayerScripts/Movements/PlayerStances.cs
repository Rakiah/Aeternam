using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Stance 
{
	public enum EnumStance { Idle, Walk, Run, Crouch, Slide, WallRunL,WallRunR, Falling };

	[System.Serializable]
	public abstract class PlayerStance
	{
		public EnumStance m_eStance;
		public AePlayerMovements m_MasterClass;
		public float m_fSpeedWhile;
		public float recoilMultiplier;
		public float energyModifer;
		public int fieldOfView;
		public bool m_bIsCurrent;
		public bool wasRunning;
		
		public abstract bool CanSwitchState();
		public abstract void ResetTimer();
		public abstract void Update ();

	}

	public class Idle : PlayerStance
	{
		public Idle (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			m_eStance = StanceID;
			recoilMultiplier = recoil;
			m_fSpeedWhile = speed;

			m_MasterClass = player;
		}

		public override bool CanSwitchState()
		{
			if(m_MasterClass.isReallyGrounded && !m_MasterClass.IsMoving()) return true;
			else return false;
		}

		public override void ResetTimer(){}

		public override void Update ()
		{
			
		}
	}

	public class Walk : PlayerStance
	{
		public Walk (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			recoilMultiplier = recoil;
			m_eStance = StanceID;
			m_fSpeedWhile = speed;

			m_MasterClass = player;
		}

		public override bool CanSwitchState()
		{
			if(m_MasterClass.isReallyGrounded && m_MasterClass.IsMoving() && !m_MasterClass.isDashing) return true;
			else return false;
		}

		public override void ResetTimer(){}

		public override void Update ()
		{
			
		}
	}

	public class Run : PlayerStance
	{
		float TimerBetween;
		float TimerBetweenDecrease;

		public Run (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			recoilMultiplier = recoil;
			m_eStance = StanceID;
			m_fSpeedWhile = speed;
			TimerBetween = 3.0f;

			m_MasterClass = player;
		}

		public override void ResetTimer()
		{
			if(m_bIsCurrent) TimerBetweenDecrease = TimerBetween;
		}

		public override bool CanSwitchState()
		{
			if(m_MasterClass.isReallyGrounded && m_MasterClass.RunInput && m_MasterClass.inputY > 0.0f && !m_MasterClass.isDashing)
			{
				if(m_MasterClass.manager.m_pStats.Energy < 1.0f || TimerBetweenDecrease > 0.0f)
				{
					ResetTimer();
					return false;
				}
				else return true;
			}
			else
			{
				return false;
			}
		}

		public override void Update ()
		{
			if(TimerBetweenDecrease > 0.0f) TimerBetweenDecrease -= Time.deltaTime;
		}
	}

	public class Crouch : PlayerStance
	{
		float normalCenter,normalHeight,BasecameraHeight,SpeedToCrouch;
		float cameraHeightY,crouchHeight,crouchCenter;
		GameObject LayerCrouch;
		CharacterController controller;
		public Crouch (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV, 
		               float optSpeedCrouch = 10f, float optCameraHeight = 0.55f, float optCrouchCenter = -0.21f, float optCrouchHeight = 1.3f)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			recoilMultiplier = recoil;
			m_eStance = StanceID;
			m_fSpeedWhile = speed;
			controller = player.manager.controller;
			LayerCrouch = player.GetGrandChild(10).gameObject;
			SpeedToCrouch = optSpeedCrouch;
			cameraHeightY = optCameraHeight;
			crouchCenter = optCrouchCenter;
			crouchHeight = optCrouchHeight;

			BasecameraHeight = LayerCrouch.transform.localPosition.z;
			normalCenter = controller.center.y;
			normalHeight = controller.height;

			m_MasterClass = player;
		}
		
		public override bool CanSwitchState()
		{
			if(m_MasterClass.isReallyGrounded && m_MasterClass.CrouchInput && !m_MasterClass.isSlidingOrRunning() && !m_MasterClass.isDashing) return true;
			else return false;
		}

		public override void ResetTimer(){}
		
		public override void Update ()
		{
			if(m_bIsCurrent)
			{
				Vector3 tempcent = new Vector3(0,crouchCenter,0);
				controller.height = crouchHeight;
				controller.center = tempcent;
				LayerCrouch.transform.localPosition = Vector3.Slerp(LayerCrouch.transform.localPosition,new Vector3(0,-cameraHeightY,0),SpeedToCrouch * Time.deltaTime);
			}
			else
			{
				Vector3 tempcent = new Vector3(0,normalCenter,0);
				if(!m_MasterClass.AllStances[4].m_bIsCurrent)
				{
					controller.height = normalHeight;
					controller.center = tempcent;
				}
				LayerCrouch.transform.localPosition = Vector3.Slerp(LayerCrouch.transform.localPosition,new Vector3(0,-BasecameraHeight,0),SpeedToCrouch * Time.deltaTime);
			}
		}
	}

	public class Slide : PlayerStance
	{
		float TimerBetween;
		float TimerBetweenDecrease;

		float TimeSlide;
		float TimeSlideDecrease;

		float baseheightcontroller,baseYcenterController,basecamerayheight,heightController,Ycenter,camerayheight;
		GameObject LayerX,LayerY;

		Quaternion AngleSlideX, AngleSlideY, AngleDefault;

		CharacterController controller;
		public Slide (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			recoilMultiplier = recoil;
			m_eStance = StanceID;
			m_fSpeedWhile = speed;
			TimerBetween = 2.0f;
			TimeSlide = 1.5f;
			TimeSlideDecrease = TimeSlide;

			LayerX = player.GetGrandChild(7).gameObject;
			LayerY = player.GetGrandChild(8).gameObject;

			controller = player.manager.controller;

			m_MasterClass = player;

			baseheightcontroller = controller.height;
			baseYcenterController = controller.center.y;
			basecamerayheight = LayerX.transform.localPosition.y;
			AngleDefault = LayerY.transform.localRotation;
			AngleSlideX = new Quaternion(-0.1f,0f,0f,1f);
			AngleSlideY = new Quaternion(0f,0f,0.3f,1f);
			Ycenter = -0.52f;
			camerayheight = -1.15f;
			heightController = 0f;

		}

		public override bool CanSwitchState()
		{
			if(m_MasterClass.isReallyGrounded && m_MasterClass.SlideInput && m_MasterClass.isSlidingOrRunning() && !m_MasterClass.isDashing)
			{
				if(m_MasterClass.manager.m_pStats.Energy < 1.0f || TimeSlideDecrease <= 0.0f || TimerBetweenDecrease > 0.0f || 
				   AeRaycasts.IsSomethingThere(m_MasterClass.gameObject,Vector3.forward,3.0f, (2.2f/5.0f)))
				{
					ResetTimer();
					return false;
				}
				return true;
				
			}

			ResetTimer();
			return false;
		}

		public override void ResetTimer ()
		{
			if(m_bIsCurrent) TimerBetweenDecrease = TimerBetween;
			TimeSlideDecrease = TimeSlide;
		}

		public override void Update ()
		{
			if(m_bIsCurrent)
			{
				TimeSlideDecrease -= Time.deltaTime;

				controller.height = heightController;
				controller.center = new Vector3(0,Ycenter,0);
				
				
				LayerX.transform.localPosition = Vector3.Slerp(LayerX.transform.localPosition,new Vector3(0,camerayheight,0),4.0f * Time.deltaTime);
				LayerX.transform.localRotation = Quaternion.Slerp(LayerX.transform.localRotation,AngleSlideX,4.0F * Time.deltaTime);
				LayerY.transform.localRotation = Quaternion.Slerp(LayerY.transform.localRotation,AngleSlideY,4.0f * Time.deltaTime);
			}
			else
			{
				TimerBetweenDecrease -= Time.deltaTime;

				controller.height = baseheightcontroller;
				controller.center = new Vector3(0,baseYcenterController,0);
				
				LayerX.transform.localPosition = Vector3.Slerp(LayerX.transform.localPosition,new Vector3(0,basecamerayheight,0),4.0f * Time.deltaTime);
				LayerX.transform.localRotation = Quaternion.Slerp(LayerX.transform.localRotation,AngleDefault,4.0F * Time.deltaTime);
				LayerY.transform.localRotation = Quaternion.Slerp(LayerY.transform.localRotation,AngleDefault,4.0f * Time.deltaTime);
			}
		}
	}

	public class WallRunLeft : PlayerStance
	{
		GameObject LayerZ,LayerY;

		float RotationCamAmountZ,RotationCamAmountY;

		public WallRunLeft (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			recoilMultiplier = recoil;
			m_eStance = StanceID;
			m_fSpeedWhile = speed;

			RotationCamAmountZ = 25;
			RotationCamAmountY = 40;

			LayerZ = player.GetGrandChild(2).gameObject;
			LayerY = player.GetGrandChild(4).gameObject;

			m_MasterClass = player;
		}

		public override bool CanSwitchState()
		{
			if(!m_MasterClass.isDashing)
			{
				if((m_MasterClass.AllStances[5].m_bIsCurrent && m_MasterClass.AllStances[5].wasRunning)  
				   || (m_MasterClass.IsWallRunning() && m_MasterClass.RunInput && m_MasterClass.inputY > 0.0f))
				{
					if(m_MasterClass.manager.m_pStats.Energy > 1 && AeRaycasts.IsSomethingThere(m_MasterClass.gameObject,Vector3.left,1.0f, (2.2f/5.0f)))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			else return false;
		}

		public override void ResetTimer(){}

		public override void Update ()
		{
			if(m_bIsCurrent)
			{
				m_MasterClass.gravity = 0;
				LayerZ.transform.localEulerAngles = Vector3.Slerp(LayerZ.transform.localEulerAngles,new Vector3(0,0,90 - RotationCamAmountZ), 4.0f * Time.deltaTime);
				LayerY.transform.localEulerAngles = Vector3.Slerp(LayerY.transform.localEulerAngles,new Vector3(0,90 + RotationCamAmountY,0), 4.0f * Time.deltaTime);
			}
		}
	}

	public class WallRunRight : PlayerStance
	{
		GameObject LayerZ,LayerY;

		float RotationCamAmountZ,RotationCamAmountY;

		public WallRunRight (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			recoilMultiplier = recoil;
			m_eStance = StanceID;
			m_fSpeedWhile = speed;

			RotationCamAmountZ = 25;
			RotationCamAmountY = 40;

			LayerZ = player.GetGrandChild(2).gameObject;
			LayerY = player.GetGrandChild(4).gameObject;

			m_MasterClass = player;
		}

		public override bool CanSwitchState()
		{
			if(!m_MasterClass.isDashing)
			{
				if((m_MasterClass.AllStances[5].m_bIsCurrent && m_MasterClass.AllStances[5].wasRunning)  || (m_MasterClass.IsWallRunning() && m_MasterClass.RunInput && m_MasterClass.inputY > 0.0f))
				{
					if(m_MasterClass.manager.m_pStats.Energy > 1 && AeRaycasts.IsSomethingThere(m_MasterClass.gameObject,Vector3.right,1.0f, (2.2f/5.0f)))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			else return false;
		}

		public override void ResetTimer(){}

		public override void Update ()
		{
			if(m_bIsCurrent)
			{
				m_MasterClass.gravity = 0;
				LayerZ.transform.localEulerAngles = Vector3.Slerp(LayerZ.transform.localEulerAngles,new Vector3(0,0,90 + RotationCamAmountZ), 4.0f * Time.deltaTime);
				LayerY.transform.localEulerAngles = Vector3.Slerp(LayerY.transform.localEulerAngles,new Vector3(0,90 - RotationCamAmountY,0), 4.0f * Time.deltaTime);
				
			}
			else if(!m_MasterClass.IsWallRunning())
			{
				m_MasterClass.gravity = 27;
				LayerZ.transform.localEulerAngles = Vector3.Slerp(LayerZ.transform.localEulerAngles,new Vector3(LayerZ.transform.localEulerAngles.x,LayerZ.transform.localEulerAngles.x,90), 4.0f * Time.deltaTime);
				LayerY.transform.localEulerAngles = Vector3.Slerp(LayerY.transform.localEulerAngles,new Vector3(LayerY.transform.localEulerAngles.x,90,LayerY.transform.localEulerAngles.z), 4.0f * Time.deltaTime);
			}
		}
	}

	public class Falling : PlayerStance
	{

		public Falling (float speed, AePlayerMovements player, EnumStance StanceID, float recoil, float EnergyChanger, int FoV = -1)
		{
			fieldOfView = FoV;
			energyModifer = EnergyChanger;
			recoilMultiplier = recoil;
			m_eStance = StanceID;
			m_fSpeedWhile = speed;

			m_MasterClass = player;
		}
		
		public override bool CanSwitchState()
		{
			if(!m_MasterClass.isReallyGrounded && !m_MasterClass.IsWallRunning() && !m_MasterClass.isDashing)
			{
				if(m_MasterClass.AllStances[2].m_bIsCurrent) wasRunning = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void ResetTimer(){}
		
		public override void Update ()
		{
			if(m_bIsCurrent)
			{
				if(wasRunning)
				{
					m_fSpeedWhile = m_MasterClass.runSpeed;
					if(!m_MasterClass.RunInput || m_MasterClass.manager.m_pStats.Energy < 1.0f || m_MasterClass.inputY <= 0.0f) wasRunning = false;
				}
				else
				{
					m_fSpeedWhile = m_MasterClass.fallingSpeed;
				}
			}
			if(m_MasterClass.isReallyGrounded) wasRunning = false;
		}
	}
}
