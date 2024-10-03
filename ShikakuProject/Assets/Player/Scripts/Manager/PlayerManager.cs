using R3;

public class PlayerManager
{
    private PlayerCharaController _playerCharaController;

    public ReactiveProperty<bool> IsDeath { get; private set; } = new ReactiveProperty<bool>(false);


    public void SetPlayerCharaController(PlayerCharaController playerCharaController)
    {
        _playerCharaController = playerCharaController;

        _playerCharaController.Datas.OnDeath.AddListener(OnDeath);

        _playerCharaController.Datas.OnBulletSpawn.AddListener(OnBulletSpawn);
    }

    //------------------------------------------------------------------------------------------------
    // プレイヤーがバレットを出した場合に呼び出す関数
    private void OnBulletSpawn()
    {

    }

    //------------------------------------------------------------------------------------------------
    // プレイヤーが死んだ場合に呼び出す関数
    private void OnDeath()
    {
        IsDeath.Value = true;
    }
}
