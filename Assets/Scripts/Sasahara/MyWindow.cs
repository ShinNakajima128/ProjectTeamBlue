using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapEditor : EditorWindow
{
    private Object blockPrefab;
    private float height; 
    private float width; 

    /// <summary>ウィンドウ表示</summary>
    [MenuItem("Window/MapEditorWindow")] //エディターのWindowメニューの下に追加される名前
    static void Open()
    {
        var window = GetWindow<MapEditor>();
        window.titleContent = new GUIContent("MapEditor");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("BlockPrefab : ", GUILayout.Width(110));
        blockPrefab = EditorGUILayout.ObjectField(blockPrefab, typeof(UnityEngine.Object), true);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mapのマスの数", GUILayout.Width(100));
        GUILayout.Label("Height :", GUILayout.Width(50));
        height = EditorGUILayout.FloatField(height);
        GUILayout.Label("Width :", GUILayout.Width(50));
        width = EditorGUILayout.FloatField(width);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

}
