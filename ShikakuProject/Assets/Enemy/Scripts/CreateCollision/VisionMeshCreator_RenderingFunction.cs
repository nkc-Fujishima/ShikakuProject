using UnityEngine;
using R3;

public class VisionMeshCreator_RenderingFunction : MonoBehaviour
{
    [SerializeField]
    private VisionMeshCreator _visionMeshCreator;

    [Header("オブジェクト設定")]
    [SerializeField]
    private Material _noAlertMaterial;
    [SerializeField]
    private Material _alertMaterial;

    private MeshRenderer _meshRenderer;


    public int raysToCast;
    private float angleOfVision;
    private float _sightRange;

    private Vector3[] _vertices;
    private Vector2[] _uvs;
    private int[] _triangles;

    private Mesh _visionConeMesh;
    private MeshFilter _meshFilter;

    private float _castAngle;
    private float _sinX;
    private float _cosX;
    private Vector3 _dir;
    private Vector3 _temp;
    private RaycastHit _hit;

    private bool setUpFlag = false;


    private void Start()
    {
        _visionMeshCreator.StartEvent.AddListener(OnStart);
    }

    private void OnStart()
    {
        _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = _noAlertMaterial;
        _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        SetDeligate();

        _sightRange = _visionMeshCreator.viewRange * 0.1f + 0.5f;
        angleOfVision = _visionMeshCreator.viewAngle * Mathf.Deg2Rad / 2;


        _vertices = new Vector3[raysToCast + 1];
        _uvs = new Vector2[_vertices.Length];
        _triangles = new int[(_vertices.Length * 3) - 9];

        // Set up procedural mesh
        _visionConeMesh = new Mesh();
        _visionConeMesh.name = "VisionCone";
        _meshFilter = gameObject.AddComponent<MeshFilter>();
        _meshFilter.mesh = _visionConeMesh;

        setUpFlag = true;
    }

    private void SetDeligate()
    {
        _visionMeshCreator.IsAlert.Subscribe(value =>
        {
            if (value) ChangeMeshAlertMaterial();
            else ChangeMeshNoAlertMaterial();
        }).AddTo(this);
    }

    void Update()
    {
        RaySweep();
    }

    void RaySweep()
    {
        if (!setUpFlag) return;

        // angle relative to players'/parents' forward vector
        _castAngle = -angleOfVision + Mathf.Deg2Rad * transform.eulerAngles.y;

        /// Sweep rays over the cone of vision ///

        // cast rays to map out the space in a cone-shaped sweep
        for (int i = 0; i < raysToCast; i++)
        {
            _sinX = _sightRange * Mathf.Sin(_castAngle);
            _cosX = _sightRange * Mathf.Cos(_castAngle);

            // Increment in proportion to the size of the cone and the number of rays used to map it
            _castAngle += 2 * angleOfVision / raysToCast;

            _dir = new Vector3(_sinX, 0, _cosX);

            //Debug.DrawRay(transform.position, _dir, Color.green); // to aid visualization

            if (Physics.Raycast(transform.position, _dir, out _hit, _sightRange))
            {
                _temp = transform.InverseTransformPoint(_hit.point);
                //_temp = _hit.point;
                _vertices[i] = new Vector3(_temp.x, 0.1f, _temp.z);
                //Debug.DrawLine (transform.position, _hit.point, Color.red); // to aid visualization
            }
            else
            {
                _temp = transform.InverseTransformPoint(transform.position + _dir);
                //_temp = transform.position + _dir;
                _vertices[i] = new Vector3(_temp.x, 0.1f, _temp.z);
            }

        } // end raycast loop

        /// Building/Updating the vision cone mesh ///

        // assign the _vertices BEFORE dealing with the _uvs and _triangles
        _visionConeMesh.vertices = _vertices;

        // created _uvs for mesh
        for (int i = 0; i < _vertices.Length; i++)
        {
            _uvs[i] = new Vector2(_vertices[i].x, _vertices[i].z);
        } // end _uvs loop

        // create _triangles for mesh, with each tri ending at the player's location (like pizza slices)
        int x = -1;
        for (int i = 0; i < _triangles.Length; i += 3)
        {
            x++;
            _triangles[i] = x + 1;
            _triangles[i + 1] = x + 2;
            _triangles[i + 2] = _vertices.Length - 1; // all _triangles end at the centre
        }

        _visionConeMesh.triangles = _triangles;
        _visionConeMesh.uv = _uvs;

        //_visionConeMesh.RecalculateNormals (); // not sure if this is necessary anymore

    }

    //ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    // 警戒状態が変わった場合マテリアルを変える
    public void ChangeMeshNoAlertMaterial()
    {
        _meshRenderer.material = _noAlertMaterial;
    }
    public void ChangeMeshAlertMaterial()
    {
        _meshRenderer.material = _alertMaterial;
    }
}