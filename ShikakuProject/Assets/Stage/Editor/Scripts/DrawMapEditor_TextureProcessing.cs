using Unity.VisualScripting;
using UnityEngine;

public class DrawMapEditor_TextureProcessing
{
    private struct TextureColor
    {
        public Color[] Colors { get; private set; }
        public int X { get; private set;}
        public int Y { get; private set;}

        public TextureColor(Texture2D texture)
        {
            Colors = texture.GetPixels();
            X = texture.width;
            Y = texture.height;
        }

        private readonly Color GetColor(int x,int y)
        {
            return Colors[y * X + x];
        }

        // �I�����ꂽ�摜��h��
        public readonly void DrawTexture(TextureColor textureColor,int drawPointX,int drawPointY)
        {
            try
            {
                for (int countY = 0; countY < textureColor.Y; ++countY)
                {
                    for (int countX = 0; countX < textureColor.X; ++countX)
                    {
                        // �����x��1�łȂ�������X�L�b�v
                        if (textureColor.GetColor(countX, countY).a != 1) continue;

                        Colors[(drawPointY + countY) * X + (drawPointX + countX)] = textureColor.GetColor(countX, countY);
                    }
                }
            }
            catch 
            {
                Debug.Log("�摜�𐶐������Ƃ��ɃG���[���o�܂���"); 
            }
        }

    }

    [SerializeField]
    private Texture2D[] _numbersTextures = null;

    private readonly TextureColor[] _numbersColors = null;

    public DrawMapEditor_TextureProcessing(Texture2D[] numberTexture)
    {
        _numbersTextures = numberTexture;

        _numbersColors = new TextureColor[_numbersTextures.Length];

        for (int i = 0; i < _numbersTextures.Length; ++i)
        {
            _numbersColors[i] = new(_numbersTextures[i]);
        }
    }

    public Texture[] TextureCreate(int number, Texture2D texture)
    {
        Texture[] newTexture = new Texture[number];

        for (int i = 0; i < number; ++i)
        {
            newTexture[i] = DrawTextureToBottomLeft(i, texture);
        }

        return newTexture;
    }

    private Texture2D DrawTextureToBottomLeft(int number, Texture2D texture)
    {
        // ������Texture2D����F���𔲂����
        TextureColor textureColor = new(texture);

        // �������猅���Ƃɐ����𒊏o���A�Ή�����摜��\��t����
        string countStr = number.ToString();

        int digitX = 0;

        for (int i = 0; i < countStr.Length; ++i)
        {
            // �������擾
            int numberOfBigits = int.Parse(countStr[i].ToString());

            // �����ɑΉ�����摜���擾
            TextureColor numberTextureColor = _numbersColors[numberOfBigits];

            // �摜�ɐF��h��
            textureColor.DrawTexture(numberTextureColor, digitX, 1);

            digitX += numberTextureColor.X;
        }

        // ���삵���摜���o��
        Texture2D texture2D = new (textureColor.X, textureColor.Y);
        texture2D.SetPixels(textureColor.Colors);
        texture2D.Apply();

        return texture2D;
    }
}
