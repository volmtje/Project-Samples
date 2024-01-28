using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] int polygonAmmount;

    // Draw Polygon Map to the GameScreen
    private void Start()
    {
        Vector2 screenSize;
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;

        //get voronoi graph
        FortunesVoronoiGraph fortune = new FortunesVoronoiGraph(Vector2.zero, screenSize, polygonAmmount);
        List<Vector2> points = fortune.GetPoints();
        List<Edge> edges = fortune.GetEdges();

        //get polygons from the voronoi graph
        PolygonCalculator pc = new PolygonCalculator(screenSize, points, edges);

        // Draw polygons to the screen
        foreach (var polygon in pc.GetPolygons())
            MakePolygonGameObject(polygon.Key, polygon.Value);
    }


    // This function makes the given polygon center and corner into a mesh and adds it to the scene
    void MakePolygonGameObject(Vector2 position, List<Vector2> corners)
    {
        // Create the mesh
        PolygonMeshGenerator meshGenerator = new PolygonMeshGenerator(position, corners);
        Mesh mesh = meshGenerator.GetPolygonMesh();

        Material meshMaterial = new Material(material);
        meshMaterial.color = Random.ColorHSV();

        //Create the Gameobject
        GameObject polygonObject = new GameObject("polygonField", typeof(MeshFilter), typeof(MeshRenderer));
        polygonObject.GetComponent<MeshFilter>().mesh = mesh;
        polygonObject.GetComponent<MeshRenderer>().material = meshMaterial;

        Vector3 pos = Camera.main.ScreenToWorldPoint(position);
        pos.z = 0;
        polygonObject.transform.position = pos;
    }
}

