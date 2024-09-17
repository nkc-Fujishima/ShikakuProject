public class PlayerManager
{
    private PlayerCharaController _playerCharaController;

    private IBulletSpawn _bulletSpawn;


    public void SetPlayerCharaController(PlayerCharaController playerCharaController)
    {
        _playerCharaController = playerCharaController;

        _playerCharaController.PlayerStatus.OnBulletSpawn.AddListener(OnBulletSpawn);
    }

    public void SetIBulletSpawn(IBulletSpawn bulletSpawn)
    {
        _bulletSpawn = bulletSpawn;
    }

    //------------------------------------------------------------------------------------------------
    // バレットが生成された場合に呼び出す関数
    private void OnBulletSpawn(BulletControllerBase bullet)
    {
        _bulletSpawn.BulletSpawn(bullet);
    }
}
