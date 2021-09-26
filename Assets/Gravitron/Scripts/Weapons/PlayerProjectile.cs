using Gravitron.AI;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private Camera cameraPlayer;
	[SerializeField] private Renderer renderer;
	[SerializeField] private Rigidbody2D rb;

	private readonly string path = "Effects/pExplosion";
	
	private void OnEnable()
	{
		StartCoroutine(WaitThenDestroy());
		
		cameraPlayer = Camera.main;
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnDisable()
	{
		rb.velocity = Vector2.ClampMagnitude(rb.velocity, 0);
	}

	private void LateUpdate()
	{
		if(!CheckRenderers())
		{
			StopCoroutine(WaitThenDestroy());
			gameObject.SetActive(false);
		}
	}
	private bool CheckRenderers() => IsVisibleFrom(renderer, cameraPlayer);
	private bool IsVisibleFrom(Renderer _renderer, Camera _camera)
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);
		return GeometryUtility.TestPlanesAABB(planes, _renderer.bounds);
	}
	
	private void FixedUpdate() => rb.velocity = transform.up * speed;			
	private void OnCollisionEnter2D(Collision2D _other)
	{
		if(_other.collider.gameObject.CompareTag("Enemy"))
		{
			var obj = _other.gameObject;
			if(obj.TryGetComponent(out BaseTurret turret))
			{
				turret.KillMe();
			}

			if(obj.TryGetComponent(out Objective objective))
			{
				objective.Damage(10);
			}
		}
		ProjectileDeath();
	}

	private void ProjectileDeath()
	{
		StopCoroutine(WaitThenDestroy());
		var obj = Resources.Load<GameObject>(path);
		Instantiate(obj, transform.position, Quaternion.identity);
		gameObject.SetActive(false);
	}

	IEnumerator WaitThenDestroy()
	{
		yield return new WaitForSeconds(5);
		gameObject.SetActive(false);
	}
}
