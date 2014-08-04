using UnityEngine;
using System.Collections.Generic;

public class NavPathDrawer : MonoBehaviour
{
	private TreantNavPathCalculator navPathCalculator;

	private void Start ()
	{
		navPathCalculator = GetComponent<TreantNavPathCalculator> ();
	}

	private void Update ()
	{
		foreach (NavMeshPath path in navPathCalculator.Paths)
			DrawPath (path);
	}

	private void OnDrawGizmos ()
	{
		if (navPathCalculator != null && navPathCalculator.Target != Vector3.zero)
			DrawTarget (navPathCalculator.Target);
	}
	
	private void DrawPath (NavMeshPath path)
	{
		for (int i = 1; i < path.corners.Length; i++)
		{
			Vector3 previousCorner = path.corners [i - 1];
			Vector3 currentCorner = path.corners [i];
			Debug.DrawLine (previousCorner, currentCorner, Color.green);
		}
	}

	private void DrawTarget (Vector3 target)
	{
		Gizmos.DrawWireSphere (target, 0.4f);
	}
}
