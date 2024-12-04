public enum StageTileType
{
    Ground = 0,
    Obstacle,
    Player,
    Enemy,
    None,
    Tutorial,
}

[System.Serializable]
public struct StageTile
{
    public StageTileType TileType;
    public int ElementCount;
    public int RotationY;

    public StageTile(StageTileType tileType, int elementCount = 0)
    {
        this.TileType = tileType;
        ElementCount = elementCount;
        RotationY = 0;
    }
}