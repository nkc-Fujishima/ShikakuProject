using R3;
using UnityEngine;

// ƒXƒLƒ‹UI‚É’l‚ð“n‚·
public class UISkillListPresenter : MonoBehaviour
{
    private PlayerCharaController _player;

    [SerializeField]
    private UISkillListManager _listManager;


    private void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerCharaController>();

        SetPresenter();
    }

    private void SetPresenter()
    {
        _listManager.OnStart(_player.GetBulletTexture);

        _player.SelectBulletType.Subscribe(vector=>
        {
            _listManager.SelectSkill(vector);
        }).AddTo(this);


        for (int i = 0; i < _player.CountTimeRates.Length; ++i)
        {
            int index = i;
            _player.CountTimeRates[index].Subscribe(vector =>
            {
                vector = 1 - vector;
                _listManager.DisplayCooldown(index, vector);
            }).AddTo(this);
        }
    }
}
