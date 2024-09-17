using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerParameter", menuName = "Player/Paramenter")]
public class PlayerStatusParameter : ScriptableObject
{

    // スキルを保存する
    [System.Serializable]
    private struct Skill
    {
        [SerializeField]
        private BulletControllerBase _bulletPlefab;

        [SerializeField]
        private float _coolTime;

        private float _coolTimeCount;


        public GameObject BulletPlefab => _bulletPlefab.gameObject;

        public float CoolTime => _coolTime;

        public float CoolTimeCount => _coolTimeCount;

        public bool IsSelectable => (_coolTimeCount >= _coolTime);


        // クールタイムを計算する
        public void CheckCoolTimeCount(in float time)
        {
            _coolTimeCount += time;
            if (!IsSelectable) return;

            _coolTimeCount = _coolTime;
        }

        // オブジェクトを生成する
        public void SpawnBullet()
        {
            // クールダウンを設定
            _coolTimeCount = 0;
        }
    }


    [SerializeField]
    private Skill[] _skillData;

    [SerializeField]
    private float _moveSpeed;

    private Rigidbody _rigidbody;

    private Transform _playerTransform;

    private Transform _spawnBulletPoint;

    private Animator _animator;


    internal int GetSkillLength => _skillData.Length;

    internal GameObject GetSkillSelectBulletPlefab => _skillData[SelectBulletType].BulletPlefab;

    internal float GetSkillCoolTime(int bulletType) => _skillData[bulletType].CoolTime;

    internal float GetSkillCoolTimeCount(int bulletType) => _skillData[bulletType].CoolTimeCount;

    internal bool GetSkillIsSelectable(int bulletType) => _skillData[bulletType].IsSelectable;

    internal void GetSkillCheckCoolTimeCount(int bulletType, float deltaTime) => _skillData[bulletType].CheckCoolTimeCount(deltaTime);

    internal void GetSkillSpawnBullet() => _skillData[SelectBulletType].SpawnBullet();


    internal float MoveSpeed => _moveSpeed;

    internal Rigidbody Rigidbody => _rigidbody;

    internal Transform PlayerTransform => _playerTransform;

    internal Transform SpawnBulletPoint => _spawnBulletPoint;

    internal Animator Animator => _animator;

    [HideInInspector]
    internal int SelectBulletType { get; set; }


    [HideInInspector]
    public UnityEvent<BulletControllerBase> OnBulletSpawn;


    internal void OnStart(Rigidbody rigidbody, Transform playerTransform, Transform spawnBulletPoint, Animator animator)
    {
        SelectBulletType = 0;

        _rigidbody = rigidbody;

        _playerTransform = playerTransform;

        _spawnBulletPoint = spawnBulletPoint;

        _animator = animator;
    }
}