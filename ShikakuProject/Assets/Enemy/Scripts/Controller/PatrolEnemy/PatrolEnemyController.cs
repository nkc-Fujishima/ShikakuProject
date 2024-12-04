using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyController : EnemyControllerBase, IWaypointHolder
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
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.updatePosition = false;
        ChaceEnemyStateManager stateHolder = new ChaceEnemyStateManager(animator, this.transform, parameter as ChaceEnemyParameterData, this, chaceableObjects, visionMeshCreator, weaponCollider, agent, rigidBody, effect as ChaceEnemyEffectData, audioSource, Waypoints);
        iState = stateHolder.patrolState;
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
        public Idle idleState { get; }
        public Patrol patrolState { get; }
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

        List<Vector3> waypoints = null;

        public IChaceable chaceTarget = null;

        public ChaceEnemyStateManager(Animator animator, Transform transform, ChaceEnemyParameterData parameter, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints)
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
            this.waypoints = waypoints;

            idleState = new Idle(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints);
            patrolState = new Patrol(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource, this.waypoints);
            alertState = new Alert(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, agent, rigidBody, effect, audioSource, waypoints);
            attackState = new Attack(this, this.animator, transform, parameter, this, stateChanger, chaceableObjects, visionMeshCreator, weaponCollider, effect, audioSource, waypoints);
        }

    }
    #endregion



    #region �X�e�[�g���N���X
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

        protected List<Vector3> waypoints = null;

        // �ǐՉ\�ȑΏۃ��X�g
        protected List<IChaceable> chaceableObjects = new List<IChaceable>(6);

        public ChaceEnemyStateBase(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints)
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



    #region �ҋ@�X�e�[�g
    private class Idle : ChaceEnemyStateBase
    {
        // �X�e�[�g�ɓ����Ă���o�����b��
        float passedSeconds = 0;

        public Idle(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints) { }

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
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.alertState);

            if (passedSeconds >= 2)
            {
                Debug.Log(waypoints.Count);
                if (waypoints.Count == 0) return;
                    stateChanger.ChangeState(manager.patrolState);
            }
            else
                passedSeconds += Time.deltaTime;

        }
    }
    #endregion



    #region ����X�e�[�g
    private class Patrol : ChaceEnemyStateBase
    {
        // �ڕW�̏���|�C���g�̓������苗��
        float arrivalLangth = 1;

        int patrolStep = 0;

        Rigidbody rigidBody = null;
        NavMeshAgent agent = null;

        public Patrol(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints)
        {
            this.agent = agent;
            this.rigidBody = rigidBody;
        }


        public override void OnEnter()
        {
            if (waypoints.Count != 0)
            {
                // �����_�ň�ԋ߂��|�C���g��
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
                        // ���ׂĂ���|�C���g���I�肵�Ă���|�C���g�Ɣ�ׂĉ����ꍇ�͏I��
                        float checkDistance = (transform.position - waypoints[i]).magnitude;
                        if (selectDistance < checkDistance) continue;

                        // �߂��ꍇ�͍X�V
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
            animator.SetBool("WalkFlag", false);
        }

        public override void OnUpdate()
        {
            foreach (var enemy in chaceableObjects)
            {
                Ray toTargetRay = new Ray(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), new Vector3(enemy.chacebleTransform.position.x - transform.position.x, transform.position.y - 0.5f, enemy.chacebleTransform.position.z - transform.position.z));
                RaycastHit toTargetHit;
                if (!Physics.Raycast(toTargetRay, out toTargetHit, Mathf.Infinity, layerMask)) return;

                IChaceable chaceableObject = null;
                if (!toTargetHit.transform.TryGetComponent<IChaceable>(out chaceableObject)) continue;

                manager.chaceTarget = chaceableObject;
            }

            PatrolMovement();
            ChangePatrolPoint();

            if (manager.chaceTarget != null) stateChanger.ChangeState(manager.alertState);

            if (waypoints.Count == 0) stateChanger.ChangeState(manager.idleState);
        }

        // ���񂷂铮��
        private void PatrolMovement()
        {
            if (waypoints.Count == 0) return;

            agent.nextPosition = transform.position;

            NavMeshPath path = agent.path;


            // NavMeash�̃p�X��p���Ĉړ�������ύX
            if (path.corners.Length > 1)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.steeringTarget - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, parameter.RotateSpeed * Time.deltaTime);
            }

            rigidBody.velocity = transform.forward * parameter.MoveSpeed;
        }

        // �p�g���[���|�C���g��ς���
        private void ChangePatrolPoint()
        {
            if (waypoints.Count == 0) return;

            // �����|�C���g�Ƃ̋����𒲂ׂ�
            float distance = (transform.position - waypoints[patrolStep]).magnitude;

            // �������苗����艓��������I��
            if (arrivalLangth < distance) return;


            // ���������Ă����玟�̏���|�C���g��
            ++patrolStep;
            if (patrolStep >= waypoints.Count) patrolStep = 0;
            agent.SetDestination(waypoints[patrolStep]);
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
        public Alert(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHollder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, NavMeshAgent agent, Rigidbody rigidBody, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHollder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints)
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
                // �ǐՑΏۃC���^�t�F�[�X�������Ă��ꍇ�A����������
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

        public Attack(ChaceEnemyStateManager manager, Animator animator, Transform transform, ChaceEnemyParameterData parameter, ChaceEnemyStateManager stateHolder, IStateChangeable stateChanger, List<IChaceable> chaceableObjects, VisionMeshCreator visionMeshCreator, BoxCollider weaponCollider, ChaceEnemyEffectData effect, AudioSource audioSource, List<Vector3> waypoints) : base(manager, animator, transform, parameter, stateHolder, stateChanger, chaceableObjects, visionMeshCreator, effect, audioSource, waypoints)
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
            countTime += Time.deltaTime;
            if (countTime > parameter.AttackCoolTime) stateChanger.ChangeState(manager.patrolState);

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


    //-----------------------------------------------------------
    //�@IWaypointHolder
    public List<Vector3> Waypoints { get; private set; } = new();

    public void SetWaypoints(List<Vector3> points)
    {
        Waypoints = new List<Vector3>(points);
    }
}