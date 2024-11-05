using UnityEditor;
using UnityEngine;

// ステージを制作するウィンドウ
public class DrawMapEditor_MadeMap
{
    private Texture2D[] _mapTextures = null;

    private Vector2 _scrollPositionDraw = Vector2.zero;

    private readonly DrawMapEditor_TextureProcessing _textureProcessing;

    public DrawMapEditor_MadeMap(DrawMapEditor_TextureProcessing textureProcessing)
    {
        _textureProcessing = textureProcessing;
    }

    public void SelectDrawPaint(DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        // グリッドが無い（0だった）場合は終了
        if (saveData.DrawMapData.X == 0 || saveData.DrawMapData.Y == 0) return;

        int scale = 40;

        _scrollPositionDraw = GUILayout.BeginScrollView(_scrollPositionDraw);

        int selectedGridIndex = GUILayout.SelectionGrid(-1, _mapTextures, saveData.DrawMapData.X,
                                GUILayout.Width(scale * saveData.DrawMapData.X), GUILayout.Height(scale * saveData.DrawMapData.Y));

        GUILayout.EndScrollView();

        // 選択されなかったら終了
        if (selectedGridIndex == -1) return;

        Vector2Int selectGrid = new (selectedGridIndex % saveData.DrawMapData.X, selectedGridIndex / saveData.DrawMapData.X);

        // もし同じ種類のタイルでボタンを押した場合
        if (selectStageTile.TileType == saveData.GetTileTypeOnTileData(selectGrid.x, selectGrid.y) &&
            selectStageTile.ElementCount == saveData.GetElementCountOnTileData(selectGrid.x, selectGrid.y))
        {
            RotateElement(selectedGridIndex, saveData, selectStageTile);

            return;
        }


        // 引数で選択した要素のボタンを設定する
        DrawPaint(selectedGridIndex, saveData, selectStageTile);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // 引数で選択した要素のボタンを設定する
    private void DrawPaint(int selectedIndex, DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        // 選択したボタンの画像を設定
        if (selectStageTile.TileType != StageTileType.None)
            _mapTextures[selectedIndex] = saveData.ElementTypeTextures[(int)selectStageTile.TileType][selectStageTile.ElementCount];
        else
            _mapTextures[selectedIndex] = null;

        // 選択内容を配列に保存
        saveData.SetTileDataToElement(selectedIndex % saveData.DrawMapData.X, selectedIndex / saveData.DrawMapData.X, selectStageTile.TileType, selectStageTile.ElementCount);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // 選択した要素のボタンを回転する
    private void RotateElement(int selectedIndex, DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        if (selectStageTile.TileType == StageTileType.None) return;

        Texture2D originalTexture = saveData.ElementTypeTextures[(int)selectStageTile.TileType][selectStageTile.ElementCount];

        Vector2Int selectGrid = new(selectedIndex % saveData.DrawMapData.X, selectedIndex / saveData.DrawMapData.X);

        saveData.TileRotate(selectGrid.x, selectGrid.y);

        _mapTextures[selectedIndex] = _textureProcessing.DrawArrowTexture(saveData.GetRotationOnTileData(selectGrid.x, selectGrid.y), originalTexture);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // マップを白紙に戻す
    public void Initialization(DrawMapEditorSaveData saveData)
    {
        // 選択したボタンの画像を初期化
        _mapTextures = new Texture2D[saveData.DrawMapData.X * saveData.DrawMapData.Y];

        // 選択内容を初期化
        saveData.ResetMap(saveData.DrawMapData.X, saveData.DrawMapData.Y);

        // 選択をNoneにする
        StageTile stageTile;
        stageTile.TileType = StageTileType.None;
        stageTile.ElementCount = -1;
        stageTile.RotationY = 0;

        for (int i = 0; i < _mapTextures.Length; ++i)
            DrawPaint(i, saveData, stageTile);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // 設定されてるマップデータと同じになるように画像を設定
    public void SetAllMapTextures(DrawMapEditorSaveData saveData)
    {
        int elementLength = saveData.DrawMapData.X * saveData.DrawMapData.Y;

        // 選択したボタンの画像を初期化
        _mapTextures = new Texture2D[elementLength];

        int count = 0;

        // 画像を設定
        for (int checkY = 0; checkY < saveData.DrawMapData.Y; ++checkY)
        {
            for (int checkX = 0; checkX < saveData.DrawMapData.X; ++checkX)
            {
                if (saveData.GetTileTypeOnTileData(checkX, checkY) != StageTileType.None)
                {
                    _mapTextures[count] = saveData.ElementTypeTextures[(int)saveData.GetTileTypeOnTileData(checkX, checkY)]
                                                                      [saveData.GetElementCountOnTileData(checkX, checkY)];

                    int rotate = saveData.GetRotationOnTileData(checkX, checkY);
                    _mapTextures[count] = _textureProcessing.DrawArrowTexture(rotate, _mapTextures[count]);
                }
                else
                    _mapTextures[count] = null;

                ++count;
            }
        }
    }


    //-------------------------------------------------------------------------------------------------------------------
    // 選択してる要素で塗りつぶす
    public void AllFillButton(DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        bool isFillCheck = EditorUtility.DisplayDialog("Warning", "塗りつぶしをするよ\n" +
                                       "今の状態は消えるけど良いかな？", "OK", "キャンセル");

        if (!isFillCheck) return;

        for (int i = 0; i < _mapTextures.Length; ++i)
            DrawPaint(i, saveData, selectStageTile);
    }
}
