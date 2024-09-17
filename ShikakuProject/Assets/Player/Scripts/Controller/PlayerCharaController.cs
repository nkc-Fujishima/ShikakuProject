using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerCharaController : MonoBehaviour, IChaceable, IDamage, IStateChangeable
{
    [SerializeField]
    public PlayerStatusParameter PlayerStatus;

    [SerializeField]
    private Transform _spawnBulletPoint;

    [SerializeField]
    private Animator _animator;


    private IState _iState = null;

    private PlayerButtonDetector _buttonDetector;


    void Start()
    {
        PlayerStatus.OnStart(GetComponent<Rigidbody>(), transform, _spawnBulletPoint, _animator);

        PlayerInput playerInput = GetComponent<PlayerInput>();
        _buttonDetector = new(playerInput);

        SetDelegate();


        PlayerStateHolder stateHolder = new (this, _buttonDetector, PlayerStatus);
        _iState = stateHolder.IdleState;
    }

    private void Update()
    {
        _iState.OnUpdate();

        CheckSkillCoolTime();
    }


    //----------------------------------------------------------------------------------
    // ステートを指定された状態に変化させる
    public void ChangeState(IState nextState)
    {
        if (_iState != null) _iState.OnExit();

        _iState = nextState;

        if (_iState != null) _iState.OnEnter();
    }

    //----------------------------------------------------------------------------------
    // ステート保持
    private class PlayerStateHolder
    {
        public Idle IdleState { get; }
        public Walk WalkState { get; }
        public Fire FireState { get; }

        readonly PlayerButtonDetector chaceableObjects = null;

        readonly IStateChangeable stateChanger = null;

        readonly PlayerStatusParameter playerStatus = null;

        public PlayerStateHolder(IStateChangeable stateChanger, PlayerButtonDetector button, PlayerStatusParameter playerStatus)
        {
            this.stateChanger = stateChanger;
            this.chaceableObjects = button;
            this.playerStatus = playerStatus;

            IdleState = new Idle(stateChanger, this, button, playerStatus);
            WalkState = new Walk(stateChanger, this, button, playerStatus);
            FireState = new Fire(stateChanger, this, button, playerStatus);
        }
    }

    //----------------------------------------------------------------------------------
    // ステートの基底クラス
    private abstract class PlayerStateBase : StateBase
    {
        protected IStateChangeable stateChanger = null;
        protected PlayerStateHolder stateHolder = null;

        protected PlayerButtonDetector buttonDetector = null;
        protected PlayerStatusParameter playerStatus = null;

        public PlayerStateBase(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus)
        {
            this.stateChanger = stateChanger;
            this.stateHolder = stateHolder;
            this.buttonDetector = buttonDetector;
            this.playerStatus = playerStatus;
        }

        // 弾を生成するボタンを押した場合の関数
        public void OnFire()
        {
            if (!playerStatus.GetSkillIsSelectable(playerStatus.SelectBulletType)) return;

            stateChanger.ChangeState(stateHolder.FireState);
        }
    }

    //----------------------------------------------------------------------------------
    // 止まるステート
    private class Idle : PlayerStateBase
    {
        public Idle(IStateChangeable stateChanger,PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector ,PlayerStatusParameter playerStatus) : base(stateChanger, stateHolder,buttonDetector,playerStatus) { }

        public override void OnEnter()
        {
            playerStatus.Animator.SetTrigger("Idle");

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
    // 移動するステート
    private class Walk : PlayerStateBase
    {
        public Walk(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus) : base(stateChanger, stateHolder, buttonDetector, playerStatus) { }

        public override void OnEnter()
        {
            playerStatus.Animator.SetTrigger("Walk");

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

        // 移動を確認する
        private void CheckCharaMove()
        {
            Vector3 movePower = buttonDetector.InputStick * playerStatus.MoveSpeed;

            playerStatus.Rigidbody.velocity = movePower;

            // 体を進行方向に回転させる
            if (movePower == Vector3.zero) return;
            float moveRotationY = Mathf.Atan2(-movePower.z, movePower.x) * Mathf.Rad2Deg + 90;
            playerStatus.PlayerTransform.rotation = Quaternion.Euler(0, moveRotationY, 0);
        }
    }

    //----------------------------------------------------------------------------------
    // 弾を出現させるステート
    private class Fire : PlayerStateBase
    {
        public Fire(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus) : base(stateChanger, stateHolder, buttonDetector, playerStatus) { }

        public override void OnEnter()
        {
            playerStatus.Animator.SetTrigger("Fire");
            playerStatus.GetSkillSpawnBullet();

            // 生成
            GameObject bullet = Instantiate(playerStatus.GetSkillSelectBulletPlefab, playerStatus.SpawnBulletPoint.position, playerStatus.SpawnBulletPoint.rotation);

            playerStatus.OnBulletSpawn.Invoke(bullet.GetComponent<BulletControllerBase>());
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
    // スキルのクールダウンを計算
    private void CheckSkillCoolTime()
    {
        for (int i = 0; i < PlayerStatus.GetSkillLength; ++i)
        {
            if (PlayerStatus.GetSkillIsSelectable(i)) continue;

            PlayerStatus.GetSkillCheckCoolTimeCount(i, Time.deltaTime);
        }
    }

    //----------------------------------------------------------------------------------
    // デリゲートを設定する
    private void SetDelegate()
    {
        _buttonDetector.OnButtonBulletSelectLeftDown.RemoveListener(OnBulletSelectLeft);
        _buttonDetector.OnButtonBulletSelectLeftDown.AddListener(OnBulletSelectLeft);

        _buttonDetector.OnButtonBulletSelectRightDown.RemoveListener(OnBulletSelectRight);
        _buttonDetector.OnButtonBulletSelectRightDown.AddListener(OnBulletSelectRight);
    }

    // 右に選択
    private void OnBulletSelectLeft()
    {
        --PlayerStatus.SelectBulletType;

        if (PlayerStatus.SelectBulletType < 0)
            PlayerStatus.SelectBulletType = PlayerStatus.GetSkillLength - 1;
    }

    // 左に選択
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
        gameObject.SetActive(false);
    }
}