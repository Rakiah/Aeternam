using UnityEngine;
using System.Collections;

public class AeStats : MonoBehaviour 
{

	public int m_iTeamID;
	[HideInInspector] public float MaxHealth = 100.0f,MaxEnergy = 100.0f,MaxArmor = 100.0f,MaxMagic = 100.0f;
	public float Health,Energy,Armor,Magic;
	[HideInInspector] public bool SpawnProtected = true;
	[HideInInspector] public bool died = false;
	[HideInInspector] public float TchAfrq = 0.5f;
	[HideInInspector] public bool AlreadyTouchedByMagic;
	ComponentsManager manager;

	void Awake () 
	{
		died = false;
		SpawnProtected = true;
		manager = GetComponent<ComponentsManager>();
		Invoke("NoMoreSpawnProtection",2.0f);
	}
	void NoMoreSpawnProtection ()
	{
		SpawnProtected = false;
	}
	void Update () 
	{
		DoStatsResize();
		if(TchAfrq > 0.0f) TchAfrq -= Time.deltaTime;

		if(TchAfrq <= 0.0f) TchAfrq = 0.0f;

		if(Energy <= MaxEnergy) Energy += (manager.m_pMovements.currentStance.energyModifer * Time.deltaTime);

		if(manager.NormalCam.transform) manager.NormalCam.transform.localRotation = Quaternion.Slerp(manager.NormalCam.transform.localRotation, Quaternion.identity, Time.deltaTime * 0.1f);

		if(Input.GetKeyDown(KeyCode.F1) && !died) Suicide();

	}
	void DoStatsResize ()
	{
		if(Health < 1.0f && Health > 0.01f)
		{
			Health = 1;
		}
		if(Health >= MaxHealth)
		{
			Health = MaxHealth;
		}

		if(Energy <= 0.0f)
		{
			Energy = 0.0f;
		}
		if(Energy >= MaxEnergy)
		{
			Energy = MaxEnergy;
		}
		if(Armor > MaxArmor)
		{
			Armor = MaxArmor;
		}
		if(Armor <= 0.0f)
		{
			Armor = 0.0f;
		}

	}
	public void UnableComponents(float timing)
	{
		died = true;
		if(networkView.isMine)
		{
			AeCore.m_pCoreGame.MyStats.InstantiatedPlayer = null;
			Destroy(manager.m_pHud.transform.root.gameObject);
			Invoke("DestroyMyObjectOverTheNetwork", timing);
		}

		manager.m_pInputs.enabled = false;
		manager.m_pBones.Raggdoll();
		manager.m_pMouseX.enabled = false;
		GetComponent<CharacterController>().enabled = false;
		manager.m_pMovements.enabled = false;
		manager.m_pNickNameHandler.TextHead.renderer.enabled = false;
		manager.m_pNickNameHandler.enabled = false;
		GetComponentInChildren<AudioListener>().enabled = false;
		GetComponentInChildren<AudioListener>().gameObject.tag = "";
		manager.m_pMouseY.enabled = false;
		manager.m_pAnimator.enabled = false;
		transform.FindChild("PublicGraphics").GetComponentInChildren<Animator>().enabled = false;
		manager.m_pWeaponHandler.enabled = false;
		Destroy(manager.m_pBones.WeaponLocalHolder);
	}
	void DestroyMyObjectOverTheNetwork ()
	{
		Network.RemoveRPCs(GetComponent<NetworkView>().viewID);
		NetworkView [] nview = GetComponentsInChildren<NetworkView>();
		foreach(NetworkView v in nview) Network.RemoveRPCs(v.viewID);
		
		Network.Destroy(GetComponent<NetworkView>().viewID);
	}

	public void CheckDamageAndHealth(int PlayerWhoAttack,float DamageTaken,int Bodypart,int WeaponUsed)
	{
		Health -= DamageTaken;
		if(Health > 0)
		{
			StartCoroutine(Kick3(manager.NormalCam.transform, new Vector3(-1.0f * DamageTaken / 15, Random.Range(-1, 1) * DamageTaken / 15, 0), 0.08f));
		}

		else
		{
			Health = 0f;
			UnableComponents(5.0f);
			if(networkView.isMine)
			{
				if(Network.isClient) AeCore.m_pCoreGame.m_pNetworkHandler.
					networkView.RPC("RegisterKillRPC",RPCMode.Server, PlayerWhoAttack, AeCore.m_pCoreGame.MyStats.m_iPlayerID, WeaponUsed, Bodypart);

				else if(Network.isServer) AeCore.m_pCoreGame.m_pNetworkHandler.
					RegisterKillRPC(PlayerWhoAttack, AeCore.m_pCoreGame.MyStats.m_iPlayerID, WeaponUsed, Bodypart);
				AeCore.m_pCoreGame.MyStats.m_bDied = true;
			}
		}
	}
	
	public void Suicide ()
	{
		CheckDamageAndHealth(AeCore.m_pCoreGame.MyStats.m_iPlayerID,500f,3,1);
	}
	


	
	
	IEnumerator Kick3(Transform goTransform, Vector3 kbDirection, float time)
	{
		Quaternion startRotation = goTransform.localRotation;
		Quaternion endRotation = goTransform.localRotation * Quaternion.Euler(kbDirection);
		float rate = 1.0f / time;
		var t = 0.0f;
		while (t < 1.0f)
		{
			t += Time.deltaTime * rate;
			goTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
			yield return null;
		}
	}
}
