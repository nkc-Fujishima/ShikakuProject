using R3;
using UnityEngine;
using UnityEngine.Events;

public class VisionMeshCreator : MonoBehaviour
{
    [Header("数値設定")]
    public float viewAngle = 90f; // 視野角
    public float viewRange = 5f; // 視野の距離
    public float viewHeight = 1f; // 視野の高さ

    public ReactiveProperty<bool> IsAlert = new(false);

    [HideInInspector]
    public UnityEvent StartEvent = new();


    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    private MeshCollider viewMeshCollider;

    public void SetUp()
    {
        viewMeshFilter = gameObject.AddComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMeshFilter.mesh = viewMesh;

        viewMeshCollider = gameObject.AddComponent<MeshCollider>();
        viewMeshCollider.convex = true;
        viewMeshCollider.isTrigger = true;
        viewMeshCollider.sharedMesh = viewMesh;

        UpdateViewMesh();

        StartEvent.Invoke();
    }

    void UpdateViewMesh()
    {
        int segments = 8; // メッシュの精度
        Vector3[] vertices = new Vector3[(segments + 2) * 2];
        int[] triangles = new int[segments * 12 + 12];

        // 下の面の頂点
        vertices[0] = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (viewAngle / segments) * i - Mathf.Deg2Rad * (viewAngle / 2);
            vertices[i + 1] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * viewRange;
        }

        // 上の面の頂点
        for (int i = 0; i <= segments + 1; i++)
        {
            vertices[i + segments + 2] = vertices[i] + Vector3.up * viewHeight;
        }

        // 下の面の三角形
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 2] = i + 1;
            triangles[i * 3 + 1] = i + 2;
        }

        // 上の面の三角形
        for (int i = 0; i < segments; i++)
        {
            triangles[(segments + i) * 3] = segments + 2;
            triangles[(segments + i) * 3 + 2] = segments + i + 4;
            triangles[(segments + i) * 3 + 1] = segments + i + 3;
        }

        // 孤の側面の三角形
        for (int i = 0; i < segments; i++)
        {
            int baseIndex = segments * 6 + i * 6;
            triangles[baseIndex] = i + 1;
            triangles[baseIndex + 2] = segments + i + 3;
            triangles[baseIndex + 1] = i + 2;

            triangles[baseIndex + 3] = i + 2;
            triangles[baseIndex + 5] = segments + i + 3;
            triangles[baseIndex + 4] = segments + i + 4;
        }

        // 側面の三角形
        {
            int baseIndex = segments * 6 + segments * 6;
            triangles[baseIndex] = 0;
            triangles[baseIndex + 1] = 1;
            triangles[baseIndex + 2] = vertices.Length / 2;

            triangles[baseIndex + 3] = 1;
            triangles[baseIndex + 4] = vertices.Length / 2 + 1;
            triangles[baseIndex + 5] = vertices.Length / 2;


            triangles[baseIndex + 6] = 0;
            triangles[baseIndex + 7] = vertices.Length - 1;
            triangles[baseIndex + 8] = vertices.Length / 2 - 1;

            triangles[baseIndex + 9] = 0;
            triangles[baseIndex + 10] = vertices.Length / 2;
            triangles[baseIndex + 11] = vertices.Length - 1;
        }


        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

        // メッシュコライダーを更新
        viewMeshCollider.sharedMesh = viewMesh;
    }

    public void ChangeMeshNoAlertMaterial()
    {
        IsAlert.Value = false;
    }
    public void ChangeMeshAlertMaterial()
    {
        IsAlert.Value = true;
    }
}