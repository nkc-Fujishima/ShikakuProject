using UnityEngine;

public class SEController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("SE�f��"), SerializeField] AudioClip[] seClips;

    AudioSource audioSource = null;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // �I��p��SE��炵�܂�
    public void RingSelectSE()
    {
        audioSource.clip = seClips[0];
        audioSource.Play();
    }

    // ���[���h����p��SE��炵�܂�
    public void RingWorldDicisionSE()
    {
        audioSource.clip = seClips[1];
        audioSource.Play();
    }

    // �X�e�[�W����p��SE��炵�܂�
    public void RingStageDicisionSE()
    {
        audioSource.clip = seClips[2];
        audioSource.Play();
    }

    // �L�����Z���p��SE��炵�܂�
    public void RingCancelSE()
    {
        audioSource.clip = seClips[3];
        audioSource.Play();
    }
}
