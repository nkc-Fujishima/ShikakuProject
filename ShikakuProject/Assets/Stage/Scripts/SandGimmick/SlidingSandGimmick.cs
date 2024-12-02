using UnityEngine;

public class SlidingSandGimmick : MonoBehaviour
{
    [Header("”’lİ’è"), Tooltip("‰Ÿ‚µ—¬‚·—Í"), SerializeField] SandGimmickParameter parameter;

    private void OnTriggerStay(Collider other)
    {
        ISandSlidable sandSlidable = null;
        if (!other.TryGetComponent<ISandSlidable>(out sandSlidable)) return;

        sandSlidable.OnSlidingSandEffect(transform.forward * parameter.SlidePower,parameter.BrekePower);
    }

    private void OnTriggerExit(Collider other)
    {
        ISandSlidable sandSlidable = null;
        if (!other.TryGetComponent<ISandSlidable>(out sandSlidable)) return;

        sandSlidable.OffSlidingSandEffect();
    }
}
