using UnityEngine;
using UnityEngine.UI;

// スキルUIのカード風のプレハブの情報
public class UISkillListPrefabData : MonoBehaviour
{
    [Header("UIを動かすときの末端")]
    [SerializeField]
    private Transform _selectPoint;
    [SerializeField]
    private Transform _nomalPoint;

    [Header("動かす対象")]
    [SerializeField]
    private Transform _imageTransform;

    [Header("スキルのイメージ画像")]
    [SerializeField]
    private Image _skillImage;

    [Header("クールタイムの画像")]
    [SerializeField]
    private Image _filterImage;


    [Header("後ろの画像")]
    [SerializeField]
    private Image _backImage;

    [Header("UIの幅")]
    [SerializeField]
    private int _cardWidth;


    public Vector3 GetSelectPoint => _selectPoint.position;

    public Vector3 GetNomalPoint => _nomalPoint.position;

    public Transform ImageTransform => _imageTransform;

    public Image BackImage => _backImage;

    public int CardWidth => _cardWidth;


    // スキルのイメージ画像を設定
    public void SetSprite(Sprite texture)
    {
        _skillImage.sprite = texture;
    }

    // クールタイムを表示
    public void SetCooldown(float range)
    {
        _filterImage.fillAmount = range;
    }
}
