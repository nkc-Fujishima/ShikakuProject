using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemyController : EnemyControllerBase
{

    // ゲーム中の経過フレーム
    ulong frameCount = 0;

    // 追跡可能な対象リスト
    List<IChaceable> chaceableObjects = new List<IChaceable>(6);
    // foreach中にリストが変更されることを防ぐための複製、基本こちらが変更され一定間隔で本リストに反映
    List<IChaceable> copyList = new List<IChaceable>(6);

    VisionMeshCreator visionMeshCreator = null;

    ShootEnemyStateManager stateManager = null;

    IChaceable target = null;

    private void Start()
    {
        base.Start();

        VisionSensor visionSensor = transform.Find("Sensor").GetComponent<VisionSensor>();

        visionMeshCreator = transform.Find("Sensor").GetComponent<VisionMeshCreator>();
        visionMeshCreator.SetUp();

        // 視界センサーのActionにターゲットリストの追加と削除メソッドを登録
        visionSensor.OnSensorInHundle += AddTarget;
        visionSensor.OnSensorOutHundle += RemoveTarget;

        stateManager = new ShootEnemyStateManager(animator, this.transform, parameter as ShootEnemyParameter, this, chaceableObjects, visionMeshCreator);

        iState = stateManager.idleState;
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



    #region ステート管理クラス
    private class ShootEnemyStateManager
    {
        // 各種ステートインスタンス
        public Idle idleState { get; }
        public Alert alertState { get; }
        public Attack attackState { get; }


        public IChaceable chaceTarget = null;

        public ShootEnemyStateManager(Animator animator, Transform transform, ShootEnemyParameter parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator)
        {
            idleState = new Idle(this, animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator);
            alertState = new Alert(this, animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator);
            attackState = new Attack(this, animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator);
        }

    }
    #endregion



    #region ステート基底クラス
    private abstract class ShootEnemyStateBase : StateBase
    {
        protected const int layerMask = ~(1 << 2);

        protected ShootEnemyStateManager manager = null;
        protected ShootEnemyParameter parameter = null;
        protected Transform transform = null;
        protected Animator animator = null;
        protected IStateChangeable stateChanger = null;
        protected VisionMeshCreator visionMeshCreator = null;

        // 追跡可能な対象リスト
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public ShootEnemyStateBase(ShootEnemyStateManager manager, Animator animator, Transform transform, ShootEnemyParameter parameter, ShootEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator)
        {
            this.manager = manager;
            this.parameter = parameter;
            this.transform = transform;
            this.animator = animator;
            this.stateChanger = stateChanger;
            this.chaceableObjects = chaceableObjects;
            this.visionMeshCreator = visionMeshCreator;
        }
    }
    #endregion



    #region 待機ステート
    private class Idle : ShootEnemyStateBase
    {
        public Idle(ShootEnemyStateManager manager, Animator animator, Transform transform, ShootEnemyParameter parameter, ShootEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator) { }

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
    private class Alert : ShootEnemyStateBase
    {
        const int layerMask = ~(1 << 2);

        float distance = 0;

        public Alert(ShootEnemyStateManager manager, Animator animator, Transform transform, ShootEnemyParameter parameter, ShootEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator)
        {
        }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();
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
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) continue;


                Debug.DrawRay(toTargetRay.origin, toTargetRay.direction * toTargetHit.distance, Color.red);

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
    private class Attack : ShootEnemyStateBase
    {
        float countTime = 0;

        public Attack(ShootEnemyStateManager manager, Animator animator, Transform transform, ShootEnemyParameter parameter, ShootEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator)
        {
        }


        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();

            animator.SetBool("AttackFlag", true);
        }

        public override void OnExit()
        {
            countTime = 0;
            manager.chaceTarget = null;
            animator.SetBool("AttackFlag", false);
        }

        public override void OnUpdate()
        {
            countTime += Time.deltaTime;

            if (chaceableObjects.Count != 0)
            {
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(manager.chaceTarget.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, manager.chaceTarget.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask);
                if (!toTargetHit.transform.TryGetComponent(out IChaceable chaceableObject)) return;


                Vector3 targetVector = new Vector3(manager.chaceTarget.chacebleTransform.position.x - transform.position.x, 0, manager.chaceTarget.chacebleTransform.position.z - transform.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(targetVector);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);
            }

            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(manager.idleState);

        }
    }
    #endregion




    // 攻撃処理
    private void Shoot()
    {
        target = stateManager.chaceTarget;
        Debug.Log(target);
    }





    // コピーリストに追跡対象を追加
    private void AddTarget(IChaceable chaceableObject)
    {
        if (copyList.Contains(chaceableObject)) return;

        copyList.Add(chaceableObject);
        chaceableObject.chacebleTransform.GetComponent<IDestroy>().OnDestroyHundle += HundleTargetDestroy;
    }

    // コピーリストから追跡対象の要素をnullに変更
    private void RemoveTarget(IChaceable chaceableObject)
    {
        if (!copyList.Contains(chaceableObject)) return;

        copyList[copyList.IndexOf(chaceableObject)] = null;

        chaceableObject.chacebleTransform.GetComponent<IDestroy>().OnDestroyHundle -= HundleTargetDestroy;
    }

    private void HundleTargetDestroy(IChaceable chaceableObject)
    {
        RemoveTarget(chaceableObject);
    }

}
