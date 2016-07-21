using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AeRaycasts
{


	public static bool IsSomethingThere(GameObject transfo,Vector3 direction, float range, float MinusHeight)
	{
		RaycastHit hit;
		Vector3 tempDir = transfo.transform.TransformDirection(direction);
		var layerMask = ~(1 << 8);
		
		if(Physics.Raycast(new Vector3(transfo.transform.position.x,transfo.transform.position.y - MinusHeight, transfo.transform.position.z),tempDir,out hit,range,layerMask))
		{
			if(hit.collider.isTrigger) return false;

			return true;
		}
	
		return false;
	}

	public static HitDamageInfo ShootingRaycast (Transform spawn, Vector3 direction, float range, float force, int damage, int weaponID)
	{
		HitDamageInfo info = new HitDamageInfo();
		RaycastHit hit;
		Vector3 tempDir = spawn.TransformDirection(direction);
		var layerMask = ~(1 << 8);

		if(Physics.Raycast(spawn.position, tempDir, out hit, range, layerMask))
		{
			if(hit.rigidbody) hit.rigidbody.AddForceAtPosition(force * tempDir, hit.point);

			if(AeTools.isEnnemy(hit.collider.tag))
		 	{
				AeStats healthInfo = hit.collider.transform.root.GetComponent<AeStats>();
				info.isEnnemy = true;
				info.damageLocation = hit.collider.tag;
				info.player = hit.collider.transform.root.networkView;
				info.stat = healthInfo;
				info.weaponDamage = damage;
				info.weaponID = weaponID;
			}
			else
			{
				info.damageLocation = hit.collider.tag;
			}
		}
		else
		{
			info.physicsFailed = true;
		}

		info.hit = hit;
		return info;
	}
}
