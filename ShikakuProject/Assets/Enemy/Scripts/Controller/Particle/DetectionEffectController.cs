using UnityEngine;

public class DetectionEffectController : MonoBehaviour
{
    [Header("数値設定"), Tooltip("エフェクト発生位置の高さ"), SerializeField] float basePosY = 3;

    Transform parent = null;

    /// <summary>
    /// 自身を追従させる対象を決定します
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
