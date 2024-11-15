using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// マップ情報の新規作成画面
public class MadeWaypoint_MapWindow : EditorWindow
{
    static MadeWaypoint_MapWindow window;

    private DrawMapEditorSaveData _saveData = null;


    // 番号がついた画像を生成するスクリプト
    private DrawMapEditor_TextureProcessing _textureProcessing;

    private int _index;

    private bool _isWaypoint = true;

    private Texture2D[] _mapTextures = null;

    private int _cleateCount = 0;


    public static void Open(DrawMapEditorSaveData saveData, int index, bool isWaypoint)
    {
        if (window == null)
        {
            window = CreateInstance<MadeWaypoint_MapWindow>();
        }

        window._saveData = saveData;

        window._textureProcessing = new(window._saveData.NumberTextures,
                                        window._saveData.UpArrowTexture, window._saveData.DiagonalArrowTexture);
        window._index = index;
        window._isWaypoint = isWaypoint;

        window.AlignEnemyData();
        window.SetTexture();

        window.ShowUtility();
    }


    private void OnGUI()
    {

        DrawToolbar();
        SelectDrawPaint();

        Repaint();
    }

    // ツールバーを描画
    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
        {
            if (GUILayout.Button("リセット", EditorStyles.toolbarButton))
            {
                if (_isWaypoint) _saveData.DrawMapData.WaypointData[_index].Waypoint = null;
                else _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint = null;

                SetTexture();
            }
        }
    }

    // グリッドボタンを描画
    private Vector2 _scrollPositionDraw = Vector2.zero;
    private void SelectDrawPaint()
    {
        // グリッドが無い（0だった）場合は終了
        if (_saveData.DrawMapData.X == 0 || _saveData.DrawMapData.Y == 0) return;

        int scale = 40;

        _scrollPositionDraw = GUILayout.BeginScrollView(_scrollPositionDraw);

        int selectedGridIndex = GUILayout.SelectionGrid(-1, _mapTextures, _saveData.DrawMapData.X,
                                GUILayout.Width(scale * _saveData.DrawMapData.X), GUILayout.Height(scale * _saveData.DrawMapData.Y));

        GUILayout.EndScrollView();


        // 選択されなかったら終了
        if (selectedGridIndex == -1) return;

        Vector2Int selectGrid = new(selectedGridIndex % _saveData.DrawMapData.X, selectedGridIndex / _saveData.DrawMapData.X);

        StageTileType tileType = _saveData.GetTileTypeOnTileData(selectGrid.x, selectGrid.y);
        bool isSubjectGround = _isWaypoint && (tileType == StageTileType.Ground || tileType == StageTileType.Enemy);
        bool isSubjectEnemy = !_isWaypoint && tileType == StageTileType.Enemy;
        if (isSubjectGround || isSubjectEnemy)
        {
            // 変更を反映
            Texture2D newTexture = _textureProcessing.TextureCreate(_cleateCount++, _saveData.ElementTypeTexture_Flag);
            _mapTextures[selectedGridIndex] = newTexture;

            if (_isWaypoint)
            {
                List<Vector2Int> hashPointList;
                Vector2Int[] points = _saveData.DrawMapData.WaypointData[_index].Waypoint;
                if (points != null) hashPointList = new(points);
                else hashPointList = new();

                hashPointList.Add(selectGrid);
                _saveData.DrawMapData.WaypointData[_index].Waypoint = hashPointList.ToArray();
            }
            else
            {
                List<Vector2Int> hashPointList;
                Vector2Int[] points = _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint;
                if (points != null) hashPointList = new(points);
                else hashPointList = new();

                hashPointList.Add(selectGrid);
                _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint = hashPointList.ToArray();

                AlignEnemyData();
            }
        }
    }

    // テクスチャを設定
    private void SetTexture()
    {
        Vector2Int mapMax = new(_saveData.DrawMapData.X, _saveData.DrawMapData.Y);

        _mapTextures = new Texture2D[mapMax.x * mapMax.y];

        bool isErrorOutPoint = false;

        // 配置要素の画像を設定
        int count = 0;
        for (int checkY = 0; checkY < mapMax.y; ++checkY)
        {
            for (int checkX = 0; checkX < mapMax.x; ++checkX)
            {
                switch (_saveData.GetTileTypeOnTileData(checkX, checkY))
                {
                    case StageTileType.Ground:
                        if (_isWaypoint) _mapTextures[count] = _saveData.ElementTypeTexture_Ground;
                        else _mapTextures[count] = _saveData.ElementTypeTexture_X;
                        break;

                    case StageTileType.Enemy:
                        if (_isWaypoint) _mapTextures[count] = _saveData.ElementTypeTexture_Ground;
                        else _mapTextures[count] = _saveData.ElementTypeTexture_Enemy;
                        break;

                    default:
                        _mapTextures[count] = _saveData.ElementTypeTexture_X;
                        break;
                }

                ++count;
            }
        }


        if (_isWaypoint && _saveData.DrawMapData.WaypointData[_index].Waypoint != null ||
           !_isWaypoint && _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint != null)
        {
            // 該当する場所だけ画像を修正
            Vector2Int[] points;
            Vector2Int[] pointDatas;

            if (_isWaypoint)
            {
                points = (Vector2Int[])_saveData.DrawMapData.WaypointData[_index].Waypoint.Clone();
                pointDatas = _saveData.DrawMapData.WaypointData[_index].Waypoint;
            }
            else
            {
                points = (Vector2Int[])_saveData.DrawMapData.WaypointData[_index].EnemyAtPoint.Clone();
                pointDatas = _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint;
            }

            _cleateCount = 0;
            foreach (Vector2Int point in points)
            {
                // 範囲内であるかを調べる
                // 条件が合わない場合弾く
                if (point.x < 0 || mapMax.x <= point.x || point.y < 0 || mapMax.y <= point.y)
                {
                    List<Vector2Int> hashPointsList = new(points);
                    hashPointsList.Remove(point);
                    pointDatas = hashPointsList.ToArray();

                    isErrorOutPoint = true;
                    continue;
                }

                // 要素を配置できる場所であるかを調べる
                // 条件が合わない場合弾く
                StageTileType tileType = _saveData.GetTileTypeOnTileData(point.x, point.y);
                bool isSubjectGround = _isWaypoint && (tileType == StageTileType.Ground || tileType == StageTileType.Enemy);
                bool isSubjectEnemy = !_isWaypoint && tileType == StageTileType.Enemy;
                if (!(isSubjectGround || isSubjectEnemy))
                {
                    List<Vector2Int> hashPointsList = new(points);
                    hashPointsList.Remove(point);
                    pointDatas = hashPointsList.ToArray();

                    isErrorOutPoint = true;
                    continue;
                }

                // 反映
                int listCount = (point.y - 1) * mapMax.x + point.x;
                Texture2D newTexture = _textureProcessing.TextureCreate(_cleateCount++, _saveData.ElementTypeTexture_Flag);
                _mapTextures[listCount] = newTexture;
            }

            // 配列情報に修正があったら修正どおりに変更する
            if (isErrorOutPoint)
            {
                if (_isWaypoint)
                    _saveData.DrawMapData.WaypointData[_index].Waypoint = (Vector2Int[])pointDatas.Clone();
                else
                    _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint = (Vector2Int[])pointDatas.Clone();

                // 修正したことをユーザーに伝える
                EditorUtility.DisplayDialog("Warning", "範囲外の情報を削除しました。", "OK");
            }
        }
    }

    // 敵のデータの重複がないように修正する
    private void AlignEnemyData()
    {
        if (_isWaypoint) return;

        if (_saveData.DrawMapData.WaypointData[_index].EnemyAtPoint != null)
        {
            _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint =
                new HashSet<Vector2Int>(_saveData.DrawMapData.WaypointData[_index].EnemyAtPoint).ToArray();
        }
    }
}