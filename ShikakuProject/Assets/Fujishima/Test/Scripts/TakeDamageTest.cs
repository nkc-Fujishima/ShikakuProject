using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageTest : MonoBehaviour,IDamage
{
    public event Action OnDestroyHundle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage()
    {
        Debug.Log("�_���[�W��H�������");
        Destroy(this.gameObject);
    }
}
