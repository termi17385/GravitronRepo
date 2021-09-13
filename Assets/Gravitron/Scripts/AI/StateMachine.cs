using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gravitron.AI
{
	public enum AgentStates
	{
		Waypoints,
		Board,
		
		ShootAndMove,
		Shoot
	}

	public delegate void StateDelegate();
	public class StateMachine : MonoBehaviour
	{
		protected Dictionary<AgentStates, StateDelegate> states = new Dictionary<AgentStates, StateDelegate>();
		[SerializeField] protected AgentStates currentState = AgentStates.Waypoints;
		public void ChangeState(AgentStates _newState) => currentState = _newState;
        
		protected virtual void Start()
		{
		}

		protected virtual void Update()
		{
			// These two lines are used to run the state machine
			// it works by attempting to retrieve the relevant function for the current state.
			// then running the function if it successfully found it 
			if(states.TryGetValue(currentState, out StateDelegate state)) state.Invoke();
			else Debug.Log($"No State Was Set For {currentState}.");
		}
	}
}