using UnityEngine;

public class MarineController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent<IFallable>(out IFallable damage))
        {
            damage.FallRiver();
        }
    }
}
