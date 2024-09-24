using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

// ステージを制作するウィンドウ
public class DrawMapEditor_MadeMap
{
    Texture[] _mapTextures = null;

    Vector2 _scrollPositionDraw = Vector2.zero;

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

        DrawPaint(selectedGridIndex, saveData, selectStageTile);
    }

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


    // マップを白紙に戻す
    public void Initialization(DrawMapEditorSaveData saveData)
    {
        // 選択したボタンの画像を初期化
        _mapTextures = new Texture[saveData.DrawMapData.X * saveData.DrawMapData.Y];

        // 選択内容を初期化
        saveData.ResetMap(saveData.DrawMapData.X, saveData.DrawMapData.Y);

        // 選択をNoneにする
        StageTile stageTile;
        stageTile.TileType = StageTileType.None;
        stageTile.ElementCount = -1;

        for (int i = 0; i < _mapTextures.Length; ++i)
            DrawPaint(i, saveData, stageTile);
    }

    // 設定されてるマップデータと同じになるように画像を設定
    public void SetAllMapTextures(DrawMapEditorSaveData saveData)
    {
        int elementLength = saveData.DrawMapData.X * saveData.DrawMapData.Y;

        // 選択したボタンの画像を初期化
        _mapTextures = new Texture[elementLength];

        int count = 0;

        // 画像を設定
        for (int checkY = 0; checkY < saveData.DrawMapData.Y; ++checkY)
        {
            for (int checkX = 0; checkX < saveData.DrawMapData.X; ++checkX)
            {
                if (saveData.GetTileTypeOnTileData(checkX, checkY) != StageTileType.None)
                    _mapTextures[count] = saveData.ElementTypeTextures[(int)saveData.GetTileTypeOnTileData(checkX, checkY)]
                                                                      [saveData.GetElementCountOnTileData(checkX, checkY)];
                else
                    _mapTextures[count] = null;

                ++count;
            }
        }
    }


    // 選択してる要素で塗りつぶす
    public void AllFillButton(DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        bool isFillCheck = EditorUtility.DisplayDialog("Warning", "塗りつぶしをするよ\n" +
                                       "今の状態は消えるけど良いかな？", "キャンセル", "OK");

        if (isFillCheck) return;

        for (int i = 0; i < _mapTextures.Length; ++i)
            DrawPaint(i, saveData, selectStageTile);
    }
}
