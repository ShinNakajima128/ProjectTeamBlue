using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class EnemyCheckPoint : MonoBehaviour
{
    public List<GameObject> PointList;
    
    // Start is called before the first frame update
    void Start()
    {
        PointList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewPoint()
    { 
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;
        PointList.Add(obj);
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
