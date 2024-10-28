using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour, IStateChangeable
{
    [Header("�I�u�W�F�N�g�ݒ�")]
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject stageSelectUI;

    [Tooltip("�X�e�[�W�摜�I�u�W�F�N�g"), SerializeField] List<Sprite> stageImages;
    [Tooltip("stageImage�ɃZ�b�g����摜"), SerializeField] List<Image> imageObjects;
    [Tooltip("�Z���N�g���C���[�W�摜"), SerializeField] Image selectImage;

    [Tooltip("�X�e�[�W�Z���N�g���Ǘ��X�N���v�^�u���I�u�W�F�N�g"), SerializeField]
    StageSelectData stageSelectData;

    StateManager manager = null;

    IState iState = null;

    PlayerInput playerInput = null;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        manager = new StateManager(titleUI, stageSelectUI, imageObjects, playerInput, this, stageSelectData, selectImage);

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


        public StateManager(GameObject titleUI, GameObject stageSelectUI, List<Image> images, PlayerInput playerInput, IStateChangeable stateChanger, StageSelectData stageSelectData, Image selectImage)
        {
            this.titleState = new Title(titleUI, stateChanger, playerInput, this);
            this.stageSelectState = new StageSelect(stageSelectUI, images, stateChanger, playerInput, this, stageSelectData, selectImage);
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

        List<Image> images = null;

        IStateChangeable stateChanger = null;

        PlayerInput playerInput = null;

        StateManager manager = null;

        StageSelectData stageSelectData = null;

        Image selectImage = null;

        public StageSelect(GameObject stageSelectUI, List<Image> images, IStateChangeable stateChanger, PlayerInput playerInput, StateManager manager, StageSelectData stageSelectData, Image selectImage)
        {
            this.stageSelectUI = stageSelectUI;
            this.images = images;
            this.stateChanger = stateChanger;
            this.playerInput = playerInput;
            this.manager = manager;
            this.stageSelectData = stageSelectData;
            this.selectImage = selectImage;
        }

        public void OnEnter()
        {
            stageSelectUI.SetActive(true);
            SetSelectImage();
        }

        public void OnExit()
        {
            stageSelectUI.SetActive(false);
        }

        public void OnUpdate()
        {
            Vector2 inputValue = playerInput.actions["Select"].ReadValue<Vector2>();

            if (playerInput.actions["Select"].WasPressedThisFrame() && inputValue.x > 0.8)
            {
                stageSelectData.StageSelectNumber += 1;

                // �X�e�[�W�摜��葽���ꍇ�A�Z���N�g�J�E���g��0�ɖ߂�
                if (stageSelectData.StageSelectNumber > images.Count - 1)
                    stageSelectData.StageSelectNumber = 0;

                SetSelectImage();
            }
            if (playerInput.actions["Select"].WasPressedThisFrame() && inputValue.x < -0.8)
            {
                stageSelectData.StageSelectNumber -= 1;

                // �X�e�[�W�摜��葽���ꍇ�A�Z���N�g�J�E���g���X�e�[�W�摜���X�g�ő�l�ɂ���
                if (stageSelectData.StageSelectNumber < 0)
                    stageSelectData.StageSelectNumber = images.Count - 1;

                SetSelectImage();
            }

            if (playerInput.actions["Dicision"].WasPressedThisFrame())
            {
                SceneManager.LoadScene("GameScene");
            }

            if (playerInput.actions["Cancel"].WasPerformedThisFrame())
                stateChanger.ChangeState(manager.titleState);
        }

        private void SetSelectImage()
        {
            selectImage.rectTransform.position = images[stageSelectData.StageSelectNumber].rectTransform.position;
        }
    }
}
