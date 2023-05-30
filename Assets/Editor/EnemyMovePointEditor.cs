using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(EnemyCheckPoint))]
public class EnemyMovePointEditor : Editor
{
    private ReorderableList reorderableList;

    //初期化reorderableList
    private void OnEnable()
    {
        if (Application.isPlaying) return;
        
        EnemyCheckPoint enemyCheckPoint = target as EnemyCheckPoint;

        //引っ張る処理を禁止(ReorderableListの第3関数でコントロール(bool)
        reorderableList = new ReorderableList(enemyCheckPoint.pointList, typeof(GameObject), false, true, true, true);

        //コールバック、リストの内容を変更する場合呼ばれる
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            GameObject go = enemyCheckPoint.pointList[index];

            //ボタンと文字長さを計算//EditorGUIUtility.singleLineHeight一行の長さ
            Rect buttonRect = new Rect(rect.x, rect.y + 2, 20, EditorGUIUtility.singleLineHeight);
            Rect objectRect = new Rect(rect.x + 25, rect.y + 2, rect.width - 25, EditorGUIUtility.singleLineHeight);

            //削除ボタンを作成、描画
            if (GUI.Button(buttonRect, "-"))
            {
                //押された場合、その行のGameObjcetを削除して、Listから削除後、Listを更新
                DestroyImmediate(go);
                reorderableList.list.RemoveAt(index);
                if (reorderableList != null)
                {
                    if (reorderableList.count > 0)
                    {
                        reorderableList.serializedProperty.arraySize--;

                    }else
                    {
                        enemyCheckPoint.RemoveMovePointArray();
                    }
                }

            }

            if (enemyCheckPoint.pointList.Count>index)
                //描画，GameObjectを表示)
                enemyCheckPoint.pointList[index] = EditorGUI.ObjectField(objectRect, go, typeof(GameObject), true) as GameObject;
        };

        //項目削除のコールバック関数の中身を作成
        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            //index
            int index = list.index;

            //indexは問題がなかったら、その項目を作成し、更新項目数
            if (index >= 0 && index < list.count)
            {
                list.list.RemoveAt(index);
                list.serializedProperty.arraySize--;
            }
        };
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying) return;
        DrawDefaultInspector();

        EnemyCheckPoint enemyCheckPoint = target as EnemyCheckPoint;


        if (GUILayout.Button("AddPoint!"))
        {

            enemyCheckPoint.AddNewPoint();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //リストを再描画
        reorderableList.DoLayoutList();
    }
}
