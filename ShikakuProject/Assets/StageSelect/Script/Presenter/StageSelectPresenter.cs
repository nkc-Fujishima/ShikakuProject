using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class StageSelectPresenter : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�X�e�[�W�Z���N�g�}�l�[�W��"), SerializeField] StageSelectManager stageSelectManager;
    [Tooltip("���[���h�I�u�W�F�N�g�R���g���[��"), SerializeField] WorldObjectsController worldObjectsController;
    [Tooltip("�X�e�[�W�Z���N�g�C���[�W�R���g���[��"), SerializeField] StageSelectImageController stageSelectImageController;
    [Tooltip("�X�e�[�W�C���[�W�R���g���[��"), SerializeField] StageImagesController stageImagesController;
    [Tooltip("�J�[�\���C���[�W�R���g���[��"), SerializeField] StageSelectCursorController stageSelectCursorController;
    [Tooltip("�C���t�H���[�V�����R���g���[��"),SerializeField] StageSelectInformationController informationController;

    // Start is called before the first frame update
    void Start()
    {
        // �Z���N�g���̃X�e�[�W---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingStage.Subscribe(image => { stageSelectImageController.MoveImagePosition(image); })
            .AddTo(this);

        // �Z���N�g���̃��[���h---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingWorld.Subscribe(worldCount =>
        {
            worldObjectsController.SetMainPosition(worldCount);
            worldObjectsController.SetPrevPosition(worldCount - 1);
            worldObjectsController.SetMainNextPosition(worldCount + 1);

            worldObjectsController.SetCurrentSelectWorldObject(worldCount);

            informationController.SetWorldName(worldCount);
        }).AddTo(this);

        // �X�e�[�W�Z���N�g�V�[�����ł̃X�e�[�g---------------------------------------------------------------------------------------------
        stageSelectManager.StageState.Subscribe(state =>
        {
            switch (state)
            {
                case StageState.WorldSelect:
                    // �Z���N�g�C���[�W
                    stageSelectImageController.Disable();
                    stageSelectImageController.ResetImagePosition();

                    // ���[���h�C���[�W
                    worldObjectsController.SetWorldSelectStatePosition();

                    // �X�e�[�W�Z���N�g�C���t�H���[�V����
                    informationController.SetWorldSelectStatePosition();
                    informationController.SetWorldSelectButtonInfo();
                    break;
                case StageState.StageSelect:
                    // �Z���N�g�C���[�W
                    stageSelectImageController.Enable();
                    stageSelectImageController.RestartZoom();

                    // ���[���h�C���[�W
                    worldObjectsController.SetStageSelectStatePosition();

                    // �J�[�\���C���[�W
                    stageSelectCursorController.HideCursorImage();

                    // �X�e�[�W�Z���N�g�C���t�H���[�V����
                    informationController.SetStageSelectStatePosition();
                    informationController.SetStageSelectButtonInfo();
                    break;
            }

            stageImagesController.SetPosition((int)state);
        }).AddTo(this);

        // ���[���h�̑I�𐔃n���h��---------------------------------------------------------------------------------------------
        stageSelectManager.worldSelectMinHundle.Subscribe(_ => stageSelectCursorController.HideLeftCursorImage());
        stageSelectManager.worldSelectMaxHundle.Subscribe(_ => stageSelectCursorController.HideRightCursorImage());
        stageSelectManager.worldSelectNotMaxorMinHundle.Subscribe(_ => stageSelectCursorController.VisibleCursorImage());
    }
}
