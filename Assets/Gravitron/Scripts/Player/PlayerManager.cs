using Gravitron.Settings;
using Gravitron.Utils;

using System;
using UnityEngine;

namespace Gravitron.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float playerHealth;
        [SerializeField] private float endCount;
        
        [NonSerialized] public bool groundedDamage;
        [NonSerialized] public bool isDead;
        [NonSerialized] public float count;
        
        [SerializeField] private GameObject mobileControls;
        [SerializeField] private MeshRenderer healthBar;
        [SerializeField] private AudioSource rocketAudio;
        [SerializeField] private GameObject mainCam; 
        
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

            isDead = false;
        }

        private void LateUpdate()
        {
            HandleGroundedDamage();
        }

        private bool playOnce = false;
        private void Update()
        {
            var isFlying = pController.isFlying;

            if(isFlying && !playOnce)
            {
                rocketAudio.Play();
                playOnce = true;
            }
            else if(!isFlying)
            {
                rocketAudio.Stop();
                playOnce = false;
            }
            
            playerHealth = Mathf.Clamp(playerHealth, 0, 100);
            SetHealth(Mathf.Clamp01(playerHealth/maxHealth));
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
            if(playerHealth <= 0) OnPlayerDeath();
        }

        private void OnPlayerDeath()
        {
            mainCam.GetComponent<AudioListener>().enabled = true;
            var resource = Resources.Load<GameObject>("Effects/PlayerDeathParticles");
            Instantiate(resource, transform.position, Quaternion.identity);
            
            isDead = true;
            SetHealth(0);
            gameObject.SetActive(false);
        }

        private void SetHealth(float _value)
        {
            var shaderVal = ShaderUtils.healthShader;
            healthBarShader.SetFloat(shaderVal,  _value);  
        }        
        public void CollectUnit() => spaceMen++;
    }
}