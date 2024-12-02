using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour, IStateChangeable
{
    [Header("�I�u�W�F�N�g�ݒ�")]
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject stageSelectUI;
    [Tooltip("SE�p�I�u�W�F�N�g"), SerializeField] AudioSource seObject;

    [Tooltip("�t�F�[�h�p�C���[�W�摜"), SerializeField] SceneChangeShaderController fadeController;


    StateManager manager = null;

    IState iState = null;

    PlayerInput playerInput = null;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        manager = new StateManager(titleUI, playerInput, this, fadeController, seObject);

        fadeController.SetUp();
        fadeController.SetFadeValueMin();

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

        // �o����s����o�^
        public async void OnEnter()
        {
            titleUI.SetActive(true);

            await fadeController.FadeIn();

            playerInput.actions["Dicision"].started += ToStageSelect;
            playerInput.actions["Cancel"].started += QuitGame;
        }

        public void OnExit()
        {
            titleUI.SetActive(false);
        }

        public void OnUpdate()
        {
            
        }

        // �o�^�����s�����폜���A�X�e�[�W�Z���N�g��
        private async void ToStageSelect(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            playerInput.actions["Dicision"].started -= ToStageSelect;
            playerInput.actions["Cancel"].started -= QuitGame;

            stateChanger.ChangeState(manager.fadingState);
            seObject.Play();

            await fadeController.FadeOut();

            SceneManager.LoadScene("StageSelect");
        }

        // �o�^�����s�����폜���A�Q�[���I��
        private async void QuitGame(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            playerInput.actions["Dicision"].started -= ToStageSelect;
            playerInput.actions["Cancel"].started -= QuitGame;

            stateChanger.ChangeState(manager.fadingState);
            seObject.Play();

            await fadeController.FadeOut();

            Application.Quit();
        }
    }

    // �t�F�[�h���ɑ��̍s�����o���Ȃ��悤�ɂ��邽�߂̖��̃N���X
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
