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
        // ���Ԃ�ݒ�
        _stageManager.TimeCounter.RemainingTimeValue.Subscribe(value =>
        {
            _timeCountManager.TimeCount(value);
        }).AddTo(this);

        // �^�[�Q�b�g����ݒ�
        // enemyManager�œG������^�C�~���O���擾����������������������������������������������������������������������������������������������
    }
}
