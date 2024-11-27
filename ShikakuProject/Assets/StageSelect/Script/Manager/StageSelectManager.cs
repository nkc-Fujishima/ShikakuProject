using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using R3;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Unity.VisualScripting;


public class StageSelectManager : MonoBehaviour, IStateChangeable
{
    [Header("オブジェクト設定"), Tooltip("ワールドイメージのパラメータ"), SerializeField] WorldImageObjectParameter parameter;
    [Tooltip("ステージ選択数値オブジェクト"), SerializeField] StageSelectData selectData;
    [Tooltip("ステージイメージオブジェクト"), SerializeField] List<Image> stageImageObjects = new List<Image>();
    [Tooltip("ステージイメージデータ"), SerializeField] List<StageImageData> stageImageData = new List<StageImageData>();
    [Tooltip("フェード用イメージ画像"), SerializeField] SceneChangeShaderController fadeController;

    [Header("数値設定"), Tooltip("ワールド選択可能数"), SerializeField] int worldObjects;

    public ReactiveProperty<Image> SelectingStage = new ReactiveProperty<Image>();
    public ReactiveProperty<int> SelectingWorld = new ReactiveProperty<int>(0);
    public ReactiveProperty<StageState> StageState = new ReactiveProperty<StageState>();
    public Subject<R3.Unit> worldSelectMinHundle = new Subject<R3.Unit>();
    public Subject<R3.Unit> worldSelectMaxHundle = new Subject<R3.Unit>();
    public Subject<R3.Unit> worldSelectNotMaxorMinHundle = new Subject<R3.Unit>();

    PlayerInput playerInput = null;

    StageSelectStateManager stateManager = null;

    IState iState = null;

    // Start is called before the first frame update
    void Awake()
    {
        SetUp();

    }

    private void Update()
    {
        iState?.OnUpdate();


    }

    async void SetUp()
    {
        SelectingStage.Value = stageImageObjects[0];

        fadeController.SetUp();
        await fadeController.FadeIn();

        playerInput = GetComponent<PlayerInput>();

        stateManager = new StageSelectStateManager(this, playerInput, SelectingWorld, SelectingStage, StageState, worldObjects, stageImageObjects, stageImageData, selectData, worldSelectMinHundle, worldSelectMaxHundle, worldSelectNotMaxorMinHundle, fadeController);

        ChangeState(stateManager.WorldSelectState);

    }



    public void ChangeState(IState nextState)
    {
        if (iState != null) iState.OnExit();
        iState = nextState;
        if (iState != null) iState.OnEnter();
    }

    class StageSelectStateManager
    {
        public WorldSelect WorldSelectState = null;
        public StageSelect StageSelectState = null;

        SelectInformationHolder informationHolder = null;

        public StageSelectStateManager(IStateChangeable stateChanger, PlayerInput playerInput, ReactiveProperty<int> selectingWorld, ReactiveProperty<Image> selectingStage,
            ReactiveProperty<StageState> stageState, int worldObjects, List<Image> stageImageObjects, List<StageImageData> stageSelectData, StageSelectData selectData,
                 Subject<R3.Unit> worldSelectMinHundle, Subject<R3.Unit> worldSelectMaxHundle, Subject<R3.Unit> worldSelectNotMaxorMinHundle, SceneChangeShaderController fadeController)
        {
            informationHolder = new SelectInformationHolder();

            WorldSelectState = new WorldSelect(stateChanger, this, informationHolder, playerInput, selectingWorld, stageState, worldObjects, worldSelectMinHundle, worldSelectMaxHundle, worldSelectNotMaxorMinHundle);
            StageSelectState = new StageSelect(stateChanger, this, informationHolder, playerInput, selectingStage, stageState, stageImageObjects, stageSelectData, selectData, fadeController);
        }
    }

    class SelectInformationHolder
    {
        public int worldSelect = 0;
        public int stageSelect = 0;
    }

    // ワールドセレクトステート----------------------------------------------------------------------------------------
    class WorldSelect : StateBase
    {
        IStateChangeable stateChanger = null;

        StageSelectStateManager stateManager = null;

        SelectInformationHolder informationHolder = null;

        PlayerInput playerInput = null;

        ReactiveProperty<int> selectingWorld = null;
        ReactiveProperty<StageState> stageState = null;

        Subject<R3.Unit> worldSelectMinHundle = null;
        Subject<R3.Unit> worldSelectMaxHundle = null;
        Subject<R3.Unit> worldSelectNotMaxorMinHundle = null;

        int worldObjects = 0;

        int worldSelectCount = 0;

        const int waitTime = 400;

        public WorldSelect(IStateChangeable stateChanger, StageSelectStateManager stateManager, SelectInformationHolder informationHolder
            , PlayerInput playerInput, ReactiveProperty<int> selectingWorld, ReactiveProperty<StageState> stageState, int worldObjects
            , Subject<R3.Unit> worldSelectMinHundle, Subject<R3.Unit> worldSelectMaxHundle, Subject<R3.Unit> worldSelectNotMaxorMinHundle)
        {
            this.stateChanger = stateChanger;
            this.stateManager = stateManager;
            this.informationHolder = informationHolder;
            this.playerInput = playerInput;
            this.selectingWorld = selectingWorld;
            this.stageState = stageState;
            this.worldObjects = worldObjects;
            this.worldSelectMinHundle = worldSelectMinHundle;
            this.worldSelectMaxHundle = worldSelectMaxHundle;
            this.worldSelectNotMaxorMinHundle = worldSelectNotMaxorMinHundle;
        }

        public override async void OnEnter()
        {
            stageState.Value = global::StageState.WorldSelect;
            InvokeWorldSelectHundle();

            await UniTask.Delay(waitTime);

            playerInput.actions["Select"].started += SelectWorld;
            playerInput.actions["Dicision"].started += WorldDicision;
        }

        public override void OnExit()
        {
            playerInput.actions["Select"].started -= SelectWorld;
            playerInput.actions["Dicision"].started -= WorldDicision;
        }

        public override void OnUpdate()
        {
        }

        void SelectWorld(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            if (context.ReadValue<Vector2>().x > 0)
            {
                informationHolder.worldSelect += 1;
                if (informationHolder.worldSelect > worldObjects - 1) informationHolder.worldSelect = worldObjects - 1;
            }

            else if (context.ReadValue<Vector2>().x < 0)
            {
                informationHolder.worldSelect -= 1;
                if (informationHolder.worldSelect < 0) informationHolder.worldSelect = 0;
            }

            InvokeWorldSelectHundle();

            selectingWorld.Value = informationHolder.worldSelect;
        }

        void WorldDicision(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            stateChanger.ChangeState(stateManager.StageSelectState);
        }

        void InvokeWorldSelectHundle()
        {
            // 一番端のステージではない場合
            worldSelectNotMaxorMinHundle?.OnNext(R3.Unit.Default);

            // 一番始めのワールドを選択した場合
            if (informationHolder.worldSelect == 0)
            {
                worldSelectMinHundle?.OnNext(R3.Unit.Default);
                return;
            }

            // 一番最後のワールドを選択した場合
            if (informationHolder.worldSelect == worldObjects - 1)
            {
                worldSelectMaxHundle?.OnNext(R3.Unit.Default);
                return;
            }
        }
    }





    // ステージセレクトステート----------------------------------------------------------------------------------------
    class StageSelect : StateBase
    {
        IStateChangeable stateChanger = null;

        StageSelectStateManager stateManager = null;

        SelectInformationHolder informationHolder = null;

        StageSelectData selectData = null;

        PlayerInput playerInput = null;

        SceneChangeShaderController fadeController = null;

        ReactiveProperty<Image> selectingImage = null;
        ReactiveProperty<StageState> stageState = null;

        List<StageImageData> stageImageData = null;
        List<Image> stageImageObjects = null;

        int stageSelectCount = 0;

        const int waitTime = 400;

        public StageSelect(IStateChangeable stateChanger, StageSelectStateManager stateManager, SelectInformationHolder informationHolder
            , PlayerInput playerInput, ReactiveProperty<Image> selectingImage, ReactiveProperty<StageState> stageState, List<Image> stageImageObjects, List<StageImageData> stageImageData, StageSelectData selectData
            , SceneChangeShaderController fadeController)
        {
            this.stateChanger = stateChanger;
            this.stateManager = stateManager;
            this.informationHolder = informationHolder;
            this.playerInput = playerInput;
            this.selectingImage = selectingImage;
            this.stageState = stageState;
            this.stageImageObjects = stageImageObjects;
            this.stageImageData = stageImageData;
            this.selectData = selectData;
            this.fadeController = fadeController;
        }

        public override async void OnEnter()
        {
            stageState.Value = global::StageState.StageSelect;

            for (int i = 0; i < stageImageData[informationHolder.worldSelect].stageImages.Length; i++)
            {
                stageImageObjects[i].color = Color.white;
                stageImageObjects[i].sprite = stageImageData[informationHolder.worldSelect].stageImages[i];
            }

            await UniTask.Delay(waitTime);

            playerInput.actions["Select"].started += SelectStage;
            playerInput.actions["Dicision"].started += StageDicision;
            playerInput.actions["Cancel"].started += StageCancel;

            selectingImage.Value = stageImageObjects[informationHolder.stageSelect];
        }

        public override async void OnExit()
        {
            playerInput.actions["Select"].started -= SelectStage;
            playerInput.actions["Dicision"].started -= StageDicision;
            playerInput.actions["Cancel"].started -= StageCancel;


            await UniTask.Delay(waitTime);

            foreach (var image in stageImageObjects)
            {
                image.color = new Color(1, 1, 1, 0);
            }

            informationHolder.stageSelect = 0;
        }

        public override void OnUpdate()
        {
        }

        void SelectStage(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            if (context.ReadValue<Vector2>().x > 0)
            {
                informationHolder.stageSelect += 1;
                if (informationHolder.stageSelect > stageImageData[informationHolder.worldSelect].stageImages.Length - 1) informationHolder.stageSelect = 0;
            }
            else if (context.ReadValue<Vector2>().x < 0)
            {
                informationHolder.stageSelect -= 1;
                if (informationHolder.stageSelect < 0) informationHolder.stageSelect = stageImageData[informationHolder.worldSelect].stageImages.Length - 1;
            }

            selectingImage.Value = stageImageObjects[informationHolder.stageSelect];
        }

        async void StageDicision(InputAction.CallbackContext context)
        {
            int selectNumber = 0;

            for (int i = 0; i < informationHolder.worldSelect; i++)
            {
                selectNumber += stageImageData[i].stageImages.Length;
            }

            selectNumber += informationHolder.stageSelect;

            selectData.StageSelectNumber = selectNumber;

            playerInput.actions["Select"].started -= SelectStage;
            playerInput.actions["Dicision"].started -= StageDicision;
            playerInput.actions["Cancel"].started -= StageCancel;

            await fadeController.FadeOut();

            SceneManager.LoadScene("GameScene");
        }

        void StageCancel(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            stateChanger.ChangeState(stateManager.WorldSelectState);

        }
    }
}
