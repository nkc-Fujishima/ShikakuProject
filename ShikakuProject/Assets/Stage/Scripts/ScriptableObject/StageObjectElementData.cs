using UnityEngine;

[CreateAssetMenu(fileName = "StageObjectElementData", menuName = "Stage/Data/StageObjectElement")]
public class StageObjectElementData : ScriptableObject
{
    [System.Serializable]
    public struct PlayerElementData
    {
        [SerializeField]
        public PlayerCharaController PlayerPlefab;

        [SerializeField]
        public Texture2D Texture;
    }

    [System.Serializable]
    public struct EnemyElementData
    {
        [SerializeField]
        public EnemyControllerBase[] EnemyPlefabs;

        [SerializeField]
        public Texture2D Texture;
    }

    [System.Serializable]
    public struct PrefabElementData
    {
        [SerializeField]
        public GameObject[] Prefabs;

        [SerializeField]
        public Texture2D Texture;
    }
    

    public PlayerElementData PlayerData;

    public EnemyElementData EnemyDatas;

    public PrefabElementData GroundData;

    public PrefabElementData ObstacleData;


    public GameObject GetGameObject(StageTile tileData)
    {
        switch (tileData.TileType)
        {
            case StageTileType.Player:
                return PlayerData.PlayerPlefab.gameObject;

            case StageTileType.Enemy:
                return EnemyDatas.EnemyPlefabs[tileData.ElementCount].gameObject;

            case StageTileType.Ground:
                return GroundData.Prefabs[tileData.ElementCount];

            case StageTileType.Obstacle:
                return ObstacleData.Prefabs[tileData.ElementCount];
           
            default:
                return null;
        }
    }
}
