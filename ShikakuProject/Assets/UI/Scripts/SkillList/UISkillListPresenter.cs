using R3;
using UnityEngine;

// �X�L��UI�ɒl��n��
public class UISkillListPresenter : MonoBehaviour
{
    [SerializeField]
    private StageManager _stageManager;

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
        _player = _stageManager.PlayerManager.PlayerCharaController;

        if (!_player)
        {
            Debug.LogWarning("�v���C���[��������Ȃ������̂�\n�X�L��UI��Off�ɂȂ�܂���");
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

        _stageManager.IsPlaying.Subscribe(value =>
        {
            _listManager.SetActiveUI(value);

            if (value)
                _listManager.SelectSkill(_player.SelectBulletType.Value);
        }).AddTo(this);
    }
}
