using Gravitron.Player;

using System;

using UnityEngine;

public class Objective : MonoBehaviour
{
	private float health;
	private float maxHealth = 100;

	private float timer;
	private float maxTime = 2;
	
	[SerializeField] private GameObject healthBar;

	private void Start()
	{
		health = maxHealth;
		RescaleBar();
	}

	private void Update()
	{
		healthBar.SetActive(!(health >= 100));
		RescaleBar();

		timer += Time.deltaTime;
		if(timer >= maxTime)
		{
			timer = maxTime;
			Heal(20);
		}
	}

	private float SetHealth(float _health)
	{
		return Mathf.Clamp01(_health / maxHealth);
	}
	private void RescaleBar()
	{
		var scale = healthBar.transform.localScale;
		scale.x = SetHealth(health); healthBar.transform.localScale = scale;
	}
	private void Heal(float _amt)
	{
		health += _amt * Time.deltaTime;
		if(health >= maxHealth) health = maxHealth;
	}
	private void DestoryMe()
	{
		var path = "Effects/ParticleShower";
		var obj = Instantiate(Resources.Load<GameObject>(path));
		obj.transform.position = transform.position;
		obj.SetActive(true);

		PlayerManager.objectiveDestoryed = true;
		gameObject.SetActive(false);
	}
	
	public void Damage(float _amt)
	{
		timer = 0;
		health -= _amt;
		if(health <= 0)
		{
			health = 0;
			DestoryMe();
		}
	}
	
}
