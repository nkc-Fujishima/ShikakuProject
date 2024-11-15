using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// �}�b�v���̐V�K�쐬���
public class MadeWaypoint_MapWindow : EditorWindow
{
    static MadeWaypoint_MapWindow window;

    private DrawMapEditorSaveData _saveData = null;


    // �ԍ��������摜�𐶐�����X�N���v�g
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

    // �c�[���o�[��`��
    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
        {
            if (GUILayout.Button("���Z�b�g", EditorStyles.toolbarButton))
            {
                if (_isWaypoint) _saveData.DrawMapData.WaypointData[_index].Waypoint = null;
                else _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint = null;

                SetTexture();
            }
        }
    }

    // �O���b�h�{�^����`��
    private Vector2 _scrollPositionDraw = Vector2.zero;
    private void SelectDrawPaint()
    {
        // �O���b�h�������i0�������j�ꍇ�͏I��
        if (_saveData.DrawMapData.X == 0 || _saveData.DrawMapData.Y == 0) return;

        int scale = 40;

        _scrollPositionDraw = GUILayout.BeginScrollView(_scrollPositionDraw);

        int selectedGridIndex = GUILayout.SelectionGrid(-1, _mapTextures, _saveData.DrawMapData.X,
                                GUILayout.Width(scale * _saveData.DrawMapData.X), GUILayout.Height(scale * _saveData.DrawMapData.Y));

        GUILayout.EndScrollView();


        // �I������Ȃ�������I��
        if (selectedGridIndex == -1) return;

        Vector2Int selectGrid = new(selectedGridIndex % _saveData.DrawMapData.X, selectedGridIndex / _saveData.DrawMapData.X);

        StageTileType tileType = _saveData.GetTileTypeOnTileData(selectGrid.x, selectGrid.y);
        bool isSubjectGround = _isWaypoint && (tileType == StageTileType.Ground || tileType == StageTileType.Enemy);
        bool isSubjectEnemy = !_isWaypoint && tileType == StageTileType.Enemy;
        if (isSubjectGround || isSubjectEnemy)
        {
            // �ύX�𔽉f
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

    // �e�N�X�`����ݒ�
    private void SetTexture()
    {
        Vector2Int mapMax = new(_saveData.DrawMapData.X, _saveData.DrawMapData.Y);

        _mapTextures = new Texture2D[mapMax.x * mapMax.y];

        bool isErrorOutPoint = false;

        // �z�u�v�f�̉摜��ݒ�
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
            // �Y������ꏊ�����摜���C��
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
                // �͈͓��ł��邩�𒲂ׂ�
                // ����������Ȃ��ꍇ�e��
                if (point.x < 0 || mapMax.x <= point.x || point.y < 0 || mapMax.y <= point.y)
                {
                    List<Vector2Int> hashPointsList = new(points);
                    hashPointsList.Remove(point);
                    pointDatas = hashPointsList.ToArray();

                    isErrorOutPoint = true;
                    continue;
                }

                // �v�f��z�u�ł���ꏊ�ł��邩�𒲂ׂ�
                // ����������Ȃ��ꍇ�e��
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

                // ���f
                int listCount = (point.y - 1) * mapMax.x + point.x;
                Texture2D newTexture = _textureProcessing.TextureCreate(_cleateCount++, _saveData.ElementTypeTexture_Flag);
                _mapTextures[listCount] = newTexture;
            }

            // �z����ɏC������������C���ǂ���ɕύX����
            if (isErrorOutPoint)
            {
                if (_isWaypoint)
                    _saveData.DrawMapData.WaypointData[_index].Waypoint = (Vector2Int[])pointDatas.Clone();
                else
                    _saveData.DrawMapData.WaypointData[_index].EnemyAtPoint = (Vector2Int[])pointDatas.Clone();

                // �C���������Ƃ����[�U�[�ɓ`����
                EditorUtility.DisplayDialog("Warning", "�͈͊O�̏����폜���܂����B", "OK");
            }
        }
    }

    // �G�̃f�[�^�̏d�����Ȃ��悤�ɏC������
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