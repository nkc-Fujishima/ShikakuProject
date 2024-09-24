using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamage iDamage = other.GetComponent<IDamage>();

        iDamage?.Damage();
    }
}