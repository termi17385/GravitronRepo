using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class SpaceShip : MonoBehaviour
{
	[Header("GroundChecking")] [SerializeField] private float rayDist;
	[SerializeField] private LayerMask groundOnly;
	public Transform dockingPort;

	[Header("Ship Inventory")] public int spaceMen = 0;

	[Header("Stats")] public float health = 100;
	public float fuel = 100;

	private float maxHealth = 100;
	private float maxFuel = 100;

	[SerializeField] private TextMeshProUGUI spaceMenTextTotal;
	[SerializeField] private TextMeshProUGUI spaceMenText;
	[SerializeField] private GameObject exitMenu;

#region Dont Touch

	[NonSerialized] public bool hasLanded;
	private PlayerController pController;
	private bool clamp;

#endregion

	private void Awake()
	{
		pController = GetComponent<PlayerController>();
		Time.timeScale = 1;
	}

	private void Update()
	{
		ShipLanded();
		if(!hasLanded)
			pController.Rotate();
	}

	public void SpaceMenPickedUp() => spaceMen++;

	private void OnCollisionEnter2D(Collision2D other)
	{
		if(other.collider.CompareTag("Finish"))
		{
			spaceMenTextTotal.text = $"={Scoring.ScoreHandler(ScoreTypes.SpaceMen, spaceMen)}";
			spaceMenText.text = $"{spaceMen}";

			exitMenu.SetActive(true);
			gameObject.SetActive(false);
			Time.timeScale = 0;
		}
	}


	/// <summary> Handles freezing the ship if it lands </summary>
	private void ShipLanded()
	{
		var rb = pController.rb2d;
		var flying = pController.isFlying;
		RaycastHit2D hit2D = Physics2D.Raycast(transform.position, -transform.up, rayDist, groundOnly);
		if(hit2D)
		{
			Debug.DrawRay(transform.position, -transform.up * rayDist, Color.black);
			hasLanded = rb.velocity.magnitude <= 1;
			Debug.Log("Grounded");
		}
		else
			hasLanded = false;

		if(hasLanded && !flying)
		{
			StartCoroutine(WaitForSecondsBeforeClamp());
			if(clamp)
			{
				rb.gravityScale = 0;
				rb.velocity = Vector2.ClampMagnitude(Vector2.zero, 0);
			}
		}
		else if(!hasLanded || flying)
		{
			rb.gravityScale = .1f;
			clamp = false;
		}
	}

	IEnumerator WaitForSecondsBeforeClamp()
	{
		yield return new WaitForSecondsRealtime(0.05f);
		clamp = true;
	}
}