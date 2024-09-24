using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    public event Action<IChaceable> OnSensorInHundle = null;
    public event Action<IChaceable> OnSensorOutHundle = null;

    private void OnTriggerEnter(Collider other)
    {
        IChaceable chaceableObject = null;

        if (!other.TryGetComponent<IChaceable>(out chaceableObject)) return;

        OnSensorInHundle?.Invoke(chaceableObject);
    }

    private void OnTriggerExit(Collider other)
    {
        IChaceable chaceableObject = null;

        if (!other.TryGetComponent<IChaceable>(out chaceableObject)) return;

        OnSensorOutHundle?.Invoke(chaceableObject);
    }
}
