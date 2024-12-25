using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MyMeshFilter : MonoBehaviour
{
    private void Start()
    {  
        GenerateMesh(); // Unity Editor에서 설정한 MeshFilter에 Mesh를 생성합니다.
    }

    private void GenerateMesh()
    {
        // 16:9 비율로 정점 정의
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-16, -9, 0), // Bottom-left
            new Vector3(-16,  9, 0), // Top-left
            new Vector3( 16,  9, 0), // Top-right
            new Vector3( 16, -9, 0)  // Bottom-right
        };

        // 두 개의 삼각형으로 메시 구성
        int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };

        // UV 좌표 (텍스처 매핑 지원)
        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };

        // 메시 생성 및 설정
        Mesh mesh = new Mesh
        {
            name = "BackgroundQuad",
            vertices = vertices,
            triangles = indices,
            uv = uv
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh; // Unity Editor에서 설정한 MeshFilter에 할당
    }
}
