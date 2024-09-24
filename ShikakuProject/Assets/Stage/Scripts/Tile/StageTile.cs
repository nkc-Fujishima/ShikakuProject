public enum StageTileType
{
    Ground = 0,
    Obstacle,
    Player,
    Enemy,
    None,
}

[System.Serializable]
public struct StageTile
{
    public StageTileType TileType;
    public int ElementCount;

    public StageTile(StageTileType tileType)
    {
        this.TileType = tileType;
        ElementCount = 0;
    }
}