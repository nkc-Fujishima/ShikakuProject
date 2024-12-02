using StageDelaunayTriangles;
using System;
using System.Collections.Generic;
using UnityEngine;
using static StageMapData;

[CreateAssetMenu(fileName = "StageGenerateData", menuName = "Stage/Generator/GenerateData")]
public class StageGenerateData : ScriptableObject
{
    public StageMapData[] StageMapData;

    public StageObjectElementData ElementData;

    [SerializeField]
    private float TileWidth = 1;
    [SerializeField]
    private float TileHeight = 1;


    [SerializeField]
    private StageWallGenerateData _stageWallGenerateData;

    [SerializeField]
    private PhysicMaterial _physicMaterialGround;

    [SerializeField]
    private PhysicMaterial _physicMaterialWall;


    private Transform stageManagerTransform = null;


    public void OnStart(Transform transform)
    {
        stageManagerTransform = transform;
    }

    // �X�e�[�W�𐶐�����
    public void StageGenerete(int stageCount, out GameObject[] enemyObjs, out GameObject playerObj, out bool isTutorial)
    {
        Vector3 centralPoint = Vector3.zero;
        StageMapData mapData = StageMapData[stageCount];
        centralPoint.x += TileWidth * mapData.X / 2 - TileWidth / 2;
        centralPoint.z -= TileWidth * mapData.Y / 2 - TileWidth / 2;

        StageGenerete(stageCount, centralPoint, out enemyObjs, out playerObj, out isTutorial);
    }

    // �X�e�[�W�𐶐�����
    public void StageGenerete(int stageCount, Vector3 centralPoint, out GameObject[] enemyObjs, out GameObject playerObj, out bool isTutorial)
    {
        StageMapData mapData = StageMapData[stageCount];

        enemyObjs = null;
        playerObj = null;
        isTutorial = false;

        if (mapData == null) return;

        List<GameObject> enemyList = new();
        GameObject playerObject = null;
        isTutorial = mapData.IsTutorial;

        List<Square> squares = new ();

        // �}�b�v�𐶐�������W��ݒ�
        Vector3 instancePosition = centralPoint;
        instancePosition.x -= TileWidth * mapData.X / 2 - TileWidth / 2;
        instancePosition.z += TileWidth * mapData.Y / 2 - TileWidth / 2;

        for (int countY = 0; countY < mapData.Y; ++countY)
        {
            for (int countX = 0; countX < mapData.X; ++countX)
            {
                // �}�b�v�f�[�^����Y������X�e�[�W�^�C�����擾
                StageTile tileData = mapData.TileDatas[countX].TileData[countY];

                // �I�u�W�F�N�g�𐶐�
                // ------------------------------------------------------------------------------
                // �O���E���h         => �O���E���h�̂ݐ���
                // ���̑��I�u�W�F�N�g => �O���E���h �{ �X�e�[�W�^�C���ɑΉ�����I�u�W�F�N�g�𐶐�
                // �����Ȃ�(None)     => �����������Ȃ�
                switch (tileData.TileType)
                {
                    case StageTileType.Enemy:
                        GameObject instanceObject = InstanceObject(tileData, countX, countY, instancePosition);
                        enemyList.Add(instanceObject);
                        AwardWaypointToEnemy(stageCount, countX, countY, ref instanceObject);
                        InstanceGroundObject(new(StageTileType.Ground), countX, countY, instancePosition);
                        break;

                    case StageTileType.Player:
                        playerObject = InstanceObject(tileData, countX, countY, instancePosition);
                        InstanceGroundObject(new(StageTileType.Ground), countX, countY, instancePosition);
                        break;

                    case StageTileType.Obstacle:
                        InstanceObject(tileData, countX, countY, instancePosition);
                        InstanceGroundObject(new(StageTileType.Ground), countX, countY, instancePosition);
                        break;

                    case StageTileType.Ground:
                        InstanceGroundObject(tileData, countX, countY, instancePosition);
                        break;

                    case StageTileType.None:
                        Vector2Int p1 = new(countX, countY);
                        Vector2Int p2 = new(countX + 1, countY);
                        Vector2Int p3 = new(countX + 1, countY + 1);
                        Vector2Int p4 = new(countX, countY + 1);
                        Square newSquare = new(p1, p2, p3, p4);
                        squares.Add(newSquare);
                        break;

                    // 11/14�ǉ��F�`���[�g���A���v�f
                    case StageTileType.Tutorial:
                        InstanceObject(new(StageTileType.Obstacle), countX, countY, instancePosition);
                        InstanceObject(tileData, countX, countY, instancePosition);
                        InstanceGroundObject(new(StageTileType.Ground), countX, countY, instancePosition);
                        break;

                    default:
                        break;
                }
            }
        }

        enemyObjs = enemyList.ToArray();
        playerObj = playerObject;


        // �ǂ����
        if (_stageWallGenerateData)
            _stageWallGenerateData.GenerateSpiralWall(mapData.X, mapData.Y, TileWidth, _physicMaterialWall);


        // �n�ʂ̃R���C�_�[�����
        List<Vector2Int> stageSidePoint = new()
        {
            // �X�e�[�W�̊p
            new(0, 0),
            new(mapData.X, 0),
            new(mapData.X, mapData.Y),
            new(0, mapData.Y),
        };

        // ���Ƃ����ɂȂ��Ă��镔��
        for (int i = 0; i < squares.Count; ++i)
        {
            Triangle[] t = squares[i].GetTriangle();
            for (int j = 0; j < t.Length; j++)
            {
                Vector2Int[] tps = t[j].GetPointsToVector2Int();
                stageSidePoint.Add(tps[0]);
                stageSidePoint.Add(tps[1]);
                stageSidePoint.Add(tps[2]);
            }
        }

        InstanceGroundMesh(stageSidePoint, squares.ToArray());
    }

    // �I�u�W�F�N�g�𐶐�
    private GameObject InstanceObject(StageTile tileData, int countX, int countY, Vector3 centralPoint)
    {
        Vector3 instancePosition = new(countX * TileWidth, 0, -countY * TileWidth);
        instancePosition += centralPoint;

        // 11/14�ǉ��F�`���[�g���A���v�f
        if (tileData.TileType == StageTileType.Tutorial)
            instancePosition.y += TileHeight;

        Quaternion instanceRotation = Quaternion.Euler(0, tileData.RotationY, 0);

        GameObject objectPlefab = ElementData.GetGameObject(tileData);

        GameObject instanceObject = Instantiate(objectPlefab, instancePosition, instanceRotation);

        if (stageManagerTransform)
            if (tileData.TileType == StageTileType.Obstacle)
                instanceObject.transform.SetParent(stageManagerTransform);

        // 11/14�ǉ��F�`���[�g���A���v�f
        if (tileData.TileType == StageTileType.Tutorial)
            if (instanceObject.TryGetComponent<ITalkable>(out ITalkable talkObject))
                talkObject.TalkText = ElementData.TutorialData.TextDatas[tileData.ElementCount].TalkText;

        return instanceObject;
    }

    // �O���E���h�I�u�W�F�N�g�𐶐�
    private GameObject InstanceGroundObject(StageTile tileData, int countX, int countY, Vector3 centralPoint)
    {
        Vector3 instancePosition = new(countX * TileWidth, -TileHeight, -countY * TileWidth);
        instancePosition += centralPoint;

        Quaternion instanceRotation = Quaternion.Euler(0, tileData.RotationY, 0);

        GameObject objectPlefab = ElementData.GetGameObject(tileData);

        GameObject instanceObject = Instantiate(objectPlefab, instancePosition, instanceRotation);

        if (stageManagerTransform)
            instanceObject.transform.SetParent(stageManagerTransform);

        return instanceObject;
    }


    //----------------------------------------------------------------------------------------------------------------
    // ����|�C���g��t�^
    private void AwardWaypointToEnemy(int stageCount, int pointX, int pointY, ref GameObject targetObject)
    {
        if (!targetObject.TryGetComponent<IWaypointHolder>(out IWaypointHolder holder)) return;

        StageMapData mapData = StageMapData[stageCount];

        if (mapData.WaypointData == null) return;

        foreach (StageWaypointData waypointData in mapData.WaypointData)
        {
            int index = Array.IndexOf(waypointData.EnemyAtPoint, new Vector2Int(pointX, pointY));

            if (index == -1) continue;

            // �Ώۂ̓G�ɊY�����Ă�����z���_�[�ɏ���ݒ�

            Vector2Int[] waypoints = waypointData.Waypoint;

            Vector3[] settingDatas = new Vector3[waypoints.Length];

            for (int i = 0; i < waypoints.Length; ++i)
            {
                Vector3 settingPoint = targetObject.transform.position;
                settingPoint.x = waypoints[i].x * TileWidth;
                settingPoint.z = -waypoints[i].y * TileWidth;

                settingDatas[i] = settingPoint;
            }

            List<Vector3> newList = new(settingDatas);
            holder.SetWaypoints(newList);

            return;
        }
    }


    //----------------------------------------------------------------------------------------------------------------
    // �O�p�`���烁�b�V���R���C�_�[������Ēn�ʂ𐶐�
    private void InstanceGroundMesh(List<Vector2Int> pointList, Square[] square = null)
    {
        DelaunayTriangles triangles = new(pointList, square);

        GameObject newObject = new ("GroundMeshCollision");
        newObject.transform.SetParent(stageManagerTransform);

        MeshCollider meshCollider = newObject.AddComponent<MeshCollider>();
        meshCollider.material = _physicMaterialGround;

        Mesh myMesh = new();

        // �R���C�_�[�̒��_��ݒ�
        Vector3[] newVertex = new Vector3[pointList.Count];
        int vertexCount = 0;
        foreach (Vector2Int point in pointList)
        {
            Vector3 newVector3 = Vector3.zero;
            newVector3.x = point.x * TileWidth - TileWidth / 2;
            newVector3.z = -point.y * TileWidth + TileWidth / 2;
            newVertex[vertexCount++] = newVector3;
        }

        // �O�p�`���w��
        List<int> myTriangles = new ();
        foreach (Triangle tri in triangles.TriangleSet)
        {
            int[] pNumber = new int[3];
            pNumber[0] = pointList.IndexOf(new((int)tri.P1.x, (int)tri.P1.y));
            pNumber[1] = pointList.IndexOf(new((int)tri.P2.x, (int)tri.P2.y));
            pNumber[2] = pointList.IndexOf(new((int)tri.P3.x, (int)tri.P3.y));

            myTriangles.AddRange(pNumber);
        }

        // ���f������
        myMesh.SetVertices(newVertex);
        myMesh.SetTriangles(myTriangles, 0);
        meshCollider.sharedMesh = myMesh;
    }
}