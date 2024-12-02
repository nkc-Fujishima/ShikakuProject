using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    const int layerMask = ~(1 << 2);

    public event Action<IChaceable> OnSensorInHundle = null;
    public event Action<IChaceable> OnSensorOutHundle = null;

    private void OnTriggerEnter(Collider other)
    {
        // 追跡対象インタフェースを持っている場合のみ、イベントを発火
        IChaceable chaceableObject = null;

        if (!other.transform.TryGetComponent<IChaceable>(out chaceableObject))
        {
            return;
        }

        OnSensorInHundle?.Invoke(chaceableObject);
    }

    private void OnTriggerExit(Collider other)
    {
        // 追跡対象インタフェースを持っている場合のみ、イベントを発火
        IChaceable chaceableObject = null;

        if (!other.TryGetComponent<IChaceable>(out chaceableObject)) return;

        OnSensorOutHundle?.Invoke(chaceableObject);
    }
}
