using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Effect/ShootEnemy", fileName = "ShootEnemyEffectObject")]
public class ShootEnemyEffectData : EnemyEffectDataBase
{
    [Tooltip("���ˉ�SE")] public AudioClip shootSE;
}
