using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	[Header("Thruster")]
	[SerializeField] private float thrust = 100;
	[SerializeField] private float maxSpeed;
	[SerializeField] private ParticleSystem thruster;
	[Space]
	[SerializeField] private Rigidbody2D rb2d;

	private bool isFlying = false;
	
	private void Awake() => rb2d = GetComponent<Rigidbody2D>();
	private void FixedUpdate() { if(isFlying) Flight(); }
	private void Update()
	{
		isFlying = Input.GetKey(KeyCode.W);
		if(isFlying) thruster.Play();
		thruster.loop = isFlying;
		
		Rotate();
	}

	private void Rotate()
	{
		var x = Input.GetAxis("Horizontal") * 1;
		transform.Rotate(transform.forward, -x);
	}
	
	private void Flight()
	{
		if(rb2d.velocity.magnitude > maxSpeed) rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxSpeed);
		rb2d.AddForce(transform.up * thrust);
	}
}
