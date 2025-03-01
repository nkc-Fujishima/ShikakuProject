using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyController : EnemyControllerBase, IWaypointHolder
{
    [Header("オブジェクト設定"), SerializeField] GameObject weapon;

    // ゲーム中の経過フレーム
    ulong frameCount = 0;

    // 追跡可能な対象リスト
    readonly List<IChaceable> chaceableObjects = new (6);
    // foreach中にリストが変更されることを防ぐための複製、こちらが変更され一定間隔で本メインのリストに反映
    readonly List<IChaceable> copyList = new (6);

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
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.updatePosition = false;
        ChaceEnemyStateManager stateHolder = new (animator, this.transform, parameter as PatrolEnemyParameterData, this, chaceableObjects, visionMeshCreator, weaponCollider, agent, rigidBody, effect as ChaceEnemyEffectData, audioSource, Waypoints);
        iState = stateHolder.PatrolState;
        iState?.OnEnter();

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
        public Idle IdleState { get; }
        public Patrol PatrolState { get; }
        public Alert AlertState { get; }
        public Attack AttackState { get; }
        //----------------------------------------------

        readonly Animator animator = null;

        readonly List<Vector3> waypoints = null;

        public IChaceable chaceTarget = null;

        public ChaceEnemyStateManager(Animator animator, Transform transform, PatrolEnemyParameterData parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints)
        {
            this.animator = animator;
            this.waypoints = waypoints;

            IdleState = new Idle(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints);
            PatrolState = new Patrol(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource, this.waypoints);
            AlertState = new Alert(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource, waypoints);
            AttackState = new Attack(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, weaponCollider, effect, audioSource, waypoints);
        }

    }
    #endregion



    #region ステート基底クラス
    private abstract class ChaceEnemyStateBase : StateBase
    {
        protected const int layerMask = ~(1 << 2);

        protected ChaceEnemyStateManager manager = null;
        protected PatrolEnemyParameterData parameter = null;
        protected Transform transform = null;
        protected Animator animator = null;
        protected IStateChangeable stateChanger = null;
        protected VisionMeshCreator visionMeshCreator = null;
        protected ChaceEnemyEffectData effect = null;
        protected AudioSource audioSource = null;

        protected List<Vector3> waypoints = null;

        // 追跡可能な対象リスト
        protected List<IChaceable> chaceableObjects = new (6);

        public ChaceEnemyStateBase(ChaceEnemyStateManager manager, Animator animator, Transform transform, PatrolEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints)
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
            this.waypoints = waypoints;
        }
    }
    #endregion



    #region 待機ステート
    private class Idle : ChaceEnemyStateBase
    {
        // ステートに入ってから経った秒数
        float passedSeconds = 0;

        public Idle(ChaceEnemyStateManager manager, Animator animator, Transform transform, PatrolEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints) { }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshNoAlertMaterial();
            animator.SetBool("WaitFlag", true);
        }

        public override void OnExit()
        {
            passedSeconds = 0;
            animator.SetBool("WaitFlag", false);
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new (new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.AlertState);

            if (passedSeconds >= parameter.IdleTime)
            {
                if (waypoints.Count == 0) return;
                    stateChanger.ChangeState(manager.PatrolState);
            }
            else
                passedSeconds += Time.deltaTime;

        }
    }
    #endregion



    #region 巡回ステート
    private class Patrol : ChaceEnemyStateBase
    {
        // 目標の巡回ポイントの到着判定距離
        readonly float arrivalLangth = 0.7f;

        int patrolStep = -1;

        readonly Rigidbody rigidBody = null;
        readonly NavMeshAgent agent = null;

        public Patrol(ChaceEnemyStateManager manager, Animator animator, Transform transform, PatrolEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints)
        {
            this.agent = agent;
            this.rigidBody = rigidBody;
        }


        public override void OnEnter()
        {
            if (waypoints.Count != 0)
            {
                // 現時点で一番近いポイントへ
                float selectDistance = 0;
                for (int i = 0; i < waypoints.Count; ++i)
                {
                    if (patrolStep == -1)
                    {
                        selectDistance = (transform.position - waypoints[i]).magnitude;
                        patrolStep = i;
                        continue;
                    }
                    else
                    {
                        // 調べているポイントが選定しているポイントと比べて遠い場合は終了
                        float checkDistance = (transform.position - waypoints[i]).magnitude;
                        if (selectDistance < checkDistance) continue;

                        // 近い場合は更新
                        selectDistance = checkDistance;
                        patrolStep = i;
                    }
                }

                agent.SetDestination(waypoints[patrolStep]);
            }

            visionMeshCreator.ChangeMeshNoAlertMaterial();
            animator.SetBool("WalkFlag", true);

        }

        public override void OnExit()
        {
            patrolStep = -1;
            animator.SetBool("WalkFlag", false);
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new (new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            PatrolMovement();
            ChangePatrolPoint();

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.AlertState);

            if (waypoints.Count == 0) stateChanger.ChangeState(manager.IdleState);
        }

        // 巡回する動き
        private void PatrolMovement()
        {
            if (waypoints.Count == 0) return;

            agent.nextPosition = transform.position;

            NavMeshPath path = agent.path;


            // NavMeashのパスを用いて移動方向を変更
            if (path.corners.Length > 1)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed_Patrol * Time.deltaTime);
            }

            rigidBody.velocity = transform.forward * parameter.MoveSpeed_Patrol;
        }

        // パトロールポイントを変える
        private void ChangePatrolPoint()
        {
            if (waypoints.Count == 0) return;

            // 到着ポイントとの距離を調べる
            float distance = (transform.position - waypoints[patrolStep]).magnitude;

            // 到着判定距離より遠かったら終了
            if (arrivalLangth < distance) return;


            // 到着をしていたら次の巡回ポイントへ
            ++patrolStep;
            if (patrolStep >= waypoints.Count) patrolStep = 0;
            agent.SetDestination(waypoints[patrolStep]);
        }
    }
    #endregion



    #region 警戒ステート
    private class Alert : ChaceEnemyStateBase
    {
        const float effectPosY = 3;

        float distance = 0;

        readonly Rigidbody rigidBody = null;
        readonly NavMeshAgent agent = null;
        public Alert(ChaceEnemyStateManager manager, Animator animator, Transform transform, PatrolEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints)
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

            animator.SetBool("RunFlag", true);
        }

        public override void OnExit()
        {
            distance = 0;
            agent.ResetPath();
            animator.SetBool("RunFlag", false);
        }

        public override void OnUpdate()
        {
            manager.chaceTarget = null;

            foreach (var enemy in chaceableObjects)
            {
                // ターゲットとの間にレイを繋ぐ
                Ray toTargetRay = new(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) continue;

                // レイに初めてヒットしたオブジェクトが
                // 追跡対象インタフェースを持ってい場合、無視をする
                IChaceable chaceableObject;
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
                agent.nextPosition = transform.position;

                agent.SetDestination(manager.chaceTarget.chacebleTransform.position);
                NavMeshPath path = agent.path;


                // NavMeashのパスを用いて移動方向を変更
                if (path.corners.Length > 1)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);
                }

                rigidBody.velocity = transform.forward * parameter.MoveSpeed;

                // もしターゲットが攻撃範囲内ならば攻撃ステートへ移行
                if (distance > parameter.AttackRange) return;

                stateChanger.ChangeState(manager.AttackState);
            }


            // 警戒範囲内に追跡対象のオブジェクトがない場合、待機ステートへ移行
            else stateChanger.ChangeState(manager.IdleState);
        }
    }
    #endregion



    #region 攻撃ステート
    // ステート切り替え時に武器オブジェクトの当たり判定のON・OFF切り替え
    // 攻撃ステートに入り一定時間経過で待機ステートに移行
    private class Attack : ChaceEnemyStateBase
    {
        float countTime = 0;

        readonly BoxCollider weaponCollider = null;

        public Attack(ChaceEnemyStateManager manager, Animator animator, Transform transform, PatrolEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints)
        {
            this.weaponCollider = weaponCollider;
        }


        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();

            // 追跡対象のオブジェクトの行動停止用メソッドを呼び出す
            IStoppable iStoppableObject;
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
            countTime += Time.deltaTime;
            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(manager.PatrolState);

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


    //-----------------------------------------------------------
    //　IWaypointHolder
    public List<Vector3> Waypoints { get; private set; } = new();

    public void SetWaypoints(List<Vector3> points)
    {
        Waypoints = new List<Vector3>(points);
    }
}