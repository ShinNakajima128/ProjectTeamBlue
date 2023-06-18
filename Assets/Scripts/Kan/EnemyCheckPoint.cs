using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EnemyCheckPoint : MonoBehaviour
{
    public GameObject gameStage;// ゲームステージを参照
    [HideInInspector]//Inspectorに隠し
    public List<GameObject> pointList;//ルートポイント

    // Start is called before the first frame update
    void Start()
    {
        pointList = new List<GameObject>();
    }

#if UNITY_EDITOR
    public void AddNewPoint()// 新しい移動ポイントを追加する関数
    {
        string parentName = "MovePoint_" + transform.name + "_Array";// 親オブジェクトの名前を定義
        Transform parent = transform.parent.Find(parentName);// 親オブジェクトを検索

        if (parent == null)
        {
            // 親オブジェクトが見つからない場合は、新しい親オブジェクトを作成
            parent = new GameObject(parentName).transform;
            parent.parent = gameStage.transform;
            parent.name = parentName;
        }

        parent.transform.localPosition = Vector3.zero;// 親オブジェクトの位置をリセット
        GameObject obj = new GameObject();
        obj.transform.localPosition = Vector3.zero;

        // アイコンを設定するためのメソッドを取得
        /*非公開（NonPublic）であり、静的（Static）です。
         * そのため、BindingFlags.NonPublic | BindingFlags.Staticフラグを使用して、
         * GetMethodメソッドを呼び出しています。*/
        MethodInfo SetIconForObject = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo CopyMonoScriptIconToImporters = typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.Static | BindingFlags.NonPublic);

        // アイコン画像を取得
        /*アイコンについて、以下を参照:
         * 公式：https://docs.unity3d.com/ja/current/ScriptReference/EditorGUIUtility.IconContent.html
         * https://baba-s.hatenablog.com/entry/2021/10/15/090000
         */
        Texture2D tex = EditorGUIUtility.IconContent("sv_label_6").image as Texture2D;
        var editorGUIUtilityType = typeof(EditorGUIUtility);
        BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;// アイコンを設定
        object[] args = new object[] { obj, tex };
        editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);

        obj.transform.parent = parent;// 新しいルートのゲームオブジェクトを親に追加
        obj.transform.localPosition = transform.localPosition;
        pointList.Add(obj);// 新しいルートのゲームオブジェクトをpointListに追加
        pointList[pointList.Count - 1].name = "MovePoint_" + transform.name + "_" + (pointList.Count - 1);//名前を付ける
    }

    // 移動ポイント配列を削除する
    public void RemoveMovePointArray()
    {
        string parentName = "MovePoint_" + transform.name + "_Array";// 親オブジェクトの名前を定義
        Transform parent = transform.parent.Find(parentName);// 親オブジェクトを検索
        DestroyImmediate(parent.gameObject);// 親オブジェクトを削除
    }
#endif

    // ポイント配列から子オブジェクトを取得する関数
    public List<GameObject> GetChildsFormPointArr()
    {
        string parentName = "MovePoint_" + transform.name + "_Array";// 親オブジェクトの名前を定義

        Transform parent = transform.parent.Find(parentName);// 親オブジェクトを検索

        List<GameObject> childs = new List<GameObject>(); // 子オブジェクトのリストを作成

        for (int childCnt = 0; childCnt < parent.transform.childCount; childCnt++)// 子オブジェクトをリストに追加
        {
            childs.Add(parent.GetChild(childCnt).gameObject);
        }

        return childs;// 子オブジェクトのリストを返す
    }
}
