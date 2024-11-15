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

    // 11/14追加：チュートリアル要素
    [System.Serializable]
    public struct TutorialElementData
    {
        [SerializeField]
        public TutorialTextData[] TextDatas;

        [SerializeField]
        public GameObject[] talkCharaPrefabs;

        [SerializeField]
        public Texture2D Texture;

        public GameObject GetGameObject(int index)
        {
            int prefabIndex = TextDatas[index].PrefabIndex;
            return talkCharaPrefabs[prefabIndex];
        }
    }

    // 11/14追加：チュートリアル要素
    [System.Serializable]
    public struct TutorialTextData
    {
        [Header("話す内容")]
        [SerializeField]
        [Multiline(3)]
        public string TalkText;

        [SerializeField]
        public int PrefabIndex;
    }
    

    public PlayerElementData PlayerData;

    public EnemyElementData EnemyDatas;

    public PrefabElementData GroundData;

    public PrefabElementData ObstacleData;

    // 11/14追加：チュートリアル要素
    public TutorialElementData TutorialData;


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

            // 11/14追加：チュートリアル要素
            case StageTileType.Tutorial:
                int index = TutorialData.TextDatas[tileData.ElementCount].PrefabIndex;
                return TutorialData.talkCharaPrefabs[index];

            default:
                return null;
        }
    }
}
