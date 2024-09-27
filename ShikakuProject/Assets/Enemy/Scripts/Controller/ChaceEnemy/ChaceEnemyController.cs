using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaceEnemyController : EnemyControllerBase
{
    [Header("�I�u�W�F�N�g�ݒ�"), SerializeField] GameObject weapon;

    // �Q�[�����̌o�߃t���[��
    ulong frameCount = 0;

    // �ǐՉ\�ȑΏۃ��X�g
    List<IChaceable> chaceableObjects = new List<IChaceable>(6);
    // foreach���Ƀ��X�g���ύX����邱�Ƃ�h�����߂̕����A��{�����炪�ύX������Ԋu�Ŗ{���X�g�ɔ��f
    List<IChaceable> copyList = new List<IChaceable>(6);

    VisionMeshCreator visionMeshCreator = null;

    private void Start()
    {
        //base.Start();

        //VisionSensor visionSensor = transform.Find("Sensor").GetComponent<VisionSensor>();
        //BoxCollider weaponCollider = weapon.GetComponent<BoxCollider>();
        //weaponCollider.enabled = false;

        //visionMeshCreator = transform.Find("Sensor").GetComponent<VisionMeshCreator>();
        //visionMeshCreator.SetUp();

        //// ���E�Z���T�[��Action�Ƀ^�[�Q�b�g���X�g�̒ǉ��ƍ폜���\�b�h��o�^
        //visionSensor.OnSensorInHundle += AddTarget;
        //visionSensor.OnSensorOutHundle += RemoveTarget;

        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //Rigidbody rigidBody = GetComponent<Rigidbody>();

        //ChaceEnemyStateManager stateHolder = new ChaceEnemyStateManager(animator, this.transform, parameter as ChaceEnemyParameter, this, chaceableObjects, visionMeshCreator, weaponCollider, agent, rigidBody);

        //iState = stateHolder.idleState;
        //if (iState != null) iState.OnEnter();
    }

    public override void OnStart()
    {
        base.OnStart();
        base.Start();

        VisionSensor visionSensor = transform.Find("Sensor").GetComponent<VisionSensor>();
        BoxCollider weaponCollider = weapon.GetComponent<BoxCollider>();
        weaponCollider.enabled = false;

        visionMeshCreator = transform.Find("Sensor").GetComponent<VisionMeshCreator>();
        visionMeshCreator.SetUp();

        // ���E�Z���T�[��Action�Ƀ^�[�Q�b�g���X�g�̒ǉ��ƍ폜���\�b�h��o�^
        visionSensor.OnSensorInHundle += AddTarget;
        visionSensor.OnSensorOutHundle += RemoveTarget;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        Rigidbody rigidBody = GetComponent<Rigidbody>();

        ChaceEnemyStateManager stateHolder = new ChaceEnemyStateManager(animator, this.transform, parameter as ChaceEnemyParameter, this, chaceableObjects, visionMeshCreator, weaponCollider, agent, rigidBody);

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

        // ���t���[���o�ߎ��Ƀ��X�g����null���폜
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


    // ---------------------------------------------------
    // ���X�g�ύX���\�b�h�Q
    #region ���X�g�ύX���\�b�h�Q
    // �R�s�[���X�g�ɒǐՑΏۂ�ǉ�
    private void AddTarget(IChaceable chaceableObject)
    {
        if (copyList.Contains(chaceableObject)) return;

        copyList.Add(chaceableObject);
        chaceableObject.chacebleTransform.GetComponent<IDestroy>().OnDestroyHundle += HundleTargetDestroy;
    }

    // �R�s�[���X�g����ǐՑΏۂ̗v�f��null�ɕύX
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
    #endregion


    // -------------------------------------------------------------
    // ��������e��X�e�[�g�N���X

    #region �X�e�[�g�Ǘ��N���X
    private class ChaceEnemyStateManager
    {
        // �e��X�e�[�g�C���X�^���X
        public Idle idleState { get; }
        public Alert alertState { get; }
        public Attack attackState { get; }

        ChaceEnemyParameter parameter = null;

        Transform transform = null;

        List<IChaceable> chaceableObjects = null;

        IStateChangeable stateChanger = null;

        VisionMeshCreator visionMeshCreator = null;

        Animator animator = null;

        Rigidbody rigidBody = null;

        BoxCollider weaponCollider = null;

        NavMeshAgent agent = null;

        public IChaceable chaceTarget = null;

        public ChaceEnemyStateManager(Animator animator, Transform transform, ChaceEnemyParameter parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, NavMeshAgent agent, Rigidbody rigidBody)
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
            idleState = new Idle(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator);
            alertState = new Alert(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody);
            attackState = new Attack(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, weaponCollider);
        }

    }
    #endregion



    #region �X�e�[�g���N���X
    private abstract class ChaceEnemyStateBase : StateBase
    {
        protected const int layerMask = ~(1 << 2);

        protected ChaceEnemyStateManager manager = null;
        protected ChaceEnemyParameter parameter = null;
        protected Transform transform = null;
        protected Animator animator = null;
        protected IStateChangeable stateChanger = null;
        protected VisionMeshCreator visionMeshCreator = null;

        // �ǐՉ\�ȑΏۃ��X�g
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public ChaceEnemyStateBase(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameter parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator)
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



    #region �ҋ@�X�e�[�g
    private class Idle : ChaceEnemyStateBase
    {
        public Idle(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameter parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator) { }

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



    #region �x���X�e�[�g
    private class Alert : ChaceEnemyStateBase
    {

        float distance = 0;

        Rigidbody rigidBody = null;
        NavMeshAgent agent = null;

        public Alert(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameter parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator)
        {
            this.agent = agent;
            this.rigidBody = rigidBody;
        }

        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();
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
                //Vector3 targetVector = new Vector3(manager.chaceTarget.chacebleTransform.position.x - transform.position.x, 0, manager.chaceTarget.chacebleTransform.position.z - transform.position.z);
                //Quaternion targetRotation = Quaternion.LookRotation(targetVector);


                agent.SetDestination(manager.chaceTarget.chacebleTransform.position);
                NavMeshPath path = agent.path;

                if (path.corners.Length > 1)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(path.corners[1] - path.corners[0]);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);
                }

                rigidBody.velocity = transform.forward * parameter.MoveSpeed * Time.deltaTime;

                // �����^�[�Q�b�g���U���͈͓��Ȃ�΍U���X�e�[�g�ֈڍs
                if (distance > parameter.AttackRange) return;

                stateChanger.ChangeState(manager.attackState);
            }


            // �x���͈͓��ɒǐՑΏۂ̃I�u�W�F�N�g���Ȃ��ꍇ�A�ҋ@�X�e�[�g�ֈڍs
            else stateChanger.ChangeState(manager.idleState);
        }
    }
    #endregion



    #region �U���X�e�[�g
    // �X�e�[�g�؂�ւ����ɕ���I�u�W�F�N�g�̓����蔻���ON�EOFF�؂�ւ�
    // �U���X�e�[�g�ɓ����莞�Ԍo�߂őҋ@�X�e�[�g�Ɉڍs
    private class Attack : ChaceEnemyStateBase
    {
        float countTime = 0;

        BoxCollider weaponCollider = null;

        public Attack(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameter parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator)
        {
            this.weaponCollider = weaponCollider;
        }


        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();
            weaponCollider.enabled = true;

            // �ǐՑΏۂ̃I�u�W�F�N�g�̍s����~�p���\�b�h���Ăяo��
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
            countTime += Time.deltaTime;
            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(manager.idleState);

        }
    }
    #endregion



}
