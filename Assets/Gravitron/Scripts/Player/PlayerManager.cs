using Gravitron.AI;
using Gravitron.Settings;
using Gravitron.Utils;

using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;

namespace Gravitron.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float playerHealth;
        [SerializeField] private float endCount;
        
        [NonSerialized] public bool groundedDamage;
        [NonSerialized] public float count;
        
        [SerializeField] private GameObject mobileControls;
        [SerializeField] private MeshRenderer healthBar;
        private Material healthBarShader;

        private float maxHealth = 100;
        private PlayerController pController;
            
        [Header("Inventory")] 
        public float fuel;
        public float shield;
        public float spaceMen;

        private void Start()
        {
            playerHealth = maxHealth;
            pController = GetComponent<PlayerController>();
            
            mobileControls.SetActive(PlatformUtil.mobileMode);
            healthBarShader = healthBar.materials[0];
        }

        private void LateUpdate()
        {
            HandleGroundedDamage();
        }

        private void Update()
        {
            var shaderVal = ShaderUtils.healthShader;
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            healthBarShader.SetFloat(shaderVal,  Mathf.Clamp01(playerHealth / maxHealth));
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