using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trigger : MonoBehaviour 
{
	public List<HitDamageInfo> Collided = new List<HitDamageInfo>();
	public Gauntlet gauntlet;

	void OnTriggerEnter (Collider collid)
	{
		if(transform.root.networkView.isMine)
		{
			if(AeTools.isEnnemy(collid.tag))
			{
				AeStats h = collid.transform.root.GetComponent<AeStats>();

				if(AeTools.canDamage(gauntlet,h,collid.transform.root.networkView.isMine)) buildInfo(collid.gameObject, h);
			}
		}
	}


	void buildInfo (GameObject obj, AeStats h)
	{
		HitDamageInfo info = new HitDamageInfo();
		h.AlreadyTouchedByMagic = true;

		info.damageLocation = obj.tag;
		info.weaponDamage = gauntlet.Damage;
		info.isEnnemy = true;
		info.physicsFailed = false;
		info.player = obj.transform.root.networkView;
		
		info.stat = h;
		
		info.weaponID = gauntlet.m_iItemID;
		
		Collided.Add(info);

		AeCore.m_pCoreGame.m_pNetworkHandler.ServerInformations.GetCurrentMode().SendAttack(info);
	}
}
