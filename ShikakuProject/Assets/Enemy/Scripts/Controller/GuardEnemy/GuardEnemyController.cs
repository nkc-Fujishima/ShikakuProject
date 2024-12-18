using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardEnemyController : EnemyControllerBase
{
    [Header("�I�u�W�F�N�g�ݒ�"), SerializeField] GameObject weapon;

    // �Q�[�����̌o�߃t���[��
    ulong frameCount = 0;

    // �ǐՉ\�ȑΏۃ��X�g
    List<IChaceable> chaceableObjects = new List<IChaceable>(6);
    // foreach���Ƀ��X�g���ύX����邱�Ƃ�h�����߂̕����A�����炪�ύX������Ԋu�Ŗ{���C���̃��X�g�ɔ��f
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

        // ���E�Z���T�[��Action�Ƀ^�[�Q�b�g���X�g�̒ǉ��ƍ폜���\�b�h��o�^
        visionSensor.OnSensorInHundle += AddTarget;
        visionSensor.OnSensorOutHundle += RemoveTarget;

        // RigidBody�̋�����NavMesh�̈ʒu�X�V���������邽�߁ANavMesh�̍X�V���~�߂�
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.updatePosition = false;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        // �X�e�[�g�𐶐��A��{�X�e�[�g�ɑҋ@��ݒ�
        ChaceEnemyStateManager stateHolder = new ChaceEnemyStateManager(animator, this.transform, parameter as GuardEnemyParameterData, this, chaceableObjects, visionMeshCreator, weaponCollider, agent, rigidBody, effect as ChaceEnemyEffectData, audioSource,startPos,startRot);

        iState = stateHolder.IdleState;
        if (iState != null) iState.OnEnter();

    }

    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        iState?.OnUpdate();

        // ���t���[���o�ߎ��Ƀ��X�g����null���폜
        frameCount += 1;
        if (frameCount % parameter.ListRefreshRate == 0)
        {
            chaceableObjects.Clear();
            chaceableObjects.AddRange(RemoveNullElements(copyList));
        }
    }



    // ---------------------------------------------------
    // ���X�g�ύX���\�b�h�Q
    #region ���X�g�ύX���\�b�h�Q
    // �R�s�[���X�g�ɒǐՑΏۂ�ǉ�
    private void AddTarget(IChaceable chaceableObject)
    {
        if (copyList.Contains(chaceableObject)) return;

        copyList.Add(chaceableObject);
        chaceableObject.chacebleTransform.GetComponent<IDestroy>().OnDestroyHundle += RemoveTarget;
    }

    // �R�s�[���X�g����ǐՑΏۂ̗v�f��null�ɕύX
    private void RemoveTarget(IChaceable chaceableObject)
    {
        if (!copyList.Contains(chaceableObject)) return;

        copyList[copyList.IndexOf(chaceableObject)] = null;

        chaceableObject.chacebleTransform.GetComponent<IDestroy>().OnDestroyHundle -= RemoveTarget;
    }

    /// <summary>
    /// ���X�g����null���폜
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
    // ��������e��X�e�[�g�֘A�N���X

    #region �X�e�[�g�Ǘ��N���X
    private class ChaceEnemyStateManager
    {
        // �e��X�e�[�g�C���X�^���X---------------------
        public Idle IdleState { get; }
        public MoveWaitingArea MoveWaitingAreaState { get; }
        public Adjustment AdjustmentState { get; }
        public Alert AlertState { get; }
        public Attack AttackState { get; }
        //----------------------------------------------

        GuardEnemyParameterData parameter = null;

        Transform transform = null;

        List<IChaceable> chaceableObjects = null;

        IStateChangeable stateChanger = null;

        VisionMeshCreator visionMeshCreator = null;

        Animator animator = null;

        Rigidbody rigidBody = null;

        BoxCollider weaponCollider = null;

        NavMeshAgent agent = null;

        public IChaceable chaceTarget = null;

        public ChaceEnemyStateManager(Animator animator, Transform transform, GuardEnemyParameterData parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, Vector3 startPos, Quaternion startRot)
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
            IdleState = new Idle(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource);
            MoveWaitingAreaState = new MoveWaitingArea(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource, startPos);
            AdjustmentState = new Adjustment(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource, startPos, startRot);
            AlertState = new Alert(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource);
            AttackState = new Attack(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, weaponCollider, effect, audioSource);
        }

    }
    #endregion



    #region �X�e�[�g���N���X
    private abstract class ChaceEnemyStateBase : StateBase
    {
        protected const int layerMask = ~(1 << 2);

        protected ChaceEnemyStateManager manager = null;
        protected GuardEnemyParameterData parameter = null;
        protected Transform transform = null;
        protected Animator animator = null;
        protected IStateChangeable stateChanger = null;
        protected VisionMeshCreator visionMeshCreator = null;
        protected ChaceEnemyEffectData effect = null;
        protected AudioSource audioSource = null;

        // �ǐՉ\�ȑΏۃ��X�g
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public ChaceEnemyStateBase(ChaceEnemyStateManager manager, Animator animator, Transform transform, GuardEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource)
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



    #region �ҋ@�X�e�[�g
    private class Idle : ChaceEnemyStateBase
    {
        public Idle(ChaceEnemyStateManager manager, Animator animator, Transform transform, GuardEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource) { }

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
            // IChaceable�������Ă���I�u�W�F�N�g�����݂��邩���C�Ŋm�F
            // ���̌�AIChaceable�Ƃ̊ԂɎՂ镨��������ΒǐՑΏۂɐݒ肵�A�X�e�[�g��ύX
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.AlertState);
        }
    }
    #endregion



    #region �ҋ@�ꏊ�ړ��X�e�[�g
    private class MoveWaitingArea : ChaceEnemyStateBase
    {
        // �ҋ@�|�C���g�̓������苗��
        readonly float arrivalLength = 0.1f;

        Rigidbody rigidBody = null;
        NavMeshAgent agent = null;

        Vector3 startPos;

        public MoveWaitingArea(ChaceEnemyStateManager manager, Animator animator, Transform transform, GuardEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, Vector3 startPos) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
        {
            this.agent = agent;
            this.rigidBody = rigidBody;

            this.startPos = startPos;
        }

        public override void OnEnter()
        {

            agent.SetDestination(startPos);

            visionMeshCreator.ChangeMeshNoAlertMaterial();
            animator.SetBool("WalkFlag", true);

        }

        public override void OnExit()
        {
            animator.SetBool("WalkFlag", false);
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            WaitingAreaMovement();
            CompletePoint();

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.AlertState);
        }

        // �ҋ@�ꏊ�ɖ߂铮��
        private void WaitingAreaMovement()
        {
            agent.nextPosition = transform.position;

            NavMeshPath path = agent.path;


            // NavMeash�̃p�X��p���Ĉړ�������ύX
            if (path.corners.Length > 1)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed_Patrol * Time.deltaTime);
            }

            rigidBody.velocity = transform.forward * parameter.MoveSpeed_Patrol;
        }

        // �X�^�[�g�̃|�C���g�ɖ߂���
        private void CompletePoint()
        {
            // �����|�C���g�Ƃ̋����𒲂ׂ�
            float distance = (transform.position - startPos).magnitude;

            // �������苗����艓��������I��
            if (arrivalLength < distance) return;


            // �ʂ����玟�̃X�e�[�g��
            stateChanger.ChangeState(manager.AdjustmentState);
        }
    }
    #endregion



    #region �����X�e�[�g
    private class Adjustment : ChaceEnemyStateBase
    {
        // �ҋ@�|�C���g�̓������苗��
        readonly float arrivalLength = 0.7f;

        Rigidbody rigidBody = null;
        NavMeshAgent agent = null;

        Vector3 startPos;

        public Adjustment(ChaceEnemyStateManager manager, Animator animator, Transform transform, GuardEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, Vector3 startPos, Quaternion startRot) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
        {
            this.agent = agent;
            this.rigidBody = rigidBody;

            Vector3 pos = startRot * Vector3.forward * 1f;
            this.startPos = startPos + pos;
        }

        public override void OnEnter()
        {
            // �ڕW�ꏊ���������O�ɃA���J�[��ݒ肷��
            agent.SetDestination(startPos);

            visionMeshCreator.ChangeMeshNoAlertMaterial();
            animator.SetBool("WalkFlag", true);

        }

        public override void OnExit()
        {
            animator.SetBool("WalkFlag", false);
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            WaitingAreaMovement();
            CompletePoint();

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.AlertState);
        }

        // �ҋ@�ꏊ�ɖ߂铮��
        private void WaitingAreaMovement()
        {
            agent.nextPosition = transform.position;

            NavMeshPath path = agent.path;


            // NavMeash�̃p�X��p���Ĉړ�������ύX
            if (path.corners.Length > 1)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed_Adjustment * Time.deltaTime);
            }

            rigidBody.velocity = transform.forward * parameter.MoveSpeed_Patrol;
        }

        // �X�^�[�g�̃|�C���g�ɖ߂���
        private void CompletePoint()
        {
            // �����|�C���g�Ƃ̋����𒲂ׂ�
            float distance = (transform.position - startPos).magnitude;

            // �������苗����艓��������I��
            if (arrivalLength < distance) return;


            // �ʂ����玟�̃X�e�[�g��
            stateChanger.ChangeState(manager.IdleState);
        }
    }
    #endregion



    #region �x���X�e�[�g
    private class Alert : ChaceEnemyStateBase
    {
        const float effectPosY = 3;

        float distance = 0;

        Rigidbody rigidBody = null;
        NavMeshAgent agent = null;
        public Alert(ChaceEnemyStateManager manager, Animator animator, Transform transform, GuardEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
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
                // �^�[�Q�b�g�Ƃ̊ԂɃ��C���q��
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) continue;

                // ���C�ɏ��߂ăq�b�g�����I�u�W�F�N�g��
                // �ǐՑΏۃC���^�t�F�[�X�������Ă��Ȃ��ꍇ�A����������
                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                if (manager.chaceTarget == null) manager.chaceTarget = chaceableObject;

                distance = new Vector3(chaceableObject.chacebleTransform.position.x - transform.position.x, 0, chaceableObject.chacebleTransform.position.z - transform.position.z).magnitude;

                if (distance < new Vector3(manager.chaceTarget.chacebleTransform.position.x - transform.position.x, 0, manager.chaceTarget.chacebleTransform.position.z - transform.position.z).magnitude)
                {
                    manager.chaceTarget = chaceableObject;
                }
            }

            // �ǐՑΏۂ����݂���ꍇ�ANavMesh�̌o�H�𗘗p���A�U���Ώۂ֋߂Â�
            if (manager.chaceTarget != null)
            {
                agent.nextPosition = transform.position;

                agent.SetDestination(manager.chaceTarget.chacebleTransform.position);
                NavMeshPath path = agent.path;


                // NavMeash�̃p�X��p���Ĉړ�������ύX
                if (path.corners.Length > 1)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);

                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);
                }

                rigidBody.velocity = transform.forward * parameter.MoveSpeed;

                // �����^�[�Q�b�g���U���͈͓��Ȃ�΍U���X�e�[�g�ֈڍs
                if (distance > parameter.AttackRange) return;

                stateChanger.ChangeState(manager.AttackState);
            }


            // �x���͈͓��ɒǐՑΏۂ̃I�u�W�F�N�g���Ȃ��ꍇ�A�X�^�[�g�ʒu�ɖ߂�X�e�[�g��
            else stateChanger.ChangeState(manager.MoveWaitingAreaState);
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

        public Attack(ChaceEnemyStateManager manager, Animator animator, Transform transform, GuardEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, ChaceEnemyEffectData effect, AudioSource audioSource) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource)
        {
            this.weaponCollider = weaponCollider;
        }


        public override void OnEnter()
        {
            visionMeshCreator.ChangeMeshAlertMaterial();

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
            // ���̃X�e�[�g�ɓ����Ă���̌o�ߎ��Ԃ��p�����[�^�̍U���N�[���^�C������ɂȂ�����
            // �U���X�e�[�g����
            countTime += Time.deltaTime;
            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(manager.MoveWaitingAreaState);
        }
    }
    #endregion



    //-----------------------------------------------------------
    //�@�U�������A�A�j���[�V�����C�x���g�ɂĎg�p
    private void Slash()
    {
        weapon.GetComponent<BoxCollider>().enabled = true;
        audioSource.clip = (effect as ChaceEnemyEffectData).slashSE;
        audioSource.Play();
    }

}