using UnityEngine;
using UnityEngine.UI;

// �X�L��UI�̃J�[�h���̃v���n�u�̏��
public class UISkillListPrefabData : MonoBehaviour
{
    [Header("�������Ώ�")]
    [SerializeField]
    private Transform _imageTransform;

    [Header("�X�L���̃C���[�W�摜")]
    [SerializeField]
    private Image _skillImage;

    [Header("�N�[���^�C���̉摜")]
    [SerializeField]
    private Image _filterImage;


    [Header("���̉摜")]
    [SerializeField]
    private Image _backImage;

    [Header("UI�̕�")]
    [SerializeField]
    private int _cardWidth;


    public Transform ImageTransform => _imageTransform;

    public Image BackImage => _backImage;

    public int CardWidth => _cardWidth;


    // �X�L���̃C���[�W�摜��ݒ�
    public void SetSprite(Sprite texture)
    {
        _skillImage.sprite = texture;
    }

    // �N�[���^�C����\��
    public void SetCooldown(float range)
    {
        _filterImage.fillAmount = range;
    }
}
