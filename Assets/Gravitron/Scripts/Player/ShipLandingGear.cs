using System;
using UnityEngine;

namespace Gravitron.Player
{
	[RequireComponent(typeof(PlayerController))]
	public class ShipLandingGear : MonoBehaviour
	{
		[SerializeField] private LayerMask groundOnly;
		[SerializeField] private float gravScale;

	#region Dont Touch

		public bool hasLanded;
		private PlayerController pController;
		private PlayerManager pManager;

		[Header("Landing Gear Debugging")]
		[SerializeField] private bool leftGear;
		[SerializeField] private bool rightGear;
	
		[SerializeField] private float downOffset = 0.2f;
		[SerializeField] private float offset = 0.22f;
		[SerializeField] private float dist = 0.07f;
	
		[SerializeField] private float maxRotationDegrees;
		[SerializeField] private bool unaligned;

	#endregion

		private void Awake()
		{
			pController = GetComponent<PlayerController>();
			pManager = GetComponent<PlayerManager>();
			Time.timeScale = 1;
		}
		
		private void Update()
		{
			if(pController.isFlying) transform.SetParent(null);
			hasLanded = LandingGear() && !pController.isFlying;
			if(!hasLanded)
			{
				pController.Rotate();
				unaligned = true;
			}
			//ShipHasLanded();
		}
		/// <summary> freezes the ship in place after
		/// it has landed so it doesnt move </summary>
		
		private void ShipHasLanded()
		{
			var rb = pController.rb2d;
			rb.velocity = Vector2.ClampMagnitude(rb.velocity, 0); // stops the ship in place
			rb.isKinematic = true;                                // makes sure the ship doesnt move
		
			//if(transform.parent == null) 
			//{
			//	var hit = Physics2D.Raycast(transform.position, transform.up * -1, 1, groundOnly);
			//	transform.SetParent(hit.collider.transform);
			//}
		}
		
		private bool reScale;
		/// <summary> Aligns the ship based on the raycast data from the landing gear.
		/// gets the average of the landing gear normal and aligns the ship accordingly </summary>
		/// <param name="_left">left gear raycast info</param>
		/// <param name="_right">right gear raycast info</param>
		private void AlignShip(RaycastHit2D _left, RaycastHit2D _right)
		{
			// checks if the ship has landed and if it is unaligned
			if(hasLanded)
			{
				if(!unaligned) ShipHasLanded(); // if the ship is no longer unaligned
				if(!reScale)                    // if the ship hasnt been rescaled 
				{
					// this is used to make sure there is no scaling issues when parented
					var scale = transform.localScale;
					scale.x = scale.y = scale.z = 1;
					transform.localScale = scale;
					reScale = true;
				}

				// get the average of the normal from the rays then orient the ship respectively
				Vector3 averageNormal = (_left.normal + _right.normal) / 2;
				// sets the target rotation to the up vector of the average normal
				Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
				// sets the final rotation to rotate from the transform rotation to the target rotation
				Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationDegrees);
				// rotates the ship then sets unaligned to false
				transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);
				unaligned = false;
			}
			else
			{
				pController.rb2d.isKinematic = false;
				pController.rb2d.gravityScale = gravScale;
			
				var scale = transform.localScale;
				scale.x = scale.y = scale.z = 1;
				transform.localScale = scale;

				reScale = false;
			}
		}
		
		/// <summary> Detects when the ship lands on the ground then
		/// sends the raycast data to the align method </summary>
		private bool LandingGear()
		{
			#region Position offsets
			var down = transform.up * -downOffset;
			var left = transform.right * -offset;
			var right = transform.right * offset;
		
			var position = left + down;
			var position2 = right + down;
			#endregion
		
			var leftHit = Physics2D.Raycast(transform.position + position, transform.up * -dist, dist,groundOnly);
			var rightHit = Physics2D.Raycast(transform.position + position2, transform.up * -dist, dist,groundOnly);
			AlignShip(leftHit, rightHit);

			// mostly for debugging
			leftGear = leftHit;
			rightGear = rightHit;
			return leftGear && rightGear;
			//var rb = pController.rb2d;
			//var flying = pController.isFlying;
			//RaycastHit2D hit2D = Physics2D.Raycast(transform.position, -transform.up, rayDist, groundOnly);
			//if(hit2D)
			//{
			//	Debug.DrawRay(transform.position, -transform.up * rayDist, Color.black);
			//	hasLanded = rb.velocity.magnitude <= 1;
			//	Debug.Log("Grounded");
			//}
			//else
			//	hasLanded = false;

			//if(hasLanded && !flying)
			//{
			//	StartCoroutine(WaitForSecondsBeforeClamp());
			//	if(clamp)
			//	{
			//		rb.gravityScale = 0;
			//		rb.velocity = Vector2.ClampMagnitude(Vector2.zero, 0);
			//	}
			//}
			//else if(!hasLanded || flying)
			//{
			//	rb.gravityScale = .1f;
			//	clamp = false;
			//}
		}

		private void OnCollisionEnter2D(Collision2D _other) {
			if(_other.collider.CompareTag("Ground"))
			{
				if(!hasLanded)
				{
					GetComponent<PolygonCollider2D>().sharedMaterial.bounciness = 1;
					pManager.DamagePlayer(5);
				}
			}
			else if(hasLanded)
			{
				pManager.groundedDamage = false;
				GetComponent<PolygonCollider2D>().sharedMaterial.bounciness = 0;
				pManager.count = 0;					
			}
		}
		private void OnCollisionStay2D(Collision2D _other)
		{
			if(_other.collider.CompareTag("Ground"))
				pManager.groundedDamage = !hasLanded;
		}
		private void OnCollisionExit2D(Collision2D _other) {
			if(_other.collider.CompareTag("Ground"))
				if(!hasLanded)
				{
					pManager.groundedDamage = false;
					GetComponent<PolygonCollider2D>().sharedMaterial.bounciness = 1;
					pManager.count = 0;
				}
		}

		private void OnDrawGizmos()
		{
			var down = transform.up * -downOffset;
		
			var left = transform.right * -offset;
			var right = transform.right * offset;
		
			var position = left + down;
			var position2 = right + down;
		
			Gizmos.DrawRay(transform.position + position, transform.up * -dist);
			Gizmos.DrawRay(transform.position + position2, transform.up * -dist);
		}
	}
}