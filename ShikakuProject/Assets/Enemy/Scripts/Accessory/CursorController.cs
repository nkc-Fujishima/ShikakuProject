using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Tooltip("回転速度"), SerializeField] float rotateSpeed;
    [Tooltip("オブジェクトのY軸の高さ"), SerializeField] float PositionY;

    Transform parent = null;
    public void Construct(Transform parent)
    {
        this.parent = parent;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + PositionY, parent.transform.position.z);

        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);

        // 親のオブジェクトの表示がオフになった場合、削除
        if (parent.gameObject.activeSelf == false) OnDestroy();
    }

    private void OnDestroy()
    {
        Destroy(this.gameObject);
    }
}
