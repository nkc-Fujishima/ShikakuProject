using UnityEngine;

public class YagezawaTestEnemy : EnemyControllerBase
{
    private new void Start()
    {
        base.Start();
        OnDestroyHundle += test;
    }

    void test()
    {
        Debug.Log("Ž€‚ñ‚¾");
    }
}
