using ObjectPool;
using UnityEngine;

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

        [SerializeField]
        private Sprite _texture;

        private float _coolTimeCount;


        readonly public GameObject BulletPlefab => _bulletPlefab.gameObject;

        readonly public BulletControllerBase Bullet => _bulletPlefab;

        readonly public float CoolTime => _coolTime;

        readonly public float CoolTimeCount => _coolTimeCount;

        readonly public bool IsSelectable => (_coolTimeCount >= _coolTime);

        readonly public Sprite Texture => _texture;


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


    internal int GetSkillLength => _skillData.Length;

    internal float GetSkillCoolTime(int bulletType) => _skillData[bulletType].CoolTime;

    internal float GetSkillCoolTimeCount(int bulletType) => _skillData[bulletType].CoolTimeCount;

    internal bool GetSkillIsSelectable(int bulletType) => _skillData[bulletType].IsSelectable;

    internal void GetSkillCheckCoolTimeCount(int bulletType, float deltaTime) => _skillData[bulletType].CheckCoolTimeCount(deltaTime);

    internal void GetSkillSpawnBullet(int selectType) => _skillData[selectType].SpawnBullet();


    internal float MoveSpeed => _moveSpeed;


    internal void OnStart()
    {

    }

    public BulletControllerBase[] GetAllBulletPlefab()
    {
        BulletControllerBase[] bulletPrefabs = new BulletControllerBase[_skillData.Length];

        for (int i = 0; i < bulletPrefabs.Length; ++i)
        {
            bulletPrefabs[i] = _skillData[i].Bullet;
        }

        return bulletPrefabs;
    }

    public Sprite[] GetAllBulletTexture()
    {
        Sprite[] textures = new Sprite[_skillData.Length];

        for (int i = 0; i < textures.Length; ++i)
        {
            textures[i] = _skillData[i].Texture;
        }

        return textures;
    }
}