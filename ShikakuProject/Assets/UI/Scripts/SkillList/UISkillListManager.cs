using DG.Tweening;
using UnityEngine;

// �X�L��UI�̃}�l�[�W���[
public class UISkillListManager : MonoBehaviour
{
    [SerializeField]
    private UISkillListPrefabData _skillImagePrefab;

    [SerializeField]
    private RectTransform _guideImageLPrefab;

    [SerializeField]
    private RectTransform _guideImageRPrefab;


    [SerializeField]
    private Transform _skillParentTransform;

    [SerializeField]
    private Transform _selectPoint;

    [SerializeField]
    private Transform _activePoint;

    [SerializeField]
    private Transform _notActivePoint;

    [SerializeField]
    private int _spaceBetweenDisplays = 0;



    private UISkillListPrefabData[] _skillImages;

    private readonly Transform[] _guideImages = new Transform[2];

    private int _selectSkill = -1;


    public void OnStart(Sprite[] textures)
    {
        OnSpawnUI(textures);

        OnStartSelected();
    }

    private void OnSpawnUI(Sprite[] textures)
    {
        int generatedWidth = _spaceBetweenDisplays + _skillImagePrefab.CardWidth;

        float generatedStandardWidth = generatedWidth * (textures.Length - 1) / 2;


        _skillImages = new UISkillListPrefabData[textures.Length];


        for (int i = 0; i < textures.Length; ++i)
        {
            GameObject spawnObj = Instantiate(_skillImagePrefab.gameObject, transform.position, Quaternion.identity);

            spawnObj.transform.SetParent(_skillParentTransform);

            spawnObj.transform.localScale = Vector3.one;


            // �|�W�V�����ݒ�
            Vector2 spawnPosition = Vector2.zero;

            spawnPosition.x = generatedWidth * i - generatedStandardWidth;

            spawnPosition.y = _notActivePoint.position.y;

            spawnObj.transform.localPosition = spawnPosition;


            _skillImages[i] = spawnObj.GetComponent<UISkillListPrefabData>();

            // �e�N�X�`����ݒ�
            _skillImages[i].SetSprite(textures[i]);

        }

        // 0���I�����ꂽ��Ԃɂ���
        SelectSkill(0);


        // �K�C�h�p��UI����������
        GameObject guideObjL = Instantiate(_guideImageLPrefab.gameObject, transform.position, Quaternion.identity);
        GameObject guideObjR = Instantiate(_guideImageRPrefab.gameObject, transform.position, Quaternion.identity);

        guideObjL.transform.SetParent(_skillParentTransform);
        guideObjR.transform.SetParent(_skillParentTransform);

        guideObjL.transform.localScale = Vector3.one;
        guideObjR.transform.localScale = Vector3.one;

        // �|�W�V�����ݒ�
        Vector2 guideSpawnPosition = Vector2.zero;
        guideSpawnPosition.y = _notActivePoint.position.y;

        guideSpawnPosition.x = -generatedWidth - generatedStandardWidth;
        guideObjL.transform.localPosition = guideSpawnPosition;

        guideSpawnPosition.x = generatedWidth * textures.Length - generatedStandardWidth;
        guideObjR.transform.localPosition = guideSpawnPosition;

        _guideImages[0] = guideObjL.transform;
        _guideImages[1] = guideObjR.transform;
    }

    //---------------------------------------------------------------------------------------------
    // �I����Ԃ�ύX����
    public void SelectSkill(int selectType)
    {
        if (selectType < 0 || _skillImages.Length <= selectType) return;

        // �I������Ă�����
        if (0 <= _selectSkill && _selectSkill < _skillImages.Length)
        {
            _skillImages[_selectSkill].ImageTransform.DOMoveY(_activePoint.position.y, 0.3f);
        }

        // �I�����ꂽ����
        _skillImages[selectType].ImageTransform.DOPause();
        _skillImages[selectType].ImageTransform.DOMoveY(_selectPoint.position.y, 0.3f);

        _selectSkill = selectType;
    }

    //---------------------------------------------------------------------------------------------
    // �N�[���^�C����\������
    [SerializeField]
    private Color _selectBackColor;

    [SerializeField]
    private Color _notSelectBackColor;

    private bool[] _coolTime;

    private void OnStartSelected()
    {
        _coolTime = new bool[_skillImages.Length];

        for (int i = 0; i < _coolTime.Length; ++i)
            _coolTime[i] = true;
    }

    public void DisplayCooldown(int type, float cooldown)
    {
        _skillImages[type].SetCooldown(cooldown);

        if (cooldown == 0)
        {
            // �g�����ԂɂȂ����ꍇ

            if (!_coolTime[type]) return;

            _coolTime[type] = false;

            _skillImages[type].BackImage.DOColor(_selectBackColor, 0.5f).SetEase(Ease.InOutElastic);
        }
        else
        {
            // �g���Ȃ���ԂɂȂ����ꍇ

            if (_coolTime[type]) return;

            _coolTime[type] = true;

            _skillImages[type].BackImage.DOColor(_notSelectBackColor, 0.5f).SetEase(Ease.OutElastic);
        }
    }

    //---------------------------------------------------------------------------------------------
    // �\���A��\�����Ǘ�����

    public void SetActiveUI(bool isActive)
    {
        Transform standardPoint = isActive ? _activePoint : _notActivePoint;

        for (int i = 0; i < _skillImages.Length; ++i)
        {
            _skillImages[i].ImageTransform.DOMoveY(standardPoint.position.y, 0.5f);
        }

        for(int i=0;i<_guideImages.Length;++i)
        {
            _guideImages[i].DOMoveY(standardPoint.position.y, 0.5f);
        }
    }
}