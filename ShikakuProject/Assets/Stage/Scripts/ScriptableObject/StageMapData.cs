using UnityEngine;

[CreateAssetMenu(fileName = "StageMapData", menuName = "Stage/Data/StageMap")]
public class StageMapData : ScriptableObject
{
    [SerializeField] private int x = 0;
    [SerializeField] private int y = 0;

    public int X
    {
        get { return x; }
        set { x = value; }
    }
    public int Y
    {
        get { return y; }
        set { y = value; }
    }


    public StageTiles[] TileDatas;

    [System.Serializable]
    public struct StageTiles
    {
        public StageTile[] TileData;
    }

    public StageTiles[] DotsMapTile
    {
        get { return this.TileDatas; }
        set { TileDatas = value; }
    }

    public void ResetArray()
    {
        DotsMapTile = new StageTiles[x];
        for (int i = 0; i < x; i++)
        {
            DotsMapTile[i].TileData = new StageTile[y];
        }
    }
}
