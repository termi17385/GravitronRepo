using Gravitron.Player;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Gravitron.AI
{
    public class BaseTurret : BaseAgent
    {
        [SerializeField] private LayerMask dontDetectPlayer;
        private Transform player;
        #region Properties
        protected float Timer => timer;
        protected float ProjectileForce => projectileForce;
        protected int AmmoCount => ammoAmount;
        #endregion
        [Header("Shooting Timers")]
        [SerializeField] private float timerMax;
        [SerializeField] private float timer;
        
        [Header("Weapon Settings")]
        [SerializeField] private float projectileForce;
        [SerializeField] private int ammoAmount;

        [Header("Extra Stuff")]
        [SerializeField] protected float radius;
        
        [Header("SEND HELP")]
        [SerializeField] protected List<GameObject> projectiles = new List<GameObject>();
        [SerializeField] protected AudioSource shootingAudio;
        [NonSerialized] public bool isDead;

        protected void Awake() => player = FindObjectOfType<PlayerManager>().transform; 
        protected override void Start()
        {
            states.Add(AgentStates.Shoot, Shoot);
            ChangeState(AgentStates.Shoot);
            base.Start();

            shootingAudio = GetComponent<AudioSource>();
            for(int i = 0; i < ammoAmount; i++)
            {
                string resourcesPath = "Projectiles/standard";
                projectiles.Add(Instantiate(Resources.Load<GameObject>(resourcesPath), ammoPool));
                projectiles[i].SetActive(false);
            }
        }

        private int index2 = 0;
        [SerializeField] private Transform ammoPool;

        private void Shoot()
        {
            if(Vector2.Distance(player.position, transform.position) <= radius && !player.GetComponent<PlayerManager>().isDead)
            {
                timer += Time.deltaTime;
                if(timer >= timerMax)
                {
                    var hit = Physics2D.Linecast(transform.position, player.position, dontDetectPlayer);
                    var color = Color.green;
                    if(hit.collider) color = Color.red;
                    if(!hit.collider && projectiles.Count > 0)
                    {
                        if(index2 >= projectiles.Count) index2 = 0;
                        if(!projectiles[index2].activeSelf)
                        {
                            var projectile = projectiles[index2];

                            projectile.GetComponent<Animator>().enabled = false;
                            projectile.SetActive(true);
                            projectile.transform.position = transform.position;
                        
                            var rb = projectile.GetComponent<Rigidbody2D>();
                            rb.velocity = Vector2.ClampMagnitude(rb.velocity, 0);

                            //var pRb = player.GetComponent<Rigidbody2D>();
                            var dir = ((player.position) - transform.position).normalized;
                            //var dir2 = ((player.position + (PlayerVelocity(pRb.velocity) * 0.5f)) - transform.position).normalized;

                            //var usedDir = index % 2 == 0 ? dir : dir2;
                            rb.AddForce(dir * projectileForce);
                            shootingAudio.Play();
                        } 
                        index2++;
                    }
                    Debug.DrawLine(transform.position, player.position, color);
                    timer = 0;
                }
            }
        }
        /// <summary> Converts the 2D velocity into a vector3 </summary>
        /// <param name="_playerVel">the players rb velocity</param>
        private Vector3 PlayerVelocity(Vector2 _playerVel) => new Vector3(_playerVel.x, _playerVel.y, transform.position.z);
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual void KillMe()
        {
            isDead = true;
            var obj = Resources.Load<GameObject>("Effects/eExplosion");
            Instantiate(obj, transform.position, transform.localRotation);
            Debug.Log("HitMe: dead");
            gameObject.SetActive(false);
        }
    }
}