using System.Collections.Generic;
using UnityEngine;

public interface IWaypointHolder
{
    // 巡回ポイントを保持する
    // 実装には以下の４行を書けばOK
    // public List<Vector3> Waypoints { get; private set; } = new();

    //public void SetWaypoints(List<Vector3> points)
    //{
    //    Waypoints = new List<Vector3>(points);
    //}


    public List<Vector3> Waypoints { get; }

    // 座標を設定するメソッド
    public void SetWaypoints(List<Vector3> points);
}
