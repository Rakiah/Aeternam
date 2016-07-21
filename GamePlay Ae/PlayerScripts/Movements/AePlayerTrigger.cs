using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AePlayerTrigger : MonoBehaviour 
{
	public List<EventToTags> tags = new List<EventToTags>();
	ComponentsManager m;

	void Start ()
	{
		m = GetComponent<ComponentsManager>();

		tags.Add(new TeamSelector("team"));
	}

	void OnTriggerEnter (Collider collid)
	{
		foreach(EventToTags t in tags) t.CheckTag(collid.tag, m, collid.gameObject);
	}
}


public abstract class EventToTags
{
	public string nameTag;
	public virtual void CheckTag (string s, ComponentsManager manager, GameObject obj) { if(s == nameTag) DoAction (manager, obj); }
	public abstract void DoAction (ComponentsManager manager, GameObject obj);
}


public class TeamSelector : EventToTags
{
	public TeamSelector (string tag) { nameTag = tag; }
	public override void DoAction (ComponentsManager manager, GameObject obj) 
	{ 
		if(Network.isClient) AeCore.m_pCoreGame.m_pNetworkHandler.networkView.RPC("ChooseTeam", RPCMode.Server, int.Parse(obj.name), AeCore.m_pCoreGame.MyStats.m_iPlayerID);
		else AeCore.m_pCoreGame.m_pNetworkHandler.ChooseTeam(int.Parse(obj.name), 0);
	}
}