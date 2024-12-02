using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DrawMapEditor : EditorWindow
{
    // 最初に開いたときに前の最後の状態を読み込むためのスクリプタブルオブジェクト
    [SerializeField] DrawMapEditorSaveData _saveData = null;
    private static readonly string SAVE_ASSET_PATH = "Assets/Stage/Editor/SctiptableObject/SaveData.asset";

    // タイルを選択するスクリプト
    private DrawMapEditor_SelectTile _selectTile;

    // マップを制作するスクリプト
    private DrawMapEditor_MadeMap _madeMap;

    // 番号がついた画像を生成するスクリプト
    private DrawMapEditor_TextureProcessing _textureProcessing;


    [MenuItem("Editor/DrawMapEditor")]
    static public void Open()
    {
        var window = EditorWindow.GetWindow<DrawMapEditor>();

        //前回の最後の状態をロード
        var saveAsset = AssetDatabase.LoadAssetAtPath<DrawMapEditorSaveData>(SAVE_ASSET_PATH);
        if (saveAsset == null)
        {
            saveAsset = CreateInstance<DrawMapEditorSaveData>();
            AssetDatabase.CreateAsset(saveAsset, SAVE_ASSET_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            window._saveData = saveAsset;
        }
        else
        {
            window._saveData = saveAsset;
        }

        window._selectTile = new();
        window._textureProcessing = new(window._saveData.NumberTextures,
                                        window._saveData.UpArrowTexture, window._saveData.DiagonalArrowTexture);
        window._madeMap = new(window._textureProcessing);

        window.Oninitialization();
        window._saveData.SurchDrawMapData();

        window._madeMap.SetAllMapTextures(window._saveData);
    }


    // 横並び
    private readonly EditorGUISplitView horizontalSplitView = new (EditorGUISplitView.Direction.Horizontal);

    private void OnGUI()
    {
        horizontalSplitView.BeginSplitView();

        // パレット
        DrawToolbar();

        MobPlacementButton();

        SetStageObjectElementData();
        _selectTile.SelectElement(_saveData);

        EditorGUILayout.LabelField("ステージ塗りツール");
        AllFillButton();

        EditorGUILayout.LabelField("その他設定項目");
        MadeWaypointButton();

        // 改行
        horizontalSplitView.Split();

        // ペイント
        _madeMap.SelectDrawPaint(_saveData, _selectTile.SelectStageTile);

        horizontalSplitView.EndSplitView();
        Repaint();
    }


    //-------------------------------------------------------------------------------------
    // 上のツールバーを表示する
    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
        {
            if (GUILayout.Button("新規作成", EditorStyles.toolbarButton))
            {
                ResetMapWindow.Open(_saveData.DrawMapData.X, _saveData.DrawMapData.Y);
            }
            if (GUILayout.Button("ロード", EditorStyles.toolbarButton))
            {
                SerchMapWindow.Open();
            }
            if (GUILayout.Button("セーブ", EditorStyles.toolbarButton))
            {
                if (_saveData.IsMobPlacement)
                {
                    // プレイヤーがいない、もしくは複数設定されてたら保存しない
                    if (!_saveData.IsCheckOnePlayer())
                    {
                        EditorUtility.DisplayDialog("Warning", "プレイヤーが配置されてないか\n複数体配置された状態になってるよ" +
                                                             "\n\n必ず１体だけになるように配置してね", "OK");
                        return;
                    }

                    // 敵が配置されてなかったら保存しない
                    if (!_saveData.IsCheckExistenceEnemy())
                    {
                        EditorUtility.DisplayDialog("Warning", "敵が一体も配置されてないよ" +
                                                             "\n\n必ず１体以上配置してね", "OK");
                        return;
                    }
                }

                SaveStageMap();
            }
        }
    }

    //-------------------------------------------------------------------------------------
    // 塗りつぶしをするボタン
    private void AllFillButton()
    {
        if (GUILayout.Button("選択してる要素で塗りつぶし"))
        {
            if (_saveData.DrawMapData.X == 0 || _saveData.DrawMapData.Y == 0)
            {
                EditorUtility.DisplayDialog("Warning", "マップを新規作成してね", "OK");
                return;
            }

            _madeMap.AllFillButton(_saveData, _selectTile.SelectStageTile);
        }
    }


    //-------------------------------------------------------------------------------------
    // モブを配置する場合のボタン
    private void MobPlacementButton()
    {
        _saveData.IsMobPlacement = EditorGUILayout.Toggle("プレイヤー、敵を配置する", _saveData.IsMobPlacement);
    }

    //-------------------------------------------------------------------------------------
    // 巡回ポイントを設定する画面に移動するボタン
    private void MadeWaypointButton()
    {
        if (GUILayout.Button("巡回ポイントを設定"))
        {
            if (_saveData.DrawMapData.X == 0 || _saveData.DrawMapData.Y == 0) return;

            DrawMapEditor_MadeWaypoint.Open(_saveData);
        }
    }

    //-------------------------------------------------------------------------------------
    // 配置する要素を保持しているスクリプタブルオブジェクトを取得
    private void SetStageObjectElementData()
    {
        EditorGUI.BeginChangeCheck();

        // ScriptableObjectを設定するフィールド
        _saveData.StageObjectElementData = (StageObjectElementData)EditorGUILayout.ObjectField("配置要素のデータ", _saveData.StageObjectElementData, typeof(StageObjectElementData), false);

        if (EditorGUI.EndChangeCheck())
        {
            // 変更を反映
            Oninitialization();
        }
    }


    //-------------------------------------------------------------------------------------
    // エディタを開いた時、スクリプタブルオブジェクトが変更されたときに初期化する　StageTileTypeを使用
    private void Oninitialization()
    {
        _saveData.ElementTypeTextures = new Texture2D[6][];

        _selectTile.Initialization();

        if (_saveData.StageObjectElementData == null) return;

        for (int elementCount = 0; elementCount < _saveData.ElementTypeTextures.Length; ++elementCount)
        {
            int length = 0;
            Texture2D elementTexture = null;
            Texture2D[] baseTexture = null;

            switch ((StageTileType)Enum.ToObject(typeof(StageTileType), elementCount))
            {
                case StageTileType.Ground:
                    length = _saveData.StageObjectElementData.GroundData.Prefabs.Length;
                    elementTexture = null;
                    baseTexture = new Texture2D[length];
                    for (int i = 0; i < length; ++i)
                        baseTexture[i] = AssetPreview.GetAssetPreview(_saveData.StageObjectElementData.GroundData.Prefabs[i]);
                    break;

                case StageTileType.Obstacle:
                    length = _saveData.StageObjectElementData.ObstacleData.Prefabs.Length;
                    elementTexture = _saveData.StageObjectElementData.ObstacleData.Texture;
                    baseTexture = new Texture2D[length];
                    for (int i = 0; i < length; ++i)
                        baseTexture[i] = AssetPreview.GetAssetPreview(_saveData.StageObjectElementData.ObstacleData.Prefabs[i]);
                    break;

                case StageTileType.Player:
                    length = 1;
                    elementTexture = _saveData.StageObjectElementData.PlayerData.Texture;
                    baseTexture = new Texture2D[length];
                    for (int i = 0; i < length; ++i)
                        baseTexture[i] = AssetPreview.GetAssetPreview(_saveData.StageObjectElementData.PlayerData.PlayerPlefab.gameObject);
                    break;

                case StageTileType.Enemy:
                    length = _saveData.StageObjectElementData.EnemyDatas.EnemyPlefabs.Length;
                    elementTexture = _saveData.StageObjectElementData.EnemyDatas.Texture;
                    baseTexture = new Texture2D[length];
                    for (int i = 0; i < length; ++i)
                        baseTexture[i] = AssetPreview.GetAssetPreview(_saveData.StageObjectElementData.EnemyDatas.EnemyPlefabs[i].gameObject);
                    break;

                // 11/14追加：チュートリアル要素
                case StageTileType.Tutorial:
                    if (_saveData.StageObjectElementData.TutorialData.TextDatas.Length == 0) continue;
                    length = _saveData.StageObjectElementData.TutorialData.TextDatas.Length;
                    elementTexture = _saveData.StageObjectElementData.TutorialData.Texture;
                    baseTexture = new Texture2D[length];
                    for (int i = 0; i < length; ++i)
                        baseTexture[i] = AssetPreview.GetAssetPreview(_saveData.StageObjectElementData.TutorialData.GetGameObject(i));
                    break;

            }
            // テクスチャを設定
            _saveData.ElementTypeTextures[elementCount] = InitializeTextures(baseTexture, length, elementTexture);
        }
    }

    //-------------------------------------------------------------------------------------
    // 画像に複数数字を入れる
    private Texture2D[] InitializeTextures(Texture2D[] baseTexture, int prefabLength, Texture2D elementTexture)
    {
        Texture2D[] textures = new Texture2D[prefabLength];
        for (int i = 0; i < prefabLength; ++i)
        {
            textures[i] = _textureProcessing.ResizeTexture(baseTexture[i], 512, 512);
        }

        Texture2D[] textureList = _textureProcessing.TextureCreate(prefabLength, textures, elementTexture);

        return textureList;
    }

    //-------------------------------------------------------------------------------------
    // ステージをセーブする
    public void SaveStageMap()
    {

        // 保存先のファイルパスを取得する
        var filePath = EditorUtility.SaveFilePanel("Save", "Assets", "StageData", "asset");


        // パスが入っていれば選択されたということ（キャンセルされたら入ってこない）
        if (!string.IsNullOrEmpty(filePath))
        {
            string[] hogePath = Regex.Split(filePath, "/Assets/");
            string assetPath = "Assets/" + hogePath[1];

            //保存処理

            //変更ここから
            AssetDatabase.StartAssetEditing();

            StageMapData saveAsset = CreateInstance<StageMapData>();
            AssetDatabase.CreateAsset(saveAsset, assetPath);

            saveAsset.CopyTileData(_saveData.DrawMapData);

            saveAsset.CopyWaypointData(_saveData.DrawMapData);

            //変更ここまで
            AssetDatabase.StopAssetEditing();

            EditorUtility.SetDirty(saveAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    //-------------------------------------------------------------------------------------
    // ロードする
    public static void LoadMap(StageMapData loadAsset)
    {
        if (loadAsset == null) return;

        var window = EditorWindow.GetWindow<DrawMapEditor>();

        window._saveData.DrawMapData.X = loadAsset.X;
        window._saveData.DrawMapData.Y = loadAsset.Y;

        window._saveData.DrawMapData.CopyTileData(loadAsset);

        window.Oninitialization();
        window._saveData.SurchDrawMapData();

        window._madeMap.SetAllMapTextures(window._saveData);
    }

    //-------------------------------------------------------------------------------------
    // 新規作成する
    public static void InstanceMap(int scaleX,int scaleY)
    {
        var window = EditorWindow.GetWindow<DrawMapEditor>();

        window._saveData.DrawMapData.X = scaleX;
        window._saveData.DrawMapData.Y = scaleY;

        window._selectTile.NothingSelectStageTile();

        window._madeMap.Initialization(window._saveData);
    }
}

// マップ情報の新規作成画面
public class ResetMapWindow : EditorWindow
{
    static ResetMapWindow resetWindow;

    int _scaleX, _scaleY;

    public static void Open(int scaleX,int scaleY)
    {

        if (resetWindow == null)
        {
            resetWindow = CreateInstance<ResetMapWindow>();

            resetWindow._scaleX = scaleX;
            resetWindow._scaleY = scaleY;
        }
        resetWindow.ShowUtility();
        resetWindow.maxSize = new Vector2(300, 80);
        resetWindow.minSize = new Vector2(300, 80);
    }

    private void OnGUI()
    {
        int leftValue = 0;
        int rightValue = 64;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("X: ", GUILayout.Width(16));
        _scaleX = EditorGUILayout.IntSlider(_scaleX, leftValue, rightValue);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Y: ", GUILayout.Width(16));
        _scaleY = EditorGUILayout.IntSlider(_scaleY, leftValue, rightValue);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("作成！", EditorStyles.toolbarButton))
        {
            DrawMapEditor.InstanceMap(_scaleX, _scaleY);
            var window = GetWindow<ResetMapWindow>();
            window.Close();
        }


        if (GUILayout.Button("キャンセル", EditorStyles.toolbarButton))
        {
            var window = GetWindow<ResetMapWindow>();
            window.Close();
        }
    }
}

// マップ情報のロード画面
public class SerchMapWindow : EditorWindow
{
    StageMapData _stageMap;
    static SerchMapWindow serchWindow;

    public static void Open()
    {
        if (serchWindow == null)
        {
            serchWindow = CreateInstance<SerchMapWindow>();
        }
        serchWindow.ShowUtility();
        serchWindow.maxSize = new Vector2(300, 64);
        serchWindow.minSize = new Vector2(300, 64);
    }

    private void OnGUI()
    {
        _stageMap = EditorGUILayout.ObjectField("StageTileData", _stageMap, typeof(StageMapData), true) as StageMapData;


        if (GUILayout.Button("ロード！", EditorStyles.toolbarButton))
        {
            DrawMapEditor.LoadMap(_stageMap);
            var window = GetWindow<SerchMapWindow>();
            window.Close();
        }


        if (GUILayout.Button("キャンセル", EditorStyles.toolbarButton))
        {
            var window = GetWindow<SerchMapWindow>();
            window.Close();
        }
    }
}