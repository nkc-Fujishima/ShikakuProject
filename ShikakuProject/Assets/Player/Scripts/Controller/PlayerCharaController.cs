using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]

public class PlayerCharaController : MonoBehaviour, IChaceable, IDamage, IStateChangeable, IDestroy
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
        public UnityEvent OnBulletSpawn = new();

        public int SelectType = 0;


        public Rigidbody Rigidbody => _rigidbody;

        public Transform PlayerTransform => _playerTransform;

        public Transform SpawnBulletPoint => _spawnBulletPoint;

        public Animator Animator => _animator;
    }


    [SerializeField]
    private PlayerStatusParameter _playerStatus;

    private IState _iState = null;

    private PlayerButtonDetector _buttonDetector;

    private bool _isMove = false;


    public PlayerData Datas;

    public event Action<IChaceable> OnDestroyHundle;

    public BulletControllerBase[] GetBulletPrefabs => _playerStatus.GetAllBulletPlefab();

    public Sprite[] GetBulletTexture => _playerStatus.GetAllBulletTexture();

    public ReactiveProperty<int> SelectBulletType = new (0);

    public ReactiveProperty<float>[] CountTimeRates;


    // フジシマ追加2024/10/03------------------------------------------
    [Tooltip("カーソルオブジェクト"),SerializeField]CursorController cursor;

    private void Awake()
    {
        Datas.OnStart(transform);

        _playerStatus.OnStart();

        PlayerInput playerInput = GetComponent<PlayerInput>();
        _buttonDetector = new(playerInput);

        SetDelegate();


        PlayerStateHolder stateHolder = new(this, _buttonDetector, _playerStatus, Datas, SelectBulletType);
        _iState = stateHolder.IdleState;


        CountTimeRates = new ReactiveProperty<float>[_playerStatus.GetSkillLength];
        for (int i = 0; i < CountTimeRates.Length; ++i)
        {
            CountTimeRates[i] = new(1);
        }

        CursorController cursorController = Instantiate(cursor);
        cursorController.Construct(transform);
    }

    private void Update()
    {
        if (!_isMove) return;

        _iState.OnUpdate();

        CheckSkillCoolTime();
    }


    //----------------------------------------------------------------------------------
    // ステートを指定された状態に変化させる
    public void ChangeState(IState nextState)
    {
        _iState?.OnExit();

        _iState = nextState;

        _iState?.OnEnter();
    }

    //----------------------------------------------------------------------------------
    // ステート保持
    private class PlayerStateHolder
    {
        public Idle IdleState { get; }
        public Walk WalkState { get; }
        public Fire FireState { get; }
        public Avoid AvoidState { get; }
        public Dead DeadState { get; }

        public PlayerStateHolder(IStateChangeable stateChanger, PlayerButtonDetector button, PlayerStatusParameter playerStatus, PlayerData datas, ReactiveProperty<int> selectBulletType)
        {
            IdleState = new Idle(stateChanger, this, button, playerStatus, datas, selectBulletType);
            WalkState = new Walk(stateChanger, this, button, playerStatus, datas, selectBulletType);
            FireState = new Fire(stateChanger, this, button, playerStatus, datas, selectBulletType);
            AvoidState = new Avoid(stateChanger, this, button, playerStatus, datas, selectBulletType);
            DeadState = new Dead(stateChanger, this, button, playerStatus, datas, selectBulletType);
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
        protected PlayerData data;
        protected ReactiveProperty<int> selectBulletType = null;

        public PlayerStateBase(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data, ReactiveProperty<int> selectBulletType)
        {
            this.stateChanger = stateChanger;
            this.stateHolder = stateHolder;
            this.buttonDetector = buttonDetector;
            this.playerStatus = playerStatus;
            this.data = data;
            this.selectBulletType = selectBulletType;

            data.OnDeath.AddListener(this.OnDeath);
        }

        // 弾を生成するボタンを押した場合の関数
        public void OnFire()
        {
            if (!playerStatus.GetSkillIsSelectable(selectBulletType.Value)) return;

            stateChanger.ChangeState(stateHolder.FireState);

            // 生成
            playerStatus.GetSkillSpawnBullet(selectBulletType.Value);
            data.OnBulletSpawn.Invoke();
        }

        // 回避ボタンを押した場合の関数
        public void OnAvoid()
        {
            data.Animator.SetBool("Avoid", true);

            stateChanger.ChangeState(stateHolder.AvoidState);

            // 向いてる方向に回避
            data.Rigidbody.AddForce(data.PlayerTransform.forward * 2800);
        }

        // 死んだときの関数
        private void OnDeath()
        {
            stateChanger.ChangeState(stateHolder.DeadState);
        }
    }

    //----------------------------------------------------------------------------------
    // 止まるステート
    private class Idle : PlayerStateBase
    {
        public Idle(IStateChangeable stateChanger,PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector ,PlayerStatusParameter playerStatus, PlayerData data, ReactiveProperty<int> selectBulletType) : base(stateChanger, stateHolder,buttonDetector,playerStatus, data, selectBulletType) { }

        public override void OnEnter()
        {
            data.Animator.SetBool("Walk", false);

            buttonDetector.OnButtonFireDown.AddListener(OnFire);
            buttonDetector.OnButtonAvoidDown.AddListener(OnAvoid);
        }

        public override void OnExit()
        {
            buttonDetector.OnButtonFireDown.RemoveListener(OnFire);
            buttonDetector.OnButtonAvoidDown.RemoveListener(OnAvoid);
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
        public Walk(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data, ReactiveProperty<int> selectBulletType) : base(stateChanger, stateHolder, buttonDetector, playerStatus,data, selectBulletType) { }

        public override void OnEnter()
        {
            data.Animator.SetBool("Walk", true);

            buttonDetector.OnButtonFireDown.AddListener(OnFire);
            buttonDetector.OnButtonAvoidDown.AddListener(OnAvoid);
        }

        public override void OnExit()
        {
            buttonDetector.OnButtonFireDown.RemoveListener(OnFire);
            buttonDetector.OnButtonAvoidDown.RemoveListener(OnAvoid);
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
            Vector3 movePower = buttonDetector.InputStick.normalized * playerStatus.MoveSpeed;

            data.Rigidbody.velocity = new Vector3(movePower.x, data.Rigidbody.velocity.y, movePower.z);

            // 体を進行方向に回転させる
            if (movePower == Vector3.zero) return;
            float moveRotationY = Mathf.Atan2(-movePower.z, movePower.x) * Mathf.Rad2Deg + 90;
            data.PlayerTransform.rotation = Quaternion.Euler(0, moveRotationY, 0);
        }
    }

    //----------------------------------------------------------------------------------
    // 弾を出現させるステート
    private class Fire : PlayerStateBase
    {
        public Fire(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data, ReactiveProperty<int> selectBulletType) : base(stateChanger, stateHolder, buttonDetector, playerStatus, data, selectBulletType) { }

        public override void OnEnter()
        {

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
    // 回避するステート
    private class Avoid : PlayerStateBase
    {
        public Avoid(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data, ReactiveProperty<int> selectBulletType) : base(stateChanger, stateHolder, buttonDetector, playerStatus, data, selectBulletType) { }

        private const float STOP_TIME = 1.4f;

        private float countTime = STOP_TIME;

        private CancellationTokenSource cts = null;

        private bool isMoved = false;

        public override void OnEnter()
        {
            SwitchStateAfterDelay();
        }

        public override void OnExit()
        {
            cts.Cancel();
        }

        public override void OnUpdate()
        {
        }

        private async void SwitchStateAfterDelay()
        {
            if (!isMoved) isMoved = true;

            cts = new CancellationTokenSource();
            var startTime = Time.time;

            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(countTime), cancellationToken: cts.Token);

                // 指定された秒数待った
                countTime = STOP_TIME;
                isMoved = false;
                data.Animator.SetBool("Avoid", false);
                stateChanger.ChangeState(stateHolder.IdleState);
            }
            catch (OperationCanceledException)
            {
                // キャンセルされた場合の処理
                if (isMoved)
                {
                    // 残り時間を計算
                    countTime -= Time.time - startTime;

                    return;
                }
            }
        }
    }

    //----------------------------------------------------------------------------------
    // 死亡ステート
    private class Dead : PlayerStateBase
    {
        public Dead(IStateChangeable stateChanger, PlayerStateHolder stateHolder, PlayerButtonDetector buttonDetector, PlayerStatusParameter playerStatus, PlayerData data, ReactiveProperty<int> selectBulletType) : base(stateChanger, stateHolder, buttonDetector, playerStatus, data, selectBulletType) { }

        public override void OnEnter()
        {
            data.Animator.SetTrigger("DeathTrigger");
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
        }
    }


    //----------------------------------------------------------------------------------
    // スキルのクールダウンを計算
    private void CheckSkillCoolTime()
    {
        for (int i = 0; i < _playerStatus.GetSkillLength; ++i)
        {
            if (_playerStatus.GetSkillIsSelectable(i)) continue;


            _playerStatus.GetSkillCheckCoolTimeCount(i, Time.deltaTime);


            float cooldownRate = _playerStatus.GetSkillCoolTimeCount(i) / _playerStatus.GetSkillCoolTime(i);
            CountTimeRates[i].Value = cooldownRate;
        }
    }

    //----------------------------------------------------------------------------------
    // 選択ボタンの設定する
    private void SetDelegate()
    {
        //_buttonDetector.OnButtonBulletSelectLeftDown.RemoveListener(OnBulletSelectLeft);
        //_buttonDetector.OnButtonBulletSelectLeftDown.AddListener(OnBulletSelectLeft);

        //_buttonDetector.OnButtonBulletSelectRightDown.RemoveListener(OnBulletSelectRight);
        //_buttonDetector.OnButtonBulletSelectRightDown.AddListener(OnBulletSelectRight);
    }

    // 右に選択
    private void OnBulletSelectLeft()
    {
        if (!_isMove) return;

        int nextType = SelectBulletType.Value - 1;

        if (nextType < 0)
            nextType = _playerStatus.GetSkillLength - 1;

        SelectBulletType.Value = nextType;
    }

    // 左に選択
    private void OnBulletSelectRight()
    {
        if (!_isMove) return;

        int nextType = SelectBulletType.Value + 1;

        if (nextType >= _playerStatus.GetSkillLength)
            nextType = 0;

        SelectBulletType.Value = nextType;
    }


    //----------------------------------------------------------------------------------
    // スタート関数
    public void ActivateMovement()
    {
        _isMove = true;

        _iState.OnEnter();
    }

    //----------------------------------------------------------------------------------
    // 停止関数
    public void DisableMovement()
    {
        _isMove = false;

        _iState.OnExit();
    }


    //----------------------------------------------------------------------------------
    // IChaceable
    public Transform chacebleTransform { get { return transform; } }

    //----------------------------------------------------------------------------------
    // IDamage
    public void Damage()
    {
        _buttonDetector.OnButtonBulletSelectLeftDown.RemoveListener(OnBulletSelectLeft);
        _buttonDetector.OnButtonBulletSelectRightDown.RemoveListener(OnBulletSelectRight);

        _isMove = false;


        Datas.OnDeath?.Invoke();

        OnDestroyHundle?.Invoke(this);
    }
}