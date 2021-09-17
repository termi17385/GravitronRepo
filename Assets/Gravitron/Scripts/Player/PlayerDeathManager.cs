using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathManager : MonoBehaviour
{
	[SerializeField] private GameObject particle1, particle2;
	[SerializeField] private float waitTime1, waitTime2;

	private void Awake() => StartCoroutine(ParticleManager());

	IEnumerator ParticleManager()
	{
		yield return new WaitForSeconds(waitTime1);
		particle1.SetActive(true);
		
		yield return new WaitForSeconds(waitTime1);
		particle2.SetActive(true);
		
		yield return new WaitForSeconds(waitTime2);
		Destroy(gameObject);
	}
}

