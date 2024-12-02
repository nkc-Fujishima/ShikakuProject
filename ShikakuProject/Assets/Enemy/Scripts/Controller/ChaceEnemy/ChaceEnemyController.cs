using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaceEnemyController : EnemyControllerBase
{
    [Header("オブジェクト設定"), SerializeField] GameObject weapon;

    // ゲーム中の経過フレーム
    ulong frameCount = 0;

    // 追跡可能な対象リスト
    List<IChaceable> chaceableObjects = new List<IChaceable>(6);
    // foreach中にリストが変更されることを防ぐための複製、こちらが変更され一定間隔で本メインのリストに反映
    List<IChaceable> copyList = new List<IChaceable>(6);

    VisionMeshCreator visionMeshCreator = null;


    public override void OnStart()
    {
        base.OnStart();
        base.Start();

        VisionSensor visionSensor = transform.Find("Sensor").GetComponent<VisionSensor>();
        BoxCollider weaponCollider = weapon.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;

        visionMeshCreator = transform.Find("Sensor").GetComponent<VisionMeshCreator>();
        visionMeshCreator.SetUp();

        // 視界センサーのActionにターゲットリストの追加と削除メソッドを登録
        visionSensor.OnSensorInHundle += AddTarget;
        visionSensor.OnSensorOutHundle += RemoveTarget;

        // RigidBodyの挙動とNavMeshの位置更新が競合するため、NavMeshの更新を止める
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.updatePosition = false;

        // ステートを生成、基本ステートに待機を設定
        ChaceEnemyStateManager stateHolder = new ChaceEnemyStateManager(animator, this.transform, parameter as ChaceEnemyParameterData, this,  chaceableObjects, visionMeshCreator, weaponCollider, agent, rigidBody, effect as ChaceEnemyEffectData, audioSource);

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



    // ---------------------------------------------------
    // リスト変更メソッド群
    #region リスト変更メソッド群
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
    #endregion


    // -------------------------------------------------------------
    // ここから各種ステート関連クラス

    #region ステート管理クラス
    private class ChaceEnemyStateManager
    {
        // 各種ステートインスタンス---------------------
        public Idle idleState { get; }
        public Alert alertState { get; }
        public Attack attackState { get; }
        //----------------------------------------------

        ChaceEnemyParameterData parameter = null;

        Transform transform = null;

        List<IChaceable> chaceableObjects = null;

        IStateChangeable stateChanger = null;

        VisionMeshCreator visionMeshCreator = null;

        Animator animator = null;

        Rigidbody rigidBody = null;

        BoxCollider weaponCollider = null;

        NavMeshAgent agent = null;

        public IChaceable chaceTarget = null;

        public ChaceEnemyStateManager(Animator animator, Transform transform, ChaceEnemyParameterData parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource)
        {
            this.animator = animator;
            this.transform = transform;
            this.parameter = parameter;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            this.visionMeshCreator = visionMeshCreator;
            this.weaponCollider = weaponCollider;
            this.agent = agent;
            this.rigidBody = rigidBody;
            idleState = new Idle(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource);
            alertState = new Alert(this, this.animator, transform, parameter, this, stateChanger,  chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource);
            attackState = new Attack(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, weaponCollider, effect, audioSource);
        }

    }
    #endregion



    #region ステート基底クラス
    private abstract class ChaceEnemyStateBase : StateBase
    {
        protected const int layerMask = ~(1 << 2);

        protected ChaceEnemyStateManager manager = null;
        protected ChaceEnemyParameterData parameter = null;
        protected Transform transform = null;
        protected Animator animator = null;
        protected IStateChangeable stateChanger = null;
        protected VisionMeshCreator visionMeshCreator = null;
        protected ChaceEnemyEffectData effect = null;
        protected AudioSource audioSource = null;

        // 追跡可能な対象リスト
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public ChaceEnemyStateBase(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource)
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
    private class Idle : ChaceEnemyStateBase
    {
        public Idle(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource) { }

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
            // IChaceableを持っているオブジェクトが存在するかレイで確認
            // その後、IChaceableとの間に遮る物が無ければ追跡対象に設定し、ステートを変更
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.alertState);
        }
    }
    #endregion



    #region 警戒ステート
    private class Alert : ChaceEnemyStateBase
    {
        const float effectPosY = 3;

        float distance = 0;

        Rigidbody rigidBody = null;
        NavMeshAgent agent = null;
        public Alert(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger,  List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
        {
            this.agent = agent;
            this.rigidBody = rigidBody;
        }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();

            DetectionEffectController effectController = Instantiate(effect.DetectionEffect, new Vector3(transform.position.x, transform.position.y + effectPosY, transform.position.z), Quaternion.identity);
            effectController.Construct(this.transform);
            audioSource.clip = effect.DetectionSE;
            audioSource.Play();

            animator.SetBool("WalkFlag", true);
        }

        public override void OnExit()
        {
            distance = 0;
            agent.ResetPath();
            animator.SetBool("WalkFlag", false);
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

            // 追跡対象が存在する場合、NavMeshの経路を利用し、攻撃対象へ近づく
            if (manager.chaceTarget != null)
            {
                agent.nextPosition = transform.position;

                agent.SetDestination(manager.chaceTarget.chacebleTransform.position);
                NavMeshPath path = agent.path;


                // NavMeashのパスを用いて移動方向を変更
                if (path.corners.Length > 1)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);
                }

                rigidBody.velocity = transform.forward * parameter.MoveSpeed ;

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
    private class Attack : ChaceEnemyStateBase
    {
        float countTime = 0;

        BoxCollider weaponCollider = null;

        public Attack(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, ChaceEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
        {
            this.weaponCollider = weaponCollider;
        }


        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();

            // 追跡対象のオブジェクトの行動停止用メソッドを呼び出す
            IStoppable iStoppableObject = null;
            if (manager.chaceTarget.chacebleTransform.TryGetComponent<IStoppable>(out iStoppableObject))
            {
                iStoppableObject?.OnStop();
            }

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
            // このステートに入ってからの経過時間がパラメータの攻撃クールタイムより上になったら
            // 攻撃ステート解除
            countTime += Time.deltaTime;
            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(manager.idleState);
        }
    }
    #endregion



    //-----------------------------------------------------------
    //　攻撃処理、アニメーションイベントにて使用
    private void Slash()
    {
        weapon.GetComponent<BoxCollider>().enabled = true;
        audioSource.clip = (effect as ChaceEnemyEffectData).slashSE;
        audioSource.Play();
    }
}
