using Cysharp.Threading.Tasks;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectPresenter : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�"), Tooltip("�X�e�[�W�Z���N�g�}�l�[�W��"), SerializeField] StageSelectManager stageSelectManager;
    [Tooltip("���[���h�I�u�W�F�N�g�R���g���[��"), SerializeField] WorldObjectsController worldObjectsController;
    [Tooltip("�X�e�[�W�Z���N�g�C���[�W�R���g���[��"), SerializeField] StageSelectImageController stageSelectImageController;
    [Tooltip("�X�e�[�W�C���[�W�R���g���[��"), SerializeField] StageImagesController stageImagesController;
    [Tooltip("�J�[�\���C���[�W�R���g���[��"), SerializeField] StageSelectCursorController stageSelectCursorController;

    [Header("���l�ݒ�"), Tooltip("�X�e�[�W�C���[�W�I�u�W�F�N�g�ړ��ʒu"), SerializeField]
    Vector3[] stageImagePositions;
    [Tooltip("�Z���N�g�C���[�W�����ʒu"), SerializeField] Vector3 initialPos;
    [Tooltip("���[���h�C���[�W�I�u�W�F�N�g�ړ��ʒu"), SerializeField] Vector3[] worldImageObjectPositions;
    [Tooltip("�X�e�[�g���̑I�����[���h�̈ړ��ʒu"), SerializeField] Vector3[] stateChangeWorldImagePosition;


    // Start is called before the first frame update
    void Start()
    {
        // �Z���N�g���̃X�e�[�W---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingStage.Subscribe(image => { stageSelectImageController.MoveImagePosition(image); })
            .AddTo(this);

        // �Z���N�g���̃��[���h---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingWorld.Subscribe(worldCount =>
        {
            worldObjectsController.SetObjectPosition(worldCount, worldImageObjectPositions[0]);
            worldObjectsController.SetObjectPosition(worldCount - 1, worldImageObjectPositions[1]);
            worldObjectsController.SetObjectPosition(worldCount + 1, worldImageObjectPositions[2]);

            worldObjectsController.SetCurrentSelectWorldObject(worldCount);
        }).AddTo(this);

        // �X�e�[�W�Z���N�g�V�[�����ł̃X�e�[�g---------------------------------------------------------------------------------------------
        stageSelectManager.StageState.Subscribe(state =>
        {
            switch (state)
            {
                case StageState.WorldSelect:
                    // �Z���N�g�C���[�W
                    stageSelectImageController.Disable();
                    stageSelectImageController.ResetImagePosition(initialPos);

                    // ���[���h�C���[�W
                    worldObjectsController.SetStageSelectStatePosition(stateChangeWorldImagePosition[0]);
                    break;
                case StageState.StageSelect:
                    // �Z���N�g�C���[�W
                    stageSelectImageController.Enable();
                    stageSelectImageController.RestartZoom();

                    // ���[���h�C���[�W
                    worldObjectsController.SetStageSelectStatePosition(stateChangeWorldImagePosition[1]);

                    // �J�[�\���C���[�W
                    stageSelectCursorController.HideCursorImage();
                    break;
            }

            stageImagesController.SetPosition(stageImagePositions[(int)state]);
        }).AddTo(this);

        // ���[���h�̑I�𐔃n���h��---------------------------------------------------------------------------------------------
        stageSelectManager.worldSelectMinHundle.Subscribe(_ => stageSelectCursorController.HideLeftCursorImage());
        stageSelectManager.worldSelectMaxHundle.Subscribe(_ => stageSelectCursorController.HideRightCursorImage());
        stageSelectManager.worldSelectNotMaxorMinHundle.Subscribe(_ => stageSelectCursorController.VisibleCursorImage());
    }
}
