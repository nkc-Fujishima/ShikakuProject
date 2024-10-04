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
    public int Rotation;

    public StageTile(StageTileType tileType)
    {
        this.TileType = tileType;
        ElementCount = 0;
        Rotation = 0;
    }
}