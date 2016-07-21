using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Gauntlet : Item 
{
	public Transform SpawnPoint;
	public Transform SpawnPointGraphic;
	public GameObject GauntletObject;
	public float Cooldown;
	public float Range;
	public float Force;
	public int Damage;

	public ComponentsManager componentsManager;
	public List<Transform> Lights;
	public AudioSource Sound;
	public GameObject ParticleOnHit;
	public bool SelfDamage;
	public bool MultipleTimeDamage;

	public abstract bool CanAction ();
	public abstract IEnumerator LaunchAction ();
	public abstract IEnumerator ActionCallBack ();

	public abstract bool Initialize (bool local, GameObject multiHolder, GameObject localHolder);
}


public class ElectricLineCatalyser : Gauntlet
{

	public List<ElectricRenderer> Beams = new List<ElectricRenderer>();

	float sizeByVertex;
	float timeBetweenVertex;
	float ShowTime;

	bool canFire = true;


	Trigger trig;


	
	public ElectricLineCatalyser ()
	{
		Cooldown = 0.8f;
		Force = 800;
		Damage = 80;

		Range = 40;

		sizeByVertex = 1.0f;
		timeBetweenVertex = 0.0013f;
		ShowTime = 0.1f;

		SelfDamage = false;
	}

	public ElectricLineCatalyser (string [] unparsedData)
	{
		
	}
	
	public ElectricLineCatalyser (Gauntlet copy)
	{
		
	}

	public ElectricLineCatalyser (ElectricLineCatalyser copy)
	{
		
	}

	public override bool CanAction ()
	{
		if(canFire && Input.GetKeyDown(AeProfils.m_pAeProfils.CurrentProfil.control.GauntletAttack)) return true;
		return false;

	}

	public override IEnumerator LaunchAction ()
	{
		componentsManager.networkView.RPC("ActionGauntlet", RPCMode.All);

		yield return null;
	}

	public override IEnumerator ActionCallBack ()
	{
		HitDamageInfo info = AeRaycasts.ShootingRaycast(SpawnPoint, Vector3.forward, Range, Force, Damage, m_iItemID);

		if(info.physicsFailed) { Vector3 point = SpawnPoint.position + SpawnPoint.TransformDirection(Vector3.forward) * Range; info.hit.point = point; info.hit.normal = point; }

		float distance = Mathf.Round(Vector3.Distance(SpawnPointGraphic.transform.position, info.hit.point) * 2.0f);

		int vertexs = (int)(distance / sizeByVertex);
		vertexs += 1;

		float timeWait = ((distance / sizeByVertex) * timeBetweenVertex);

		canFire = false;

		if(componentsManager.m_pNetworkCaller.networkView.isMine) Beams[0].parent.collider.enabled = true;
		componentsManager.m_pWeaponHandler.StartCoroutine(GenerateNormalVertex(vertexs, info.hit.point));
		for(int i = 1; i < Beams.Count; i++) componentsManager.m_pWeaponHandler.StartCoroutine(GenerateSinusoidalVertex(i,vertexs, info.hit.point));
		
		float t = 0.0f;
		while (t < timeWait)
		{
			for(int j = 0; j < Beams.Count; j++)
			{
				Beams[j].Rotate();
			}

			t += Time.deltaTime;
			yield return null;
		}

		DoExplosion(AeTools.CreateParticle(ParticleOnHit, info, true, info.physicsFailed == false ? 0.4f : 0.0f));



		float t2 = 0.0f;
		while(t2 < ShowTime)
		{
			for(int j = 0; j < Beams.Count; j++)
			{
				Beams[j].Rotate();
			}
			
			t2 += Time.deltaTime;
			yield return null;
		}

		Beams[0].parent.collider.enabled = false;
		resetBeams();
	
		
		yield return new WaitForSeconds(Cooldown);
		
		canFire = true;
	}

	public override bool Initialize (bool local, GameObject multiHolder, GameObject localHolder)
	{
		try
		{
			if(local)
			{
				GauntletObject = MonoBehaviour.Instantiate(Resources.Load("Gauntlets/Gauntlet_Electric_Line") as GameObject, localHolder.transform.position, localHolder.transform.rotation) as GameObject;

				Vector3 localScale = GauntletObject.transform.localScale;
				GauntletObject.transform.parent = localHolder.transform;
				GauntletObject.transform.localScale = localScale;

				componentsManager = GauntletObject.transform.root.GetComponent<ComponentsManager>();

				SpawnPointGraphic = GauntletObject.transform;
				SpawnPoint = componentsManager.m_pWeaponHandler.transform;
				ParticleOnHit = Resources.Load("Particles/ElectricHole") as GameObject;

				Transform ElectricLineCast = GauntletObject.transform.GetChild(0);

				Beams.Add(new ElectricRenderer(ElectricLineCast.GetChild(0), 8.0f));
				Beams.Add(new ElectricRenderer(ElectricLineCast.GetChild(1), -8.0f));
				Beams.Add(new ElectricRenderer(ElectricLineCast.GetChild(2), 8.0f));

				trig = Beams[0].parent.GetComponent<Trigger>();

				trig.gauntlet = this;
			}
			else
			{

			}

			return true;
		}
		catch
		{
			return false;
		}
	}


	IEnumerator GenerateNormalVertex (int nbVertex, Vector3 point)
	{
		int VertexByFrameDone = 0;

		BoxCollider b = Beams[0].parent.collider as BoxCollider;
		Beams[0].LookAt(point);
		Beams[0].AddVertex(new Vector3(0,0,0));
		for(int i = 0; i < nbVertex; i++)
		{
			//this operation mean, how many vertex should i put in a single frame to make it 1 vertex per timeBetweenVertex
			//delta T /Tv = vT
			int nbPerFrame = (int)(Time.deltaTime / timeBetweenVertex);

			Beams[0].AddVertex(new Vector3(sizeByVertex * i,0,0));

			b.size = new Vector3(1,1,i);
			b.center = new Vector3(0,0, i /2);
			VertexByFrameDone++;
			
			if(VertexByFrameDone > nbPerFrame) { VertexByFrameDone = 0; yield return null; }
		}
	}

	IEnumerator GenerateSinusoidalVertex (int i, int nbVertex, Vector3 point)
	{
		int VertexByFrameDone = 0;

		Beams[i].AddVertex(new Vector3(0,0,0));
		Beams[i].AddVertex(new Vector3(sizeByVertex,0,0));
		Beams[i].LookAt(point);


		for(int j = 2; j < nbVertex - 2; j++)
		{
			//this operation mean, how many vertex should i put in a single frame to make it 1 vertex per timeBetweenVertex
			int nbPerFrame = (int)(Time.deltaTime / timeBetweenVertex);

			Beams[i].AddVertex(new Vector3(sizeByVertex * j,  Mathf.Sin((j - 1) + Time.time) /4, Mathf.Sin((j - 1)) /4));

			VertexByFrameDone++;

			if(VertexByFrameDone > nbPerFrame) { VertexByFrameDone = 0; yield return null; }
		}

		Beams[i].AddVertex(new Vector3(sizeByVertex * (nbVertex - 1), 0, 0));
		Beams[i].AddVertex(new Vector3(sizeByVertex * (nbVertex - 2), 0, 0));
	}   

	void DoExplosion(GameObject epicenter)
	{
		var layerMask = ~(1 << 8);
		Collider [] RangedToExplosion = Physics.OverlapSphere(epicenter.transform.position,3.0f);

		for(int i = 0; i < RangedToExplosion.Length;i++)
		{

			if(AeTools.isEnnemy(RangedToExplosion[i].tag))
			{
				RaycastHit newHit;
				AeStats stat = RangedToExplosion[i].transform.root.GetComponent<AeStats>();

				if(stat != null && AeTools.canDamage(this,stat, stat.networkView.isMine))
				{
					if(Physics.Linecast(epicenter.transform.position,RangedToExplosion[i].transform.position,out newHit, layerMask))
					{
						if(AeTools.isEnnemy(newHit.transform.tag))
						{
							HitDamageInfo info = new HitDamageInfo();
							stat.AlreadyTouchedByMagic = true;
							info.stat = stat;
							info.damageLocation = newHit.transform.tag;
							info.hit = newHit;
							info.isEnnemy = true;
							info.weaponID = m_iItemID;
							info.weaponDamage = Damage;
							info.player = stat.networkView;
							info.physicsFailed = false;

							AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().SendAttack(info);
						}
					}
					else
					{
						HitDamageInfo info = new HitDamageInfo();
						stat.AlreadyTouchedByMagic = true;
						info.stat = stat;
						info.damageLocation = RangedToExplosion[i].transform.tag;
						info.isEnnemy = true;
						info.weaponID = m_iItemID;
						info.weaponDamage = Damage;
						info.player = stat.networkView;
						info.physicsFailed = false;

						AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().SendAttack(info);

					}
				}
			}
		}
	}

	void resetBeams ()
	{
		BoxCollider b = Beams[0].parent.collider as BoxCollider;
		b.size = new Vector3(1,1,0.1f);
		b.center = new Vector3(0,0,0);
		for(int i = 0; i < Beams.Count; i++)
		{
			Beams[i].Reset();
		}

		for(int i = 0; i < trig.Collided.Count; i++)
		{
			trig.Collided[i].stat.AlreadyTouchedByMagic = false;
		}
		
		trig.Collided.Clear();
	}
}

[System.Serializable]
public class ElectricRenderer 
{
	public float speedRotate;
	public LineRenderer line;
	public Transform parent;
	
	public int vertexCount;

	public ElectricRenderer (Transform par, float rotatesp)
	{
		parent = par;
		line = par.GetChild(0).GetComponent<LineRenderer>();
		speedRotate = rotatesp;
	}

	public void AddVertex (Vector3 vertex)
	{
		vertexCount ++;
		line.SetVertexCount(vertexCount);
		line.SetPosition(vertexCount - 1, vertex);
	}
	
	public void Reset ()
	{
		vertexCount = 0;
		line.SetVertexCount(0);
	}
	
	public void Rotate ()
	{
		parent.Rotate(new Vector3(0, 0, speedRotate));
	}
	
	public void LookAt (Vector3 point)
	{
		parent.LookAt(point);
	}
}