using Gravitron.Player;

using System.Collections.Generic;

using UnityEngine;

namespace Gravitron.AI
{
    public class BaseTurret : BaseAgent
    {
        [SerializeField] private LayerMask dontDetectPlayer;
        private Transform player;
        
        [Header("Debugging")]
        [SerializeField] private float timerMax;
        [SerializeField] private float radius;
        [SerializeField] private float timer;
        [SerializeField] private float projectileForce;
        [SerializeField] private float offset;
        [SerializeField] private int ammoAmount;
        [SerializeField] private List<GameObject> projectiles = new List<GameObject>();

        protected override void Start()
        {
            states.Add(AgentStates.Shoot, Shoot);
            player = FindObjectOfType<PlayerManager>().transform;
            ChangeState(AgentStates.Shoot);
            base.Start();

            for(int i = 0; i < ammoAmount; i++)
            {
                string resourcesPath = "Projectiles/standard";
                projectiles.Add(Instantiate(Resources.Load<GameObject>(resourcesPath), transform));
                projectiles[i].SetActive(false);
            }
        }

        private int index = 0;
        private void Shoot()
        {
            if(Vector2.Distance(player.position, transform.position) <= radius)
            {
                timer += Time.deltaTime;
                if(timer >= timerMax)
                {
                    var hit = Physics2D.Linecast(transform.position, player.position, dontDetectPlayer);
                    var color = Color.green;
                    if(hit.collider) color = Color.red;
                    if(!hit.collider && projectiles.Count > 0)
                    {
                        if(index >= projectiles.Count) index = 0;
                        if(!projectiles[index].activeSelf)
                        {
                            var projectile = projectiles[index];

                            projectile.GetComponent<Animator>().enabled = false;
                            projectile.SetActive(true);
                            projectile.transform.position = transform.position;
                        
                            var rb = projectile.GetComponent<Rigidbody2D>();
                            rb.velocity = Vector2.ClampMagnitude(rb.velocity, 0);

                            var pRb = player.GetComponent<Rigidbody2D>();
                            var dir = ((player.position * offset) - transform.position).normalized;
                            var dir2 = ((player.position + PlayerVelocity(pRb.velocity)) - transform.position).normalized;

                            var usedDir = index % 2 == 0 ? dir : dir2;
                            rb.AddForce(usedDir * projectileForce);
                        } 
                        index++;
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
    }
}