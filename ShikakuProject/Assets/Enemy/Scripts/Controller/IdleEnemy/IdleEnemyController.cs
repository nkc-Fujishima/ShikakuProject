using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleEnemyController : EnemyControllerBase
{
    // 追跡可能な対象リスト
    protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

    private void Start()
    {
        base.Start();
        IdleEnemyStateHolder stateHolder = new IdleEnemyStateHolder(animator, this, chaceableObjects);

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
    }

    #region ステート保持クラス
    private class IdleEnemyStateHolder
    {
        public Idle idleState { get; }
        public Attack attackState { get; }

        List<IChaceable> chaceableObjects = null;

        IStateChangeable stateChanger = null;

        Animator animator = null;

        public IdleEnemyStateHolder(Animator animator, IStateChangeable stateChanger, List<IChaceable> chaceableObjects)
        {
            this.animator = animator;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            idleState = new Idle(this.animator, this, stateChanger, chaceableObjects);
            attackState = new Attack(this.animator, this, stateChanger, chaceableObjects);
        }

    }
    #endregion

    #region ステート基底クラス
    private abstract class IdleEnemyStateBase : StateBase
    {
        protected Animator animator = null;
        protected IdleEnemyStateHolder stateHolder = null;
        protected IStateChangeable stateChanger = null;

        // 追跡可能な対象リスト
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public IdleEnemyStateBase(Animator animator, IdleEnemyStateHolder stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects)
        {
            this.animator = animator;
            this.stateHolder = stateHollder;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
        }
    }
    #endregion

    private class Idle : IdleEnemyStateBase
    {
        const float limit = 30;
        float countTime = 0;

        public Idle(Animator animator, IdleEnemyStateHolder stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects) : base(animator, stateHolder, stateChanger, chaceableObjects) { }

        public override void OnEnter()
        {
            Debug.Log("待機ステート : OnEnter");
            animator.SetBool("WalkFlag", true);
        }

        public override void OnExit()
        {
            Debug.Log("待機ステート : OnExit");
            countTime = 0;
            animator.SetBool("WalkFlag", false);
        }

        public override void OnUpdate()
        {
            countTime += Time.deltaTime;

            if (countTime > limit) stateChanger.ChangeState(stateHolder.attackState);


            //foreach (var enemy in chaceableObjects) Debug.Log(enemy.chacebleTransform.name);
            Debug.Log(chaceableObjects.Count);
        }
    }

    private class Attack : IdleEnemyStateBase
    {
        const float limit = 3;
        float countTime = 0;


        public Attack(Animator animator, IdleEnemyStateHolder stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects) : base(animator, stateHolder, stateChanger, chaceableObjects) { }


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

    private void AddTarget(IChaceable target)
    {
        chaceableObjects.Add(target);
    }

    private void RemoveTarget(IChaceable target)
    {
        chaceableObjects.Remove(target);
    }
}