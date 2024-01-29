using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PolygonCalculator
{
	// This list will be filld by this class. A polygon consists of a center point and a List of corners
	private Dictionary<Vector2, List<Vector2>> polygons = new Dictionary<Vector2, List<Vector2>>();

	// Each centerpoint belongs to one polygon
	private List<Vector2> centerPoints;
	Vector2 screenSize;


	public PolygonCalculator(Vector2 screenSize, List<Vector2> points, List<Edge> edges)
	{
		this.screenSize = screenSize;
		centerPoints = points;

		foreach (Vector2 p in centerPoints)
			polygons.Add(p, new List<Vector2>());

		GetCornersFromEdges(edges);
		SortPolygonCorners();
	}


	public Dictionary<Vector2, List<Vector2>> GetPolygons()
	{
		return polygons;
	}


	// This function passes all neccessary points on screen to be matched as corner points
	private void GetCornersFromEdges(List<Edge> edges)
	{
		//match all start and end points of edges to center points
		foreach (Edge e in edges)
		{
			MatchPointToCenter(e.point1);
			MatchPointToCenter(e.point2);
		}

		//add the corners of the screen to the according polygons
		MatchPointToCenter(Vector2.zero);
		MatchPointToCenter(new Vector2(0, screenSize.y));
		MatchPointToCenter(new Vector2(screenSize.x, 0));
		MatchPointToCenter(screenSize);
	}


	// This function finds all centerpoints associated with the given point by their proximity.
	private void MatchPointToCenter(Vector2 point)
	{
		float minDist = 100000;
		List<Vector2> closestPoints = new List<Vector2>();

		//find the closest points to the corner
		foreach (Vector2 p in centerPoints)
		{
			float pToE = (point - p).magnitude;
			float difference = minDist - pToE;

			if (Mathf.Abs(difference) < 0.009f)   //add points with the same distance to the corner
			{
				closestPoints.Add(p);
				continue;
			}
			else if (difference > 0)        //a closer point has been found
			{
				minDist = pToE;
				closestPoints.Clear();
				closestPoints.Add(p);
			}
		}

		foreach (Vector2 p in closestPoints)
		{
			Vector2 corner = point - p; //set position relative to center point
			if (!polygons[p].Contains(corner))
				polygons[p].Add(corner);
		}
	}


	// This function sorts the corner vectors in all polygon lists by the angle relative to the according center point
	private void SortPolygonCorners()
	{
		//sort corners clockwise
		Dictionary<Vector2, List<Vector2>> tempPolygons = new Dictionary<Vector2, List<Vector2>>();

		foreach (Vector2 key in polygons.Keys)
			tempPolygons.Add(key, polygons[key].OrderByDescending(x => x, new SignedAngleComparer()).ToList());

		polygons = tempPolygons;
	}
}

public class SignedAngleComparer : IComparer<Vector2>
{
    public int Compare(Vector2 x, Vector2 y)
    {
		float xAngle = Vector2.SignedAngle(Vector2.one, x);
		float yAngle = Vector2.SignedAngle(Vector2.one, y);

		if (xAngle < 0)
			xAngle = 180 + (180 - Mathf.Abs(xAngle));
        if (yAngle < 0)
            yAngle = 180 + (180 - Mathf.Abs(yAngle));

        return xAngle.CompareTo(yAngle);
    }
}