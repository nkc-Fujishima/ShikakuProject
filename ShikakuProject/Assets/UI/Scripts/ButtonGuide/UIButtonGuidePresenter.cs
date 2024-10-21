using R3;
using UnityEngine;

public class UIButtonGuidePresenter : MonoBehaviour
{
    [SerializeField]
    private StageManager _stageManager;

    private PlayerCharaController _player;

    [SerializeField]
    private UIButtonGuideManager _guideManager;


    private void Start()
    {
        GetPlayer();

        SetPresenter();
    }

    private void GetPlayer()
    {
        _player = _stageManager.PlayerManager.PlayerCharaController;

        if (!_player)
        {
            Debug.LogWarning("プレイヤーが見つからなかったので\nスキルUIがOffになりました");
            gameObject.SetActive(false);
        }
    }

    private void SetPresenter()
    {
        _stageManager.IsPlaying.Subscribe(value =>
        {
            _guideManager.SetActiveUI(value);

        }).AddTo(this);
    }
}
