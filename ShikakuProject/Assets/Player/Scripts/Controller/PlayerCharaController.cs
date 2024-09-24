using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerCharaController : MonoBehaviour, IChaceable, IDamage, IStateChangeable
{
    [System.Serializable]
    public class PlayerData
    {
        public void OnStart(Transform playerTransform)
        {
            _playerTransform = playerTransform;

            _rigidbody = playerTransform.GetComponent<Rigidbody>();
        }

        private Rigidbody _rigidbody;

        private Transform _playerTransform;

        [SerializeField]
        private Transform _spawnBulletPoint;

        [SerializeField]
        private Animator _animator;


        [HideInInspector]
        public UnityEvent OnDeath = new();

        [HideInInspector]
        public UnityEvent<BulletControllerBase> OnBulletSpawn = new();


        public Rigidbody Rigidbody => _rigidbody;

        public Transform PlayerTransform => _playerTransform;

        public Transform SpawnBulletPoint => _spawnBulletPoint;

        public Animator Animator => _animator;
    }

    [SerializeField]
    public PlayerData Datas;

    [SerializeField]
    private PlayerStatusParameter PlayerStatus;


    private IState _iState = null;

    private PlayerButtonDetector _buttonDetector;




    void Start()
    {
        Datas.OnStart(transform);

        PlayerStatus.OnStart();

        PlayerInput playerInput = GetComponent<PlayerInput>();
        _buttonDetector = new(playerInput);

        SetDelegate();


        PlayerStateHolder stateHolder = new(this, _buttonDetector, PlayerStatus, Datas);
        _iState = stateHolder.IdleState;
        _iState.OnEnter();
    }

    private void Update()
    {
        _iState.OnUpdate();

        CheckSkillCoolTime();
    }


    //----------------------------------------------------------------------------------
    // �X�e�[�g���w�肳�ꂽ��Ԃɕω�������
    public void ChangeState(IState nextState)
    {
        if (_iState != null) _iState.OnExit();

        _iState = nextState;

        if (_iState != null) _iState.OnEnter();
    }

    //----------------------------------------------------------------------------------
    // �X�e�[�g�ێ�
    private class PlayerStateHolder
    {
        public Idle IdleState { get; }
        public Walk WalkState { get; }
        public Fire FireState { get; }

        readonly PlayerButtonDetector chaceableObjects = null;

        readonly IStateChangeable stateChanger = null;

        readonly PlayerStatusParameter playerStatus = null;

        readonly PlayerData data = null;

        public PlayerStateHolder(IStateChangeable stateChanger, PlayerButtonDetector button, PlayerStatusParameter playerStatus, PlayerData datas)
        {
            this.stateChanger = stateChanger;
            this.chaceableObjects = button;
            this.playerStatus = playerStatus;
            this.data = datas;

            IdleState = new Idle(stateChanger, this, button, playerStatus, datas);
            WalkState = new Walk(stateChanger, this, button, playerStatus, datas);
            FireState = new Fire(stateChanger, this, button, playerStatus, datas);
        }
    }

    //----------------------------------------------------------------------------------
    // �X�e�[�g�̊��N���X
    private abstract class PlayerStateBase : StateBase
    {
        protected IStateChangeable stateChanger = null;
        protected PlayerStateHolder stateHolder = null;

        protected PlayerButtonDetector buttonDetector = null;
        protected PlayerStatusParameter playerStatus = null;
        protected PlayerData data;

        public PlayerStateBase(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data)
        {
            this.stateChanger = stateChanger;
            this.stateHolder = stateHolder;
            this.buttonDetector = buttonDetector;
            this.playerStatus = playerStatus;
            this.data = data;
        }

        // �e�𐶐�����{�^�����������ꍇ�̊֐�
        public void OnFire()
        {
            if (!playerStatus.GetSkillIsSelectable(playerStatus.SelectBulletType)) return;

            stateChanger.ChangeState(stateHolder.FireState);
        }
    }

    //----------------------------------------------------------------------------------
    // �~�܂�X�e�[�g
    private class Idle : PlayerStateBase
    {
        public Idle(IStateChangeable stateChanger,PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector ,PlayerStatusParameter playerStatus, PlayerData data) : base(stateChanger, stateHolder,buttonDetector,playerStatus, data) { }

        public override void OnEnter()
        {
            data.Animator.SetTrigger("Idle");

            buttonDetector.OnButtonFireDown.RemoveListener(OnFire);
            buttonDetector.OnButtonFireDown.AddListener(OnFire);
        }

        public override void OnExit()
        {
            buttonDetector.OnButtonFireDown.RemoveListener(OnFire);
        }

        public override void OnUpdate()
        {
            if (buttonDetector.InputStick != Vector3.zero)
                stateChanger.ChangeState(stateHolder.WalkState);

        }
    }

    //----------------------------------------------------------------------------------
    // �ړ�����X�e�[�g
    private class Walk : PlayerStateBase
    {
        public Walk(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data) : base(stateChanger, stateHolder, buttonDetector, playerStatus,data) { }

        public override void OnEnter()
        {
            data.Animator.SetTrigger("Walk");

            buttonDetector.OnButtonFireDown.RemoveListener(OnFire);
            buttonDetector.OnButtonFireDown.AddListener(OnFire);
        }

        public override void OnExit()
        {
            buttonDetector.OnButtonFireDown.RemoveListener(OnFire);
        }

        public override void OnUpdate()
        {
            CheckCharaMove();

            if (buttonDetector.InputStick == Vector3.zero)
                stateChanger.ChangeState(stateHolder.IdleState);
        }

        // �ړ����m�F����
        private void CheckCharaMove()
        {
            Vector3 movePower = buttonDetector.InputStick * playerStatus.MoveSpeed;

            data.Rigidbody.velocity = movePower;

            // �̂�i�s�����ɉ�]������
            if (movePower == Vector3.zero) return;
            float moveRotationY = Mathf.Atan2(-movePower.z, movePower.x) * Mathf.Rad2Deg + 90;
            data.PlayerTransform.rotation = Quaternion.Euler(0, moveRotationY, 0);
        }
    }

    //----------------------------------------------------------------------------------
    // �e���o��������X�e�[�g
    private class Fire : PlayerStateBase
    {
        public Fire(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data) : base(stateChanger, stateHolder, buttonDetector, playerStatus, data) { }

        public override void OnEnter()
        {
            data.Animator.SetTrigger("Fire");
            playerStatus.GetSkillSpawnBullet();

            // ����
            GameObject bullet = Instantiate(playerStatus.GetSkillSelectBulletPlefab, data.SpawnBulletPoint.position, data.SpawnBulletPoint.rotation);

            data.OnBulletSpawn.Invoke(bullet.GetComponent<BulletControllerBase>());
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate()
        {
            ChangeState();
        }

        private void ChangeState()
        {
            if (buttonDetector.InputStick == Vector3.zero)
                stateChanger.ChangeState(stateHolder.IdleState);
            else
                stateChanger.ChangeState(stateHolder.WalkState);
        }
    }


    //----------------------------------------------------------------------------------
    // �X�L���̃N�[���_�E�����v�Z
    private void CheckSkillCoolTime()
    {
        for (int i = 0; i < PlayerStatus.GetSkillLength; ++i)
        {
            if (PlayerStatus.GetSkillIsSelectable(i)) continue;

            PlayerStatus.GetSkillCheckCoolTimeCount(i, Time.deltaTime);
        }
    }

    //----------------------------------------------------------------------------------
    // �f���Q�[�g��ݒ肷��
    private void SetDelegate()
    {
        _buttonDetector.OnButtonBulletSelectLeftDown.RemoveListener(OnBulletSelectLeft);
        _buttonDetector.OnButtonBulletSelectLeftDown.AddListener(OnBulletSelectLeft);

        _buttonDetector.OnButtonBulletSelectRightDown.RemoveListener(OnBulletSelectRight);
        _buttonDetector.OnButtonBulletSelectRightDown.AddListener(OnBulletSelectRight);
    }

    // �E�ɑI��
    private void OnBulletSelectLeft()
    {
        --PlayerStatus.SelectBulletType;

        if (PlayerStatus.SelectBulletType < 0)
            PlayerStatus.SelectBulletType = PlayerStatus.GetSkillLength - 1;
    }

    // ���ɑI��
    private void OnBulletSelectRight()
    {
        ++PlayerStatus.SelectBulletType;

        if (PlayerStatus.SelectBulletType >= PlayerStatus.GetSkillLength)
            PlayerStatus.SelectBulletType = 0;
    }


    //----------------------------------------------------------------------------------
    // IChaceable
    public Transform chacebleTransform { get { return transform; } }

    //----------------------------------------------------------------------------------
    // IDamage
    public void Damage()
    {
        Datas.OnDeath.Invoke();

        gameObject.SetActive(false);
    }
}