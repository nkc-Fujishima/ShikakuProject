using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionEffectController : MonoBehaviour
{
    [Header("���l�ݒ�"), Tooltip("�G�t�F�N�g�����ʒu�̍���"), SerializeField] float basePosY = 3;

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
