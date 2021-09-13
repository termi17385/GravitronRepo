using Gravitron.Player;
using UnityEngine;

namespace Gravitron.AI
{
    public class FriendlyAgent : BaseAgent
    {
        private Transform shipTarget;
        public bool boarded;

        protected override void Start()
        {
            shipTarget = FindObjectOfType<PlayerController>().transform;
            
            base.Start();
            states.Add(AgentStates.Waypoints, delegate { MoveTowardsTarget(waypoint, true);});
            states.Add(AgentStates.Board, delegate { MoveTowardsTarget(shipTarget); BeamMeUpScotty();});
        }

        /// <summary> Handles the
        /// collection of the agent </summary>
        public void BeamMeUpScotty()
        {
            if(Vector2.Distance(transform.position, shipTarget.position) <= 0.2f)
            {
                shipTarget.GetComponent<PlayerManager>().CollectUnit();
                boarded = true;
                gameObject.SetActive(false);
            }
        }
    }
}