using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using R3;
using Cysharp.Threading.Tasks;


public class StageSelectManager : MonoBehaviour, IStateChangeable
{
    [Header("�I�u�W�F�N�g�ݒ�")]
    [Tooltip("�X�e�[�W�I�𐔒l�I�u�W�F�N�g"), SerializeField] StageSelectData selectData;
    [Tooltip("�X�e�[�W�C���[�W�I�u�W�F�N�g"), SerializeField] List<Image> stageImageObjects = new List<Image>();
    [Tooltip("�X�e�[�W�C���[�W�f�[�^"), SerializeField] List<StageImageData> stageImageData = new List<StageImageData>();
    [Tooltip("�t�F�[�h�p�C���[�W�摜"), SerializeField] SceneChangeShaderController fadeController;
    [Tooltip("SE�R���g���[���[�I�u�W�F�N�g"), SerializeField] SEController seController;

    [Header("���l�ݒ�"), Tooltip("���[���h�I���\��"), SerializeField] int worldObjects;

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

        stateManager = new StageSelectStateManager(this, playerInput, SelectingWorld, SelectingStage, StageState, worldObjects, stageImageObjects, stageImageData, selectData, worldSelectMinHundle, worldSelectMaxHundle, worldSelectNotMaxorMinHundle, fadeController, seController);

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
                 Subject<R3.Unit> worldSelectMinHundle, Subject<R3.Unit> worldSelectMaxHundle, Subject<R3.Unit> worldSelectNotMaxorMinHundle, SceneChangeShaderController fadeController, SEController seController)
        {
            informationHolder = new SelectInformationHolder();

            WorldSelectState = new WorldSelect(stateChanger, this, informationHolder, playerInput, selectingWorld, stageState, worldObjects, worldSelectMinHundle, worldSelectMaxHundle, worldSelectNotMaxorMinHundle, fadeController, seController);
            StageSelectState = new StageSelect(stateChanger, this, informationHolder, playerInput, selectingStage, stageState, stageImageObjects, stageSelectData, selectData, fadeController, seController);
        }
    }

    class SelectInformationHolder
    {
        public int WorldSelect = 0;
        public int StageSelect = 0;
    }

    // ���[���h�Z���N�g�X�e�[�g----------------------------------------------------------------------------------------
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

        SceneChangeShaderController fadeController = null;
        SEController seController = null;

        int worldObjects = 0;

        int worldSelectCount = 0;

        const int waitTime = 400;

        public WorldSelect(IStateChangeable stateChanger, StageSelectStateManager stateManager, SelectInformationHolder informationHolder
            , PlayerInput playerInput, ReactiveProperty<int> selectingWorld, ReactiveProperty<StageState> stageState, int worldObjects
            , Subject<R3.Unit> worldSelectMinHundle, Subject<R3.Unit> worldSelectMaxHundle, Subject<R3.Unit> worldSelectNotMaxorMinHundle
            , SceneChangeShaderController fadeController, SEController seController)
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
            this.fadeController = fadeController;
            this.seController = seController;
        }

        public override async void OnEnter()
        {
            stageState.Value = global::StageState.WorldSelect;
            InvokeWorldSelectHundle();

            await UniTask.Delay(waitTime);

            playerInput.actions["Select"].started += SelectWorld;
            playerInput.actions["Cancel"].started += ReturnTitle;
            playerInput.actions["Dicision"].started += WorldDicision;
        }

        public override void OnExit()
        {
            playerInput.actions["Select"].started -= SelectWorld;
            playerInput.actions["Cancel"].started -= ReturnTitle;
            playerInput.actions["Dicision"].started -= WorldDicision;
        }

        public override void OnUpdate()
        {
        }

        // ���E�{�^���������A���[���h�I��
        void SelectWorld(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            if (context.ReadValue<Vector2>().x > 0)
            {
                if (informationHolder.WorldSelect == worldObjects - 1) return;
                informationHolder.WorldSelect += 1;
                seController.RingSelectSE();
            }

            else if (context.ReadValue<Vector2>().x < 0)
            {
                if (informationHolder.WorldSelect == 0) return;
                informationHolder.WorldSelect -= 1;
                seController.RingSelectSE();
            }

            InvokeWorldSelectHundle();

            selectingWorld.Value = informationHolder.WorldSelect;
        }

        void WorldDicision(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            seController.RingWorldDicisionSE();
            stateChanger.ChangeState(stateManager.StageSelectState);
        }

        void InvokeWorldSelectHundle()
        {
            // ��Ԏn�߂̃��[���h��I�������ꍇ
            if (informationHolder.WorldSelect == 0)
            {
                worldSelectMinHundle?.OnNext(R3.Unit.Default);
                return;
            }

            // ��ԍŌ�̃��[���h��I�������ꍇ
            if (informationHolder.WorldSelect == worldObjects - 1)
            {
                worldSelectMaxHundle?.OnNext(R3.Unit.Default);
                return;
            }

            // ��Ԓ[�̃X�e�[�W�ł͂Ȃ��ꍇ
            worldSelectNotMaxorMinHundle?.OnNext(R3.Unit.Default);
        }

        private async void ReturnTitle(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            playerInput.actions["Select"].started -= SelectWorld;
            playerInput.actions["Cancel"].started -= ReturnTitle;
            playerInput.actions["Dicision"].started -= WorldDicision;

            seController.RingStageDicisionSE();

            await fadeController.FadeOut();

            SceneManager.LoadScene("TitleScene");
        }
    }





    // �X�e�[�W�Z���N�g�X�e�[�g----------------------------------------------------------------------------------------
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

        SEController seController = null;

        int stageSelectCount = 0;

        const int waitTime = 400;

        const float deadZone = 0.01f;
        public StageSelect(IStateChangeable stateChanger, StageSelectStateManager stateManager, SelectInformationHolder informationHolder
            , PlayerInput playerInput, ReactiveProperty<Image> selectingImage, ReactiveProperty<StageState> stageState, List<Image> stageImageObjects, List<StageImageData> stageImageData, StageSelectData selectData
            , SceneChangeShaderController fadeController, SEController seController)
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
            this.seController = seController;
        }

        public override async void OnEnter()
        {
            stageState.Value = global::StageState.StageSelect;

            for (int i = 0; i < stageImageData[informationHolder.WorldSelect].stageImages.Length; i++)
            {
                stageImageObjects[i].color = Color.white;
                stageImageObjects[i].sprite = stageImageData[informationHolder.WorldSelect].stageImages[i];
            }

            await UniTask.Delay(waitTime);

            playerInput.actions["Select"].started += SelectStage;
            playerInput.actions["Dicision"].started += StageDicision;
            playerInput.actions["Cancel"].started += StageCancel;

            selectingImage.Value = stageImageObjects[informationHolder.StageSelect];
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

            informationHolder.StageSelect = 0;
        }

        public override void OnUpdate()
        {
        }

        // ���E�㉺�{�^���������I�𒆂̃X�e�[�W���̕ύX
        void SelectStage(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            if (context.ReadValue<Vector2>().y < -deadZone)
            {
                informationHolder.StageSelect += 2;
                if (informationHolder.StageSelect > stageImageData[informationHolder.WorldSelect].stageImages.Length - 1)
                    informationHolder.StageSelect -= stageImageData[informationHolder.WorldSelect].stageImages.Length;
            }

            else if (context.ReadValue<Vector2>().y > deadZone)
            {
                informationHolder.StageSelect -= 2;
                if (informationHolder.StageSelect < 0)
                    informationHolder.StageSelect += stageImageData[informationHolder.WorldSelect].stageImages.Length;
            }

            else if (context.ReadValue<Vector2>().x > deadZone)
            {
                informationHolder.StageSelect += 1;
                if (informationHolder.StageSelect > stageImageData[informationHolder.WorldSelect].stageImages.Length - 1) informationHolder.StageSelect = 0;
            }
            else if (context.ReadValue<Vector2>().x < -deadZone)
            {
                informationHolder.StageSelect -= 1;
                if (informationHolder.StageSelect < 0) informationHolder.StageSelect = stageImageData[informationHolder.WorldSelect].stageImages.Length - 1;
            }

            seController.RingSelectSE();

            selectingImage.Value = stageImageObjects[informationHolder.StageSelect];
        }

        async void StageDicision(InputAction.CallbackContext context)
        {
            int selectNumber = 0;

            for (int i = 0; i < informationHolder.WorldSelect; i++)
            {
                selectNumber += stageImageData[i].stageImages.Length;
            }

            selectNumber += informationHolder.StageSelect;

            selectData.StageSelectNumber = selectNumber;

            playerInput.actions["Select"].started -= SelectStage;
            playerInput.actions["Dicision"].started -= StageDicision;
            playerInput.actions["Cancel"].started -= StageCancel;

            seController.RingStageDicisionSE();

            await fadeController.FadeOut();

            SceneManager.LoadScene("GameScene");
        }

        void StageCancel(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            seController.RingCancelSE();
            stateChanger.ChangeState(stateManager.WorldSelectState);

        }
    }
}
