using System.Collections.Generic;
using UnityEngine;

public interface IWaypointHolder
{
    public List<Vector3> Waypoints { get; }

    // 座標を設定するメソッド
    public void SetWaypoints(List<Vector3> points)
    {
        Waypoints.Clear();
        Waypoints.AddRange(points);
    }
}
