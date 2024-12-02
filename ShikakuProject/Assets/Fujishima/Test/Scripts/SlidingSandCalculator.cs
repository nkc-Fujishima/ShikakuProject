using UnityEngine;

public class SlidingSandCalculator : MonoBehaviour, ISandSlidable
{
    Rigidbody rigidbody = null;

    Vector3 returnValue = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void OnSlidingSandEffect(Vector3 slideDirection, float brekePower)
    {
        returnValue = (rigidbody.velocity * brekePower) + slideDirection;
    }

    public void OffSlidingSandEffect()
    {
        returnValue = Vector3.zero;
    }

    public Vector3 GetSlideDirection()
    {
        return returnValue;
    }
}
