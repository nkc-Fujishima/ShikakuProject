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
}
