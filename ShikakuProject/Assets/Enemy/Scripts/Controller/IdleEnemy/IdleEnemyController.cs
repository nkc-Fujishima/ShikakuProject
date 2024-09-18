using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleEnemyController : EnemyControllerBase
{
    [Header("オブジェクト設定"), SerializeField] BoxCollider weaponCollider;

    // ゲーム中の経過フレーム
    ulong frameCount = 0;

    // 追跡可能な対象リスト
    List<IChaceable> chaceableObjects = new List<IChaceable>(6);
    // foreach中にリストが変更されることを防ぐための複製、基本こちらが変更され一定間隔で本リストに反映
    List<IChaceable> copyList = new List<IChaceable>(6);

    VisionMeshCreator visionMeshCreator = null;

    private void Start()
    {
        base.Start();

        VisionSensor visionSensor = transform.Find("Sensor").GetComponent<VisionSensor>();
        weaponCollider.enabled = false;
        visionMeshCreator = transform.Find("Sensor").GetComponent<VisionMeshCreator>();
        visionMeshCreator.SetUp();
        visionSensor.OnSensorInHundle += AddTarget;
        visionSensor.OnSensorOutHundle += RemoveTarget;

        IdleEnemyStateHolder stateHolder = new IdleEnemyStateHolder(animator, this.transform, parameter, this, chaceableObjects, visionMeshCreator, weaponCollider);

        iState = stateHolder.idleState;
        if (iState != null) iState.OnEnter();
    }

    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        iState.OnUpdate();

        // 一定フレーム経過時にリスト内のnullを削除
        frameCount += 1;
        if (frameCount % parameter.ListRefreshRate == 0)
        {
            chaceableObjects.Clear();
            chaceableObjects.AddRange(RemoveNullElements(copyList));
        }

        Debug.Log(iState);
    }

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

    #region ステート保持クラス
    private class IdleEnemyStateHolder
    {
        public Idle idleState { get; }
        public Alert alertState { get; }
        public Attack attackState { get; }

        IdleEnemyParameter parameter = null;

        Transform transform = null;

        List<IChaceable> chaceableObjects = null;

        IStateChangeable stateChanger = null;

        VisionMeshCreator visionMeshCreator = null;

        Animator animator = null;

        BoxCollider waeponCollider = null;

        public IdleEnemyStateHolder(Animator animator, Transform transform, IdleEnemyParameter parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider)
        {
            this.animator = animator;
            this.transform = transform;
            this.parameter = parameter;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            this.visionMeshCreator = visionMeshCreator;
            this.waeponCollider = weaponCollider;
            idleState = new Idle(this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator);
            alertState = new Alert(this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator);
            attackState = new Attack(this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, weaponCollider);
        }

    }
    #endregion

    #region ステート基底クラス
    private abstract class IdleEnemyStateBase : StateBase
    {
        protected IdleEnemyParameter parameter = null;
        protected Transform transform = null;
        protected Animator animator = null;
        protected IdleEnemyStateHolder stateHolder = null;
        protected IStateChangeable stateChanger = null;
        protected VisionMeshCreator visionMeshCreator = null;
        static protected IChaceable chaceTarget = null;

        // 追跡可能な対象リスト
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public IdleEnemyStateBase(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator)
        {
            this.parameter = parameter;
            this.transform = transform;
            this.animator = animator;
            this.stateHolder = stateHollder;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            this.visionMeshCreator = visionMeshCreator;
        }
    }
    #endregion

    #region 待機ステート
    private class Idle : IdleEnemyStateBase
    {
        public Idle(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator) : base(animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator) { }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshNoAlertMaterial();
            animator.SetBool("WalkFlag", true);
        }

        public override void OnExit()
        {
            animator.SetBool("WalkFlag", false);
        }

        public override void OnUpdate()
        {
            if (chaceableObjects.Count != 0) stateChanger.ChangeState(stateHolder.alertState);
        }
    }
    #endregion

    #region 警戒ステート
    private class Alert : IdleEnemyStateBase
    {
        const int layerMask = ~(1 << 2);

        float distance = 0;
        public Alert(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator) : base(animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator)
        {
        }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;


                Debug.DrawRay(toTargetRay.origin, toTargetRay.direction * toTargetHit.distance, Color.red);

                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                distance = new Vector3(chaceableObject.chacebleTransform.position.x - transform.position.x, 0, chaceableObject.chacebleTransform.position.z - transform.position.z).magnitude;

                if (chaceTarget == null) chaceTarget = chaceableObject;

                if (distance < new Vector3(chaceTarget.chacebleTransform.position.x - transform.position.x, 0, chaceTarget.chacebleTransform.position.z - transform.position.z).magnitude)
                {
                    chaceTarget = chaceableObject;
                    Debug.Log(chaceTarget.chacebleTransform.name);
                }
            }

            if (chaceTarget != null)
            {
                Vector3 targetVector = new Vector3(chaceTarget.chacebleTransform.position.x - transform.position.x, 0, chaceTarget.chacebleTransform.position.z - transform.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(targetVector);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);
            }

            // もしターゲットが攻撃範囲内ならば攻撃ステートへ移行
            if (distance < parameter.AttackRange) stateChanger.ChangeState(stateHolder.attackState);


            // 警戒範囲内に追跡対象のオブジェクトがない場合、待機ステートへ移行
            if (chaceableObjects.Count == 0) stateChanger.ChangeState(stateHolder.idleState);

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

        public Attack(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider) : base(animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator)
        {
            this.weaponCollider = weaponCollider;
        }


        public override void OnEnter()
        {
            Debug.Log("攻撃ステート : OnEnter");
            visionMeshCreator.ChangeMeshAlertMaterial();
            weaponCollider.enabled = true;

            Debug.Log(chaceTarget);
            // 追跡対象のオブジェクトの行動停止用メソッドを呼び出す
            IStoppable iStoppableObject = chaceTarget.chacebleTransform.GetComponent<IStoppable>();
            iStoppableObject?.OnStop();
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
            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(stateHolder.idleState);

        }
    }
    #endregion

    private void AddTarget(IChaceable chaceableObject)
    {
        ; copyList.Add(chaceableObject);
    }

    private void RemoveTarget(IChaceable chaceableObject)
    {
        copyList[copyList.IndexOf(chaceableObject)] = null;
    }
}