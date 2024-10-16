using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Rigidbody rigidBody = null;

    float timeCount = 0;

    float lifeLimitTime = 0;

    // ��������Ă���̎��Ԃ��v���A��莞�Ԑ����̏ꍇ�A�j��
    private void Update()
    {
        timeCount += Time.deltaTime;

        if (timeCount > lifeLimitTime) Destroy(this.gameObject);
    }

    // �U�������A�ő吶�����Ԃ�����
    public void Construct(Vector3 direction, float bulletSpeed,float lifeLimitTime)
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = direction * bulletSpeed;
        this.lifeLimitTime = lifeLimitTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;

        IDamage iDamage = null;
        if(other.TryGetComponent<IDamage>(out iDamage))
        {
            iDamage.Damage();
        }

        Debug.Log(other.name);
        Destroy(this.gameObject);
    }
}
