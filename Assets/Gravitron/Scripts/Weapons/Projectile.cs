using System.Collections;
using Gravitron.Player;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private AudioSource deathSound;
	
	private void OnEnable()
	{
		var color = GetComponentsInChildren<SpriteRenderer>();
		foreach(var a in color)
		{
			Color aColor = a.color;
			aColor.a = 255;
			a.color = aColor;
		}
		StartCoroutine(Despawn());
	}

	private void OnCollisionEnter2D(Collision2D _other)
	{
		if(_other.collider.CompareTag("Player"))
		{
			var player = _other.gameObject.GetComponent<PlayerManager>();
			player.DamagePlayer(30);
		}

		if(!_other.collider.CompareTag("Enemy") && !_other.collider.CompareTag("Projectile"))
		{
			var rb = GetComponent<Rigidbody2D>();
			rb.velocity = Vector2.ClampMagnitude(rb.velocity, 0);
			
			var o = Instantiate(Resources.Load<GameObject>("Effects/Explosion"), transform.position, Quaternion.identity);
			deathSound = o.GetComponent<AudioSource>();
			deathSound.Play();
			
			HitObjectDisable();
			gameObject.SetActive(false);
		}
	}

	/// <summary> Handles what happens if the
	/// projectile hits an object </summary>
	private void HitObjectDisable() => StopCoroutine(Despawn());
	IEnumerator Despawn()
	{
		yield return new WaitForSeconds(5);
		var anim = GetComponent<Animator>();
		anim.enabled = true;
		
		yield return new WaitForSeconds(2);
		gameObject.SetActive(false);
	}	
}
