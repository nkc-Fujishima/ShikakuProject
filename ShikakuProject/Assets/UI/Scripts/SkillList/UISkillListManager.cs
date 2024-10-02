using DG.Tweening;
using UnityEngine;

// �X�L��UI�̃}�l�[�W���[
public class UISkillListManager : MonoBehaviour
{
    [SerializeField]
    private UISkillListPrefabData _skillImagePrefab;

    [SerializeField]
    private Transform _skillParentTransform;

    [SerializeField]
    private int _spaceBetweenDisplays = 0;


    private UISkillListPrefabData[] _skillImageList;

    private int _selectSkill = -1;


    public void OnStart(Sprite[] textures)
    {
        OnSpawnUI(textures);
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


            // �|�W�V�����ݒ�
            Vector3 spawnPosition = _skillParentTransform.position;

            spawnPosition.x = generatedWidth * i - generatedStandardWidth;

            spawnObj.transform.localPosition = spawnPosition;


            _skillImageList[i] = spawnObj.GetComponent<UISkillListPrefabData>();

            // �e�N�X�`����ݒ�
            _skillImageList[i].SetSprite(textures[i]);

        }

        // 0���I�����ꂽ��Ԃɂ���
        SelectSkill(0);
    }

    //---------------------------------------------------------------------------------------------
    // �I����Ԃ�ύX����
    public void SelectSkill(int selectType)
    {
        if (selectType < 0 || _skillImageList.Length <= selectType) return;


        if (0 <= _selectSkill && _selectSkill < _skillImageList.Length)
        {
            _skillImageList[_selectSkill].ImageTransform.DOMove(_skillImageList[_selectSkill].GetNomalPoint, 0.3f);
        }


        _skillImageList[selectType].ImageTransform.DOMove(_skillImageList[selectType].GetSelectPoint, 0.3f);

        _selectSkill = selectType;
    }

    //---------------------------------------------------------------------------------------------
    // �N�[���^�C����\������
    public void DisplayCooldown(int type, float cooldown)
    {
        _skillImageList[type].SetCooldown(cooldown);

    }
}