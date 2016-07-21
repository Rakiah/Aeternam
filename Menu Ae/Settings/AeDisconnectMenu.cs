using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AeDisconnectMenu : MonoBehaviour
{
	public AudioSource AudioComp;
	public AudioClip HighLightSound;
	public AudioClip PressedSound;

	public Transform ObjRotate;
	
	void Awake () {	AudioComp = GetComponent<AudioSource>(); }
	void Update ()
	{
		AudioComp.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeMenuEffects;
		ObjRotate.Rotate(0,1.0f,0);
	}

	public void DisconnectQuit (bool disconnect)
	{
		if(disconnect) Disconnect();
		else Quit();
	}

	public void PlayHighLightSound (Object butObj)
	{
		GameObject convertBut = (GameObject)butObj;
		if(convertBut.GetComponent<Button>().interactable) AudioComp.PlayOneShot(HighLightSound);
		
	}

	public void PlayPressedSound (Object butObj)
	{
		GameObject convertBut = (GameObject)butObj;
		if(convertBut.GetComponent<Button>().interactable) AudioComp.PlayOneShot(PressedSound);
	}

	void Disconnect () { AeGameLoader.m_pAeGameLoader.LoadAGame(1); }
	void Quit () { Application.Quit(); }
}