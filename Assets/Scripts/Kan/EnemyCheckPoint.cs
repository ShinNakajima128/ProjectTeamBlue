using System.Collections;
using System.Collections.Generic;
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
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.parent = gameStage.transform;
        obj.transform.localPosition = transform.localPosition;
        pointList.Add(obj);
        pointList[pointList.Count - 1].name = "MovePoint_"+transform.name + "_"+(pointList.Count - 1);
    }

    //public override void OnInspectorGUI()
    //{
    //    //元のInspector部分を表示
    //    base.OnInspectorGUI();

    //    //ボタンを表示
    //    if (GUILayout.Button("Button"))
    //    {
    //        Debug.Log("押した!");
    //    }
    //}
}
