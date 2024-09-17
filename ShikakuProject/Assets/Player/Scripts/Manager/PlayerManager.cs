public class PlayerManager
{
    private PlayerCharaController _playerCharaController;

    private IBulletSpawn _bulletSpawn;


    public void SetPlayerCharaController(PlayerCharaController playerCharaController)
    {
        _playerCharaController = playerCharaController;

        _playerCharaController.Datas.OnBulletSpawn.AddListener(OnBulletSpawn);
        _playerCharaController.Datas.OnDeath.AddListener(OnDeath);
    }

    public void SetIBulletSpawn(IBulletSpawn bulletSpawn)
    {
        _bulletSpawn = bulletSpawn;
    }

    //------------------------------------------------------------------------------------------------
    // �o���b�g���������ꂽ�ꍇ�ɌĂяo���֐�
    private void OnBulletSpawn(BulletControllerBase bullet)
    {
        _bulletSpawn.BulletSpawn(bullet);
    }

    //------------------------------------------------------------------------------------------------
    // �v���C���[�����񂾏ꍇ�ɌĂяo���֐�
    private void OnDeath()
    {
    }
}
