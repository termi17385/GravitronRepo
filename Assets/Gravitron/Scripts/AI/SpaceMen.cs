using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMen : MonoBehaviour
{
	public Transform currentWaypoint;
	public int index;
	public int count;
	[SerializeField] private float speed = 1;

	private void Update()
	{
		FollowWaypoint();
	}

	private void FollowWaypoint() 
	{
		if(currentWaypoint!=null) 
			transform.position = Vector2.MoveTowards(transform.position, 
				currentWaypoint.position, (speed * Time.deltaTime)); 
	}

	public void BeamMeUpScotty()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		Destroy(gameObject, 5);
	}
}
