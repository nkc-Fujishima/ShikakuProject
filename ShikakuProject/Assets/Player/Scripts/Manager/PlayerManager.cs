using R3;
using System.Numerics;
using UnityEngine;

public class PlayerManager
{
    public PlayerCharaController PlayerCharaController { get; private set; }

    public Subject<Unit> OnDieHundle = new ();

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

    public void StopPlayer()
    {
        PlayerCharaController.DisableMovement();
    }

    //------------------------------------------------------------------------------------------------
    // 時間切れになった場合に呼び出す関数
    public void TimeUp()
    {
        PlayerCharaController.Death();
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

    public UnityEngine.Vector3 GetPlayerOnScreenPos()
    {
        return Camera.main.WorldToScreenPoint(PlayerCharaController.transform.position);
    }
}
