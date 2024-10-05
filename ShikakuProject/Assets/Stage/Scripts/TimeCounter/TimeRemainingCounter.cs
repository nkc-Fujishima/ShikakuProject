using R3;
using System;
using UnityEngine;

public class TimeRemainingCounter
{
    // タイムアップになったら動くイベント
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
        // 一時停止されていたら終了
        if (_isStop) return;

        // タイムアップしていたら終了
        if (RemainingTimeValue.Value == 0) return;


        TimeCount();
    }

    // 時間を計測する
    private void TimeCount()
    {
        float time = RemainingTimeValue.Value - Time.deltaTime;

        if (RemainingTimeValue.Value < 0)
            time = 0;

        RemainingTimeValue.Value = time;


        // 時間が過ぎていたらイベントを走らせる
        if (time <= 0)
        {
            OnTimeUpEvent?.Invoke();
            OnPause();
        }
    }

    //-----------------------------------------------------------------------------------
    // 一時停止
    public void OnPause()
    {
        _isStop = true;
    }

    //-----------------------------------------------------------------------------------
    // カウント再開
    public void OnResume()
    {
        _isStop = false;
    }
}
