using Gravitron.AI;
using Gravitron.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyManager : MonoBehaviour
{
	[SerializeField] private List<FriendlyAgent> agents = new List<FriendlyAgent>();
	[SerializeField] private GameObject player, waypoints;

	private int index = 0;
	[SerializeField] private bool boardingMode;

	private void Update()
	{
		if(agents.Count <= 0)
		{
			Destroy(waypoints);
			Destroy(gameObject);
		}
		
		if(boardingMode)
		{
			if(player.GetComponent<ShipLandingGear>().hasLanded)
			{
				if(agents.Count <= 0) return;
				var agent = agents[index];
				agent.ChangeState(AgentStates.Board);
				
				if(agent.boarded) {agents.RemoveAt(index); index++;}
				if(index >= agents.Count) index = 0;
			}
		}
	}
	private void OnTriggerEnter2D(Collider2D _other)
	{
		if(_other.CompareTag("Player"))
			boardingMode = true;
	}

	private void OnTriggerExit2D(Collider2D _other)
	{
		if(_other.CompareTag("Player"))
		{
			boardingMode = false;
			foreach(var agent in agents)
				agent.ChangeState(AgentStates.Waypoints);
		}
	}
}
