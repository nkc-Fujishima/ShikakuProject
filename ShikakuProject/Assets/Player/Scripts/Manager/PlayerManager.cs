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
    // ���Ԑ؂�ɂȂ����ꍇ�ɌĂяo���֐�
    public void TimeUp()
    {
        PlayerCharaController.Death();
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

    public UnityEngine.Vector3 GetPlayerOnScreenPos()
    {
        return Camera.main.WorldToScreenPoint(PlayerCharaController.transform.position);
    }
}
