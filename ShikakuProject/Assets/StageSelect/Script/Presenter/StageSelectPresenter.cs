using Cysharp.Threading.Tasks;
using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectPresenter : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("ステージセレクトマネージャ"), SerializeField] StageSelectManager stageSelectManager;
    [Tooltip("ワールドオブジェクトコントローラ"), SerializeField] WorldObjectsController worldObjectsController;
    [Tooltip("ステージセレクトイメージコントローラ"), SerializeField] StageSelectImageController stageSelectImageController;
    [Tooltip("ステージイメージコントローラ"), SerializeField] StageImagesController stageImagesController;
    [Tooltip("カーソルイメージコントローラ"), SerializeField] StageSelectCursorController stageSelectCursorController;

    [Header("数値設定"), Tooltip("ステージイメージオブジェクト移動位置"), SerializeField]
    Vector3[] stageImagePositions;
    [Tooltip("セレクトイメージ初期位置"), SerializeField] Vector3 initialPos;
    [Tooltip("ワールドイメージオブジェクト移動位置"), SerializeField] Vector3[] worldImageObjectPositions;
    [Tooltip("ステート毎の選択ワールドの移動位置"), SerializeField] Vector3[] stateChangeWorldImagePosition;


    // Start is called before the first frame update
    void Start()
    {
        // セレクト中のステージ---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingStage.Subscribe(image => { stageSelectImageController.MoveImagePosition(image); })
            .AddTo(this);

        // セレクト中のワールド---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingWorld.Subscribe(worldCount =>
        {
            worldObjectsController.SetObjectPosition(worldCount, worldImageObjectPositions[0]);
            worldObjectsController.SetObjectPosition(worldCount - 1, worldImageObjectPositions[1]);
            worldObjectsController.SetObjectPosition(worldCount + 1, worldImageObjectPositions[2]);

            worldObjectsController.SetCurrentSelectWorldObject(worldCount);
        }).AddTo(this);

        // ステージセレクトシーン内でのステート---------------------------------------------------------------------------------------------
        stageSelectManager.StageState.Subscribe(state =>
        {
            switch (state)
            {
                case StageState.WorldSelect:
                    // セレクトイメージ
                    stageSelectImageController.Disable();
                    stageSelectImageController.ResetImagePosition(initialPos);

                    // ワールドイメージ
                    worldObjectsController.SetStageSelectStatePosition(stateChangeWorldImagePosition[0]);
                    break;
                case StageState.StageSelect:
                    // セレクトイメージ
                    stageSelectImageController.Enable();
                    stageSelectImageController.RestartZoom();

                    // ワールドイメージ
                    worldObjectsController.SetStageSelectStatePosition(stateChangeWorldImagePosition[1]);

                    // カーソルイメージ
                    stageSelectCursorController.HideCursorImage();
                    break;
            }

            stageImagesController.SetPosition(stageImagePositions[(int)state]);
        }).AddTo(this);

        // ワールドの選択数ハンドル---------------------------------------------------------------------------------------------
        stageSelectManager.worldSelectMinHundle.Subscribe(_ => stageSelectCursorController.HideLeftCursorImage());
        stageSelectManager.worldSelectMaxHundle.Subscribe(_ => stageSelectCursorController.HideRightCursorImage());
        stageSelectManager.worldSelectNotMaxorMinHundle.Subscribe(_ => stageSelectCursorController.VisibleCursorImage());
    }
}
