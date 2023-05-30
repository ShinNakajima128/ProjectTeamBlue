using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class EnemyCheckPoint : MonoBehaviour
{
    public GameObject gameStage;
    [HideInInspector]
    public List<GameObject> pointList;

    // Start is called before the first frame update
    void Start()
    {
        pointList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewPoint()
    {
        string parentName = "MovePoint_" + transform.name + "_Array";
        Transform parent = transform.parent.Find(parentName);
        if (parent == null)
        {
            parent = new GameObject(parentName).transform;
            parent.parent = gameStage.transform;
            parent.name = parentName;
        }

        parent.transform.localPosition = Vector3.zero;
        GameObject obj = new GameObject();
        obj.transform.localPosition = Vector3.zero;
        MethodInfo SetIconForObject = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
        MethodInfo CopyMonoScriptIconToImporters = typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.Static | BindingFlags.NonPublic);

        Texture2D tex = EditorGUIUtility.IconContent("sv_label_6").image as Texture2D;
        var editorGUIUtilityType = typeof(EditorGUIUtility);
        BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
        object[] args = new object[] { obj, tex };
        editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        obj.transform.parent = parent;
        obj.transform.localPosition = transform.localPosition;
        pointList.Add(obj);
        pointList[pointList.Count - 1].name = "MovePoint_"+transform.name + "_"+(pointList.Count - 1);
    }

    public List<GameObject> GetChildsFormPointArr()
    {
        string parentName = "MovePoint_" + transform.name + "_Array";

        Transform parent = transform.parent.Find(parentName);

        List<GameObject> childs = new List<GameObject>();

        for(int childCnt = 0;childCnt<parent.transform.childCount;childCnt++)
        {
            childs.Add(parent.GetChild(childCnt).gameObject);
        }

        return childs;
    }

    public void RemoveMovePointArray()
    {
        string parentName = "MovePoint_" + transform.name + "_Array";
        Transform parent = transform.parent.Find(parentName);
        DestroyImmediate(parent.gameObject);
    }
}
