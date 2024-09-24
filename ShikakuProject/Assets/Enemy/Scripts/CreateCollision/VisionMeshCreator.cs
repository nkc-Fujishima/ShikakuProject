using UnityEngine;

public class VisionMeshCreator : MonoBehaviour
{
    public float viewAngle = 90f; // ����p
    public float viewDistance = 5f; // ����̋���
    public float viewHeight = 1f; // ����̍���
    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;
    private MeshCollider viewMeshCollider;

    void Start()
    {
        viewMeshFilter = gameObject.AddComponent<MeshFilter>();
        viewMesh = new Mesh();
        viewMeshFilter.mesh = viewMesh;

        viewMeshCollider = gameObject.AddComponent<MeshCollider>();
        viewMeshCollider.convex = true;
        viewMeshCollider.isTrigger = true;
        viewMeshCollider.sharedMesh = viewMesh;

        UpdateViewMesh();
    }

    void Update()
    {
        // ����p���ς�����ꍇ�Ƀ��b�V�����X�V
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            viewAngle += 10f;
            UpdateViewMesh();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            viewAngle -= 10f;
            UpdateViewMesh();
        }
    }

    void UpdateViewMesh()
    {
        int segments = 8; // ���b�V���̐��x
        Vector3[] vertices = new Vector3[(segments + 2) * 2];
        int[] triangles = new int[segments * 12 + 12];

        // ���̖ʂ̒��_
        vertices[0] = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (viewAngle / segments) * i - Mathf.Deg2Rad * (viewAngle / 2);
            vertices[i + 1] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * viewDistance;
        }

        // ��̖ʂ̒��_
        for (int i = 0; i <= segments + 1; i++)
        {
            vertices[i + segments + 2] = vertices[i] + Vector3.up * viewHeight;
        }

        // ���̖ʂ̎O�p�`
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 2] = i + 1;
            triangles[i * 3 + 1] = i + 2;
        }

        // ��̖ʂ̎O�p�`
        for (int i = 0; i < segments; i++)
        {
            triangles[(segments + i) * 3] = segments + 2;
            triangles[(segments + i) * 3 + 2] = segments + i + 4;
            triangles[(segments + i) * 3 + 1] = segments + i + 3;
        }

        // �ǂ̑��ʂ̎O�p�`
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

        // ���ʂ̎O�p�`
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

        // ���b�V���R���C�_�[���X�V
        viewMeshCollider.sharedMesh = viewMesh;
    }
}