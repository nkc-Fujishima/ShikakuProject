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

    private readonly Texture2D _upArrowTexture = null;

    private readonly Texture2D _diagonalTexture = null;

    public DrawMapEditor_TextureProcessing(Texture2D[] numberTexture, Texture2D upArrow, Texture2D diagonalArrow)
    {
        _numbersTextures = numberTexture;

        _numbersColors = new TextureColor[_numbersTextures.Length];

        for (int i = 0; i < _numbersTextures.Length; ++i)
        {
            _numbersColors[i] = new(_numbersTextures[i]);
        }

        _upArrowTexture = upArrow;
        _diagonalTexture = diagonalArrow;
    }

    public Texture2D[] TextureCreate(int number, Texture2D[] texture, Texture2D elementTexture)
    {
        Texture2D[] newTexture = new Texture2D[number];

        for (int i = 0; i < number; ++i)
        {
            newTexture[i] = DrawNumberTextureToBottomLeft(i, texture[i]);

            if (elementTexture != null)
                newTexture[i] = DrawElementTextureToOverRight(newTexture[i], elementTexture);
        }

        return newTexture;
    }

    public Texture2D TextureCreate(int number, Texture2D texture)
    {
        return DrawNumberTextureToBottomLeft(number, texture);
    }

    private Texture2D DrawElementTextureToOverRight(Texture2D texture, Texture2D elementTexture)
    {
        // ������Texture2D����F���𔲂����
        TextureColor textureColor = new(texture);
        TextureColor elementTextureColor = new(elementTexture);

        int digitX = textureColor.X - elementTextureColor.X;

        // �摜�ɐF��h��
        textureColor.DrawTexture(elementTextureColor, digitX, textureColor.Y - elementTextureColor.Y);

        // ���삵���摜���o��
        Texture2D texture2D = new(textureColor.X, textureColor.Y);
        texture2D.SetPixels(textureColor.Colors);
        texture2D.Apply();

        return texture2D;
    }

    private Texture2D DrawNumberTextureToBottomLeft(int number, Texture2D texture)
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

    public Texture2D DrawArrowTexture(int originalAngle, Texture2D texture)
    {   
        // ������Texture2D����F���𔲂����
        TextureColor textureColor = new(texture);

        int textureAngle = 0;
        Texture2D arrowTexture = null;

        // �ǂ̊p�x�̃e�N�X�`�����g���΂������𒲂ׂ�
        switch(originalAngle)
        {
            case 0:
                textureAngle = 0;
                arrowTexture = _upArrowTexture;
                break;
            case 45:
                textureAngle = 0;
                arrowTexture = _diagonalTexture;
                break;
            case 90:
                textureAngle = 90;
                arrowTexture = _upArrowTexture;
                break;
            case 135:
                textureAngle = 90;
                arrowTexture = _diagonalTexture;
                break;
            case 180:
                textureAngle = 180;
                arrowTexture = _upArrowTexture;
                break;
            case 225:
                textureAngle = 180;
                arrowTexture = _diagonalTexture;
                break;
            case 270:
                textureAngle = 260;
                arrowTexture = _upArrowTexture;
                break;
            case 315:
                textureAngle = 260;
                arrowTexture = _diagonalTexture;
                break;
        }


        // ���̃e�N�X�`������]������
        Texture2D rotatedArrowTexture = RotateTexture(arrowTexture, textureAngle);

        // ��]���������̃e�N�X�`�������̃e�N�X�`���ɓ\��t��
        TextureColor arrowTextureColor = new(rotatedArrowTexture);
        textureColor.DrawTexture(arrowTextureColor, 0, 0);

        // ���삵���摜���o��
        Texture2D texture2D = new(textureColor.X, textureColor.Y);
        texture2D.SetPixels(textureColor.Colors);
        texture2D.Apply();

        return texture2D;
    }


    // �e�N�X�`������]����
    private Texture2D RotateTexture(Texture2D originalTexture, float angle)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;
        Texture2D rotatedTexture = new (width, height);

        Color32[] originalPixels = originalTexture.GetPixels32();
        Color32[] rotatedPixels = new Color32[originalPixels.Length];

        float angleRad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);

        int xCenter = width / 2;
        int yCenter = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int xOffset = x - xCenter;
                int yOffset = y - yCenter;

                int newX = Mathf.RoundToInt(cos * xOffset - sin * yOffset) + xCenter;
                int newY = Mathf.RoundToInt(sin * xOffset + cos * yOffset) + yCenter;

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    rotatedPixels[y * width + x] = originalPixels[newY * width + newX];
                }
                else
                {
                    rotatedPixels[y * width + x] = new Color32(0, 0, 0, 0); // �����F
                }
            }
        }

        rotatedTexture.SetPixels32(rotatedPixels);
        rotatedTexture.Apply();

        return rotatedTexture;
    }

    // �e�N�X�`�������T�C�Y����
    public Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new(width, height);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
}
