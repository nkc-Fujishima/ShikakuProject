using R3;

public class PlayerManager
{
    public PlayerCharaController PlayerCharaController { get; private set; }

    public Subject<Unit> OnDieHundle = new Subject<Unit>();

    public void SetPlayerCharaController(PlayerCharaController playerCharaController)
    {
        PlayerCharaController = playerCharaController;

        PlayerCharaController.Datas.OnDeath.AddListener(OnDeath);

        PlayerCharaController.Datas.OnBulletSpawn.AddListener(OnBulletSpawn);
    }

    public void ExecutePlayerStart()
    {
        PlayerCharaController.ActivateMovement();
    }

    public void ExecutePlayerStop()
    {
        PlayerCharaController.DisableMovement();
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
        OnDieHundle.OnNext(Unit.Default);
    }
}
