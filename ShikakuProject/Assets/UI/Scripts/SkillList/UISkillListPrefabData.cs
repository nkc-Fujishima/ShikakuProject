using UnityEngine;
using UnityEngine.UI;

// �X�L��UI�̃J�[�h���̃v���n�u�̏��
public class UISkillListPrefabData : MonoBehaviour
{
    [Header("UI�𓮂����Ƃ��̖��[")]
    [SerializeField]
    private Transform _selectPoint;
    [SerializeField]
    private Transform _nomalPoint;

    [Header("�������Ώ�")]
    [SerializeField]
    private Transform _imageTransform;

    [Header("�X�L���̃C���[�W�摜")]
    [SerializeField]
    private Image _skillImage;

    [Header("�N�[���^�C���̉摜")]
    [SerializeField]
    private Image _filterImage;


    [Header("���̉摜�̐F��ύX")]
    [SerializeField]
    private Image _backImage;
    [SerializeField]
    private Color _selectBackColor;
    [SerializeField]
    private Color _notSelectBackColor;

    [Header("UI�̕�")]
    [SerializeField]
    private int _cardWidth;


    public Vector3 GetSelectPoint => _selectPoint.position;

    public Vector3 GetNomalPoint => _nomalPoint.position;

    public Transform ImageTransform => _imageTransform;

    public int CardWidth => _cardWidth;


    // �X�L���̃C���[�W�摜��ݒ�
    public void SetSprite(Sprite texture)
    {
        _skillImage.sprite = texture;
    }

    // �N�[���^�C����\��
    public void SetCooldown(float Range)
    {
        _filterImage.fillAmount = Range;

        if (Range == 0)
            SetBackColor(true);
        else
            SetBackColor(false);
    }

    // �F��ݒ肷��
    private void SetBackColor(bool isSelected)
    {
        if(isSelected)
            _backImage.color = _selectBackColor;
        else
            _backImage.color = _notSelectBackColor;
    }
}
