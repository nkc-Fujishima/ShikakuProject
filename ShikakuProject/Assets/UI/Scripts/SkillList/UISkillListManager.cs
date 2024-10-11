using DG.Tweening;
using UnityEngine;

// スキルUIのマネージャー
public class UISkillListManager : MonoBehaviour
{
    [SerializeField]
    private UISkillListPrefabData _skillImagePrefab;

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



    private UISkillListPrefabData[] _skillImageList;

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


        _skillImageList = new UISkillListPrefabData[textures.Length];


        for (int i = 0; i < textures.Length; ++i)
        {
            GameObject spawnObj = Instantiate(_skillImagePrefab.gameObject, transform.position, Quaternion.identity);

            spawnObj.transform.SetParent(_skillParentTransform);

            spawnObj.transform.localScale = Vector3.one;


            // ポジション設定
            Vector2 spawnPosition = Vector2.zero;

            spawnPosition.x = generatedWidth * i - generatedStandardWidth;

            spawnPosition.y = _notActivePoint.position.y;

            spawnObj.transform.localPosition = spawnPosition;


            _skillImageList[i] = spawnObj.GetComponent<UISkillListPrefabData>();

            // テクスチャを設定
            _skillImageList[i].SetSprite(textures[i]);

        }

        // 0が選択された状態にする
        SelectSkill(0);
    }

    //---------------------------------------------------------------------------------------------
    // 選択状態を変更する
    public void SelectSkill(int selectType)
    {
        if (selectType < 0 || _skillImageList.Length <= selectType) return;

        // 選択されてた項目
        if (0 <= _selectSkill && _selectSkill < _skillImageList.Length)
        {
            _skillImageList[_selectSkill].ImageTransform.DOMoveY(_activePoint.position.y, 0.3f);
        }

        // 選択された項目
        _skillImageList[selectType].ImageTransform.DOPause();
        _skillImageList[selectType].ImageTransform.DOMoveY(_selectPoint.position.y, 0.3f);

        _selectSkill = selectType;
    }

    //---------------------------------------------------------------------------------------------
    // クールタイムを表示する
    [SerializeField]
    private Color _selectBackColor;

    [SerializeField]
    private Color _notSelectBackColor;

    private bool[] _coolTime;

    private void OnStartSelected()
    {
        _coolTime = new bool[_skillImageList.Length];

        for (int i = 0; i < _coolTime.Length; ++i)
            _coolTime[i] = true;
    }

    public void DisplayCooldown(int type, float cooldown)
    {
        _skillImageList[type].SetCooldown(cooldown);

        if (cooldown == 0)
        {
            // 使える状態になった場合

            if (!_coolTime[type]) return;

            _coolTime[type] = false;

            _skillImageList[type].BackImage.DOColor(_selectBackColor, 0.5f).SetEase(Ease.InOutElastic);
        }
        else
        {
            // 使えない状態になった場合

            if (_coolTime[type]) return;

            _coolTime[type] = true;

            _skillImageList[type].BackImage.DOColor(_notSelectBackColor, 0.5f).SetEase(Ease.OutElastic);
        }
    }

    //---------------------------------------------------------------------------------------------
    // 表示、非表示を管理する

    public void SetActiveUI(bool isActive)
    {
        Transform standardPoint = isActive ? _activePoint : _notActivePoint;

        for (int i = 0; i < _skillImageList.Length; ++i)
        {
            _skillImageList[i].ImageTransform.DOMoveY(standardPoint.position.y, 0.5f);
        }
    }
}