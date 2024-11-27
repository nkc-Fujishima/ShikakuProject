using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageMapData", menuName = "Stage/Data/StageMap")]
[System.Serializable]
public class StageMapData : ScriptableObject
{
    [System.Serializable]
    public struct StageTiles
    {
        public StageTile[] TileData;
    }

    [System.Serializable]
    public class StageWaypointData
    {
        public Vector2Int[] Waypoint; 
        public Vector2Int[] EnemyAtPoint;
    }


    [SerializeField] private int x = 0;
    [SerializeField] private int y = 0;

    private StageTiles[] DotsMapTile { get { return this.TileDatas; } set { TileDatas = value; } }


    public StageTiles[] TileDatas;

    public List<StageWaypointData> WaypointData = new();

    public bool IsTutorial = false;


    public int X { get { return x; } set { x = value; } }
    public int Y { get { return y; } set { y = value; } }


    public void ResetArray()
    {
        ResetArray(x, y);
    }

    private void ResetArray(int selectX,int selectY)
    {
        DotsMapTile = new StageTiles[selectX];
        for (int i = 0; i < selectX; i++)
        {
            DotsMapTile[i].TileData = new StageTile[selectY];
        }
    }

    public void CopyTileData(StageMapData mapData)
    {
        x = mapData.x; 
        y = mapData.y;

        ResetArray(x, y);

        for (int selectX = 0; selectX < x; ++selectX)
        {
            for (int selectY = 0; selectY < y; ++selectY)
            {
                DotsMapTile[selectX].TileData[selectY] = mapData.TileDatas[selectX].TileData[selectY];
            }
        }
    }

    public void CopyWaypointData(StageMapData mapData)
    {
        WaypointData = new(mapData.WaypointData);
    }
}
