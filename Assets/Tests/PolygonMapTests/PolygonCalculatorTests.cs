using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PolygonCalculatorTests
{
    [Test]
    public void PolygonCalculatorTestsEdgesAreScreenEdges()
    {
        //setup
        //edges go from screen corner to screen corner
        Vector2 screensize = new Vector2(100, 100);
        List<Edge> edges = new List<Edge>();
        edges.Add(new Edge(new Vector2(0, 0), new Vector2(100, 0)));
        edges.Add(new Edge(new Vector2(100, 0), new Vector2(100, 100)));
        edges.Add(new Edge(new Vector2(0, 100), new Vector2(100, 100)));
        edges.Add(new Edge(new Vector2(0, 100), new Vector2(100, 0)));
        //point is in the middle of the screen
        List<Vector2> points = new List<Vector2>() { new Vector2(50, 50) };

        //expected
        KeyValuePair<Vector2, List<Vector2>> expected = new KeyValuePair<Vector2, List<Vector2>>(
            new Vector2(50, 50), 
            new List<Vector2>() { 
                new Vector2(50, -50), 
                new Vector2(-50, -50), 
                new Vector2(-50, 50), 
                new Vector2(50, 50)});

        //run
        PolygonCalculator polygonCalculator = new PolygonCalculator(screensize, points, edges);
        Dictionary<Vector2, List<Vector2>> polygons =  polygonCalculator.GetPolygons();


        //assert
        Assert.AreEqual(1, polygons.Count);
        Assert.IsTrue(polygons[expected.Key] != null);
        Assert.AreEqual(expected.Value, polygons[expected.Key]);
    }

    [Test]
    public void PolygonCalculatorTestsFourEvenSquares()
    {
        //setup
        Vector2 screensize = new Vector2(1, 1);
        List<Edge> edges = new List<Edge>
        {
            new Edge(new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f)),
            new Edge(new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f)),
            new Edge(new Vector2(0.5f, 1), new Vector2(0.5f, 0.5f)),
            new Edge(new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f))
        };
        List<Vector2> points = new List<Vector2>() { 
            new Vector2(0.25f, 0.75f), 
            new Vector2(0.25f, 0.25f),
            new Vector2(0.75f, 0.25f),
            new Vector2(0.75f, 0.75f)};

        //expected
        Dictionary<Vector2, List<Vector2>> expected = new Dictionary<Vector2, List<Vector2>>();
        expected.Add(new Vector2(0.25f, 0.75f), new List<Vector2>() { new Vector2(0.25f, -0.25f), new Vector2(-0.25f, -0.25f), new Vector2(-0.25f, 0.25f), new Vector2(0.25f, 0.25f) });
        expected.Add(new Vector2(0.25f, 0.25f), new List<Vector2>() { new Vector2(0.25f, -0.25f), new Vector2(-0.25f, -0.25f), new Vector2(-0.25f, 0.25f), new Vector2(0.25f, 0.25f) });
        expected.Add(new Vector2(0.75f, 0.25f), new List<Vector2>() { new Vector2(0.25f, -0.25f), new Vector2(-0.25f, -0.25f), new Vector2(-0.25f, 0.25f), new Vector2(0.25f, 0.25f) });
        expected.Add(new Vector2(0.75f, 0.75f), new List<Vector2>() { new Vector2(0.25f, -0.25f), new Vector2(-0.25f, -0.25f), new Vector2(-0.25f, 0.25f), new Vector2(0.25f, 0.25f) });

        //run
        PolygonCalculator polygonCalculator = new PolygonCalculator(screensize, points, edges);
        Dictionary<Vector2, List<Vector2>> polygons = polygonCalculator.GetPolygons();

        //assert
        Assert.AreEqual(4, polygons.Count);
        foreach (KeyValuePair<Vector2, List<Vector2>> polygon in polygons)
        {
            Assert.IsTrue(expected.ContainsKey(polygon.Key));
            Assert.AreEqual(expected[polygon.Key], polygon.Value);
        }
    }

    [Test]
    public void PolygonCalculatorTestsSimplePasses()
    {
        //setup
        Vector2 screensize = new Vector2(10, 10);
        List<Edge> edges = new List<Edge>();
        edges.Add(new Edge(new Vector2(7.13f, 4.63f), new Vector2(10.00f, 5.20f)));
        edges.Add(new Edge(new Vector2(4.50f, 2.00f), new Vector2(7.13f, 4.63f)));
        edges.Add(new Edge(new Vector2(4.50f, 2.00f), new Vector2(2.50f, 0.00f)));
        edges.Add(new Edge(new Vector2(3.17f, 7.00f), new Vector2(0.00f, 8.90f)));
        edges.Add(new Edge(new Vector2(3.17f, 7.00f), new Vector2(7.13f, 4.63f)));
        List<Vector2> points = new List<Vector2>() {
            new Vector2(6, 7),
            new Vector2(7, 2),
            new Vector2(4.5f, 4.5f)};

        //expected
        Dictionary<Vector2, List<Vector2>> expected = new Dictionary<Vector2, List<Vector2>>();
        expected.Add(new Vector2(6, 7), new List<Vector2>() {
            new Vector2(10, 10) - new Vector2(6, 7),
            new Vector2(10, 5.2f) - new Vector2(6, 7),
            new Vector2(7.13f, 4.63f) - new Vector2(6, 7),
            new Vector2(3.17f, 7.00f) - new Vector2(6, 7),
            new Vector2(0, 8.90f) - new Vector2(6, 7),
            new Vector2(0, 10) - new Vector2(6, 7)});
        expected.Add(new Vector2(7, 2), new List<Vector2>() {
            new Vector2(10, 0) - new Vector2(7, 2),
            new Vector2(2.5f, 0) - new Vector2(7, 2),
            new Vector2(4.50f, 2.00f) - new Vector2(7, 2),
            new Vector2(7.13f, 4.63f) - new Vector2(7, 2),
            new Vector2(10, 5.2f) - new Vector2(7, 2)});
        expected.Add(new Vector2(4.5f, 4.5f), new List<Vector2>() {
            new Vector2(7.13f, 4.63f) - new Vector2(4.5f, 4.5f),
            new Vector2(4.50f, 2.00f) - new Vector2(4.5f, 4.5f),
            new Vector2(2.5f, 0) - new Vector2(4.5f, 4.5f),
            new Vector2(0, 0f) - new Vector2(4.5f, 4.5f),
            new Vector2(0, 8.90f) - new Vector2(4.5f, 4.5f),
            new Vector2(3.17f, 7.00f) - new Vector2(4.5f, 4.5f)});

        //run 
        PolygonCalculator polygonCalculator = new PolygonCalculator(screensize, points, edges);
        Dictionary<Vector2, List<Vector2>> polygons = polygonCalculator.GetPolygons();

        //assert
        Assert.AreEqual(3, polygons.Count);
        foreach (KeyValuePair<Vector2, List<Vector2>> polygon in polygons)
        {
            Assert.IsTrue(expected.ContainsKey(polygon.Key));
            Assert.AreEqual(expected[polygon.Key], polygon.Value);
        }
    }
}
