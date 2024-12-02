using UnityEngine;

public class DetectionEffectController : MonoBehaviour
{
    [Header("���l�ݒ�"), Tooltip("�G�t�F�N�g�����ʒu�̍���"), SerializeField] float basePosY = 3;

    Transform parent = null;

    /// <summary>
    /// ���g��Ǐ]������Ώۂ����肵�܂�
    /// </summary>
    /// <param name="parent"></param>
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
