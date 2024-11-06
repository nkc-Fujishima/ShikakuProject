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
        // �J�X�^���X�^�C���̐ݒ�
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
        if (GUILayout.Button("�v�f��ǉ�"))
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


            _isOpenWaypoint[i] = EditorGUILayout.BeginFoldoutHeaderGroup(_isOpenWaypoint[i], ($"�v�f�F{i}"));

            EditorGUILayout.EndFoldoutHeaderGroup();


            if (_isOpenWaypoint[i])
            {
                EditorGUILayout.BeginVertical(_boxStyle);

                // ����|�C���g
                EditorGUILayout.PropertyField(propertyWaypoint, new GUIContent("����|�C���g(Waypoint)"), true);

                if (GUILayout.Button("����|�C���g���}�b�v����ݒ�"))
                {
                    MadeWaypoint_MapWindow.Open(_saveData, i, true);
                }
                
                EditorGUILayout.Space();


                // ���񂳂���G
                EditorGUILayout.PropertyField(propertyEnemyAtPoint, new GUIContent("���񂷂�G���z�u����Ă�����W(EnemyAtPoint)"), true);

                if (GUILayout.Button("�G�̍��W���}�b�v����ݒ�"))
                {
                    MadeWaypoint_MapWindow.Open(_saveData, i, false);
                }

                EditorGUILayout.Space();
                GUILayout.Box("", GUILayout.Height(5), GUILayout.ExpandWidth(true));


                // �폜�p�{�^��
                bool isRemove = false;
                if (GUILayout.Button("�폜"))
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

    // �{�b�N�X�̐F���C������
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