using UnityEditor;
using UnityEngine;

// 配置する要素を決めるボタン　StageTileTypeを使用
public class DrawMapEditor_SelectTile
{
    // 選択している配置要素
    private StageTile _selectStageTile;

    // 選択状態
    private int[] _selectedStageElementButtonIndices;


    public StageTile SelectStageTile => _selectStageTile;


    public void SelectElement(DrawMapEditorSaveData saveData)
    {
        if (saveData.StageObjectElementData == null) return;

        StageTileType typeGround = StageTileType.Ground;
        StageTileType typeObstacle = StageTileType.Obstacle;
        StageTileType typePlayer = StageTileType.Player;
        StageTileType typeEnemy = StageTileType.Enemy;

        EditorGUILayout.LabelField("配置するオブジェクトを選択", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("地面");
        int selectedIndex0 = CheckSelectedIndex(typeGround, saveData);

        EditorGUILayout.LabelField("壁、障害物");
        int selectedIndex1 = CheckSelectedIndex(typeObstacle, saveData);

        EditorGUILayout.LabelField("プレイヤー");
        int selectedIndex2 = CheckSelectedIndex(typePlayer, saveData);

        EditorGUILayout.LabelField("敵");
        int selectedIndex3 = CheckSelectedIndex(typeEnemy, saveData);


        if (selectedIndex0 != _selectedStageElementButtonIndices[(int)typeGround]) SelectStageElementButton(typeGround, selectedIndex0);
        else if (selectedIndex1 != _selectedStageElementButtonIndices[(int)typeObstacle]) SelectStageElementButton(typeObstacle, selectedIndex1);
        else if (selectedIndex2 != _selectedStageElementButtonIndices[(int)typePlayer]) SelectStageElementButton(typePlayer, selectedIndex2);
        else if (selectedIndex3 != _selectedStageElementButtonIndices[(int)typeEnemy]) SelectStageElementButton(typeEnemy, selectedIndex3);

        EditorGUILayout.LabelField("消しゴム");
        // なしが押された場合は初期化する
        if (GUILayout.Button("なし", GUILayout.Width(50), GUILayout.Height(50)))
        {
            SelectStageElementButton(StageTileType.None, -1);
        }
    }

    // 選択肢のボタンを表示
    private int CheckSelectedIndex(StageTileType elementType, DrawMapEditorSaveData saveData)
    {
        int _buttonScale = 50;

        int type = (int)elementType;

        return GUILayout.SelectionGrid(_selectedStageElementButtonIndices[type], saveData.ElementTypeTextures[type], saveData.ElementTypeTextures[type].Length,
                             GUILayout.Width(_buttonScale * saveData.ElementTypeTextures[type].Length), GUILayout.Height(_buttonScale));
    }

    // 引数で指定された要素を選択状態にする
    private void SelectStageElementButton(StageTileType elementType, int selectedIndex)
    {
        // 選択されている要素に引数を入れ、それ以外は-1にする
        for (int i = 0; i < _selectedStageElementButtonIndices.Length; ++i)
        {
            if ((int)elementType == i)
                _selectedStageElementButtonIndices[i] = selectedIndex;
            else
                _selectedStageElementButtonIndices[i] = -1;
        }

        // 選択している配置要素を設定
        _selectStageTile.TileType = elementType;
        _selectStageTile.ElementCount = selectedIndex;
    }

    // 何も選択されていないようにする
    public void Initialization()
    {
        _selectedStageElementButtonIndices = new int[4];

        SelectStageElementButton(StageTileType.None, -1);
    }

    // 何も選択してない状態にする
    public void NothingSelectStageTile()
    {
        _selectStageTile.TileType = StageTileType.None;
        _selectStageTile.ElementCount = 0;
    }
}
