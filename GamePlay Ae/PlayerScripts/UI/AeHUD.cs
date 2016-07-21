using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AeHUD : MonoBehaviour
{


	public RawImage HealthBar,EnergyBar,MagicBar;
	public RectTransform DestroyedHealth,DestroyedEnergy,DestroyedMagic;
	Rect HealthUV,EnergyUV,MagicUV;

	public RectTransform WeaponHolder;

	public RawImage WeaponImage;

	public RectTransform CrossHairLeft,CrossHairRight,CrossHairTop,CrossHairBot;

	Vector2 ModifiedCrossLeft,ModifiedCrossRight,ModifiedCrossTop,ModifiedCrossBot;
	public RawImage TextureSniper;

	public GameObject BulletPrefab;

	public List<BufferScoreItem> ScoreBufferList = new List<BufferScoreItem>();

	public Animator hitMarker;
	
	public Animator ScoreBufferAnim;
	Text ScoreBufferText;
	public Text IKilledWHoText;
	public Text ScoreMaxText;
	public string ScoreBufferString;
	public string BountyDisplayer;
	
	public bool PushDownScoreBuffer;
	
	public float ScoreDecreaser = 150;
	int ScoreDisplayer;

	public float ScoreMaxIncreaser = 0;
	int ScoreMaxDisplayer;

	int BountyKiller = 0;

	float BountyTimer = 0.0f;
	public List<RectTransform> BulletsRect = new List<RectTransform>();
	
	public AudioClip HeadShot;

	public AudioClip DoubleKill, MultiKill, MegaKill, UltraKill, MonsterKill;

	public AudioSource Announcer;

	public float HealthPercentage,EnergyPercentage,MagicPercentage;
	Vector2 VectDestroyedHealth,VectDestroyedEnergy,VectDestroyedMagic;
	public float MaxXHealth,MaxXEnergy,MaxXMagic;

	Texture2D HitMarker;
	
	public float CrossHairTallVer = 30;
	public float CrossHairTallHor = 30;
	
	public float OffSet = 5;
	float MaxOffSet = 150;
	public float BasicOffset = 5;
	float OffSetModifier = 20;
	float Stability = 80;

	ComponentsManager manager;
	
	[System.NonSerialized] public bool ShowSniperScope = false;
	

	public void AssignNewCrossHairModifiers()
	{
		BasicOffset    = manager.m_pWeaponHandler.GetCurrentWeapon().m_sCrossHair.CrossHairBasicOffSet;
		Stability 	   = manager.m_pWeaponHandler.GetCurrentWeapon().m_sCrossHair.CrossHairStability;
		OffSetModifier = manager.m_pWeaponHandler.GetCurrentWeapon().m_sCrossHair.CrossHairOffSetModifier;
		MaxOffSet      = manager.m_pWeaponHandler.GetCurrentWeapon().m_sCrossHair.CrossHairMaxOffSet;
		OffSet         = BasicOffset;
	}
	
	public void AddKnockBack ()
	{
		if(OffSet < MaxOffSet)
		{
			OffSet += OffSetModifier;
		}
	}

	void PlayBarSprite ()
	{
		HealthUV.x += Time.deltaTime /2;
		HealthUV.y += Time.deltaTime /4;

		EnergyUV.x += Time.deltaTime /2;
		EnergyUV.y += Time.deltaTime /4;

		MagicUV.x += Time.deltaTime /2;
		MagicUV.y += Time.deltaTime /4;



		HealthBar.uvRect = HealthUV;
		EnergyBar.uvRect = EnergyUV;
		MagicBar.uvRect = MagicUV;
	}
	void Awake () 
	{
		manager = AeCore.m_pCoreGame.MyStats.PlayerComponents;
		Announcer = GetComponent<AudioSource>();
		ScoreBufferText = ScoreBufferAnim.GetComponent<Text>();
	}
	void Start ()
	{
		MaxXHealth = DestroyedHealth.localPosition.x;
		MaxXEnergy = DestroyedEnergy.localPosition.x;
		MaxXMagic = DestroyedMagic.localPosition.x;
		EnergyUV.width = 1;
		EnergyUV.height = 1;
		MagicUV.width = 1;
		MagicUV.height = 1;
		HealthUV.width = 1;
		HealthUV.height = 1;
		ScoreMaxText.CrossFadeAlpha(0.0f,0.05f,true);
	}
	public void SwappedGun()
	{
		WeaponImage.texture = manager.m_pWeaponHandler.GetCurrentWeapon().m_tIconTexture;
		AssignNewCrossHairModifiers();
	}

	public void MadeAKill (bool headshot, int Score,string PlayerName,int ScoreBeforeAdd)
	{
		BountyTimer = 4.0f;
		BountyKiller++;

		ScoreBufferList.Add(new BufferScoreItem(headshot,Score,BountyKiller,PlayerName, ScoreBeforeAdd));
		if(ScoreBufferList.Count <= 0) StartCoroutine(ScoreBuffer(ScoreBufferList[ScoreBufferList.Count -1]));

	}

	void Announce (bool headshot, int KillSpree)
	{
		if(!headshot && KillSpree < 2) BountyDisplayer = "Kill \n";
		else if(headshot && KillSpree < 2){ Announcer.PlayOneShot(HeadShot); BountyDisplayer = "Head Shot \n"; }

		switch(KillSpree)
		{
			case 2 : Announcer.PlayOneShot(DoubleKill);  BountyDisplayer = "Double Kill \n";  break;
			case 3 : Announcer.PlayOneShot(MultiKill);   BountyDisplayer = "Multi Kill \n";   break;
			case 4 : Announcer.PlayOneShot(MegaKill);    BountyDisplayer = "Mega Kill \n";    break;
			case 5 : Announcer.PlayOneShot(UltraKill);   BountyDisplayer = "Ultra Kill \n";   break;
			case 6 : Announcer.PlayOneShot(MonsterKill); BountyDisplayer = "Monster Kill \n"; break;
		}
	}

	void SetDestroyedStats ()
	{
		HealthPercentage = (manager.m_pStats.Health / manager.m_pStats.MaxHealth);
		EnergyPercentage = (manager.m_pStats.Energy / manager.m_pStats.MaxEnergy);
		MagicPercentage = (manager.m_pStats.Magic / manager.m_pStats.MaxMagic);

		VectDestroyedHealth.x = MaxXHealth * HealthPercentage;
		VectDestroyedEnergy.x = MaxXEnergy * EnergyPercentage;
		VectDestroyedMagic.x = MaxXMagic * MagicPercentage;

		DestroyedHealth.localPosition = VectDestroyedHealth;
		DestroyedEnergy.localPosition = VectDestroyedEnergy;
		DestroyedMagic.localPosition = VectDestroyedMagic;
	}

	void Update () 
	{
		PlayBarSprite();
		SetDestroyedStats();
		ScoreDisplayer = (int)ScoreDecreaser;
		ScoreMaxDisplayer = (int)ScoreMaxIncreaser;
		Announcer.volume = AeCore.m_pCoreGame.m_pSoundManager.VolumeBruitage;

		if(OffSet > BasicOffset) OffSet -= Time.deltaTime * Stability;
		else if(OffSet < BasicOffset) OffSet = BasicOffset;


		if(BountyTimer > 0 && BountyKiller > 0) BountyTimer -= Time.deltaTime;
		else if(BountyTimer <= 0.0f) { BountyKiller = 0; BountyTimer = 0.0f; }

		if(PushDownScoreBuffer)
		{
			ScoreDecreaser -= Time.deltaTime * 50.0f;
			ScoreMaxIncreaser += Time.deltaTime * 50.0f;
			ScoreBufferString = ScoreDisplayer.ToString();
			if(ScoreDecreaser <= 0.0f)
			{
				IKilledWHoText.CrossFadeAlpha(0,0.5f,true);
				ScoreMaxText.CrossFadeAlpha(0,0.5F,true);
				ScoreBufferAnim.SetBool("Show",false);
				ScoreBufferList.RemoveAt(0);
				PushDownScoreBuffer = false;
				if(ScoreBufferList.Count > 0) StartCoroutine(ScoreBuffer(ScoreBufferList[0]));
			}
		}


		ScoreBufferText.text = BountyDisplayer + "+" + ScoreBufferString;
		ScoreMaxText.text = ScoreMaxDisplayer.ToString();

		CrossHairPositions();
	}

	void CrossHairPositions ()
	{
		if(!ShowSniperScope)
		{
			ModifiedCrossBot.y = OffSet;
			ModifiedCrossTop.y = -OffSet;
			ModifiedCrossRight.x = OffSet;
			ModifiedCrossLeft.x = -OffSet;

			CrossHairBot.localPosition = ModifiedCrossBot;
			CrossHairTop.localPosition = ModifiedCrossTop;

			CrossHairLeft.localPosition = ModifiedCrossLeft;
			CrossHairRight.localPosition = ModifiedCrossRight;
		}
		else
		{
			CrossHairBot.gameObject.SetActive(false);
			CrossHairTop.gameObject.SetActive(false);
			CrossHairRight.gameObject.SetActive(false);
			CrossHairLeft.gameObject.SetActive(false);


			TextureSniper.gameObject.SetActive(true);
		}
	}




	public IEnumerator Hitmark (bool HS)
	{
		hitMarker.SetBool("Show",true);
		hitMarker.SetBool("Headshot", HS);

		yield return new WaitForSeconds(0.5f);

		hitMarker.SetBool("Show",false);
		hitMarker.SetBool("Headshot",false);
	}
	
	public IEnumerator ScoreBuffer (BufferScoreItem TempBuff)
	{
		if(ScoreBufferList.Count > 0) yield return new WaitForSeconds(0.3f);

		Announce(TempBuff.HeadShot,TempBuff.BountyKill);
		ScoreDecreaser = TempBuff.ScoreToAdd;
		ScoreDisplayer = TempBuff.ScoreToAdd;
		ScoreMaxIncreaser = TempBuff.ScoreBeforeAdd;
		IKilledWHoText.text = "You killed " + TempBuff.NameKilled;
		IKilledWHoText.CrossFadeAlpha(1,0.5f,true);
		ScoreMaxText.CrossFadeAlpha(1,0.5f,true);
		ScoreBufferAnim.SetBool("Show",true);
		yield return new WaitForSeconds(0.3f);
		PushDownScoreBuffer = true;
	}
}

[System.Serializable]
public class BufferScoreItem
{
	public int BountyKill;
	public bool HeadShot;
	public int ScoreToAdd;
	public int ScoreBeforeAdd;
	public string NameKilled;
	
	public BufferScoreItem (bool headshot, int Score, int BountyKiller, string PlayerName, int scorebefore)
	{
		HeadShot = headshot;
		ScoreToAdd = Score;
		BountyKill = BountyKiller;
		NameKilled = PlayerName;
		ScoreBeforeAdd = scorebefore;
	}
}
