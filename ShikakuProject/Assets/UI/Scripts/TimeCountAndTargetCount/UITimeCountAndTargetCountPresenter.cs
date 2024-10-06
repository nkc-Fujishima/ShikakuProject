using R3;
using UnityEngine;

public class UITimeCountAndTargetCountPresenter : MonoBehaviour
{
    [SerializeField]
    private StageManager _stageManager;

    [SerializeField]
    private UITimeCountManager _timeCountManager;

    [SerializeField]
    private UITargetCountManager _targetCountManager;


    private EnemyManager _enemyManager;


    private void Start()
    {
        _enemyManager = _stageManager.EnemyManager;

        SetOnStart();
     
        SetPresenter();
    }

    private void SetOnStart()
    {
        _timeCountManager.OnStart(_stageManager.TimeCounter.TimeMax);

        _targetCountManager.OnStart(_enemyManager.EnemyList.Count);
    }

    private void SetPresenter()
    {
        // 時間を設定
        _stageManager.TimeCounter.RemainingTimeValue.Subscribe(value =>
        {
            _timeCountManager.TimeCount(value);
        }).AddTo(this);

        // ターゲットの消えた瞬間を取得
        _enemyManager.OnEnemyDestroyHundle += OnEnemyDestroy;
    }

    private void OnEnemyDestroy()
    {
        _targetCountManager.TargetCount(_enemyManager.EnemyList.Count);
    }
}
