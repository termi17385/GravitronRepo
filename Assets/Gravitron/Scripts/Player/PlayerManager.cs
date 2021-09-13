using Gravitron.AI;
using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;

namespace Gravitron.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private float playerHealth;
        [SerializeField] private float endCount;
        
        [NonSerialized] public bool groundedDamage;
        [NonSerialized] public float count;

        private float maxHealth = 100;
        private PlayerController pController;
        private List<WaypointHandler> pickupPoints = new List<WaypointHandler>();
            
        [Header("Inventory")] 
        public float fuel;
        public float shield;
        public float spaceMen;

        private void Awake() 
        {
            var objects = FindObjectsOfType<WaypointHandler>();
            foreach(var waypoint in objects)
                if(waypoint.CompareTag("Friendlies"))
                    pickupPoints.Add(waypoint);
            
        }
        private void Start()
        {
            playerHealth = maxHealth;
            pController = GetComponent<PlayerController>();
        }

        private void LateUpdate()
        {
            HandleGroundedDamage();
        }

        private void Update()
        {
        }

        private void HandleGroundedDamage()
        {
            if(groundedDamage)
            {
                count += Time.deltaTime;
                if(count >= endCount)
                {
                    DamagePlayer(5);
                    count = 0;
                }
            }
        }
        
        /// <summary> when called damages the player </summary>
        /// <param name="_amt">how much to damage the player by</param>
        public void DamagePlayer(float _amt)
        {
            playerHealth -= _amt;
        }

        public void CollectUnit() => spaceMen++;
    }
}