using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour, IStateChangeable
{
    [Header("オブジェクト設定")]
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject stageSelectUI;

    [Tooltip("ステージ画像オブジェクト"), SerializeField] List<Sprite> stageImages;
    [Tooltip("stageImageにセットする画像"), SerializeField] List<Image> imageObjects;
    [Tooltip("セレクト中イメージ画像"), SerializeField] Image selectImage;
    [Tooltip("フェード用イメージ画像"), SerializeField] SceneChangeShaderController fadeController;

    [Tooltip("ステージセレクト数管理スクリプタブルオブジェクト"), SerializeField]
    StageSelectData stageSelectData;

    StateManager manager = null;

    IState iState = null;

    PlayerInput playerInput = null;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        manager = new StateManager(titleUI, playerInput, this, fadeController);

        for (int i = 0; i < imageObjects.Count; i++)
        {
            imageObjects[i].sprite = stageImages[i];
        }

        ChangeState(manager.titleState);

        fadeController.SetUp();
        fadeController.SetFadeValueMin();
    }

    public void ChangeState(IState nextState)
    {
        if (iState != null) iState.OnExit();

        iState = nextState;

        if (iState != null) iState.OnEnter();
    }

    // Update is called once per frame
    void Update()
    {
        iState.OnUpdate();
    }

    class StateManager
    {
        public Title titleState = null;
        public Fading fadingState = null;


        public StateManager(GameObject titleUI, PlayerInput playerInput, IStateChangeable stateChanger, SceneChangeShaderController fadeController)
        {
            this.titleState = new Title(titleUI, stateChanger, playerInput, fadeController, this);
            this.fadingState = new Fading();
        }
    }

    class Title : IState
    {
        GameObject titleUI = null;
        IStateChangeable stateChanger = null;
        PlayerInput playerInput = null;

        SceneChangeShaderController fadeController = null;

        StateManager manager = null;

        public Title(GameObject titleUI, IStateChangeable stateChanger, PlayerInput playerInput, SceneChangeShaderController fadeController, StateManager manager)
        {
            this.titleUI = titleUI;
            this.stateChanger = stateChanger;
            this.playerInput = playerInput;
            this.fadeController = fadeController;
            this.manager = manager;
        }

        public void OnEnter()
        {
            titleUI.SetActive(true);
        }

        public void OnExit()
        {
            titleUI.SetActive(false);
        }

        public async void OnUpdate()
        {
            if (playerInput.actions["Dicision"].WasPerformedThisFrame())
            {
                stateChanger.ChangeState(manager.fadingState);

                await fadeController.FadeOut();

                SceneManager.LoadScene("StageSelect");
            }
        }
    }

    class Fading : IState
    {
        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnUpdate()
        {
        }
    }
}
