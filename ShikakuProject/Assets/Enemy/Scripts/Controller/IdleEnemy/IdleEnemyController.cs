using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleEnemyController : EnemyControllerBase
{
    [Header("オブジェクト設定"), SerializeField] IdleEnemyParameter parameter;

    // ゲーム中の経過フレーム
    ulong frameCount = 0;

    // 追跡可能な対象リスト
    List<IChaceable> chaceableObjects = new List<IChaceable>(6);
    // foreach中にリストが変更されることを防ぐための複製、基本こちらが変更され一定間隔で本リストに反映
    List<IChaceable> copyList = new List<IChaceable>(6);


    private void Start()
    {
        base.Start();
        IdleEnemyStateHolder stateHolder = new IdleEnemyStateHolder(animator, this.transform, parameter, this, chaceableObjects);

        VisionSensor visionSensor = transform.Find("Sensor").GetComponent<VisionSensor>();
        visionSensor.OnSensorInHundle += AddTarget;
        visionSensor.OnSensorOutHundle += RemoveTarget;

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
        public Alart alartState { get; }
        public Attack attackState { get; }

        IdleEnemyParameter parameter = null;

        Transform transform = null;

        List<IChaceable> chaceableObjects = null;

        IStateChangeable stateChanger = null;

        Animator animator = null;

        public IdleEnemyStateHolder(Animator animator, Transform transform, IdleEnemyParameter parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects)
        {
            this.animator = animator;
            this.transform = transform;
            this.parameter = parameter;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            idleState = new Idle(this.animator, transform, parameter, this, stateChanger, chaceableObjects);
            alartState = new Alart(this.animator, transform, parameter, this, stateChanger, chaceableObjects);
            attackState = new Attack(this.animator, transform, parameter, this, stateChanger, chaceableObjects);
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
        protected Transform target = null;

        // 追跡可能な対象リスト
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public IdleEnemyStateBase(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects)
        {
            this.parameter = parameter;
            this.transform = transform;
            this.animator = animator;
            this.stateHolder = stateHollder;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
        }
    }
    #endregion

    #region 待機ステート
    private class Idle : IdleEnemyStateBase
    {
        public Idle(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects) : base(animator, transform, parameter, stateHolder, stateChanger, chaceableObjects) { }

        public override void OnEnter()
        {
            Debug.Log("待機ステート : OnEnter");
            animator.SetBool("WalkFlag", true);
        }

        public override void OnExit()
        {
            Debug.Log("待機ステート : OnExit");
            animator.SetBool("WalkFlag", false);
        }

        public override void OnUpdate()
        {
            Debug.Log(chaceableObjects.Count);
            if (chaceableObjects.Count != 0) stateChanger.ChangeState(stateHolder.alartState);
        }
    }
    #endregion

    private class Alart : IdleEnemyStateBase
    {
        public Alart(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects) : base(animator, transform, parameter, stateHollder, stateChanger, chaceableObjects)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("警戒ステート : OnEnter");
        }

        public override void OnExit()
        {
            Debug.Log("警戒ステート : OnExit");
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new Ray(transform.position, enemy.chacebleTransform.position - transform.position);
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity)) return;

                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                float distance = new Vector3(chaceableObject.chacebleTransform.position.x - transform.position.x, 0, chaceableObject.chacebleTransform.position.z - transform.position.z).magnitude;
                if (distance < parameter.AttackRange) stateChanger.ChangeState(stateHolder.attackState);
            }

        }
    }

    #region
    private class Attack : IdleEnemyStateBase
    {
        const float limit = 3;
        float countTime = 0;


        public Attack(Animator animator, Transform transform, IdleEnemyParameter parameter, IdleEnemyStateHolder stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects) : base(animator, transform, parameter, stateHolder, stateChanger, chaceableObjects) { }


        public override void OnEnter()
        {
            Debug.Log("攻撃ステート : OnEnter");
            animator.SetBool("AttackFlag", true);
        }

        public override void OnExit()
        {
            animator.SetBool("AttackFlag", false);
        }

        public override void OnUpdate()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99) stateChanger.ChangeState(stateHolder.idleState);
        }
    }
    #endregion

    private void AddTarget(IChaceable chaceableObject)
    {
        Debug.Log("警戒範囲内の入りました");
        ; copyList.Add(chaceableObject);
    }

    private void RemoveTarget(IChaceable chaceableObject)
    {
        copyList[copyList.IndexOf(chaceableObject)] = null;
    }


}