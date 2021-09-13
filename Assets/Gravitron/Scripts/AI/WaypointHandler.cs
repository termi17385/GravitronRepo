using UnityEngine;
namespace Gravitron.AI
{
	public class WaypointHandler : MonoBehaviour
	{
		private const float DISTANCE_FROM_POINT = 0.05f;
	
		[Header("Artificial Intelligence")] 
		public Transform[] points = new Transform[2];
		private bool pointsGenerated;

		[Header("Parent Locations")] 
		[SerializeField] private string objectParent = "[AI]";
		[SerializeField] private string pointsParent = "[Points]";
		private Transform tObjectParent, tPointsParent;

		private void OnValidate() => SetupWaypoints();
		
		/// <summary> Sets up the waypoints when dragged into the scene
		/// mostly used for debugging to speed up development </summary>
		private void SetupWaypoints()
		{
			tPointsParent = transform.Find(pointsParent);
			tObjectParent = transform.Find(objectParent);
			
			for(int i = 0; i < points.Length; i++)
			{
				points[i].gameObject.name = "point" + i;
				points[i].SetParent(tPointsParent);
			}
		}

		public void HandleWaypoints(BaseAgent _agent)
		{
			if(_agent.gameObject.activeSelf) _agent.waypoint = points[_agent.index]; // sets the destination to the current waypoint
    
			// when the agent reaches the current waypoint
			// change to the next waypoint in the list
			if(Vector3.Distance(_agent.transform.position, points[_agent.index].position) < DISTANCE_FROM_POINT)
			{
				// if the agent reaches the end of the count it will then start going backwards down the list
				_agent.index += _agent.count;
				switch (_agent.index >= points.Length - 1)
				{
					case true: _agent.count = -1; break;
					default:
					{
						if (_agent.index <= 0)
						{
							_agent.count = 1;
						}
						break;
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			for(int i = 0; i < points.Length; i++)
			{
				if(points[i] != null)
				{
					var x = i;
					if(x < points.Length - 1) x++; 
				
					var lastPos = points[i].position;
					var nextPos = points[x].position;
				
					var color = Color.green;
					Gizmos.DrawIcon(lastPos, "", true, color);
				
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(lastPos, nextPos);
					lastPos.z = 0; points[i].position = lastPos;
				}
			}
		}
	}
}