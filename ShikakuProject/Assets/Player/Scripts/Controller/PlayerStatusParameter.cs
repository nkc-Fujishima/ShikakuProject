using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameter", menuName = "Player/Paramenter")]
public class PlayerStatusParameter : ScriptableObject
{

    // �X�L����ۑ�����
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


        // �N�[���^�C�����v�Z����
        public void CheckCoolTimeCount(in float time)
        {
            _coolTimeCount += time;
            if (!IsSelectable) return;

            _coolTimeCount = _coolTime;
        }

        // �I�u�W�F�N�g�𐶐�����
        public void SpawnBullet()
        {
            // �N�[���_�E����ݒ�
            _coolTimeCount = 0;
        }
    }


    [SerializeField]
    private Skill[] _skillData;

    [SerializeField]
    private float _moveSpeed;


    internal int GetSkillLength => _skillData.Length;

    internal GameObject GetSkillSelectBulletPlefab => _skillData[SelectBulletType].BulletPlefab;

    internal float GetSkillCoolTime(int bulletType) => _skillData[bulletType].CoolTime;

    internal float GetSkillCoolTimeCount(int bulletType) => _skillData[bulletType].CoolTimeCount;

    internal bool GetSkillIsSelectable(int bulletType) => _skillData[bulletType].IsSelectable;

    internal void GetSkillCheckCoolTimeCount(int bulletType, float deltaTime) => _skillData[bulletType].CheckCoolTimeCount(deltaTime);

    internal void GetSkillSpawnBullet() => _skillData[SelectBulletType].SpawnBullet();


    internal float MoveSpeed => _moveSpeed;

    [HideInInspector]
    internal int SelectBulletType { get; set; }


    internal void OnStart()
    {
        SelectBulletType = 0;
    }
}