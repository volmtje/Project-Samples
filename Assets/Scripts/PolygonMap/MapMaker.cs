using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] int polygonAmmount;

    private void Start()
    {
        Vector2 screenSize;
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;

        FortunesVoronoiGraph fortune = new FortunesVoronoiGraph(Vector2.zero, screenSize, polygonAmmount);
        PolygonCalculator pc = new PolygonCalculator(screenSize, fortune.GetPoints(), fortune.GetEdges()); // prep the polygons

        foreach (var polygon in pc.GetPolygons())
            MakePolygonGameObject(polygon.Key, polygon.Value);
    }


    //this function takes the orgin and cornerpoints of a polygon
    //and creates a gameobject to the scene root with a mesh of that polygon
    void MakePolygonGameObject(Vector2 position, List<Vector2> corners)
    {
        PolygonMeshGenerator meshGenerator = new PolygonMeshGenerator(position, corners);
        Mesh mesh = meshGenerator.GetPolygonMesh();

        Material meshMaterial = new Material(material);
        meshMaterial.color = Random.ColorHSV();

        GameObject polygonObject = new GameObject("polygonField", typeof(MeshFilter), typeof(MeshRenderer));
        polygonObject.GetComponent<MeshFilter>().mesh = mesh;
        polygonObject.GetComponent<MeshRenderer>().material = meshMaterial;

        Vector3 pos = Camera.main.ScreenToWorldPoint(position);
        pos.z = 0;
        polygonObject.transform.position = pos;
    }
}

