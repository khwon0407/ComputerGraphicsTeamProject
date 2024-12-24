using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
 public class MyMeshFilter : MonoBehaviour

 {
    private MeshFilter meshFilter;
    private Vector3[] vertices;
     private int[] indices;
    private Vector2[] uv;

    private void Awake()
    {  
        meshFilter = GetComponent<MeshFilter>();
        GenerateMesh();
    }  
    private void GenerateMesh(){
        vertices = new Vector3[3]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0)
        };
        indices = new int[3] { 0, 1, 2 };
        uv = new Vector2[3]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0)
        };
        
        Mesh mesh = new Mesh();
        mesh.name = "test1";
        mesh.vertices = vertices;
        mesh.triangles= indices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }
}

