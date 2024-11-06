using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawMapEditor_MadeWaypoint : EditorWindow
{
    private DrawMapEditorSaveData _saveData = null;

    private SerializedObject _serializedObject;
    private SerializedProperty _waypointDataProperty;

    private GUIStyle _boxStyle;

    private readonly List<bool> _isOpenWaypoint = new();
    
    private Vector2 _scrollPosisition;

    static public void Open(DrawMapEditorSaveData saveData)
    {
        DrawMapEditor_MadeWaypoint window = CreateInstance<DrawMapEditor_MadeWaypoint>();
        window._saveData = saveData;

        window._serializedObject = new (window._saveData.DrawMapData);
        window._waypointDataProperty = window._serializedObject.FindProperty("WaypointData");

        for (int i = 0; i < window._waypointDataProperty.arraySize; ++i)
        {
            window._isOpenWaypoint.Add(true);
        }

        window.ShowUtility();
    }

    private void OnGUI()
    {
        // カスタムスタイルの設定
        if (_boxStyle == null)
        {
            _boxStyle = new GUIStyle(GUI.skin.box);
            _boxStyle.normal.background = MakeTex(2, 2, new Color(0.8f, 0.8f, 0.8f, 1.0f));
        }

        _serializedObject.Update();

        AddButton();

        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosisition))
        {
            _scrollPosisition = scrollView.scrollPosition;
            SetWaypointData();
        }

        _serializedObject.ApplyModifiedProperties();
    
    }


    private void AddButton()
    {
        if (GUILayout.Button("要素を追加"))
        {
            _saveData.DrawMapData.WaypointData.Add(new());

            _isOpenWaypoint.Add(true);
        }
    }

    private void SetWaypointData()
    {
        for (int i = 0; i < _waypointDataProperty.arraySize; i++)
        {
            SerializedProperty element = _waypointDataProperty.GetArrayElementAtIndex(i);
            SerializedProperty propertyWaypoint = element.FindPropertyRelative("Waypoint");
            SerializedProperty propertyEnemyAtPoint = element.FindPropertyRelative("EnemyAtPoint");


            _isOpenWaypoint[i] = EditorGUILayout.BeginFoldoutHeaderGroup(_isOpenWaypoint[i], ($"要素：{i}"));

            EditorGUILayout.EndFoldoutHeaderGroup();


            if (_isOpenWaypoint[i])
            {
                EditorGUILayout.BeginVertical(_boxStyle);

                // 巡回ポイント
                EditorGUILayout.PropertyField(propertyWaypoint, new GUIContent("巡回ポイント(Waypoint)"), true);

                if (GUILayout.Button("巡回ポイントをマップから設定"))
                {
                    MadeWaypoint_MapWindow.Open(_saveData, i, true);
                }
                
                EditorGUILayout.Space();


                // 巡回させる敵
                EditorGUILayout.PropertyField(propertyEnemyAtPoint, new GUIContent("巡回する敵が配置されている座標(EnemyAtPoint)"), true);

                if (GUILayout.Button("敵の座標をマップから設定"))
                {
                    MadeWaypoint_MapWindow.Open(_saveData, i, false);
                }

                EditorGUILayout.Space();
                GUILayout.Box("", GUILayout.Height(5), GUILayout.ExpandWidth(true));


                // 削除用ボタン
                bool isRemove = false;
                if (GUILayout.Button("削除"))
                {
                    isRemove = true;

                    _saveData.DrawMapData.WaypointData.RemoveAt(i);

                    _isOpenWaypoint.RemoveAt(i);
                }

                EditorGUILayout.EndVertical();

                if (isRemove) break;
            }
            
            EditorGUILayout.Space();
        }
    }

    // ボックスの色を修正する
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}