using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/*Unityエディタは、EnemyCheckPointクラスのオブジェクトを選択したときに、
 * このクラスのOnInspectorGUI()メソッドを呼び出して、
 * カスタムインスペクターを描画します。
 */
[CustomEditor(typeof(EnemyCheckPoint))]

public class EnemyMovePointEditor : Editor//敵移動ポイント管理エディター
{
    //Unityエディタ拡張機能で使用されるクラスで、リストの要素を編集するためのカスタムUIを提供します。
    private ReorderableList reorderableList;

    //初期化reorderableList
    private void OnEnable()
    {
        if (Application.isPlaying) return;// アプリケーションが実行中の場合は処理をしない

        EnemyCheckPoint enemyCheckPoint = target as EnemyCheckPoint;// 対象となるEnemyCheckPointクラスのオブジェクトを取得

        //引っ張る処理を禁止(ReorderableListの第3関数でコントロール(bool)
        //ReorderableList(
        //編集するリスト,
        //リスト内の要素の型,
        //リスト内の要素をドラッグして並べ替えることができるかどうかを示すフラグ,
        //リストのヘッダーを表示するかどうかを示すフラグ,
        //リストに新しい要素を追加することができるかどうかを示すフラグ
        //リストから要素を削除することができるかどうかを示すフラグ
        //)
        reorderableList = new ReorderableList(enemyCheckPoint.pointList, typeof(GameObject), false, true, true, true);

        //drawElementCallback(
        //要素を描画する領域を表す矩形,
        //描画する要素のインデックス,
        //描画する要素がアクティブかどうかを示すフラグ,
        //描画する要素がフォーカスされているかどうかを示すフラグ
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
                        //serializedPropertyはリスト内の要素が追加または削除された場合に、
                        //その変更をシリアライズされたプロパティに反映するために使用
                        reorderableList.serializedProperty.arraySize--;

                    }else
                    {
                        enemyCheckPoint.RemoveMovePointArray();// 移動ポイント配列を削除する
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
                //serializedPropertyはリスト内の要素が追加または削除された場合に、
                //その変更をシリアライズされたプロパティに反映するために使用
                list.serializedProperty.arraySize--;
            }
        };
    }

    public override void OnInspectorGUI()
    {
        if (Application.isPlaying) return;// アプリケーションが実行中の場合は処理をしない

        //カスタムエディタ内に、通常のインスペクターと同じように、対象となるオブジェクトのプロパティが表示されます。
        DrawDefaultInspector();

        // 対象となるEnemyCheckPointクラスのオブジェクトを取得
        EnemyCheckPoint enemyCheckPoint = target as EnemyCheckPoint;

        // 新しいポイントを追加するためのボタンを配置する
        if (GUILayout.Button("AddPoint!"))
        {

            enemyCheckPoint.AddNewPoint();
        }

        //LabelField
        //第1引数に空文字列、
        //第2引数にGUI.skin.horizontalSliderを指定することで、水平スライダーが描画されます
        //https://docs.unity3d.com/ja/current/Manual/class-GUISkin.html
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);//水平スライダーを描画する

        //リストを再描画
        reorderableList.DoLayoutList();
    }
}
