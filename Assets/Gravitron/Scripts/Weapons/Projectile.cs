using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
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

	private void OnDisable()
	{
	}

	/// <summary> Handles what happens if the
	/// projectile hits an object </summary>
	private void HitObjectDisable()
	{
		StopCoroutine(Despawn());
	}
	
	IEnumerator Despawn()
	{
		yield return new WaitForSeconds(5);
		var anim = GetComponent<Animator>();
		anim.enabled = true;
		
		yield return new WaitForSeconds(2);
		gameObject.SetActive(false);
	}	
}
