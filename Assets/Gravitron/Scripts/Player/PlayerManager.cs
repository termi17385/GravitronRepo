using Gravitron.Settings;
using Gravitron.Utils;

using System;
using System.Collections.Generic;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gravitron.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float playerHealth;
        [SerializeField] private float playerFuel;
        [SerializeField] private float endCount;
        [SerializeField] private int altimeter;
        [NonSerialized] public bool groundedDamage;
        [NonSerialized] public bool isDead;
        [NonSerialized] public float count;
        [Header("Menus")]
        [SerializeField] private GameObject mobileControls;
        [SerializeField] private GameObject leavingAreaMenu;
        [SerializeField] private GameObject winMenu;
        [Header("Misc")]
        [SerializeField] private MeshRenderer healthBar;
        [SerializeField] private AudioSource rocketAudio;
        [SerializeField] private GameObject mainCam;
        [Header("WeaponStuff")]
        [SerializeField] private List<Transform> bullets = new List<Transform>();
        [SerializeField] private Transform spawnPos;
        [SerializeField] private Transform ammoHolder;
        // ReSharper disable once InconsistentNaming
        [SerializeField, Tooltip("Debugging Purposes Only")] private bool GOD_MODE = false;
        
        private Material healthBarShader;
        private float maxHealth = 100;
        private float maxFuel = 100;
        private PlayerController pController;
            
        [Header("Inventory")] 
        public float fuel;
        public float shield;
        public int spaceMen;

        public static bool objectiveDestoryed = false;

        private void Start()
        {
            playerHealth = maxHealth;
            pController = GetComponent<PlayerController>();
            
            mobileControls.SetActive(PlatformUtil.mobileMode);
            healthBarShader = healthBar.materials[0];

            isDead = false;

            for(int i = 0; i < 5; i++)
            {
                var path = "Projectiles/pStandard";
                var obj = Resources.Load<GameObject>(path);
                
                var bullet = Instantiate(obj, ammoHolder);
                bullet.SetActive(false);
                bullets.Add(bullet.transform);
            }
            
            //gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            HandleGroundedDamage();
        }

        private bool playOnce = false;
        private float timer;
        [SerializeField] private float timerMax;

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
            Shoot();
            LeavingArea();
        }
        private int index = 0;
        
        public void Shoot(bool _pressed = false)
        {
            timer += Time.deltaTime;
            if(timer >= timerMax)
            {
                timer = timerMax;
                if(Input.GetKeyDown(KeyCode.O) || _pressed)
                {
                    if(bullets.Count > 0)
                    {
                        if(index >= bullets.Count) index = 0;
                        if(!bullets[index].gameObject.activeSelf)
                        {
                            GetComponent<AudioSource>().Play();
                            var projectile = bullets[index];
                            projectile.gameObject.SetActive(true);
                            
                            projectile.transform.position = spawnPos.position;
                            projectile.rotation = transform.localRotation;

                            var rb = projectile.GetComponent<Rigidbody2D>();
                            rb.velocity += GetComponent<Rigidbody2D>().velocity;
                        }
                        index++;
                    }
                    timer = 0;
                }
            }
        }
        private void Altimeter()
        {
            altimeter = Mathf.Abs(Mathf.RoundToInt(transform.position.y));
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

        private void LeavingArea()
        {
            Altimeter();
            if(altimeter >= 15)
            {
                if(altimeter >= 25)
                {
                    if(objectiveDestoryed)
                    {
                        winMenu.SetActive(true);
                        ScoreUtil.ScoreHandler(ScoreTypes.SpaceMen, spaceMen);
                        Cursor.lockState = CursorLockMode.None;
                        
                        leavingAreaMenu.SetActive(false);
                        gameObject.SetActive(false);                        
                    }
                    else SceneManager.LoadScene("LevelOne");
                }
                leavingAreaMenu.SetActive(true);
            }
            else leavingAreaMenu.SetActive(false);
        }
        
        /// <summary> when called damages the player </summary>
        /// <param name="_amt">how much to damage the player by</param>
        public void DamagePlayer(float _amt)
        {
            if(!GOD_MODE)
            {
                playerHealth -= _amt;
                if(playerHealth <= 0) OnPlayerDeath();
            }
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