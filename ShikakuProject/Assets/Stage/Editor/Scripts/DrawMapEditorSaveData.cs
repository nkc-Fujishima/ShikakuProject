using UnityEditor;
using UnityEngine;

public class DrawMapEditorSaveData : ScriptableObject
{
    // タイル画像の情報
    public Texture2D[][] ElementTypeTextures;

    // 配置する要素画像の情報
    public Texture2D ElementTypeTexture_Ground;
    public Texture2D ElementTypeTexture_Enemy;
    public Texture2D ElementTypeTexture_X;
    public Texture2D ElementTypeTexture_Flag;

    //パレットのデータ
    public StageObjectElementData StageObjectElementData = null;

    //ペイント結果の内容
    public StageMapData DrawMapData = null;
    private static readonly string SAVE_ASSET_PATH = "Assets/Stage/Editor/SctiptableObject/SaveData_DrawMapData/SaveData_DrawMapData.asset";

    // 複数の要素をテクスチャで見分けるようにする文字のテクスチャ
    public Texture2D[] NumberTextures;

    // 回転の情報を指し示すための矢印のテクスチャ
    public Texture2D UpArrowTexture;
    public Texture2D DiagonalArrowTexture;

    // ステージにプレイヤーを配置するかどうか
    public bool IsMobPlacement = true;


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

        SetTileDataToElement(selectX, selectY, 0);
    }

    public void TileRotate(int selectX, int selectY)
    {
        // TileTypeによって１クリックで回す角度を変える
        int rotate = 0;
        switch (GetTileTypeOnTileData(selectX, selectY))
        {
            case StageTileType.Enemy:
            case StageTileType.Player:

            // 11/14追加：チュートリアル要素
            case StageTileType.Tutorial:
                rotate = GetRotationOnTileData(selectX, selectY) + 45;
                break;


            case StageTileType.Ground:
            case StageTileType.Obstacle:
                rotate = GetRotationOnTileData(selectX, selectY) + 90;
                break;
        }

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
    // 今制作しているマップにプレイヤーが適量数存在するか
    public bool IsCheckOnePlayer()
    {
        bool isCountOne = false;

        for (int checkY = 0; checkY < DrawMapData.Y; ++checkY)
        {
            for (int checkX = 0; checkX < DrawMapData.X; ++checkX)
            {
                // プレイヤーでなければスキップ
                if (DrawMapData.TileDatas[checkX].TileData[checkY].TileType != StageTileType.Player) continue;

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
                if (DrawMapData.TileDatas[checkX].TileData[checkY].TileType != StageTileType.Enemy) continue;

                return true;
            }
        }

        return false;
    }
}