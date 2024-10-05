using R3;
using System;
using UnityEngine;

public class TimeRemainingCounter
{
    // �^�C���A�b�v�ɂȂ����瓮���C�x���g
    public event Action OnTimeUpEvent = null;

    public ReactiveProperty<float> RemainingTimeValue { get; private set; } = new(0);

    public float TimeMax { get; private set; } = 0;


    private bool _isStop = true;


    public void SetTimer(float time)
    {
        RemainingTimeValue.Value = time;
        TimeMax = time;
    }

    public void OnUpdate()
    {
        // �ꎞ��~����Ă�����I��
        if (_isStop) return;

        // �^�C���A�b�v���Ă�����I��
        if (RemainingTimeValue.Value == 0) return;


        TimeCount();
    }

    // ���Ԃ��v������
    private void TimeCount()
    {
        float time = RemainingTimeValue.Value - Time.deltaTime;

        if (RemainingTimeValue.Value < 0)
            time = 0;

        RemainingTimeValue.Value = time;


        // ���Ԃ��߂��Ă�����C�x���g�𑖂点��
        if (time <= 0)
        {
            OnTimeUpEvent?.Invoke();
            OnPause();
        }
    }

    //-----------------------------------------------------------------------------------
    // �ꎞ��~
    public void OnPause()
    {
        _isStop = true;
    }

    //-----------------------------------------------------------------------------------
    // �J�E���g�ĊJ
    public void OnResume()
    {
        _isStop = false;
    }
}
