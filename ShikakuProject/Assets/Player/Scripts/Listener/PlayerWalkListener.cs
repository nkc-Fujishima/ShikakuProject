using UnityEngine;

public class PlayerWalkListener : MonoBehaviour
{
    [SerializeField]
    PlayerCharaController _playerController;

    public void OnWalk()
    {
        _playerController.OnOneStep();
    }
}
