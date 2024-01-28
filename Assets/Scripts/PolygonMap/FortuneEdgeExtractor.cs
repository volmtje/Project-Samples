using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class translates the VoronoiEdges of a given fortuneVoronoiGraph to Edges usable for polygon map calculation
public class FortuneEdgeExtractor 
{
	Vector2 screenSize;

	// This function translates the fortunes VoronoiEdges to screen fit Edges
	public List<Edge> GetEdges(FortunesVoronoiGraph fortune, Vector2 screenSize)
    {
		this.screenSize = screenSize;
		List<Edge> edges = new List<Edge>();

		foreach (VoronoiEdge ve in VoronoiEdge.edges)
        {
			Edge e = new Edge(ve.point1, ve.point2);
			// Make sure the edge stops where the screen ends
			CheckCutEdge(e);
			edges.Add(e);
		}
		
		return edges;
	}


	// This function sets Up Edges that go beyond the edges of the scren, to be cut
	private void CheckCutEdge(Edge edge)
	{
		//check for points outside of screen
		Vector2 dir = Vector2.zero;
		if (PointIsOutsideScreen(edge.point1, out dir))
			edge.point1 = CutEdge(edge, dir);
		if (PointIsOutsideScreen(edge.point2, out dir))
			edge.point2 = CutEdge(edge, dir);
	}


	// This function replaces a point outside the screen with its edges last point on the screen
	private Vector2 CutEdge(Edge edge, Vector2 dir)
	{
		//screen corner coordinates
		Vector2 ll = new Vector2(0, 0);
		Vector2 lr = new Vector2(screenSize.x, 0);
		Vector2 ul = new Vector2(0, screenSize.y);
		Vector2 ur = screenSize;

		Vector2 intersection;
		if (dir.x != 0)
		{
			if (dir.x == 1)     //intersects right screen edge
				intersection = GetIntersection(edge.point1, edge.point2, lr, ur);
			else                //intersects left screen edge
				intersection = GetIntersection(edge.point1, edge.point2, ll, ul);

			if (!PointIsOutsideScreen(intersection)) //make sure the intersection was not on the extended screen edge
				return intersection;
		}
		if (dir.y != 0)
		{
			if (dir.y == 1)     //top screen edge
				intersection = GetIntersection(edge.point1, edge.point2, ul, ur);
			else                //bottom screen edge
				intersection = GetIntersection(edge.point1, edge.point2, ll, lr);

			if (!PointIsOutsideScreen(intersection))
				return intersection;
		}

		throw new Exception("error - no intersection found");
	}


	// This function returns true if the given point is not within the bounds of the screen.
	private bool PointIsOutsideScreen(Vector2 point)
	{
		return (point.x < 0) || (point.x > screenSize.x) || (point.y < 0) || (point.y > screenSize.y);
	}


	// This function returns true if the given point is not within the bounds of the screen.
	// Direction will hold the directions in which the edge might cut the side of the screen
	private bool PointIsOutsideScreen(Vector2 point, out Vector2 direction)
	{
		// Direction in which the point exceedes the bound of the screen
		direction = Vector2.zero;

		if (point.x < 0)	//left
			direction.x = -1;
		else if (point.x > screenSize.x) //right
			direction.x = 1;

		if (point.y < 0) // bottom
			direction.y = -1;
		else if (point.y > screenSize.y) //top
			direction.y = 1;

		if (direction.magnitude > 0) //the point lies outside the screen 
			return true;
		else
			return false;
	}


	// This function returns the intersection of two infinit lines, defined by p1-p2 and p3-p4
	private Vector2 GetIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
	{
		Vector2 intersection = new Vector2();

		intersection.x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
		intersection.y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));

		return intersection;
	}
}


public class Edge
{
	// Endpunkte der Kante
	public Vector2 point1;
	public Vector2 point2;

	public Edge(Vector2 pointOne, Vector2 pointTwo)
	{
		point1 = pointOne;
		point2 = pointTwo;
	}
}