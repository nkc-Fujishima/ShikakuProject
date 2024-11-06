using System.Collections.Generic;
using UnityEngine;

public interface IWaypointHolder
{
    // ����|�C���g��ێ�����
    // �����ɂ͈ȉ��̂S�s��������OK
    // public List<Vector3> Waypoints { get; private set; } = new();

    //public void SetWaypoints(List<Vector3> points)
    //{
    //    Waypoints = new List<Vector3>(points);
    //}


    public List<Vector3> Waypoints { get; }

    // ���W��ݒ肷�郁�\�b�h
    public void SetWaypoints(List<Vector3> points);
}
