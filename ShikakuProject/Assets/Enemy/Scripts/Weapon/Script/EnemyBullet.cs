using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Rigidbody rigidBody = null;

    AudioClip hitSE = null;

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
        rigidBody.velocity = direction * bulletSpeed;
        this.lifeLimitTime = lifeLimitTime;
        this.hitEffect = hitEffect;
        this.hitSE = hitSE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;

        IDamage iDamage = null;
        if (other.TryGetComponent<IDamage>(out iDamage))
        {
            iDamage.Damage();
        }

        DestroyAudioPlay.PlayClipAtPoint(hitSE, transform.position, 1f);
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}
