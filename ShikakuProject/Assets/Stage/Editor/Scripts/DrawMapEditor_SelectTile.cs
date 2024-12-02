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

        // 11/14�ǉ��F�`���[�g���A���v�f
        StageTileType typeTutorial = StageTileType.Tutorial;


        EditorGUILayout.LabelField("�z�u����I�u�W�F�N�g��I��", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("�n��");
        int selectedIndex0 = CheckSelectedIndex(typeGround, saveData);

        EditorGUILayout.LabelField("�ǁA��Q��");
        int selectedIndex1 = CheckSelectedIndex(typeObstacle, saveData);

        int selectedIndex2 = 0;
        int selectedIndex3 = 0;
        if (saveData.IsMobPlacement)
        {
            EditorGUILayout.LabelField("�v���C���[");
            selectedIndex2 = CheckSelectedIndex(typePlayer, saveData);

            EditorGUILayout.LabelField("�G");
            selectedIndex3 = CheckSelectedIndex(typeEnemy, saveData);
        }

        // 11/14�ǉ��F�`���[�g���A���v�f
        int selectedIndex4 = 0;
        if (saveData.StageObjectElementData.TutorialData.TextDatas.Length != 0)
        {
            EditorGUILayout.LabelField("�`���[�g���A���֘A");
            selectedIndex4 = CheckSelectedIndex(typeTutorial, saveData);
        }


        if (selectedIndex0 != _selectedStageElementButtonIndices[(int)typeGround]) SelectStageElementButton(typeGround, selectedIndex0);
        else if (selectedIndex1 != _selectedStageElementButtonIndices[(int)typeObstacle]) SelectStageElementButton(typeObstacle, selectedIndex1);

        else if (saveData.IsMobPlacement)
        {
            if (selectedIndex2 != _selectedStageElementButtonIndices[(int)typePlayer]) SelectStageElementButton(typePlayer, selectedIndex2);
            else if (selectedIndex3 != _selectedStageElementButtonIndices[(int)typeEnemy]) SelectStageElementButton(typeEnemy, selectedIndex3);
            
            // 11/14�ǉ��F�`���[�g���A���v�f
            else if (saveData.StageObjectElementData.TutorialData.TextDatas.Length != 0)
            {
                if (selectedIndex4 != _selectedStageElementButtonIndices[(int)typeTutorial])
                {
                    Debug.Log("haihaiiaiaoiiai");
                    SelectStageElementButton(typeTutorial, selectedIndex4);
                }
            }
        }

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

        int selectInt= GUILayout.SelectionGrid(_selectedStageElementButtonIndices[type], saveData.ElementTypeTextures[type], saveData.ElementTypeTextures[type].Length,
                             GUILayout.Width(_buttonScale * saveData.ElementTypeTextures[type].Length), GUILayout.Height(_buttonScale));

        return selectInt;
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
        _selectedStageElementButtonIndices = new int[6];

        SelectStageElementButton(StageTileType.None, -1);
    }

    // �����I�����ĂȂ���Ԃɂ���
    public void NothingSelectStageTile()
    {
        _selectStageTile.TileType = StageTileType.None;
        _selectStageTile.ElementCount = 0;
    }
}
