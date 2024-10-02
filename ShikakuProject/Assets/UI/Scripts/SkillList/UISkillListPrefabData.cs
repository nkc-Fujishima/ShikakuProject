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


    [Header("後ろの画像の色を変更")]
    [SerializeField]
    private Image _backImage;
    [SerializeField]
    private Color _selectBackColor;
    [SerializeField]
    private Color _notSelectBackColor;

    [Header("UIの幅")]
    [SerializeField]
    private int _cardWidth;


    public Vector3 GetSelectPoint => _selectPoint.position;

    public Vector3 GetNomalPoint => _nomalPoint.position;

    public Transform ImageTransform => _imageTransform;

    public int CardWidth => _cardWidth;


    // スキルのイメージ画像を設定
    public void SetSprite(Sprite texture)
    {
        _skillImage.sprite = texture;
    }

    // クールタイムを表示
    public void SetCooldown(float Range)
    {
        _filterImage.fillAmount = Range;

        if (Range == 0)
            SetBackColor(true);
        else
            SetBackColor(false);
    }

    // 色を設定する
    private void SetBackColor(bool isSelected)
    {
        if(isSelected)
            _backImage.color = _selectBackColor;
        else
            _backImage.color = _notSelectBackColor;
    }
}
