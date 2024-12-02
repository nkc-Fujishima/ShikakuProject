using UnityEngine;

public interface ISandSlidable
{
    public void OnSlidingSandEffect(Vector3 slideDirection, float brekePower)
    {
        Debug.Log("OnSlidingSandEffect : デフォルト実装起動中、流砂の影響を受けています");
    }

    public void OffSlidingSandEffect() { Debug.Log("OffSlidingSandEffect : デフォルト実装起動中、流砂から抜けました"); }

    public Vector3 GetSlideDirection()
    {
        Debug.Log("OffSlidingSandEffect : デフォルト実装起動中、流砂の方向を参照してます");
        return Vector3.zero;
    }
}
