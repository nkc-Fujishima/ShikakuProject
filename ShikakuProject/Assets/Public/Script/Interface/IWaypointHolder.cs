using System.Collections.Generic;
using UnityEngine;

public interface IWaypointHolder
{
    public List<Vector3> Waypoints { get; }

    // ���W��ݒ肷�郁�\�b�h
    public void SetWaypoints(List<Vector3> points)
    {
        Waypoints.Clear();
        Waypoints.AddRange(points);
    }
}
