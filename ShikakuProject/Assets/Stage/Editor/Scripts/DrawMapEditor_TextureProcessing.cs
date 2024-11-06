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

        // 選択された画像を塗る
        public readonly void DrawTexture(TextureColor textureColor,int drawPointX,int drawPointY)
        {
            try
            {
                for (int countY = 0; countY < textureColor.Y; ++countY)
                {
                    for (int countX = 0; countX < textureColor.X; ++countX)
                    {
                        // 透明度が1でなかったらスキップ
                        if (textureColor.GetColor(countX, countY).a != 1) continue;

                        Colors[(drawPointY + countY) * X + (drawPointX + countX)] = textureColor.GetColor(countX, countY);
                    }
                }
            }
            catch 
            {
                Debug.Log("画像を生成したときにエラーが出ました"); 
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
        // 引数のTexture2Dから色情報を抜き取る
        TextureColor textureColor = new(texture);
        TextureColor elementTextureColor = new(elementTexture);

        int digitX = textureColor.X - elementTextureColor.X;

        // 画像に色を塗る
        textureColor.DrawTexture(elementTextureColor, digitX, textureColor.Y - elementTextureColor.Y);

        // 制作した画像を出力
        Texture2D texture2D = new(textureColor.X, textureColor.Y);
        texture2D.SetPixels(textureColor.Colors);
        texture2D.Apply();

        return texture2D;
    }

    private Texture2D DrawNumberTextureToBottomLeft(int number, Texture2D texture)
    {
        // 引数のTexture2Dから色情報を抜き取る
        TextureColor textureColor = new(texture);

        // 数字から桁ごとに数字を抽出し、対応する画像を貼り付ける
        string countStr = number.ToString();

        int digitX = 0;

        for (int i = 0; i < countStr.Length; ++i)
        {
            // 数字を取得
            int numberOfBigits = int.Parse(countStr[i].ToString());

            // 数字に対応する画像を取得
            TextureColor numberTextureColor = _numbersColors[numberOfBigits];

            // 画像に色を塗る
            textureColor.DrawTexture(numberTextureColor, digitX, 1);

            digitX += numberTextureColor.X;
        }

        // 制作した画像を出力
        Texture2D texture2D = new (textureColor.X, textureColor.Y);
        texture2D.SetPixels(textureColor.Colors);
        texture2D.Apply();

        return texture2D;
    }

    public Texture2D DrawArrowTexture(int originalAngle, Texture2D texture)
    {   
        // 引数のTexture2Dから色情報を抜き取る
        TextureColor textureColor = new(texture);

        int textureAngle = 0;
        Texture2D arrowTexture = null;

        // どの角度のテクスチャを使えばいいかを調べる
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


        // 矢印のテクスチャを回転させる
        Texture2D rotatedArrowTexture = RotateTexture(arrowTexture, textureAngle);

        // 回転させた矢印のテクスチャを元のテクスチャに貼り付け
        TextureColor arrowTextureColor = new(rotatedArrowTexture);
        textureColor.DrawTexture(arrowTextureColor, 0, 0);

        // 制作した画像を出力
        Texture2D texture2D = new(textureColor.X, textureColor.Y);
        texture2D.SetPixels(textureColor.Colors);
        texture2D.Apply();

        return texture2D;
    }


    // テクスチャを回転する
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
                    rotatedPixels[y * width + x] = new Color32(0, 0, 0, 0); // 透明色
                }
            }
        }

        rotatedTexture.SetPixels32(rotatedPixels);
        rotatedTexture.Apply();

        return rotatedTexture;
    }

    // テクスチャをリサイズする
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
