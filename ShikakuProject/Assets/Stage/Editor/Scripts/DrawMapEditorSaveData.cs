using UnityEditor;
using UnityEngine;

public class DrawMapEditorSaveData : ScriptableObject
{
    // �^�C���摜�̏��
    public Texture2D[][] ElementTypeTextures;

    //�p���b�g�̃f�[�^
    public StageObjectElementData StageObjectElementData = null;

    //�y�C���g���ʂ̓��e
    public StageMapData DrawMapData = null;
    private static readonly string SAVE_ASSET_PATH = "Assets/Stage/Editor/SctiptableObject/SaveData_DrawMapData/SaveData_DrawMapData.asset";

    // �����̗v�f���e�N�X�`���Ō�������悤�ɂ��镶���̃e�N�X�`��
    public Texture2D[] NumberTextures;

    // ��]�̏����w���������߂̖��̃e�N�X�`��
    public Texture2D UpArrowTexture;
    public Texture2D DiagonalArrowTexture;


    public void SurchDrawMapData()
    {
        if (DrawMapData != null) return;

        // �y�C���g�p�̃f�[�^�t�@�C�������[�h
        var saveAsset = AssetDatabase.LoadAssetAtPath<StageMapData>(SAVE_ASSET_PATH);
        if (saveAsset == null)
        {
            saveAsset = CreateInstance<StageMapData>();
            AssetDatabase.CreateAsset(saveAsset, SAVE_ASSET_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            DrawMapData = saveAsset;
        }
        else
        {
            DrawMapData = saveAsset;
        }
    }

    public void ResetMap(int scaleX,int scaleY)
    {
        DrawMapData.X = scaleX;
        DrawMapData.Y = scaleY;

        DrawMapData.ResetArray();
    }


    public void SetTileDataToElement(int selectX, int selectY, StageTileType tileType, int elementCount)
    {
        DrawMapData.TileDatas[selectX].TileData[selectY].TileType = tileType;
        DrawMapData.TileDatas[selectX].TileData[selectY].ElementCount = elementCount;

        SetTileDataToElement(selectX, selectY, 0);
    }

    public void TileRotate(int selectX, int selectY)
    {
        if (GetTileTypeOnTileData(selectX, selectY) != StageTileType.Enemy &&
            GetTileTypeOnTileData(selectX, selectY) != StageTileType.Player) return;

        int rotate = GetRotationOnTileData(selectX, selectY) + 45;

        if (rotate >= 360)
            rotate = 0;

        SetTileDataToElement(selectX, selectY, rotate);
    }

    public void SetTileDataToElement(int selectX, int selectY, int rotate)
    {
        DrawMapData.TileDatas[selectX].TileData[selectY].RotationY = rotate;
    }


    public StageTileType GetTileTypeOnTileData(int selectX, int selectY)
    {
        return DrawMapData.TileDatas[selectX].TileData[selectY].TileType;
    }

    public int GetElementCountOnTileData(int selectX, int selectY)
    {
        return DrawMapData.TileDatas[selectX].TileData[selectY].ElementCount;
    }

    public int GetRotationOnTileData(int selectX, int selectY)
    {
        return DrawMapData.TileDatas[selectX].TileData[selectY].RotationY;
    }


    //-----------------------------------------------------------------------------------------------------
    // �����삵�Ă���}�b�v�Ƀv���C���[���K�ʐ����݂��邩
    public bool IsCheckOnePlayer()
    {
        bool isCountOne = false;

        for (int checkY = 0; checkY < DrawMapData.Y; ++checkY)
        {
            for (int checkX = 0; checkX < DrawMapData.X; ++checkX)
            {
                // �v���C���[�łȂ���΃X�L�b�v
                if (DrawMapData.TileDatas[checkX].TileData[checkY].TileType != StageTileType.Player) continue;

                // ����ture�̏ꍇ�̓v���C���[���������݂��Ă���̂�false�ŕԂ�
                if (isCountOne == true) return false;

                isCountOne = true;
            }
        }

        return isCountOne;
    }

    //-----------------------------------------------------------------------------------------------------
    // �����삵�Ă���}�b�v�ɓG���K�ʐ����݂��邩
    public bool IsCheckExistenceEnemy()
    {
        for (int checkY = 0; checkY < DrawMapData.Y; ++checkY)
        {
            for (int checkX = 0; checkX < DrawMapData.X; ++checkX)
            {
                // �G�łȂ���΃X�L�b�v
                if (DrawMapData.TileDatas[checkX].TileData[checkY].TileType != StageTileType.Enemy) continue;

                return true;
            }
        }

        return false;
    }
}
