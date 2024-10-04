using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour, IStateChangeable
{
    [Header("オブジェクト設定")]
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject stageSelectUI;

    [Tooltip("ステージ画像オブジェクト"), SerializeField] List<Sprite> stageImages;
    [Tooltip("stageImageにセットする画像"), SerializeField] List<Image> imageObjects;

    [Tooltip("ステージセレクト数管理スクリプタブルオブジェクト"), SerializeField]
    StageSelectData stageSelectData;

    StateManager manager = null;

    IState iState = null;

    PlayerInput playerInput = null;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        manager = new StateManager(titleUI, stageSelectUI, stageImages, playerInput, this, stageSelectData);

        for (int i = 0; i < imageObjects.Count; i++)
        {
            imageObjects[i].sprite = stageImages[i];
        }

        ChangeState(manager.titleState);
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
        public StageSelect stageSelectState = null;


        public StateManager(GameObject titleUI, GameObject stageSelectUI, List<Sprite> images, PlayerInput playerInput, IStateChangeable stateChanger, StageSelectData stageSelectData)
        {
            this.titleState = new Title(titleUI, stateChanger, playerInput, this);
            this.stageSelectState = new StageSelect(stageSelectUI, images, stateChanger, playerInput, this, stageSelectData);
        }
    }

    class Title : IState
    {
        GameObject titleUI = null;
        IStateChangeable stateChanger = null;
        PlayerInput playerInput = null;

        StateManager manager = null;

        public Title(GameObject titleUI, IStateChangeable stateChanger, PlayerInput playerInput, StateManager manager)
        {
            this.titleUI = titleUI;
            this.stateChanger = stateChanger;
            this.playerInput = playerInput;
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

        public void OnUpdate()
        {

            if (playerInput.actions["Dicision"].WasPerformedThisFrame())
            {
                stateChanger.ChangeState(manager.stageSelectState);
            }
        }
    }

    class StageSelect : IState
    {
        GameObject stageSelectUI = null;

        List<Sprite> images = null;

        IStateChangeable stateChanger = null;

        PlayerInput playerInput = null;

        StateManager manager = null;

        StageSelectData stageSelectData = null;

        public StageSelect(GameObject stageSelectUI, List<Sprite> images, IStateChangeable stateChanger, PlayerInput playerInput, StateManager manager, StageSelectData stageSelectData)
        {
            this.stageSelectUI = stageSelectUI;
            this.images = images;
            this.stateChanger = stateChanger;
            this.playerInput = playerInput;
            this.manager = manager;
            this.stageSelectData = stageSelectData;
        }

        public void OnEnter()
        {
            stageSelectUI.SetActive(true);
        }

        public void OnExit()
        {
            stageSelectUI.SetActive(false);
        }

        public void OnUpdate()
        {
            Vector2 inputValue = playerInput.actions["Select"].ReadValue<Vector2>();

            if (playerInput.actions["Select"].WasPressedThisFrame() && inputValue.y > 0.8)
            {
                stageSelectData.StageSelectNumber += 1;

                // ステージ画像より多い場合、セレクトカウントを0に戻す
                if (stageSelectData.StageSelectNumber > images.Count - 1)
                    stageSelectData.StageSelectNumber = 0;

                Debug.Log(stageSelectData.StageSelectNumber);
            }
            if (playerInput.actions["Select"].WasPressedThisFrame() && inputValue.y < -0.8)
            {
                stageSelectData.StageSelectNumber -= 1;

                // ステージ画像より多い場合、セレクトカウントを0に戻す
                if (stageSelectData.StageSelectNumber < 0)
                    stageSelectData.StageSelectNumber = images.Count - 1;

                Debug.Log(stageSelectData.StageSelectNumber);
            }

            if (playerInput.actions["Cancel"].WasPerformedThisFrame())
                stateChanger.ChangeState(manager.titleState);
        }
    }
}
