using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class DrawMapEditor : EditorWindow
{
    // �ŏ��ɊJ�����Ƃ��ɑO�̍Ō�̏�Ԃ�ǂݍ��ނ��߂̃X�N���v�^�u���I�u�W�F�N�g
    [SerializeField] DrawMapEditorSaveData _saveData = null;
    private static readonly string SAVE_ASSET_PATH = "Assets/Stage/Editor/SctiptableObject/SaveData.asset";

    // �^�C����I������X�N���v�g
    private DrawMapEditor_SelectTile _selectTile;

    // �}�b�v�𐧍삷��X�N���v�g
    private DrawMapEditor_MadeMap _madeMap;

    // �ԍ��������摜�𐶐�����X�N���v�g
    private DrawMapEditor_TextureProcessing _textureProcessing;


    [MenuItem("Editor/DrawMapEditor")]
    static public void Open()
    {
        var window = EditorWindow.GetWindow<DrawMapEditor>();

        //�O��̍Ō�̏�Ԃ����[�h
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
        window._madeMap = new();
        window._textureProcessing = new(window._saveData.NumberTextures);

        window.Oninitialization();
        window._saveData.SurchDrawMapData();

        window._madeMap.SetAllMapTextures(window._saveData);
    }


    // ������
    private readonly EditorGUISplitView horizontalSplitView = new (EditorGUISplitView.Direction.Horizontal);

    private void OnGUI()
    {
        horizontalSplitView.BeginSplitView();

        // �p���b�g
        DrawToolbar();
        SetStageObjectElementData();
        _selectTile.SelectElement(_saveData);

        AllFillButton();

        // ���s
        horizontalSplitView.Split();

        // �y�C���g
        _madeMap.SelectDrawPaint(_saveData, _selectTile.SelectStageTile);

        horizontalSplitView.EndSplitView();
        Repaint();
    }


    //-------------------------------------------------------------------------------------
    // ��̃c�[���o�[��\������
    private void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.ExpandWidth(true)))
        {
            if (GUILayout.Button("�V�K�쐬", EditorStyles.toolbarButton))
            {
                ResetMapWindow.Open(_saveData.DrawMapData.X, _saveData.DrawMapData.Y);
            }
            if (GUILayout.Button("���[�h", EditorStyles.toolbarButton))
            {
                SerchMapWindow.Open();
            }
            if (GUILayout.Button("�Z�[�u", EditorStyles.toolbarButton))
            {
                // �v���C���[�����Ȃ��A�������͕����ݒ肳��Ă���ۑ����Ȃ�
                if (!_saveData.IsCheckOnePlayer())
                {
                    EditorUtility.DisplayDialog("Warning", "�v���C���[���z�u����ĂȂ���\n�����̔z�u���ꂽ��ԂɂȂ��Ă��" +
                                                         "\n\n�K���P�̂����ɂȂ�悤�ɔz�u���Ă�", "OK");
                    return;
                }

                // �G���z�u����ĂȂ�������ۑ����Ȃ�
                if (!_saveData.IsCheckExistenceEnemy())
                {
                    EditorUtility.DisplayDialog("Warning", "�G����̂��z�u����ĂȂ���" +
                                                         "\n\n�K���P�̈ȏ�z�u���Ă�", "OK");
                    return;
                }

                SaveStageMap();
            }
        }
    }

    //-------------------------------------------------------------------------------------
    // �h��Ԃ�������{�^��
    private void AllFillButton()
    {
        EditorGUILayout.LabelField("�I�����Ă�v�f�œh��Ԃ�");

        if (GUILayout.Button("�h��Ԃ�", EditorStyles.toolbarButton))
        {
            if (_saveData.DrawMapData.X == 0 || _saveData.DrawMapData.Y == 0)
            {
                EditorUtility.DisplayDialog("Warning", "�}�b�v��V�K�쐬���Ă�", "OK");
                return;
            }

            _madeMap.AllFillButton(_saveData, _selectTile.SelectStageTile);
        }
    }


    //-------------------------------------------------------------------------------------
    // �z�u����v�f��ێ����Ă���X�N���v�^�u���I�u�W�F�N�g���擾
    private void SetStageObjectElementData()
    {
        EditorGUI.BeginChangeCheck();

        // ScriptableObject��ݒ肷��t�B�[���h
        _saveData.StageObjectElementData = (StageObjectElementData)EditorGUILayout.ObjectField("�z�u�v�f�̃f�[�^", _saveData.StageObjectElementData, typeof(StageObjectElementData), false);

        if (EditorGUI.EndChangeCheck())
        {
            // �ύX�𔽉f
            Oninitialization();
        }
    }


    //-------------------------------------------------------------------------------------
    // �G�f�B�^���J�������A�X�N���v�^�u���I�u�W�F�N�g���ύX���ꂽ�Ƃ��ɏ���������@StageTileType���g�p
    private void Oninitialization()
    {
        _saveData.ElementTypeTextures = new Texture[4][];

        _selectTile.Initialization();

        if (_saveData.StageObjectElementData == null) return;

        for (int elementCount = 0; elementCount < _saveData.ElementTypeTextures.Length; ++elementCount)
        {
            int length = 0;
            Texture2D elementTexture = null;

            switch ((StageTileType)Enum.ToObject(typeof(StageTileType), elementCount))
            {
                case StageTileType.Ground:
                    length = _saveData.StageObjectElementData.GroundData.Prefabs.Length;
                    elementTexture = _saveData.StageObjectElementData.GroundData.Texture;
                    break;
                case StageTileType.Obstacle:
                    length = _saveData.StageObjectElementData.ObstacleData.Prefabs.Length;
                    elementTexture = _saveData.StageObjectElementData.ObstacleData.Texture;
                    break;
                case StageTileType.Player:
                    length = 1;
                    elementTexture = _saveData.StageObjectElementData.PlayerData.Texture;
                    break;
                case StageTileType.Enemy:
                    length = _saveData.StageObjectElementData.EnemyDatas.EnemyPlefabs.Length;
                    elementTexture = _saveData.StageObjectElementData.EnemyDatas.Texture;
                    break;
            }

            // �e�N�X�`����ݒ�
            _saveData.ElementTypeTextures[elementCount] = InitializeTextures(elementTexture, length);
        }
    }

    //-------------------------------------------------------------------------------------
    // �摜�ɕ�������������
    private Texture[] InitializeTextures(Texture2D baseTexture, int prefabLength)
    {
        Texture[] textureList = _textureProcessing.TextureCreate(prefabLength, baseTexture);

        return textureList;
    }


    //-------------------------------------------------------------------------------------
    // �X�e�[�W���Z�[�u����
    public void SaveStageMap()
    {

        // �ۑ���̃t�@�C���p�X���擾����
        var filePath = EditorUtility.SaveFilePanel("Save", "Assets", "StageData", "asset");

        string[] hogePath = Regex.Split(filePath, "/Assets/");
        string assetPath = "Assets/" + hogePath[1];

        // �p�X�������Ă���ΑI�����ꂽ�Ƃ������Ɓi�L�����Z�����ꂽ������Ă��Ȃ��j
        if (!string.IsNullOrEmpty(filePath))
        {
            //�ۑ�����

            //�ύX��������
            AssetDatabase.StartAssetEditing();

            StageMapData saveAsset = new ();
            AssetDatabase.CreateAsset((ScriptableObject)saveAsset, assetPath);
            saveAsset.X = _saveData.DrawMapData.X;
            saveAsset.Y = _saveData.DrawMapData.Y;

            saveAsset.ResetArray();

            saveAsset.TileDatas = _saveData.DrawMapData.TileDatas;


            //�ύX�����܂�
            AssetDatabase.StopAssetEditing();

            EditorUtility.SetDirty(saveAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    //-------------------------------------------------------------------------------------
    // ���[�h����
    public static void LoadMap(StageMapData loadAsset)
    {
        if (loadAsset == null) return;

        var window = EditorWindow.GetWindow<DrawMapEditor>();

        window._saveData.DrawMapData.X = loadAsset.X;
        window._saveData.DrawMapData.Y = loadAsset.Y;

        window._saveData.DrawMapData.ResetArray();

        window._saveData.DrawMapData.TileDatas = loadAsset.TileDatas;

        window.Oninitialization();
        window._saveData.SurchDrawMapData();

        window._madeMap.SetAllMapTextures(window._saveData);
    }

    //-------------------------------------------------------------------------------------
    // �V�K�쐬����
    public static void InstanceMap(int scaleX,int scaleY)
    {
        var window = EditorWindow.GetWindow<DrawMapEditor>();

        window._saveData.DrawMapData.X = scaleX;
        window._saveData.DrawMapData.Y = scaleY;

        window._selectTile.NothingSelectStageTile();

        window._madeMap.Initialization(window._saveData);
    }
}

// �}�b�v���̐V�K�쐬���
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

        if (GUILayout.Button("�쐬�I", EditorStyles.toolbarButton))
        {
            DrawMapEditor.InstanceMap(_scaleX, _scaleY);
            var window = GetWindow<ResetMapWindow>();
            window.Close();
        }


        if (GUILayout.Button("�L�����Z��", EditorStyles.toolbarButton))
        {
            var window = GetWindow<ResetMapWindow>();
            window.Close();
        }
    }
}

// �}�b�v���̃��[�h���
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


        if (GUILayout.Button("���[�h�I", EditorStyles.toolbarButton))
        {
            DrawMapEditor.LoadMap(_stageMap);
            var window = GetWindow<SerchMapWindow>();
            window.Close();
        }


        if (GUILayout.Button("�L�����Z��", EditorStyles.toolbarButton))
        {
            var window = GetWindow<SerchMapWindow>();
            window.Close();
        }
    }
}