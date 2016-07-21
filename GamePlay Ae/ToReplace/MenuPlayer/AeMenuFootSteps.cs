using UnityEngine;
using System.Collections;

public class AeMenuFootSteps : MonoBehaviour 
{
	public AeMenuCS AeCS;
	public AudioClip[] concrete;
	public AudioClip[] snow;
	
	public float runLength = 0.25f;
	public float walkLength = 0.4f;
	public float crouchLength = 0.65f;
	
	public AudioSource theSource;
	
	public float timer;
	
	public string curMat = "";
	public RaycastHit hit;
	
	
	void Start ()
	{
		theSource.panLevel = 0.0f;
	}
	void Update()
	{
		theSource.volume = AeCore.m_pCoreGame.GetComponent<AeSoundManager>().VolumeBruitage/5;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 2))
		{
			curMat = hit.transform.tag;
		}
		if (AeCS.grounded)
		{
			if (AeCS.velMag > 1)
			{
				if (timer <= 0)
				{
					if (AeCS.running)
					{
						if (curMat == "Snow")
							OneStep(snow, runLength);
						else
							OneStep(concrete, runLength);
					}
					else
					{
						if (curMat == "Snow")
							OneStep(snow, walkLength);
						else
							OneStep(concrete, walkLength);
					}
				}
			}
		}
		if (timer > 0) timer -= Time.deltaTime * (AeCS.velPercent);
	}
	
	void OneStep(AudioClip[] clips, float length)
	{
		int random = Random.Range(0, clips.Length);
		theSource.clip = clips[random];
		theSource.Play();
		timer = length;
	}
}