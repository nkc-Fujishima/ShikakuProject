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
    // プレイヤーがバレットを出した場合に呼び出す関数
    private void OnBulletSpawn()
    {

    }

    //------------------------------------------------------------------------------------------------
    // プレイヤーが死んだ場合に呼び出す関数
    private void OnDeath()
    {
        OnDieHundle.OnNext(Unit.Default);
    }
}
