using Gravitron.Player;

using System.Collections.Generic;
using UnityEngine;

namespace Gravitron.Old
{
	public class Waypoints : MonoBehaviour
	{
		[Header("Waypoints and SpaceMens")]
		[SerializeField] private List<SpaceMen> spaceMens = new List<SpaceMen>();
		[SerializeField] private Transform[] points = new Transform[2];
		[SerializeField] private float distance;
	
		[Header("Parent Locations")]
		[SerializeField] private Transform spacemensParent;
		[SerializeField] private Transform pointsParent;
	
		[Header("debugging")]
		[SerializeField] private bool populateArray;
		[SerializeField] private bool destroy;
	
	#region Private Variables
		// private
		private Transform ship;
	
		[SerializeField] private bool shipLanded;
		private bool boarding;

		private int boardingCount = 0;
		private bool runOnce = false;
	#endregion

		private void Update()
		{
			if(ship != null) shipLanded = ship.GetComponent<ShipLandingGear>().hasLanded;
		
			if(!shipLanded) HandleWaypoints();
			else if(shipLanded) PickUpUnits();
		}


		private void OnTriggerStay2D(Collider2D _other)
		{
			if(_other.gameObject.CompareTag("Player"))
			{
				ShipLandingGear shipLandingGear = _other.GetComponent<ShipLandingGear>();
				if(shipLandingGear.hasLanded)
				{
					Debug.Log("Has Landed");
					ship = shipLandingGear.transform;
					shipLanded = true;
				}
			}
		}
		private void OnTriggerExit2D(Collider2D _other)
		{
			if(_other.gameObject.CompareTag("Player"))
			{
				ShipLandingGear shipLandingGear = _other.GetComponent<ShipLandingGear>();
				if(shipLandingGear.hasLanded)
				{
					Debug.Log("Has Landed");
					ship = null;
					shipLanded = false;
				}
			}
		}

		/// <summary>
		/// Handles the assigning of waypoints individually for each agent
		/// </summary>
		private void HandleWaypoints()
		{
			foreach (var t in spaceMens)
			{
				var script = t.GetComponent<SpaceMen>();                              // gets the agents index
				if(t.gameObject.activeSelf) t.currentWaypoint = points[script.index]; // sets the destination to the current waypoint
            
				// when the agent reaches the current waypoint
				// change to the next waypoint in the list
				if(Vector3.Distance(t.transform.position, points[script.index].position) < distance)
				{
					// if the agent reaches the end of the count it will then start going backwards down the list
					script.index += t.count;
					switch (script.index >= points.Length - 1)
					{
						case true:
							t.count = -1;
							//break;
							break;
						default:
						{
							if (script.index <= 0)
							{
								t.count = 1;
							}

							break;
						}
					}
				}
			}
		}
	
		/// <summary>
		/// Handles picking up the agents when the ship has landed in range
		/// </summary>
		private void PickUpUnits()
		{
			SpaceMen currentSpaceMen = null;
			if(spaceMens.Count > 0) currentSpaceMen = spaceMens[boardingCount];
			if(boardingCount >= spaceMens.Count) boardingCount = spaceMens.Count - 1;
			
			boarding = true;
			if(boarding && currentSpaceMen != null)
			{
				//currentSpaceMen.currentWaypoint = ship.GetComponent<ShipLandingGear>().dockingPort;
				var menPos = currentSpaceMen.transform.position;
				var dist = Vector2.Distance(menPos, ship.position);
		
				if(dist <= 0.2f)
				{
					currentSpaceMen.BeamMeUpScotty();
					//ship.GetComponent<ShipLandingGear>().SpaceMenPickedUp();
					spaceMens.Remove(currentSpaceMen);
					boarding = false;
				}
			}
			else
			{
				boardingCount++;
				//boardingComplete = spaceMens.Count <= 0;
			}
		}
	
		private void OnDrawGizmos()
		{
			if(points[0] != null || points[1] != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(points[0].position, points[1].position);

				foreach(var point in points)
				{
					Gizmos.DrawIcon(point.position, "");
				}
			}

			if(spaceMens.Count > 0)
				foreach(var dude in spaceMens)
					dude.transform.SetParent(spacemensParent);
		
			if(populateArray)
			{
				if(destroy)
				{
					if(points[0] || points[1])
					{
						var go = points[0].gameObject;
						points[0] = null;
						DestroyImmediate(go);
				
						go = points[1].gameObject;
						points[1] = null;
						DestroyImmediate(go);
					
						destroy = false;
					}
				}
				Transform obj = new GameObject().transform;
				Transform obj2 = new GameObject().transform;
		
				points[0] = obj;
				points[1] = obj2;
			
				obj.SetParent(pointsParent);
				obj2.SetParent(pointsParent);

				obj2.name = obj.name = "DESTROY IN INSPECTOR";
				obj2.transform.position = obj.transform.position = transform.position;
				populateArray = false;
			}
		}
	}
}