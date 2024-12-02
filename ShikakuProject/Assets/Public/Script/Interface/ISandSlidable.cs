using UnityEngine;

public interface ISandSlidable
{
    public void OnSlidingSandEffect(Vector3 slideDirection, float brekePower)
    {
        Debug.Log("OnSlidingSandEffect : �f�t�H���g�����N�����A�����̉e�����󂯂Ă��܂�");
    }

    public void OffSlidingSandEffect() { Debug.Log("OffSlidingSandEffect : �f�t�H���g�����N�����A�������甲���܂���"); }

    public Vector3 GetSlideDirection()
    {
        Debug.Log("OffSlidingSandEffect : �f�t�H���g�����N�����A�����̕������Q�Ƃ��Ă܂�");
        return Vector3.zero;
    }
}
