using System;
using UnityEngine;

namespace Gravitron.AI
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseAgent : StateMachine
    {
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected WaypointHandler point;
        public int index; 
        
        [NonSerialized] public Transform waypoint;
        [NonSerialized] public int count;

        protected override void Start()
        {
            point = GetComponentInParent<WaypointHandler>();
            base.Start();
        }

        /// <summary> Handles moving the object towards a set target </summary>
        /// <param name="_target">where to move to</param>
        /// <param name="_waypoint">this bool is to enable waypoints if we are using waypoints</param>
        protected virtual void MoveTowardsTarget(Transform _target, bool _waypoint = false)
        {
            if(_waypoint) point.HandleWaypoints(this);
            if(_target != null) transform.position = Vector2.MoveTowards(transform.position, 
            _target.position, (moveSpeed * Time.deltaTime));
        }
    }
}