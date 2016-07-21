using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AePlayerMovements))]
public class PlayerMovementsEditor : Editor 
{
	bool idleOpen;
	bool walkOpen;
	bool runOpen;
	bool crouchOpen;
	bool slideOpen;
	bool fallOpen;
	bool wallRunLeftOpen;
	bool wallRunRightOpen;


	bool dashOpen;
	bool jumpOpen;
	bool doubleJumpOpen;


	bool otherSettings;

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Base Player Movements", EditorStyles.whiteLargeLabel);

		AePlayerMovements S = (AePlayerMovements)target;

		EditorGUI.indentLevel = 0;

		idleOpen = EditorGUILayout.Foldout (idleOpen, "Idle Stance");

		if(idleOpen)
		{
			EditorGUI.indentLevel = 1;

			S.idleRecoilFactor = EditorGUILayout.FloatField("Recoil",S.idleRecoilFactor);
			S.idleEnergyChanger = EditorGUILayout.FloatField("Energy", S.idleEnergyChanger);
			S.idleFieldOfView = EditorGUILayout.IntField("FoV", S.idleFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		walkOpen = EditorGUILayout.Foldout (walkOpen, "Walk Stance");
		
		if(walkOpen)
		{
			EditorGUI.indentLevel = 1;

			S.walkSpeed = EditorGUILayout.FloatField("Speed",S.walkSpeed);
			S.walkRecoilFactor = EditorGUILayout.FloatField("Recoil",S.walkRecoilFactor);
			S.walkEnergyChanger = EditorGUILayout.FloatField("Energy", S.walkEnergyChanger);
			S.walkFieldOfView = EditorGUILayout.IntField("FoV", S.walkFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		runOpen = EditorGUILayout.Foldout (runOpen, "Run Stance");
		
		if(runOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.runSpeed = EditorGUILayout.FloatField("Speed",S.runSpeed);
			S.runRecoilFactor = EditorGUILayout.FloatField("Recoil",S.runRecoilFactor);
			S.runEnergyChanger = EditorGUILayout.FloatField("Energy", S.runEnergyChanger);
			S.runFieldOfView = EditorGUILayout.IntField("FoV", S.runFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		crouchOpen = EditorGUILayout.Foldout (crouchOpen, "Crouch Stance");
		
		if(crouchOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.crouchSpeed = EditorGUILayout.FloatField("Speed",S.crouchSpeed);
			S.crouchRecoilFactor = EditorGUILayout.FloatField("Recoil",S.crouchRecoilFactor);
			S.crouchEnergyChanger = EditorGUILayout.FloatField("Energy", S.crouchEnergyChanger);
			S.crouchFieldOfView = EditorGUILayout.IntField("FoV", S.crouchFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		slideOpen = EditorGUILayout.Foldout (slideOpen, "Slide Stance");
		
		if(slideOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.slideSpeed = EditorGUILayout.FloatField("Speed",S.slideSpeed);
			S.slideRecoilFactor = EditorGUILayout.FloatField("Recoil",S.slideRecoilFactor);
			S.slideEnergyChanger = EditorGUILayout.FloatField("Energy", S.slideEnergyChanger);
			S.slideFieldOfView = EditorGUILayout.IntField("FoV", S.slideFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		fallOpen = EditorGUILayout.Foldout (fallOpen, "Falling Stance");
		
		if(fallOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.fallingSpeed = EditorGUILayout.FloatField("Speed",S.fallingSpeed);
			S.fallingRecoilFactor = EditorGUILayout.FloatField("Recoil",S.fallingRecoilFactor);
			S.fallingEnergyChanger = EditorGUILayout.FloatField("Energy", S.fallingEnergyChanger);
			S.fallingFieldOfView = EditorGUILayout.IntField("FoV", S.fallingFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		wallRunLeftOpen = EditorGUILayout.Foldout (wallRunLeftOpen, "Wall Run Left Stance");
		
		if(wallRunLeftOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.wrLeftSpeed = EditorGUILayout.FloatField("Speed",S.wrLeftSpeed);
			S.wrLeftRecoilFactor = EditorGUILayout.FloatField("Recoil",S.wrLeftRecoilFactor);
			S.wrLeftEnergyChanger = EditorGUILayout.FloatField("Energy", S.wrLeftEnergyChanger);
			S.wrLeftFieldOfView = EditorGUILayout.IntField("FoV", S.wrLeftFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		wallRunRightOpen = EditorGUILayout.Foldout (wallRunRightOpen, "Wall Run Right Stance");
		
		if(wallRunRightOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.wrRightSpeed = EditorGUILayout.FloatField("Speed",S.wrRightSpeed);
			S.wrRightRecoilFactor = EditorGUILayout.FloatField("Recoil",S.wrRightRecoilFactor);
			S.wrRightEnergyChanger = EditorGUILayout.FloatField("Energy", S.wrRightEnergyChanger);
			S.wrRightFieldOfView = EditorGUILayout.IntField("FoV", S.wrRightFieldOfView);
		}

		EditorGUI.indentLevel = 0;
		jumpOpen = EditorGUILayout.Foldout (jumpOpen, "Jump Stance");
		
		if(jumpOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.jumpSpeed = EditorGUILayout.FloatField("Height",S.jumpSpeed);
			S.jumpEnergyChanger = EditorGUILayout.FloatField("Energy",S.jumpEnergyChanger);
		}

		EditorGUI.indentLevel = 0;
		doubleJumpOpen = EditorGUILayout.Foldout (doubleJumpOpen, "Double Jump Stance");

		if(doubleJumpOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.doubleJumpSpeed = EditorGUILayout.FloatField("Height",S.doubleJumpSpeed);
			S.doubleJumpEnergyChanger = EditorGUILayout.FloatField("Energy",S.doubleJumpEnergyChanger);
			S.doubleJumpTimingCan = EditorGUILayout.FloatField("Timing in air",S.doubleJumpTimingCan);
		}

		EditorGUI.indentLevel = 0;
		dashOpen = EditorGUILayout.Foldout (dashOpen, "Dash Stance");
		
		if(dashOpen)
		{
			EditorGUI.indentLevel = 1;
			
			S.speedDashBase = EditorGUILayout.FloatField("base speed",S.speedDashBase);
			S.speedDashByFrame = EditorGUILayout.FloatField("speed by frame",S.speedDashByFrame);
			S.dashDuration = EditorGUILayout.FloatField("duration",S.dashDuration);
			S.dashEnergyCost = EditorGUILayout.FloatField("Energy",S.dashEnergyCost);
			S.dashBetweenTiming = EditorGUILayout.FloatField("Timing between dashes",S.dashBetweenTiming);

			S.trail = (GameObject) EditorGUILayout.ObjectField(S.trail,typeof(Object),true);
		}

		EditorGUI.indentLevel = 0;
		otherSettings = EditorGUILayout.Foldout (otherSettings, "Others settings");

		if(otherSettings)
		{
			S.gravity = EditorGUILayout.FloatField("gravity",S.gravity);
			S.RangeBlocker = EditorGUILayout.FloatField("Range Block",S.RangeBlocker);
		}

		EditorUtility.SetDirty(target);
	}
}
