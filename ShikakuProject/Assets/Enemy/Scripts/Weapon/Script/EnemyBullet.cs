using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Rigidbody rigidBody = null;

    AudioSource audioSource = null;

    ParticleSystem hitEffect = null;

    float timeCount = 0;

    float lifeLimitTime = 0;

    // 生成されてからの時間を計測、一定時間生存の場合、破棄
    private void Update()
    {
        timeCount += Time.deltaTime;

        if (timeCount > lifeLimitTime) Destroy(this.gameObject);
    }

    // 攻撃方向、最大生存時間を決定
    public void Construct(Vector3 direction, float bulletSpeed, float lifeLimitTime, ParticleSystem hitEffect, AudioClip hitSE)
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        rigidBody.velocity = direction * bulletSpeed;
        this.lifeLimitTime = lifeLimitTime;
        this.hitEffect = hitEffect;
        audioSource.clip = hitSE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;

        IDamage iDamage = null;
        if (other.TryGetComponent<IDamage>(out iDamage))
        {
            iDamage.Damage();
        }

        audioSource.Play();
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
