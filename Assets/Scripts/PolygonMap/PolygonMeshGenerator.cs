using System.Collections.Generic;
using UnityEngine;


public class PolygonMeshGenerator
{
    Vector3[] vertecies;
    Vector2[] uv;
    int[] triangles;

    int numberOfVertecies;
    int numberOfTriangles;

    // This Function creates a new polygon mesh generator for a given polygon
    public PolygonMeshGenerator(Vector2 origin, List<Vector2> corners)
    {
        numberOfVertecies = corners.Count;
        numberOfTriangles = corners.Count - 2;

        vertecies = new Vector3[numberOfVertecies];
        uv = new Vector2[numberOfVertecies];
        triangles = new int[numberOfTriangles * 3];

        TranslateCornersToVertecies(origin, corners);
        MapVerteciesToUV();
        GenerateTriangles();
    }


    public Mesh GetPolygonMesh()
    {
        Mesh mesh = new Mesh();

        mesh.SetVertices(vertecies);
        mesh.uv = uv;
        mesh.triangles = triangles;

        return mesh;
    }


    //this function translates screen space corners to world space vertecies
    private void TranslateCornersToVertecies(Vector2 origin, List<Vector2> corners)
    {
        for (int i = 0; i < corners.Count; i++)
        {
            //transform screen position to world position
            Vector3 pivotWorldPos = Camera.main.ScreenToWorldPoint(origin);                 // pivot position in world space
            Vector3 cornerWorldPos = Camera.main.ScreenToWorldPoint(corners[i] + origin);   // corner position in Worldspace relative to 0|0
            Vector3 localCornerWorldPos = cornerWorldPos - pivotWorldPos;                   // world pos relative to pivot
            localCornerWorldPos.z = 0;

            vertecies[i] = localCornerWorldPos;
        }
    }

    //this function fills the triangles array - all triangles share the last vertex in vertecies as a corner
    private void GenerateTriangles()
    {
        for (int i = 0; i < numberOfTriangles; i++)
        {
            triangles[i * 3] = i;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = numberOfVertecies - 1;
        }
    }


    //maps each vertex to its according position in uv space (0 to 1)
    private void MapVerteciesToUV()
    {
        Vector3 meshMin;
        Vector3 meshMax;
        GetBounds(out meshMin, out meshMax);

        //make bounds square
        float delta = Mathf.Abs(meshMax.x - meshMin.x) - Mathf.Abs(meshMax.y - meshMin.y);
        if (delta > 0) //wider than high
        {
            meshMin.y -= delta / 2;
            meshMax.y += delta / 2;
        }
        else if (delta < 0) //higher than wide
        {
            delta = Mathf.Abs(delta);
            meshMin.x -= delta / 2;
            meshMax.x += delta / 2;
        }

        //map vertecies to 0-1 plane of the UV
        for (int i = 0; i < vertecies.Length; i++)
            uv[i] = ExtensionMethods.Map(vertecies[i], meshMin, meshMax, Vector3.zero, Vector3.one);
    }


    // returns the highest and lowes value for x and y in the vertecies
    public void GetBounds(out Vector3 min, out Vector3 max)
    {
        float xMax = vertecies[0].x;
        float xMin = vertecies[0].x;
        float yMax = vertecies[0].y;
        float yMin = vertecies[0].y;

        //find the outermost corner vertecies in x and y direction
        foreach (Vector2 currentCorner in vertecies)
        {
            if (xMax < currentCorner.x) xMax = currentCorner.x;
            if (xMin > currentCorner.x) xMin = currentCorner.x;
            if (yMax < currentCorner.y) yMax = currentCorner.y;
            if (yMin > currentCorner.y) yMin = currentCorner.y;
        }

        min = new Vector3(xMin, yMin);
        max = new Vector3(xMax, yMax);
    }
}