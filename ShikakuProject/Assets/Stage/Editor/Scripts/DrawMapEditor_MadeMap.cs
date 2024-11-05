using UnityEditor;
using UnityEngine;

// �X�e�[�W�𐧍삷��E�B���h�E
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
        // �O���b�h�������i0�������j�ꍇ�͏I��
        if (saveData.DrawMapData.X == 0 || saveData.DrawMapData.Y == 0) return;

        int scale = 40;

        _scrollPositionDraw = GUILayout.BeginScrollView(_scrollPositionDraw);

        int selectedGridIndex = GUILayout.SelectionGrid(-1, _mapTextures, saveData.DrawMapData.X,
                                GUILayout.Width(scale * saveData.DrawMapData.X), GUILayout.Height(scale * saveData.DrawMapData.Y));

        GUILayout.EndScrollView();

        // �I������Ȃ�������I��
        if (selectedGridIndex == -1) return;

        Vector2Int selectGrid = new (selectedGridIndex % saveData.DrawMapData.X, selectedGridIndex / saveData.DrawMapData.X);

        // ����������ނ̃^�C���Ń{�^�����������ꍇ
        if (selectStageTile.TileType == saveData.GetTileTypeOnTileData(selectGrid.x, selectGrid.y) &&
            selectStageTile.ElementCount == saveData.GetElementCountOnTileData(selectGrid.x, selectGrid.y))
        {
            RotateElement(selectedGridIndex, saveData, selectStageTile);

            return;
        }


        // �����őI�������v�f�̃{�^����ݒ肷��
        DrawPaint(selectedGridIndex, saveData, selectStageTile);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // �����őI�������v�f�̃{�^����ݒ肷��
    private void DrawPaint(int selectedIndex, DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        // �I�������{�^���̉摜��ݒ�
        if (selectStageTile.TileType != StageTileType.None)
            _mapTextures[selectedIndex] = saveData.ElementTypeTextures[(int)selectStageTile.TileType][selectStageTile.ElementCount];
        else
            _mapTextures[selectedIndex] = null;

        // �I����e��z��ɕۑ�
        saveData.SetTileDataToElement(selectedIndex % saveData.DrawMapData.X, selectedIndex / saveData.DrawMapData.X, selectStageTile.TileType, selectStageTile.ElementCount);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // �I�������v�f�̃{�^������]����
    private void RotateElement(int selectedIndex, DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        if (selectStageTile.TileType == StageTileType.None) return;

        Texture2D originalTexture = saveData.ElementTypeTextures[(int)selectStageTile.TileType][selectStageTile.ElementCount];

        Vector2Int selectGrid = new(selectedIndex % saveData.DrawMapData.X, selectedIndex / saveData.DrawMapData.X);

        saveData.TileRotate(selectGrid.x, selectGrid.y);

        _mapTextures[selectedIndex] = _textureProcessing.DrawArrowTexture(saveData.GetRotationOnTileData(selectGrid.x, selectGrid.y), originalTexture);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // �}�b�v�𔒎��ɖ߂�
    public void Initialization(DrawMapEditorSaveData saveData)
    {
        // �I�������{�^���̉摜��������
        _mapTextures = new Texture2D[saveData.DrawMapData.X * saveData.DrawMapData.Y];

        // �I����e��������
        saveData.ResetMap(saveData.DrawMapData.X, saveData.DrawMapData.Y);

        // �I����None�ɂ���
        StageTile stageTile;
        stageTile.TileType = StageTileType.None;
        stageTile.ElementCount = -1;
        stageTile.RotationY = 0;

        for (int i = 0; i < _mapTextures.Length; ++i)
            DrawPaint(i, saveData, stageTile);
    }

    //-------------------------------------------------------------------------------------------------------------------
    // �ݒ肳��Ă�}�b�v�f�[�^�Ɠ����ɂȂ�悤�ɉ摜��ݒ�
    public void SetAllMapTextures(DrawMapEditorSaveData saveData)
    {
        int elementLength = saveData.DrawMapData.X * saveData.DrawMapData.Y;

        // �I�������{�^���̉摜��������
        _mapTextures = new Texture2D[elementLength];

        int count = 0;

        // �摜��ݒ�
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
    // �I�����Ă�v�f�œh��Ԃ�
    public void AllFillButton(DrawMapEditorSaveData saveData, StageTile selectStageTile)
    {
        bool isFillCheck = EditorUtility.DisplayDialog("Warning", "�h��Ԃ��������\n" +
                                       "���̏�Ԃ͏����邯�Ǘǂ����ȁH", "OK", "�L�����Z��");

        if (!isFillCheck) return;

        for (int i = 0; i < _mapTextures.Length; ++i)
            DrawPaint(i, saveData, selectStageTile);
    }
}
