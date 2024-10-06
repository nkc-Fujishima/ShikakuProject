using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageGenereteData", menuName = "Stage/Generator/GenerateData")]
public class StageGenereteData : ScriptableObject
{
    public StageMapData[] StageMapData;

    public StageObjectElementData ElementData;

    [SerializeField]
    private float TileWidth = 1;
    [SerializeField]
    private float TileHeight = 1;


    [SerializeField]
    private StageWallGenerateData _stageWallGenerateData;


    private Transform stageManagerTransform;

    public void OnStart(Transform transform)
    {
        stageManagerTransform = transform;
    }

    // ステージを生成する
    public void StageGenerete(int stageCount, out GameObject[] enemyObjs, out GameObject playerObj)
    {
        StageMapData mapData = StageMapData[stageCount];

        List<GameObject> enemyList = new();
        GameObject playerObject = null;

        for (int countY = 0; countY < mapData.Y; ++countY)
        {
            for (int countX = 0; countX < mapData.X; ++countX)
            {
                // マップデータから該当するステージタイルを取得
                StageTile tileData = mapData.TileDatas[countX].TileData[countY];

                // オブジェクトを生成
                // ------------------------------------------------------------------------------
                // グラウンド         => グラウンドのみ生成
                // その他オブジェクト => グラウンド ＋ ステージタイルに対応するオブジェクトを生成
                // 何もなし(None)     => 何も生成しない
                switch (tileData.TileType)
                {
                    case StageTileType.Enemy:
                        enemyList.Add(InstanceObject(tileData, countX, countY));
                        InstanceGroundObject(new StageTile(StageTileType.Ground), countX, countY);
                        break;

                    case StageTileType.Player:
                        playerObject = InstanceObject(tileData, countX, countY);
                        InstanceGroundObject(new StageTile(StageTileType.Ground), countX, countY);
                        break;

                    case StageTileType.Obstacle:
                        InstanceObject(tileData, countX, countY);
                        InstanceGroundObject(new StageTile(StageTileType.Ground), countX, countY);
                        break;

                    case StageTileType.Ground:
                        InstanceGroundObject(tileData, countX, countY);
                        break;

                    case StageTileType.None:
                    default:
                        break;
                }
            }
        }

        enemyObjs = enemyList.ToArray();
        playerObj = playerObject;

        // 壁を作る
        if (_stageWallGenerateData)
            _stageWallGenerateData.GenerateSpiralWall(mapData.X, mapData.Y, TileWidth);
    }

    // オブジェクトを生成
    private GameObject InstanceObject(StageTile tileData, int countX, int countY)
    {
        Vector3 instancePosition = new(countX * TileWidth, 0, -countY * TileWidth);

        Quaternion instanceRotation = Quaternion.Euler(0, tileData.RotationY, 0);

        GameObject objectPlefab = ElementData.GetGameObject(tileData);

        GameObject instanceObject = Instantiate(objectPlefab, instancePosition, instanceRotation);

        if (tileData.TileType == StageTileType.Obstacle)
            instanceObject.transform.parent = stageManagerTransform;

        return instanceObject;
    }

    // グラウンドオブジェクトを生成
    private GameObject InstanceGroundObject(StageTile tileData, int countX, int countY)
    {
        Vector3 instancePosition = new(countX * TileWidth, -TileHeight, -countY * TileWidth);

        GameObject objectPlefab = ElementData.GetGameObject(tileData);

        GameObject instanceObject = Instantiate(objectPlefab, instancePosition, Quaternion.identity);

        instanceObject.transform.parent = stageManagerTransform;

        return instanceObject;
    }
}
