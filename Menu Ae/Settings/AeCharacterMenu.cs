using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AeCharacterMenu : MonoBehaviour 
{
	public List<GroupButton> ButtonGroup = new List<GroupButton>();
	
	public List<Button> VideoButtons = new List<Button>();
	
	public List<Button> ControlsButtons = new List<Button>();
	
	public List<Button> SoundsButtons = new List<Button>();

	public State myState = State.Zero;

	public AudioSource AudioComp;
	public AudioClip HighLightSound;
	public AudioClip PressedSound;

	public List<int> ModifiedPoints = new List<int>();

	public List<Button> AddButtons = new List<Button>();
	public List<Button> DeleteButtons = new List<Button>();
	public List<Text> PointText = new List<Text>();
	public Text PointLeftTxt;

	public int PointLeft;

	public Button ApplyCarButton;
	public Button ResetCarButton;

	public Text Kills;
	public Text Deaths;
	public Text Assists;
	public Text Win;
	public Text Loose;
	public Text RatioKD;
	public Text RatioVD;
	public Text Credits;

	public GameObject SlotParent;
	public GameObject Slot;
	
	void Awake () 
	{
		foreach(GroupButton button in ButtonGroup)
		{
			if(myState != button.StateButton)
			{
				button.Container.GetComponent<Animator>().SetTrigger("Disable");
			}
		}
		AudioComp = GetComponent<AudioSource>();
	}
	void Update ()
	{
		AudioComp.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeMenuEffects;
	}

	void Start ()
	{
		CheckEveryPoints();
		SetStatistics();
	}

	void SetStatistics()
	{
		PlayerStats stats = AeCore.m_pCoreGame.MyStats;

		Kills.text = stats.m_iNbKills.ToString();
		Deaths.text = stats.m_iNbDeaths.ToString();
		Assists.text = stats.m_iNbAssists.ToString();
		Win.text = stats.m_iNbVictory.ToString();
		Loose.text = stats.m_iNbDefeat.ToString();
		Credits.text = stats.m_iMonneyRecolted.ToString();
		if (stats.m_iNbDeaths == 0) RatioKD.text = "Perfect";
		else RatioKD.text = string.Format("{0:N2}%", (float)stats.m_iNbKills / (float)stats.m_iNbDeaths);
		if (stats.m_iNbDefeat == 0) RatioVD.text = "Perfect";
		else RatioVD.text = string.Format("{0:N2}%", (float)stats.m_iNbVictory / (float)stats.m_iNbDefeat);
	}
	public void MoveToGroup (int StateID)
	{
		RandomizeButtonsAnimations ();
		foreach(GroupButton button in ButtonGroup)
		{
			if(button.StateButton == myState)
			{
				button.Container.GetComponent<Animator>().SetTrigger("Disable");
			}
			if((int)button.StateButton == StateID)
			{
				button.Container.GetComponent<Animator>().SetTrigger("Enable");
			}
		}
		myState = (State)StateID;
	}
	public void AddPoint (int TypePoints)
	{
		ModifiedPoints[TypePoints]++;
		PointText[TypePoints].text = ModifiedPoints[TypePoints].ToString();
		PointLeft--;
		CheckEveryPoints();
	}
	public void UnAddPoint (int TypePoints)
	{
		ModifiedPoints[TypePoints]--;
		PointText[TypePoints].text = ModifiedPoints[TypePoints].ToString();
		PointLeft++;
		CheckEveryPoints();
	}
	public void ApplySettings ()
	{
		ResetModifiedPoints();
	}
	public void RestoreSettings ()
	{
		int PointsUsed = 0;
		for(int i = 0; i < ModifiedPoints.Count;i++)
		{
			PointsUsed += ModifiedPoints[i];
			ModifiedPoints[i] = 0;
			PointText[i].text = ModifiedPoints[i].ToString();
		}
		PointLeft += PointsUsed;
		CheckEveryPoints();
	}
	void ResetModifiedPoints ()
	{
		for(int i = 0;i < ModifiedPoints.Count;i++)
		{
			ModifiedPoints[i] = 0;
		}
		CheckEveryPoints();
	}
	void CheckEveryPoints ()
	{
		int PointDistributed = 0;
		PointLeftTxt.text = PointLeft.ToString();
		if(PointLeft > 0)
		{
			for(int i = 0;i < AddButtons.Count;i++)
			{
				AddButtons[i].interactable = true;
			}
		}
		else
		{
			for(int i = 0;i < AddButtons.Count;i++)
			{
				AddButtons[i].interactable = false;
			}
		}
		for(int i = 0; i < ModifiedPoints.Count;i++)
		{
			if(ModifiedPoints[i] > 0) DeleteButtons[i].interactable = true;
			else DeleteButtons[i].interactable = false;

			PointDistributed += ModifiedPoints[i];
		}

		if(PointDistributed > 0)
		{
			ApplyCarButton.interactable = true;
			ResetCarButton.interactable = true;
		}
		else 
		{
			ApplyCarButton.interactable = false;
			ResetCarButton.interactable = false;
		}
	}
	void RandomizeButtonsAnimations ()
	{
		foreach(GroupButton grpbut in ButtonGroup)
		{
			grpbut.Container.GetComponent<Animator>().SetInteger("IdDisable",Random.Range(0,3));
			grpbut.Container.GetComponent<Animator>().SetInteger("IdEnable",Random.Range(0,3));
		}
	}
	
	public void PlayHighLightSound (Object butObj)
	{
		GameObject convertBut = (GameObject)butObj;
		if(convertBut.GetComponent<Button>().interactable)
		{
			AudioComp.PlayOneShot(HighLightSound);
		}
	}
	public void PlayPressedSound (Object butObj)
	{
		GameObject convertBut = (GameObject)butObj;
		if(convertBut.GetComponent<Button>().interactable)
		{
			AudioComp.PlayOneShot(PressedSound);
		}
	}
	
	public void SelectMe(Object ObjToSelect)
	{
		GameObject ObjectConvert = (GameObject)ObjToSelect;
		ObjectConvert.GetComponent<Animator>().SetBool("Selected",true);
	}
}
