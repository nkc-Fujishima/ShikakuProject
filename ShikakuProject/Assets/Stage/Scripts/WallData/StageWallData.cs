using UnityEngine;

public class StageWallData : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _scale;

    public Vector2Int Scale { get { return _scale; } }

    public Vector2Int GetUpLeftPoint(Vector2Int point,int rotate)
    {
        Vector2Int upLeftPoint = point;

        Vector2Int misalignmentPoint = new (Scale.x - 1, Scale.y - 1);

        switch (rotate)
        {
            case 0:
            default:
                return upLeftPoint;
            case 90:
                upLeftPoint.x = point.x - misalignmentPoint.y;
                return upLeftPoint;
            case 180:
                upLeftPoint.x = point.x - misalignmentPoint.x;
                upLeftPoint.y = point.y + misalignmentPoint.y;
                return upLeftPoint;
            case 270:
                upLeftPoint.y = point.y + misalignmentPoint.x;
                return upLeftPoint;
        }
    }
}
