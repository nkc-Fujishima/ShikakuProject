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
    [Tooltip("SE用オブジェクト"), SerializeField] AudioSource seObject;

    [Tooltip("フェード用イメージ画像"), SerializeField] SceneChangeShaderController fadeController;


    StateManager manager = null;

    IState iState = null;

    PlayerInput playerInput = null;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        manager = new StateManager(titleUI, playerInput, this, fadeController, seObject);

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


        public StateManager(GameObject titleUI, PlayerInput playerInput, IStateChangeable stateChanger, SceneChangeShaderController fadeController, AudioSource seObject)
        {
            this.titleState = new Title(titleUI, stateChanger, playerInput, fadeController, this, seObject);
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

        AudioSource seObject = null;

        public Title(GameObject titleUI, IStateChangeable stateChanger, PlayerInput playerInput, SceneChangeShaderController fadeController, StateManager manager, AudioSource seObject)
        {
            this.titleUI = titleUI;
            this.stateChanger = stateChanger;
            this.playerInput = playerInput;
            this.fadeController = fadeController;
            this.manager = manager;
            this.seObject = seObject;
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
                seObject.Play();

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
