using UnityEditor;
using UnityEngine;

// �z�u����v�f�����߂�{�^���@StageTileType���g�p
public class DrawMapEditor_SelectTile
{
    // �I�����Ă���z�u�v�f
    private StageTile _selectStageTile;

    // �I�����
    private int[] _selectedStageElementButtonIndices;


    public StageTile SelectStageTile => _selectStageTile;


    public void SelectElement(DrawMapEditorSaveData saveData)
    {
        if (saveData.StageObjectElementData == null) return;

        StageTileType typeGround = StageTileType.Ground;
        StageTileType typeObstacle = StageTileType.Obstacle;
        StageTileType typePlayer = StageTileType.Player;
        StageTileType typeEnemy = StageTileType.Enemy;

        EditorGUILayout.LabelField("�z�u����I�u�W�F�N�g��I��", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("�n��");
        int selectedIndex0 = CheckSelectedIndex(typeGround, saveData);

        EditorGUILayout.LabelField("�ǁA��Q��");
        int selectedIndex1 = CheckSelectedIndex(typeObstacle, saveData);

        EditorGUILayout.LabelField("�v���C���[");
        int selectedIndex2 = CheckSelectedIndex(typePlayer, saveData);

        EditorGUILayout.LabelField("�G");
        int selectedIndex3 = CheckSelectedIndex(typeEnemy, saveData);


        if (selectedIndex0 != _selectedStageElementButtonIndices[(int)typeGround]) SelectStageElementButton(typeGround, selectedIndex0);
        else if (selectedIndex1 != _selectedStageElementButtonIndices[(int)typeObstacle]) SelectStageElementButton(typeObstacle, selectedIndex1);
        else if (selectedIndex2 != _selectedStageElementButtonIndices[(int)typePlayer]) SelectStageElementButton(typePlayer, selectedIndex2);
        else if (selectedIndex3 != _selectedStageElementButtonIndices[(int)typeEnemy]) SelectStageElementButton(typeEnemy, selectedIndex3);

        EditorGUILayout.LabelField("�����S��");
        // �Ȃ��������ꂽ�ꍇ�͏���������
        if (GUILayout.Button("�Ȃ�", GUILayout.Width(50), GUILayout.Height(50)))
        {
            SelectStageElementButton(StageTileType.None, -1);
        }
    }

    // �I�����̃{�^����\��
    private int CheckSelectedIndex(StageTileType elementType, DrawMapEditorSaveData saveData)
    {
        int _buttonScale = 50;

        int type = (int)elementType;

        return GUILayout.SelectionGrid(_selectedStageElementButtonIndices[type], saveData.ElementTypeTextures[type], saveData.ElementTypeTextures[type].Length,
                             GUILayout.Width(_buttonScale * saveData.ElementTypeTextures[type].Length), GUILayout.Height(_buttonScale));
    }

    // �����Ŏw�肳�ꂽ�v�f��I����Ԃɂ���
    private void SelectStageElementButton(StageTileType elementType, int selectedIndex)
    {
        // �I������Ă���v�f�Ɉ��������A����ȊO��-1�ɂ���
        for (int i = 0; i < _selectedStageElementButtonIndices.Length; ++i)
        {
            if ((int)elementType == i)
                _selectedStageElementButtonIndices[i] = selectedIndex;
            else
                _selectedStageElementButtonIndices[i] = -1;
        }

        // �I�����Ă���z�u�v�f��ݒ�
        _selectStageTile.TileType = elementType;
        _selectStageTile.ElementCount = selectedIndex;
    }

    // �����I������Ă��Ȃ��悤�ɂ���
    public void Initialization()
    {
        _selectedStageElementButtonIndices = new int[4];

        SelectStageElementButton(StageTileType.None, -1);
    }

    // �����I�����ĂȂ���Ԃɂ���
    public void NothingSelectStageTile()
    {
        _selectStageTile.TileType = StageTileType.None;
        _selectStageTile.ElementCount = 0;
    }
}