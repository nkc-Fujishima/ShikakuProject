using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public class StageSelectPresenter : MonoBehaviour
{
    [Header("オブジェクト設定"), Tooltip("ステージセレクトマネージャ"), SerializeField] StageSelectManager stageSelectManager;
    [Tooltip("ワールドオブジェクトコントローラ"), SerializeField] WorldObjectsController worldObjectsController;
    [Tooltip("ステージセレクトイメージコントローラ"), SerializeField] StageSelectImageController stageSelectImageController;
    [Tooltip("ステージイメージコントローラ"), SerializeField] StageImagesController stageImagesController;
    [Tooltip("カーソルイメージコントローラ"), SerializeField] StageSelectCursorController stageSelectCursorController;
    [Tooltip("インフォメーションコントローラ"),SerializeField] StageSelectInformationController informationController;

    // Start is called before the first frame update
    void Start()
    {
        // セレクト中のステージ---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingStage.Subscribe(image => { stageSelectImageController.MoveImagePosition(image); })
            .AddTo(this);

        // セレクト中のワールド---------------------------------------------------------------------------------------------
        stageSelectManager.SelectingWorld.Subscribe(worldCount =>
        {
            worldObjectsController.SetMainPosition(worldCount);
            worldObjectsController.SetPrevPosition(worldCount - 1);
            worldObjectsController.SetMainNextPosition(worldCount + 1);

            worldObjectsController.SetCurrentSelectWorldObject(worldCount);

            informationController.SetWorldName(worldCount);
        }).AddTo(this);

        // ステージセレクトシーン内でのステート---------------------------------------------------------------------------------------------
        stageSelectManager.StageState.Subscribe(state =>
        {
            switch (state)
            {
                case StageState.WorldSelect:
                    // セレクトイメージ
                    stageSelectImageController.Disable();
                    stageSelectImageController.ResetImagePosition();

                    // ワールドイメージ
                    worldObjectsController.SetWorldSelectStatePosition();

                    // ステージセレクトインフォメーション
                    informationController.SetWorldSelectStatePosition();
                    informationController.SetWorldSelectButtonInfo();
                    break;
                case StageState.StageSelect:
                    // セレクトイメージ
                    stageSelectImageController.Enable();
                    stageSelectImageController.RestartZoom();

                    // ワールドイメージ
                    worldObjectsController.SetStageSelectStatePosition();

                    // カーソルイメージ
                    stageSelectCursorController.HideCursorImage();

                    // ステージセレクトインフォメーション
                    informationController.SetStageSelectStatePosition();
                    informationController.SetStageSelectButtonInfo();
                    break;
            }

            stageImagesController.SetPosition((int)state);
        }).AddTo(this);

        // ワールドの選択数ハンドル---------------------------------------------------------------------------------------------
        stageSelectManager.worldSelectMinHundle.Subscribe(_ => stageSelectCursorController.HideLeftCursorImage());
        stageSelectManager.worldSelectMaxHundle.Subscribe(_ => stageSelectCursorController.HideRightCursorImage());
        stageSelectManager.worldSelectNotMaxorMinHundle.Subscribe(_ => stageSelectCursorController.VisibleCursorImage());
    }
}
