using UnityEngine;

namespace Gravitron.Player
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerController : MonoBehaviour
	{
		[Header("Thruster")]
		[SerializeField] private float thrust = 100;
		[SerializeField] private ParticleSystem thruster;
		[Header("Speed variables")]
		[SerializeField] private float rotationalSpeed = 1;
		[SerializeField] private float maxSpeed;
		[Header("Misc"), Space]
		public Rigidbody2D rb2d;
		public bool isFlying;

		private void Awake() => rb2d = GetComponent<Rigidbody2D>();
		private void FixedUpdate() { if(isFlying) Flight(); }

		private void Update()
		{
			isFlying = Input.GetKey(KeyCode.W);
			if(isFlying)
			{
				thruster.Play();
				rotationalSpeed = 150;
			}
			else rotationalSpeed = 250;
		
		#pragma warning disable 618
			thruster.loop = isFlying;
		#pragma warning restore 618
		}
	
		/// <summary> Handles rotating
		/// the player when a or d is pressed </summary>
		public void Rotate()
		{
			var x = Input.GetAxis("Horizontal") * rotationalSpeed;
			transform.Rotate(transform.forward, -x * Time.deltaTime);
		}
		/// <summary> Handles flight and clamping
		/// of the players speed </summary>
		private void Flight()
		{
			if(rb2d.velocity.magnitude > maxSpeed) rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxSpeed);
			rb2d.AddForce(transform.up * thrust);
		}
	}
}