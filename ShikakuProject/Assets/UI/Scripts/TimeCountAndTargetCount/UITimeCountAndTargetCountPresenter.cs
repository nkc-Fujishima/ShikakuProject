using R3;
using UnityEngine;

public class UITimeCountAndTargetCountPresenter : MonoBehaviour
{
    [SerializeField]
    StageManager _stageManager;

    [SerializeField]
    UITimeCountManager _timeCountManager;

    [SerializeField]
    UITargetCountManager _targetCountManager;

    private void Start()
    {
        SetOnStart();
     
        SetPresenter();
    }

    private void SetOnStart()
    {
        _timeCountManager.OnStart(_stageManager.TimeCounter.TimeMax);

        _targetCountManager.OnStart(_stageManager.EnemyManager.EnemyList.Count);
    }

    private void SetPresenter()
    {
        // 時間を設定
        _stageManager.TimeCounter.RemainingTimeValue.Subscribe(value =>
        {
            _timeCountManager.TimeCount(value);
        }).AddTo(this);

        // ターゲット数を設定
        // enemyManagerで敵が減るタイミングを取得したいっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっっｄ
    }
}
