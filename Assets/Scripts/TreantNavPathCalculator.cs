using UnityEngine;
using System.Collections.Generic;

public class TreantNavPathCalculator : MonoBehaviour
{
	private const string BUILDING_TAG = "Building";
	private const string WALL_TAG = "Wall";

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	private List<NavMeshPath> paths = new List<NavMeshPath> ();
	public List<NavMeshPath> Paths
	{
		get { return paths; }
	}

	public Vector3 Target
	{
		get;
		private set;
	}

	private void Start ()
	{
		CalculateTarget ();
	}

	private void CalculateTarget ()
	{
		Target = Vector3.zero;
		CalculatePathsToAllBuildings ();
		Target = GetNearestPathWithAnOffMeshLink ();
	}

	private void CalculatePathsToAllBuildings ()
	{
		paths.Clear ();
		foreach (GameObject building in GetAllBuildings ())
		{
			Vector3 targetPosition = building.transform.position;
			NavMeshPath path = CalculatePathToBuilding (targetPosition);
			paths.Add (path);
		}
	}

	private Vector3 GetNearestPathWithAnOffMeshLink ()
	{
		float minDistanceToOffMeshLink = float.MaxValue;
		Vector3 targetPosition = Vector3.zero;

		foreach (NavMeshPath path in paths)
		{
			Vector3 position = GetNextOffMeshLinkPositionInPath (path);
			if (position != Vector3.zero)
			{
				float distance = GetDistanceBetweenPoints (path.corners);
				if (distance < minDistanceToOffMeshLink)
				{
					minDistanceToOffMeshLink = distance;
					targetPosition = position;
				}
			}
		}
		return targetPosition;
	}

	private Vector3 GetNextOffMeshLinkPositionInPath (NavMeshPath navPath)
	{
		navMeshAgent.SetPath (navPath);
		Vector3 nextOffMeshLinkPosition = navMeshAgent.nextOffMeshLinkData.startPos;
		navMeshAgent.ResetPath ();
		return nextOffMeshLinkPosition;
	}

	private NavMeshPath CalculatePathToBuilding (Vector3 targetPosition)
	{
		NavMeshPath navPath = new NavMeshPath ();
		navMeshAgent.CalculatePath (targetPosition, navPath);
		return navPath;
	}

	private float GetDistanceBetweenPoints (Vector3[] corners)
	{
		float distance = 0;
		for (int i = 1; i < corners.Length; i++)
		{
			Vector3 previousCorner = corners [i - 1];
			Vector3 currentCorner = corners [i];
			distance += Vector3.Distance (previousCorner, currentCorner);
		}
		return distance;
	}

	private GameObject[] GetAllBuildings ()
	{
		return GameObject.FindGameObjectsWithTag (BUILDING_TAG);
	}

	private void OnGUI ()
	{
		if (GUI.Button (new Rect (10,10,100,30), "Calculate"))
			CalculateTarget ();
	}
}
