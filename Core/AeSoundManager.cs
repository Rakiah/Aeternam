using UnityEngine;
using System.Collections;

public class AeSoundManager : MonoBehaviour 
{

	public AudioClip ConnectionScreenMusic;
	public AudioClip MainMenuMusic;
	public AudioClip InGameMusic;

	public float MasterVolume;
	public float VolumeMenuEffects;
	public float VolumeMusic;
	public float VolumeBruitage;

	public float VolumeMusicFade;

	public AudioSource AudioComp;
	public bool FadeMusic = false;
	public bool NextMusicReadyToPlay;
	public float LowerMusicVolume = 1.0f;

	public AudioClip NextMusicToPlay;

	void Awake () 
	{
		AudioComp = GetComponent<AudioSource>();
	}


	void Update () 
	{
		MasterVolume = AeProfils.m_pAeProfils.CurrentProfil.sound.MasterVolume;
		VolumeBruitage = AeProfils.m_pAeProfils.CurrentProfil.sound.SoundsEffectVolume * MasterVolume;
		VolumeMusic = AeProfils.m_pAeProfils.CurrentProfil.sound.MusicVolume * MasterVolume;
		VolumeMenuEffects = AeProfils.m_pAeProfils.CurrentProfil.sound.MenuEffectsVolume * MasterVolume;

		if(FadeMusic)
		{
			LowerMusicVolume -= Time.deltaTime * 0.20f;
			AudioComp.volume = LowerMusicVolume;
		}
		if(LowerMusicVolume <= 0.0f && FadeMusic == true)
		{
			LowerMusicVolume = VolumeMusic;
			AudioComp.volume = VolumeMusic;
			NextMusicReadyToPlay = true;
			AudioComp.clip = NextMusicToPlay;
			AudioComp.Play();

			FadeMusic = false;
		}

		if(!FadeMusic)
		{
			AudioComp.volume = VolumeMusic;
		}
	}

	public void PlayerInitalized (GameObject player)
	{
		AudioSource [] sources = player.transform.GetComponentsInChildren<AudioSource>();

		foreach(AudioSource source in sources)
		{
			if(networkView.isMine)
			{
				source.panLevel = 0.0f;
				source.volume = VolumeBruitage/5;
			}
			else
			{
				source.panLevel = 1.0f;
				source.volume = VolumeBruitage;
			}
		}
	}

	public void PlayMusic (int Level)
	{
		AudioComp.volume = VolumeMusic;
		LowerMusicVolume = VolumeMusic;
		if(!AudioComp.isPlaying)
		{
			if(Level == 1)
			{
				AudioComp.clip = ConnectionScreenMusic;
				AudioComp.Play();
				NextMusicToPlay = ConnectionScreenMusic;
			}
			else if(Level == 2)
			{
				AudioComp.clip = MainMenuMusic;
				AudioComp.Play();
				NextMusicToPlay = MainMenuMusic;
			}
			else if(Level == 3 || Level == 4 || Level == 5 || Level == 6 || Level == 7)
			{
				AudioComp.clip = InGameMusic;
				AudioComp.Play();
				NextMusicToPlay = InGameMusic;
			}
		}
		else
		{
			if(Level == 1)
			{
				NextMusicToPlay = ConnectionScreenMusic;
				FadeMusic = true;
			}
			else if(Level == 2)
			{
				NextMusicToPlay = MainMenuMusic;
				FadeMusic = true;
			}
			else if(Level == 3 || Level == 4 || Level == 5 || Level == 6 || Level == 7)
			{
				NextMusicToPlay = InGameMusic;
				FadeMusic = true;
			}
		}
	}
}
