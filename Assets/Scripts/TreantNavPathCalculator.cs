using UnityEngine;
using System.Collections.Generic;
using System;

public class TreantNavPathCalculator : MonoBehaviour
{
	private const string BUILDING_TAG = "Building";

	[SerializeField]
	private NavMeshAgent navMeshAgent;

	public List<NavMeshPath> Paths
	{
		get;
		private set;
	}
	
	public GameObject Target
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
		Target = GetNearestWallInPathOfBuildings (navMeshAgent, GetAllBuildings());
	}

	public GameObject GetNearestWallInPathOfBuildings (NavMeshAgent navMeshAgent, List<GameObject> buildings)
	{
		this.navMeshAgent = navMeshAgent;
		List<NavMeshPath> paths = CalculatePathsToBuildings (buildings);
		this.Paths = paths;
		OffMeshLink target = FindNearestPathWithAnOffMeshLink (paths);
		return GetAttackableElement (target);
	}
	
	private List<NavMeshPath> CalculatePathsToBuildings (List<GameObject> buildings)
	{
		List<NavMeshPath> paths = new List<NavMeshPath> ();

		foreach (GameObject building in buildings)
		{
			Transform targetPoint = GetBuildingNearestTargetPoint (building);;
			NavMeshPath path = CalculatePathToBuilding (targetPoint.position);
			paths.Add (path);
		}
		return paths;
	}

	private Transform GetBuildingNearestTargetPoint (GameObject building)
	{
		float minDistance = float.PositiveInfinity;
		Transform targetPoint = null;
		foreach (Transform tryTargetPoint in building.transform)
		{
			float distanceToPoint = Vector3.Distance (tryTargetPoint.position, navMeshAgent.transform.position);
			if (distanceToPoint < minDistance)
			{
				minDistance = distanceToPoint;
				targetPoint = tryTargetPoint;
			}
		}
		return targetPoint;
	}

	private NavMeshPath CalculatePathToBuilding (Vector3 targetPosition)
	{
		NavMeshPath navPath = new NavMeshPath ();
		navMeshAgent.CalculatePath (targetPosition, navPath);
		return navPath;
	}
	
	private OffMeshLink FindNearestPathWithAnOffMeshLink (List<NavMeshPath> paths)
	{
		OffMeshLink target = null;
		float minDistanceToOffMeshLink = float.PositiveInfinity;

		foreach (NavMeshPath path in paths)
			CheckIfIsTheNearestTarget (path, ref minDistanceToOffMeshLink, ref target);

		return target;
	}

	private void CheckIfIsTheNearestTarget (NavMeshPath path, ref float minDistanceToOffMeshLink, ref OffMeshLink target)
	{
		OffMeshLink offMeshLink = GetFirstOffMeshLink (path);
		float distance = GetPathDistance (path, offMeshLink);

		if (distance < minDistanceToOffMeshLink)
		{
			minDistanceToOffMeshLink = distance;
			target = offMeshLink;
		}
	}
	
	private float GetPathDistance (NavMeshPath path, OffMeshLink offMeshLink)
	{
		if (offMeshLink == null)
			return float.PositiveInfinity;
		else
			return GetDistanceBetweenPoints (path.corners);
	}

	private OffMeshLink GetFirstOffMeshLink (NavMeshPath path)
	{
		navMeshAgent.SetPath (path);
		OffMeshLink offMeshLink = navMeshAgent.nextOffMeshLinkData.offMeshLink;
		navMeshAgent.ResetPath ();
		return offMeshLink;
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

	private GameObject GetAttackableElement (OffMeshLink offMeshLink)
	{
		if (offMeshLink == null)
			return null;

		return offMeshLink.gameObject;
	}

	private List<GameObject> GetAllBuildings ()
	{
		GameObject[] buildings = GameObject.FindGameObjectsWithTag (BUILDING_TAG);
		return new List<GameObject> (buildings);
	}

	private void OnGUI ()
	{
		if (GUI.Button (new Rect (10,10,100,30), "Calculate"))
			CalculateTarget ();
	}
}
