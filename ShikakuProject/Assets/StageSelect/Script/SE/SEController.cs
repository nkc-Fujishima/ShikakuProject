using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEController : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("SE�f��"), SerializeField] AudioClip[] seClips;

    AudioSource audioSource = null;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void RingSelectSE()
    {
        audioSource.clip = seClips[0];
        audioSource.Play();
    }

    public void RingWorldDicisionSE()
    {
        audioSource.clip = seClips[1];
        audioSource.Play();
    }

    public void RingStageDicisionSE()
    {
        audioSource.clip = seClips[2];
        audioSource.Play();
    }

    public void RingCancelSE()
    {
        audioSource.clip = seClips[3];
        audioSource.Play();
    }
}
