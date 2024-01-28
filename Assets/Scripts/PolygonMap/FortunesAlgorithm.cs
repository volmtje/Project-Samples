using System;
using System.Collections.Generic;
using UnityEngine;


// Diese Klasse implementiert eine Vergleichsmethode für das Sortieren der Punkte
class PointComparer : Comparer<Vector2>
{
	// Vergleichsmethode, die die Punkte von links nach rechts und bei Gleichheit von oben nach unten sortiert
	public override int Compare(Vector2 point1, Vector2 point2)
	{
		int result = point1.x.CompareTo(point2.x);
		if (result == 0)
		{
			return point1.y.CompareTo(point2.y);
		}
		return result;
	}
}

// Diese Klasse implementiert eine Vergleichsmethode für das Sortieren der CircleEvents
class EventComparer : Comparer<CircleEvent>
{
	// Vergleichsmethode, die die CircleEvents
	public override int Compare(CircleEvent event1, CircleEvent event2)
	{
		return event1.x.CompareTo(event2.x);
	}
}

// Klasse für die CircleEvents
class CircleEvent
{
	public double x; // x-Koordinate der Sweep Line
	public Vector2 point; // Das Zentrum, das der Brennpunkt der Parabel ist
	public ParabolaArc arc; // Parabelbogen
	public bool isValid; // Gibt an, ob das CircleEvent aktuell ist

	// Konstruktor für die Initialisierung der Attribute
	public CircleEvent(double _x, Vector2 _point, ParabolaArc _arc)
	{
		x = _x;
		point = _point;
		arc = _arc;
		isValid = true;
	}
};

// Klasse für die Parabelbögen
class ParabolaArc
{
	public Vector2 point; // Das Zentrum, das der Brennpunkt der Parabel ist
	public ParabolaArc previousArc, nextArc; // Benachbarte Parabelbögen
	public CircleEvent circleEvent; // Zugeordnetes CircleEvent
	public VoronoiEdge edge1, edge2; // Benachbarte Voronoi-Kanten

	// Konstruktor für die Initialisierung der Attribute
	public ParabolaArc(Vector2 _point, ParabolaArc arc1, ParabolaArc arc2)
	{
		{
			point = _point;
			previousArc = arc1;
			nextArc = arc2;
			circleEvent = null;
			edge1 = null;
			edge2 = null;
		}
	}
}

// Klasse für die Voronoi-Kanten
class VoronoiEdge
{
	public Vector2 point1, point2; // Endpunkte der Kante
	public bool isFinished; // Gibt an, ob die Kante fertiggestellt wurde
	public static List<VoronoiEdge> edges = new List<VoronoiEdge>(); // Liste der Kanten

	// Konstruktor für die Initialisierung der Attribute
	public VoronoiEdge(Vector2 point)
	{
		point1 = point; // Setzt den ersten Endpunkt
		point2.x = 0;
		point2.y = 0;
		isFinished = false;
		edges.Add(this); // Fügt die Kante der Liste hinzu
	}

	// Diese Methode stellt die Kante fertig
	public void Finish(Vector2 point)
	{
		if (!isFinished)
		{
			point2 = point; // Setzt den zweiten Endpunkt
			isFinished = true;
		}
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

// Klasse, die die Methoden für den Algorithmus von Fortune deklariert
class Fortune
{
	public ParabolaArc root = null;
	public List<Vector2> points = new List<Vector2>(); // Liste der Punkte
	public List<CircleEvent> circleEvents = new List<CircleEvent>(); // Liste der CircleEvents

	// Verarbeitet den verbleibenden Punkt (rechts der Sweep Line), der ganz links liegt
	public void ProcessPoint(double x)
	{
		Vector2 point = points[0];
		points.RemoveAt(0); // Entfernt den ersten Punkt aus der Liste
		AddNewArc(point, x); // Fügt der Beach Line einen Parabelbogen hinzu
	}

	// Diese Methode verarbeitet das CircleEvent mit der kleinsten x-Koordinate
	public void ProcessCircleEvent()
	{
		CircleEvent circleEvent = circleEvents[0];
		circleEvents.RemoveAt(0); // Entfernt das erste CircleEvent aus der Liste
		if (circleEvent.isValid) // Wenn das CircleEvent aktuell ist
		{
			VoronoiEdge edge = new VoronoiEdge(circleEvent.point); // Erzeugt eine neue Kante
																	// Entfernt den zugehörigen Parabelbogen
			ParabolaArc arc = circleEvent.arc;
			if (arc.previousArc != null)
			{
				arc.previousArc.nextArc = arc.nextArc;
				arc.previousArc.edge2 = edge;
			}
			if (arc.nextArc != null)
			{
				arc.nextArc.previousArc = arc.previousArc;
				arc.nextArc.edge1 = edge;
			}
			// Stellt die benachbarten Kanten des Parabelbogens fertig
			if (arc.edge1 != null)
			{
				arc.edge1.Finish(circleEvent.point);
			}
			if (arc.edge2 != null)
			{
				arc.edge2.Finish(circleEvent.point);
			}
			// Prüft die CircleEvents auf beiden Seiten des Parabelbogens
			if (arc.previousArc != null)
			{
				CheckCircleEvent(arc.previousArc, circleEvent.x);
			}
			if (arc.nextArc != null)
			{
				CheckCircleEvent(arc.nextArc, circleEvent.x);
			}
		}
	}

	// Diese Methode fügt einen neuen Parabelbogen mit dem gegebenen Brennpunkt hinzu
	public void AddNewArc(Vector2 point, double x)
	{
		if (root == null)
		{
			root = new ParabolaArc(point, null, null);
			return;
		}
		// Bestimmt den aktuellen Parabelbogen mit der y-Koordinate des Punkts point
		ParabolaArc arc;
		for (arc = root; arc != null; arc = arc.nextArc) // Diese for-Schleife durchläuft die Parabelbögen
		{
			Vector2 intersection1 = new Vector2(0, 0), intersection2 = new Vector2(0, 0);
			if (GetIntersection(point, arc, ref intersection1)) // Wenn die neue Parabel den Parabelbogen schneidet
			{
				// Dupliziert gegebenenfalls den Parabelbogen
				if (arc.nextArc != null && !GetIntersection(point, arc.nextArc, ref intersection2))
				{
					arc.nextArc.previousArc = new ParabolaArc(arc.point, arc, arc.nextArc);
					arc.nextArc = arc.nextArc.previousArc;
				}
				else
				{
					arc.nextArc = new ParabolaArc(arc.point, arc, null);
				}
				arc.nextArc.edge2 = arc.edge2;
				// Fügt einen neuen Parabelbogen zwischen den Parabelbögen arc und arc.nextArc ein.
				arc.nextArc.previousArc = new ParabolaArc(point, arc, arc.nextArc);
				arc.nextArc = arc.nextArc.previousArc;
				arc = arc.nextArc;
				// Verbindet 2 neue Kanten mit den Endpunkten des Parabelbogens
				arc.previousArc.edge2 = arc.edge1 = new VoronoiEdge(intersection1);
				arc.nextArc.edge1 = arc.edge2 = new VoronoiEdge(intersection1);
				// Prüft die benachbarten CircleEvents des Parabelbogens
				CheckCircleEvent(arc, point.x);
				CheckCircleEvent(arc.previousArc, point.x);
				CheckCircleEvent(arc.nextArc, point.x);
				return;
			}
		}
		// Spezialfall: Wenn der Parabelbogen keinen anderen schneidet, wird er der doppelt verketteten Liste hinzugefügt
		for (arc = root; arc.nextArc != null; arc = arc.nextArc) ; // Bestimmt den letzten Parabelbogen
		arc.nextArc = new ParabolaArc(point, arc, null);
		// Fügt eine Kante zwischen den Parabelbögen ein
		Vector2 start = new Vector2(0, 0);
		start.x = (float)x;
		start.y = (arc.nextArc.point.y + arc.point.y) / 2;
		arc.edge2 = arc.nextArc.edge1 = new VoronoiEdge(start);
	}

	// Diese Methode erzeugt wenn nötig ein neues CircleEvent für den gegebenen Parabelbogen
	public void CheckCircleEvent(ParabolaArc arc, double _x)
	{
		// Setzt das bisherige CircleEvent auf nicht aktuell
		if (arc.circleEvent != null && arc.circleEvent.x != _x)
		{
			arc.circleEvent.isValid = false;
		}
		arc.circleEvent = null;
		if (arc.previousArc == null || arc.nextArc == null)
		{
			return;
		}
		double x = 0;
		Vector2 point = new Vector2(0, 0);
		if (GetRightmostCirclePoint(arc.previousArc.point, arc.point, arc.nextArc.point, ref x, ref point) && x > _x)
		{
			arc.circleEvent = new CircleEvent(x, point, arc); // Erzeugt ein neues CircleEvent
			circleEvents.Add(arc.circleEvent);
			circleEvents.Sort(new EventComparer()); // Sortiert die Liste
		}
	}

	// Diese Methode bestimmt die x-Koordinate des Kreises durch die 3 Punkte, der ganz rechts liegt und prüft, ob die 3 Punkte auf einer Geraden liegen
	public bool GetRightmostCirclePoint(Vector2 point1, Vector2 point2, Vector2 point3, ref double x, ref Vector2 point)
	{
		// Prüft, dass die 3 Punkt im Uhrzeigersinn orientiert sind
		if ((point2.x - point1.x) * (point3.y - point1.y) > (point3.x - point1.x) * (point2.y - point1.y))
		{
			return false;
		}
		double x1 = point2.x - point1.x;
		double y1 = point2.y - point1.y;
		double a = 2 * (x1 * (point3.y - point2.y) - y1 * (point3.x - point2.x));
		if (a == 0)
		{
			return false;  // Wenn die 3 Punkte auf einer Geraden liegen, wird false zurückgegeben
		}
		double x2 = point3.x - point1.x;
		double y2 = point3.y - point1.y;
		double a1 = x1 * (point1.x + point2.x) + y1 * (point1.y + point2.y);
		double a2 = x2 * (point1.x + point3.x) + y2 * (point1.y + point3.y);
		// Berechnet den Mittelpunkt des Kreises durch die 3 Punkte
		point.x = (float)((y2 * a1 - y1 * a2) / a);
		point.y = (float)((x1 * a2 - x2 * a1) / a);
		x = point.x + Math.Sqrt(Math.Pow(point1.x - point.x, 2) + Math.Pow(point1.y - point.y, 2)); // x-Koordinate des Mittelpunkts plus Radius
		return true;
	}

	// Diese Methode bestimmt gegebenenfalls den Schnittpunkt zwischen der Parabel mit dem gegebenen Brennpunkt und dem Parabelbogen und prüft, ob er existiert
	public bool GetIntersection(Vector2 point, ParabolaArc arc, ref Vector2 intersection)
	{
		if (arc.point.x == point.x)
		{
			return false; // Wenn die Brennpunkte übereinstimmen, wird false zurückgegeben
		}
		double y1 = 0, y2 = 0;
		if (arc.previousArc != null)
		{
			y1 = GetParabolasIntersection(arc.previousArc.point, arc.point, point.x).y; // Berechnet die y-Koordinate des Schnittpunkts zwischen dem aktuellen und dem vorherigen Parabelbogen
		}
		if (arc.nextArc != null)
		{
			y2 = GetParabolasIntersection(arc.point, arc.nextArc.point, point.x).y; // Berechnet die y-Koordinate des Schnittpunkts zwischen dem aktuellen und dem nächsten Parabelbogen
		}
		// Berechnet die Koordinaten des Schnittpunkts, falls vorhanden
		if ((arc.previousArc == null || y1 <= point.y) && (arc.nextArc == null || point.y <= y2))
		{
			intersection.y = point.y;
			intersection.x = (arc.point.x * arc.point.x + (arc.point.y - intersection.y) * (arc.point.y - intersection.y) - point.x * point.x) / (2 * arc.point.x - 2 * point.x);
			return true;
		}
		return false;
	}

	// Diese Methode bestimmt den Schnittpunkt zwischen den Parabeln mit den gegebenen Brennpunkten für die Sweep Line mit der gegebenen x-Koordinate
	public Vector2 GetParabolasIntersection(Vector2 point1, Vector2 point2, double x)
	{
		Vector2 intersection = new Vector2(0, 0), point = point1;
		// Spezialfälle
		if (point1.x == point2.x)
		{
			intersection.y = (point1.y + point2.y) / 2;
		}
		else if (point2.x == x)
		{
			intersection.y = point2.y;
		}
		else if (point1.x == x)
		{
			intersection.y = point1.y;
			point = point2;
		}
		else // Standardfall
		{
			// Verwendet die Lösungsformel für quadratische Gleichungen, um die y-Koordinate des Schnittpunkts zu berechnen
			double x1 = 2 * (point1.x - x);
			double x2 = 2 * (point2.x - x);
			double a = 1 / x1 - 1 / x2;
			double b = -2 * (point1.y / x1 - point2.y / x2);
			double c = (point1.y * point1.y + point1.x * point1.x - x * x) / x1 - (point2.y * point2.y + point2.x * point2.x - x * x) / x2;
			intersection.y = (float)((-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a));
		}
		// Setzt die y-Koordinate in die Parabelgleichung ein, um die x-Koordinate zu berechnen
		intersection.x = (float)((point.x * point.x + (point.y - intersection.y) * (point.y - intersection.y) - x * x) / (2 * point.x - 2 * x));
		return intersection;
	}

	// Diese Methode stellt die benachbarten Kanten der Parabelbögen fertig
	public void FinishEdges(double x1, double y1, double x2, double y2)
	{
		// Verschiebt die Sweep Line, sodass keine Parabel die Zeichenfläche schneiden kann
		double x = x2 + (x2 - x1) + (y2 - y1);
		// Verlängert jede benachbarte Kante bis zum Schnittpunkt der neuen Parabeln
		for (ParabolaArc arc = root; arc.nextArc != null; arc = arc.nextArc)
		{
			if (arc.edge2 != null)
			{
				arc.edge2.Finish(GetParabolasIntersection(arc.point, arc.nextArc.point, 2 * x));
			}
		}
	}
}

// Klasse für das Hauptfenster
public partial class FortunesVoronoiGraph
{
	private List<Vector2> points = new List<Vector2>(); // Liste der Punkte
	private double x1, y1, x2, y2;

	public FortunesVoronoiGraph(Vector2 minCoordinates, Vector2 maxCoordinates, int numberOfPoints)
	{
		// Setzt die Koordinaten der Eckpunkte
		x1 = minCoordinates.x;
		y1 = minCoordinates.y;
		x2 = maxCoordinates.x;
		y2 = maxCoordinates.y;

		System.Random random = new System.Random(); // Initialisiert den Zufallsgenerator
		for (int i = 0; i < numberOfPoints; i++) // Diese for-Schleife erzeugt 10 zufällige Punkte innerhalb der quadratischen Zeichenfläche
		{
			Vector2 point = new Vector2();
			point.x = (float)(random.NextDouble() * (x2 - x1) + x1);
			point.y = (float)(random.NextDouble() * (y2 - y1) + y1);
			points.Add(point); // Fügt den Punkt der Liste hinzu
		}
		points.Sort(new PointComparer()); // Sortiert die Punkte
		Fortune fortune = new Fortune(); // Erzeugt ein Objekt der Klasse Fortune
		fortune.points.AddRange(points); // Fügt die Punkte der Liste hinzu

		// Diese for-Schleife verarbeitet bei jedem Durchlauf das Element mit der kleinsten x-Koordinate
		while (fortune.points.Count != 0) // Solange die Liste der Punkte nicht leer ist
		{
			if (fortune.circleEvents.Count != 0 && fortune.circleEvents[0].x <= fortune.points[0].x)
			{
				fortune.ProcessCircleEvent(); // Aufruf der Methode, verarbeitet das CircleEvent
			}
			else
			{
				fortune.ProcessPoint(x1); // Aufruf der Methode, verarbeitet den Punkt
			}
		}
		// Nachdem alle Punkte verarbeitet wurden, werden die verbleibenden circleEvents verarbeitet.
		while (fortune.circleEvents.Count != 0) // Solange die Liste der CircleEvents nicht leer ist
		{
			fortune.ProcessCircleEvent();
		}
		fortune.FinishEdges(x1, y1, x2, y2); // Aufruf der Methode, stellt die benachbarten Kanten der Parabelbögen fertig
	}

	public List<Vector2> GetPoints()
	{
		return points;
	}

	public List<Edge> GetEdges()
	{
		List<Edge> edges = new List<Edge>();
		foreach (VoronoiEdge ve in VoronoiEdge.edges)
			edges.Add(new Edge(ve.point1, ve.point2));
		return edges;
	}
}
