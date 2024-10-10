using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UIElements;

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


    private Transform stageManagerTransform = null;


    public void OnStart(Transform transform)
    {
        stageManagerTransform = transform;
    }

    // �X�e�[�W�𐶐�����
    public void StageGenerete(int stageCount, out GameObject[] enemyObjs, out GameObject playerObj)
    {
        Vector3 centralPoint = Vector3.zero;
        StageMapData mapData = StageMapData[stageCount];
        centralPoint.x += TileWidth * mapData.X / 2 - TileWidth / 2;
        centralPoint.z -= TileWidth * mapData.Y / 2 - TileWidth / 2;

        StageGenerete(stageCount, centralPoint, out enemyObjs, out playerObj);
    }

    // �X�e�[�W�𐶐�����
    public void StageGenerete(int stageCount, Vector3 centralPoint, out GameObject[] enemyObjs, out GameObject playerObj)
    {
        StageMapData mapData = StageMapData[stageCount];

        List<GameObject> enemyList = new();
        GameObject playerObject = null;

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
                        enemyList.Add(InstanceObject(tileData, countX, countY, instancePosition));
                        InstanceGroundObject(new StageTile(StageTileType.Ground), countX, countY, instancePosition);
                        break;

                    case StageTileType.Player:
                        playerObject = InstanceObject(tileData, countX, countY, instancePosition);
                        InstanceGroundObject(new StageTile(StageTileType.Ground), countX, countY, instancePosition);
                        break;

                    case StageTileType.Obstacle:
                        InstanceObject(tileData, countX, countY, instancePosition);
                        InstanceGroundObject(new StageTile(StageTileType.Ground), countX, countY, instancePosition);
                        break;

                    case StageTileType.Ground:
                        InstanceGroundObject(tileData, countX, countY, instancePosition);
                        break;

                    case StageTileType.None:
                    default:
                        break;
                }
            }
        }

        enemyObjs = enemyList.ToArray();
        playerObj = playerObject;

        // �ǂ����
        if (_stageWallGenerateData)
            _stageWallGenerateData.GenerateSpiralWall(mapData.X, mapData.Y, TileWidth);
    }

    // �I�u�W�F�N�g�𐶐�
    private GameObject InstanceObject(StageTile tileData, int countX, int countY, Vector3 centralPoint)
    {
        Vector3 instancePosition = new(countX * TileWidth, 0, -countY * TileWidth);
        instancePosition += centralPoint;

        Quaternion instanceRotation = Quaternion.Euler(0, tileData.RotationY, 0);

        GameObject objectPlefab = ElementData.GetGameObject(tileData);

        GameObject instanceObject = Instantiate(objectPlefab, instancePosition, instanceRotation);

        if (stageManagerTransform)
            if (tileData.TileType == StageTileType.Obstacle)
                instanceObject.transform.parent = stageManagerTransform;

        return instanceObject;
    }

    // �O���E���h�I�u�W�F�N�g�𐶐�
    private GameObject InstanceGroundObject(StageTile tileData, int countX, int countY, Vector3 centralPoint)
    {
        Vector3 instancePosition = new(countX * TileWidth, -TileHeight, -countY * TileWidth);
        instancePosition += centralPoint;

        GameObject objectPlefab = ElementData.GetGameObject(tileData);

        GameObject instanceObject = Instantiate(objectPlefab, instancePosition, Quaternion.identity);

        if (stageManagerTransform)
            instanceObject.transform.parent = stageManagerTransform;

        return instanceObject;
    }
}
