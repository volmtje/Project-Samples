using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonCalculator
{
	private Dictionary<Vector2, List<Vector2>> polygons = new Dictionary<Vector2, List<Vector2>>();
	private List<Vector2> centerPoints;
	Vector2 screenSize;

    public PolygonCalculator(int numberOfPoints)
    {
		screenSize.x = Screen.width;
		screenSize.y = Screen.height;

		MainForm form = new MainForm(Vector2.zero, screenSize, numberOfPoints);
		centerPoints = form.points;

		foreach (Vector2 p in centerPoints)
			polygons.Add(p, new List<Vector2>());

		ProcessPoints();
		SortPolygonCorners();
	}

	public Dictionary<Vector2, List<Vector2>> GetPolygons()
	{
		return polygons;
	}

	private void ProcessPoints()
    {
		//match all start and end points of edges to center points
		foreach (VoronoiEdge e in VoronoiEdge.edges) 
		{
			CheckCutEdge(e); //cut the edge where it leaves the screen
			MatchPointToCenter(e.point1);
			MatchPointToCenter(e.point2);
		}

		//add the corners of the screen to the according polygons
		MatchPointToCenter(Vector2.zero);
		MatchPointToCenter(new Vector2(0, screenSize.y));
		MatchPointToCenter(new Vector2(screenSize.x, 0));
		MatchPointToCenter(screenSize);
	}


	//this function finds all centerpoints associated with the given point by their proximity
	private void MatchPointToCenter(Vector2 point)
    {
		float minDist = 100000;
		List<Vector2> closestPoints = new List<Vector2>();

		//find the closest points to the corner
		foreach (Vector2 p in centerPoints)
		{
			float pToE = (point - p).magnitude;
			float difference = minDist - pToE;

			if (Mathf.Abs(difference) < 0.0001f)   //add points with the same distance to the corner
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


	// this function Sets Up Edges to be cut, that go beyond the edges of the scren
	private void CheckCutEdge(VoronoiEdge edge)
    {
		//check for points outside of screen
		Vector2 dir = Vector2.zero;
		if (PointIsOutsideScreen(edge.point1, out dir))
			edge.point1 = CutEdge(edge, dir);
		if(PointIsOutsideScreen(edge.point2, out dir))
			edge.point2 = CutEdge(edge, dir);
	}

	//this function finds the side at which the line leaves the screen and replaces its endpoint with the point on the screen
	private Vector2 CutEdge(VoronoiEdge edge, Vector2 dir)
    {
		//screen corner coordinates
		Vector2 ll = new Vector2(0, 0);
		Vector2 lr = new Vector2(screenSize.x, 0);
		Vector2 ul = new Vector2(0, screenSize.y);
		Vector2 ur = screenSize;

		Vector2 intersection;
		if (dir.x != 0)
		{
			if (dir.x == 1)		//intersects right screen edge
				intersection = GetIntersection(edge.point1, edge.point2, lr, ur);
			else				//intersects left screen edge
				intersection = GetIntersection(edge.point1, edge.point2, ll, ul);

			if (!PointIsOutsideScreen(intersection)) //make sure the intersection was not on the extended screen edge
				return intersection;
		}
		if (dir.y != 0)
		{
			if (dir.y == 1)		//top screen edge
				intersection = GetIntersection(edge.point1, edge.point2, ul, ur);
			else				//bottom screen edge
				intersection = GetIntersection(edge.point1, edge.point2, ll, lr);

			if (!PointIsOutsideScreen(intersection))
				return intersection;
		}

		throw new Exception("error - no intersection found");
	}

	//returns true if given point is not within the bounds of the screen
	private bool PointIsOutsideScreen(Vector2 point) 
	{
		return (point.x < 0) || (point.x > screenSize.x) || (point.y < 0) || (point.y > screenSize.y);
	}

	// returns true if given point is not within the bounds of the screen;
	// direction includes the directions in which the edge might cut the side of the screen
	private bool PointIsOutsideScreen(Vector2 point, out Vector2 direction)
    {
		direction = Vector2.zero;

		if (point.x < 0)
			direction.x = -1;
		else if (point.x > screenSize.x)
			direction.x = 1;

		if (point.y < 0)
			direction.y = -1;
		else if (point.y > screenSize.y)
			direction.y = 1;

		if (direction.magnitude > 0) //the point lies outside the screen 
			return true;
		else
			return false;

    }

	//this function returns the intersection of two infinit lines, defined by p1-p2 and p3-p4
	private Vector2 GetIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
		Vector2 intersection = new Vector2();

		intersection.x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
		intersection.y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));

		return intersection;
	}

	//this function sorts the corner vectors in all polygon lists by the angle relative to the according center point
	private void SortPolygonCorners()
    {
		//sort corners clockwise
		Dictionary<Vector2, List<Vector2>> tempPolygons = new Dictionary<Vector2, List<Vector2>>();

		foreach (Vector2 key in polygons.Keys)
			tempPolygons.Add(key, polygons[key].OrderByDescending(x => Vector2.SignedAngle(key, x)).ToList());

		polygons = tempPolygons;
	}
}