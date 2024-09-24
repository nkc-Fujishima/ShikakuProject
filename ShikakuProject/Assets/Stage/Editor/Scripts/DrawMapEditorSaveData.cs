using UnityEditor;
using UnityEngine;

public class DrawMapEditorSaveData : ScriptableObject
{
    // タイル画像の情報
    public Texture[][] ElementTypeTextures;

    //パレットのデータ
    public StageObjectElementData StageObjectElementData = null;

    //ペイント結果の内容
    public StageMapData DrawMapData = null;
    private static readonly string SAVE_ASSET_PATH = "Assets/Stage/Editor/SctiptableObject/SaveData_DrawMapData/SaveData_DrawMapData.asset";

    // 複数の要素をテクスチャで見分けるようにする文字のテクスチャ
    public Texture2D[] NumberTextures;


    public void SurchDrawMapData()
    {
        if (DrawMapData != null) return;

        // ペイント用のデータファイルをロード
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
    }

    public StageTileType GetTileTypeOnTileData(int selectX, int selectY)
    {
        return DrawMapData.TileDatas[selectX].TileData[selectY].TileType;
    }

    public int GetElementCountOnTileData(int selectX, int selectY)
    {
        return DrawMapData.TileDatas[selectX].TileData[selectY].ElementCount;
    }


    //-----------------------------------------------------------------------------------------------------
    // 今制作しているマップにプレイヤーが適量数存在するか
    public bool IsCheckOnePlayer()
    {
        bool isCountOne = false;

        for (int checkY = 0; checkY < DrawMapData.Y; ++checkY)
        {
            for (int checkX = 0; checkX < DrawMapData.X; ++checkX)
            {
                // プレイヤーでなければスキップ
                if (DrawMapData.DotsMapTile[checkX].TileData[checkY].TileType != StageTileType.Player) continue;

                // 既にtureの場合はプレイヤーが複数存在しているのでfalseで返す
                if (isCountOne == true) return false;

                isCountOne = true;
            }
        }

        return isCountOne;
    }

    //-----------------------------------------------------------------------------------------------------
    // 今制作しているマップに敵が適量数存在するか
    public bool IsCheckExistenceEnemy()
    {
        for (int checkY = 0; checkY < DrawMapData.Y; ++checkY)
        {
            for (int checkX = 0; checkX < DrawMapData.X; ++checkX)
            {
                // 敵でなければスキップ
                if (DrawMapData.DotsMapTile[checkX].TileData[checkY].TileType != StageTileType.Enemy) continue;

                return true;
            }
        }

        return false;
    }
}
