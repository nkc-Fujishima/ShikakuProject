using System;
using UnityEngine;

public abstract class EnemyControllerBase : MonoBehaviour, IStateChangeable, IDamage, IFallable
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�G�l�~�[�̃p�����[�^�X�N���v�^�u���I�u�W�F�N�g"), SerializeField] protected EnemyParameterDataBase parameter;
    [Tooltip("�G�l�~�[�Ŏg�p����G�t�F�N�g�Q�X�N���v�^�u���I�u�W�F�N�g"), SerializeField] protected EnemyEffectDataBase effect;
    [Tooltip("�G�l�~�[�p�J�[�\��"), SerializeField] protected CursorController cursor;

    protected IState iState = null;

    // �S�G�l�~�[���ʂ̂��ꂽ�����̃X�e�[�g
    private DieState dieState = null;

    protected Rigidbody rigidBody = null;

    protected Animator animator = null;

    protected AudioSource audioSource = null;

    private CapsuleCollider[] colliders = null;

    public event Action<EnemyControllerBase> OnDestroyHundle = null;
    protected void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        colliders = GetComponents<CapsuleCollider>();

        dieState = new DieState(this.gameObject, animator, rigidBody, colliders, audioSource, parameter, effect);

        // �G�̓���ɏo���J�[�\������
        CursorController cursorController = Instantiate(cursor);
        cursorController?.Construct(transform);
    }

    public virtual void OnStart() { }

    public void ChangeState(IState nextState)
    {
        if (iState != null) iState.OnExit();

        iState = nextState;

        if (iState != null) iState.OnEnter();
    }

    // �_���[�W��H������ꍇ�̏���
    public void Damage(Vector3 position)
    {
        // �_���[�W��^���Ă���Ώۂւ̃x�N�g���Ǝ��g�̑O���x�N�g���œ���
        Vector2 forwardVector2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 targetVector2D = new Vector2(position.x - transform.position.x, position.z - transform.position.z).normalized;

        float dotAngle = MathF.Acos((forwardVector2D.x * targetVector2D.x) + (forwardVector2D.y * targetVector2D.y) / (MathF.Sqrt(MathF.Pow(forwardVector2D.x, 2) + MathF.Pow(forwardVector2D.y, 2)) * MathF.Sqrt(MathF.Pow(targetVector2D.x, 2) + MathF.Pow(targetVector2D.y, 2))));
        float angle = dotAngle * Mathf.Rad2Deg;

        // �p�����[�^���̃_���[�W���󂯂Ȃ��p�x�ȏ�̂݃_���[�W���󂯂ăC�x���g�𔭉�
        if (angle < parameter.InvincibleAngle) return;

        Death();
    }

    // �X�e�[�W���痎�����ꍇ�̏���
    public void FallRiver()
    {
        Death();
    }

    // ���ʂƂ��̏���
    private void Death()
    {
        ChangeState(dieState);

        //this.gameObject.SetActive(false);

        OnDestroyHundle?.Invoke(this);
    }

    // �S�G�l�~�[���ʂ��ꂽ�ꍇ�̏���
    protected class DieState : IState
    {
        GameObject gameObject = null;
        Animator animator = null;
        Rigidbody rigidbody = null;
        CapsuleCollider[] colliders = null;
        AudioSource audioSource = null;

        EnemyParameterDataBase parameter = null;
        EnemyEffectDataBase effect = null;

        const float effectPosY = 1.5f;

        float countTime = 0;
        public DieState(GameObject gameObject, Animator animator, Rigidbody rigidbody, CapsuleCollider[] colliders, AudioSource audioSource, EnemyParameterDataBase parameter, EnemyEffectDataBase effect)
        {
            this.gameObject = gameObject;
            this.animator = animator;
            this.rigidbody = rigidbody;
            this.colliders = colliders;
            this.audioSource = audioSource;
            this.parameter = parameter;
            this.effect = effect;
        }

        public void OnEnter()
        {
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            rigidbody.isKinematic = true;
            animator?.Play("Die", 0, 0.0f);

            rigidbody?.AddForce(gameObject.transform.forward * parameter.DownForcePower, ForceMode.Impulse);
            audioSource.clip = effect.DownSE;
            audioSource?.Play();

            Instantiate(effect.DownEffect, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + effectPosY, gameObject.transform.position.z), Quaternion.identity);
        }

        public void OnExit()
        {
        }

        public void OnUpdate()
        {
            countTime += Time.deltaTime;

            if (countTime > parameter.ToDestroyTime)
            {
                Instantiate(effect.DestroyEffect, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + effectPosY, gameObject.transform.position.z), Quaternion.Euler(90, 0, 0));
                DestroyAudioPlay.PlayClipAtPoint(effect.DestroySE, gameObject.transform.position, 1f);

                gameObject.SetActive(false);
            }
        }
    }
}
