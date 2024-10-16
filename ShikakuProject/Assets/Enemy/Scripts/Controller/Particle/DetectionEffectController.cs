using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionEffectController : MonoBehaviour
{
    [Header("数値設定"), Tooltip("エフェクト発生位置の高さ"), SerializeField] float basePosY = 3;

    Transform parent = null;

    public void Construct(Transform parent)
    {
        this.parent = parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (parent == null) return;

        transform.position = new Vector3(parent.position.x, parent.position.y + basePosY, parent.position.z);
    }
}
