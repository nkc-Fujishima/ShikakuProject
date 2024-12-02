using System.Collections.Generic;
using UnityEngine;

public class IdleEnemyController : EnemyControllerBase
{

    [Header("オブジェクト設定"), SerializeField] GameObject weapon;

    // ゲーム中の経過フレーム
    ulong frameCount = 0;

    // 追跡可能な対象リスト
    List<IChaceable> chaceableObjects = new List<IChaceable>(6);
    // foreach中にリストが変更されることを防ぐための複製、基本こちらが変更され一定間隔で本リストに反映
    List<IChaceable> copyList = new List<IChaceable>(6);

    VisionMeshCreator visionMeshCreator = null;

    private void Start()
    {
    }

    public override void OnStart()
    {
        base.Start();

        VisionSensor visionSensor = transform.Find("Sensor").GetComponent<VisionSensor>();
        BoxCollider weaponCollider = weapon.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;

        visionMeshCreator = transform.Find("Sensor").GetComponent<VisionMeshCreator>();
        visionMeshCreator.SetUp();

        // 視界センサーのActionにターゲットリストの追加と削除メソッドを登録
        visionSensor.OnSensorInHundle += AddTarget;
        visionSensor.OnSensorOutHundle += RemoveTarget;

        // ステートを生成、基本ステートに待機を設定
        IdleEnemyStateManager stateHolder = new IdleEnemyStateManager(animator, this.transform, parameter as IdleEnemyParameterData, this, chaceableObjects, visionMeshCreator, weaponCollider, effect as IdleEnemyEffectData, audioSource);

        iState = stateHolder.idleState;
        if (iState != null) iState.OnEnter();

    }

    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        iState?.OnUpdate();

        // 一定フレーム経過時にリスト内のnullを削除
        frameCount += 1;
        if (frameCount % parameter.ListRefreshRate == 0)
        {
            chaceableObjects.Clear();
            chaceableObjects.AddRange(RemoveNullElements(copyList));
        }
    }

    /// <summary>
    /// リスト内のnullを削除
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    protected List<IChaceable> RemoveNullElements(List<IChaceable> list)
    {
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null)
            {
                list[count] = list[i];
                count++;
            }
        }
        list.RemoveRange(count, list.Count - count);

        return list;
    }



    #region ステート管理クラス
    private class IdleEnemyStateManager
    {
        // 各種ステートインスタンス
        public Idle idleState { get; }
        public Alert alertState { get; }
        public Attack attackState { get; }

        IdleEnemyParameterData parameter = null;

        Transform transform = null;

        List<IChaceable> chaceableObjects = null;

        IStateChangeable stateChanger = null;

        VisionMeshCreator visionMeshCreator = null;

        Animator animator = null;

        BoxCollider weaponCollider = null;

        public IChaceable chaceTarget = null;

        public IdleEnemyStateManager(Animator animator, Transform transform, IdleEnemyParameterData parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, IdleEnemyEffectData effect, AudioSource audioSource)
        {
            this.animator = animator;
            this.transform = transform;
            this.parameter = parameter;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            this.visionMeshCreator = visionMeshCreator;
            this.weaponCollider = weaponCollider;
            idleState = new Idle(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource);
            alertState = new Alert(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource);
            attackState = new Attack(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, weaponCollider, effect, audioSource);
        }

    }
    #endregion



    #region ステート基底クラス
    private abstract class IdleEnemyStateBase : StateBase
    {
        protected const int layerMask = ~(1 << 2);

        protected IdleEnemyStateManager manager = null;
        protected IdleEnemyParameterData parameter = null;
        protected Transform transform = null;
        protected Animator animator = null;
        protected IStateChangeable stateChanger = null;
        protected VisionMeshCreator visionMeshCreator = null;
        protected IdleEnemyEffectData effect = null;
        protected AudioSource audioSource = null;

        // 追跡可能な対象リスト
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public IdleEnemyStateBase(IdleEnemyStateManager manager, Animator animator, Transform transform, IdleEnemyParameterData parameter, IdleEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, IdleEnemyEffectData effect, AudioSource audioSource)
        {
            this.manager = manager;
            this.parameter = parameter;
            this.transform = transform;
            this.animator = animator;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            this.visionMeshCreator = visionMeshCreator;
            this.effect = effect;
            this.audioSource = audioSource;
        }
    }
    #endregion



    #region 待機ステート
    private class Idle : IdleEnemyStateBase
    {
        public Idle(IdleEnemyStateManager manager, Animator animator, Transform transform, IdleEnemyParameterData parameter, IdleEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, IdleEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource) { }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshNoAlertMaterial();
            animator.SetBool("WaitFlag", true);
        }

        public override void OnExit()
        {
            animator.SetBool("WaitFlag", false);
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                // IChaceableを持っているオブジェクトが存在するかレイで確認
                // その後、IChaceableとの間に遮る物が無ければ追跡対象に設定し、ステートを変更
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;


                Debug.DrawRay(toTargetRay.origin, toTargetRay.direction * toTargetHit.distance, Color.red);

                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.alertState);
        }
    }
    #endregion



    #region 警戒ステート
    private class Alert : IdleEnemyStateBase
    {
        const float effectPosY = 3;

        float distance = 0;

        public Alert(IdleEnemyStateManager manager, Animator animator, Transform transform, IdleEnemyParameterData parameter, IdleEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, IdleEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
        {
        }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();

            DetectionEffectController effectController = Instantiate(effect.DetectionEffect, new Vector3(transform.position.x, transform.position.y + effectPosY, transform.position.z), Quaternion.identity);
            effectController.Construct(this.transform);

            audioSource.clip = effect.DetectionSE;
            audioSource.Play();
        }

        public override void OnExit()
        {
            distance = 0;
        }

        public override void OnUpdate()
        {
            manager.chaceTarget = null;

            foreach (var enemy in chaceableObjects)
            {
                // ターゲットとの間にレイを繋ぐ
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) continue;

                // レイに初めてヒットしたオブジェクトが
                // 追跡対象インタフェースを持っていない場合、無視をする
                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                if (manager.chaceTarget == null) manager.chaceTarget = chaceableObject;

                distance = new Vector3(chaceableObject.chacebleTransform.position.x - transform.position.x, 0, chaceableObject.chacebleTransform.position.z - transform.position.z).magnitude;

                if (distance < new Vector3(manager.chaceTarget.chacebleTransform.position.x - transform.position.x, 0, manager.chaceTarget.chacebleTransform.position.z - transform.position.z).magnitude)
                {
                    manager.chaceTarget = chaceableObject;
                }
            }

            if (manager.chaceTarget != null)
            {
                Vector3 targetVector = new Vector3(manager.chaceTarget.chacebleTransform.position.x - transform.position.x, 0, manager.chaceTarget.chacebleTransform.position.z - transform.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(targetVector);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);

                // もしターゲットが攻撃範囲内ならば攻撃ステートへ移行
                if (distance > parameter.AttackRange) return;

                stateChanger.ChangeState(manager.attackState);

            }


            // 警戒範囲内に追跡対象のオブジェクトがない場合、待機ステートへ移行
            else stateChanger.ChangeState(manager.idleState);
        }
    }
    #endregion



    #region 攻撃ステート
    // ステート切り替え時に武器オブジェクトの当たり判定のON・OFF切り替え
    // 攻撃ステートに入り一定時間経過で待機ステートに移行
    private class Attack : IdleEnemyStateBase
    {
        float countTime = 0;

        BoxCollider weaponCollider = null;

        public Attack(IdleEnemyStateManager manager, Animator animator, Transform transform, IdleEnemyParameterData parameter, IdleEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, IdleEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
        {
            this.weaponCollider = weaponCollider;
        }


        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();

            // 追跡対象のオブジェクトの行動停止用メソッドを呼び出す
            IStoppable iStoppableObject = manager.chaceTarget.chacebleTransform.GetComponent<IStoppable>();
            iStoppableObject?.OnStop();

            manager.chaceTarget = null;

            animator.SetBool("AttackFlag", true);
        }

        public override void OnExit()
        {
            weaponCollider.enabled = false;
            countTime = 0;
            animator.SetBool("AttackFlag", false);
        }

        public override void OnUpdate()
        {
            countTime += Time.deltaTime;
            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(manager.idleState);

        }
    }
    #endregion


    // 攻撃処理、アニメーションイベントにて使用
    private void Slash()
    {
        weapon.GetComponent<BoxCollider>().enabled = true;
        audioSource.clip = (effect as IdleEnemyEffectData).slashSE;
        audioSource.Play();
    }


    // コピーリストに追跡対象を追加
    private void AddTarget(IChaceable chaceableObject)
    {
        if (copyList.Contains(chaceableObject)) return;

        copyList.Add(chaceableObject);
        chaceableObject.chacebleTransform.GetComponent<IDestroy>().OnDestroyHundle += RemoveTarget;
    }

    // コピーリストから追跡対象の要素をnullに変更
    private void RemoveTarget(IChaceable chaceableObject)
    {
        if (!copyList.Contains(chaceableObject)) return;

        copyList[copyList.IndexOf(chaceableObject)] = null;

        chaceableObject.chacebleTransform.GetComponent<IDestroy>().OnDestroyHundle -= RemoveTarget;
    }
}