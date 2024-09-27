public class PlayerManager
{
    private PlayerCharaController _playerCharaController;



    public void SetPlayerCharaController(PlayerCharaController playerCharaController)
    {
        _playerCharaController = playerCharaController;

        _playerCharaController.Datas.OnDeath.AddListener(OnDeath);

        _playerCharaController.Datas.OnBulletSpawn.AddListener(OnBulletSpawn);
    }

    //------------------------------------------------------------------------------------------------
    // �v���C���[���o���b�g���o�����ꍇ�ɌĂяo���֐�
    private void OnBulletSpawn()
    {

    }

    //------------------------------------------------------------------------------------------------
    // �v���C���[�����񂾏ꍇ�ɌĂяo���֐�
    private void OnDeath()
    {
    }
}
