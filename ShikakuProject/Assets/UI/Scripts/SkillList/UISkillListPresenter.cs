using R3;
using UnityEngine;

// スキルUIに値を渡す
public class UISkillListPresenter : MonoBehaviour
{
    [SerializeField]
    private StageManager stageManager;

    private PlayerCharaController _player;

    [SerializeField]
    private UISkillListManager _listManager;


    private void Start()
    {
        GetPlayer();

        ExecuteOnStart();

        SetPresenter();
    }

    private void GetPlayer()
    {
        _player = stageManager.PlayerManager.PlayerCharaController;

        if (!_player)
        {
            Debug.LogWarning("プレイヤーが見つからなかったので\nスキルUIがOffになりました");
            gameObject.SetActive(false);
        }
    }

    private void ExecuteOnStart()
    {
        _listManager.OnStart(_player.GetBulletTexture);
    }

    private void SetPresenter()
    {
        _player.SelectBulletType.Subscribe(value=>
        {
            _listManager.SelectSkill(value);
        }).AddTo(this);


        for (int i = 0; i < _player.CountTimeRates.Length; ++i)
        {
            int index = i;
            _player.CountTimeRates[index].Subscribe(value =>
            {
                value = 1 - value;
                _listManager.DisplayCooldown(index, value);
            }).AddTo(this);
        }
    }
}
