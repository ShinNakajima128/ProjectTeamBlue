/*
参考：https://qiita.com/k_yanase/items/686fc3134c363ffc5239
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// エディター上でStartCorutineのような処理を使用可能にするクラスです 
/// </summary>
[UnityEditor.InitializeOnLoad]
public sealed class EditorCoroutine
{
    [System.Obsolete]
    static EditorCoroutine()
    {
        EditorApplication.update += Update;
        Debug.Log("EditorCoroutine SetUp");
    }

    static Dictionary<IEnumerator, EditorCoroutine.Coroutine> asyncList = new Dictionary<IEnumerator, Coroutine>();
    static List<EditorCoroutine.WaitForSeconds> waitForSecondsList = new List<EditorCoroutine.WaitForSeconds>();

    [System.Obsolete]
    static void Update()
    {

        CheackIEnumerator();
        CheackWaitForSeconds();
    }

    [System.Obsolete]
    static void CheackIEnumerator()
    {
        List<IEnumerator> removeList = new List<IEnumerator>();
        foreach (KeyValuePair<IEnumerator, EditorCoroutine.Coroutine> pair in asyncList)
        {
            if (pair.Key != null)
            {

                //IEnumratorのCurrentがCoroutineを返しているかどうか 
                EditorCoroutine.Coroutine c = pair.Key.Current as EditorCoroutine.Coroutine;
                if (c != null)
                {
                    if (c.isActive) continue;
                }
                //wwwクラスのダウンロードが終わっていなければ進まない 
                WWW www = pair.Key.Current as WWW;
                if (www != null)
                {
                    if (!www.isDone) continue;
                }
                //これ以上MoveNextできなければ終了 
                if (!pair.Key.MoveNext())
                {
                    if (pair.Value != null)
                    {
                        pair.Value.isActive = false;
                    }
                    removeList.Add(pair.Key);
                }
            }
            else
            {
                removeList.Add(pair.Key);
            }
        }

        foreach (IEnumerator async in removeList)
        {
            asyncList.Remove(async);
        }
    }

    static void CheackWaitForSeconds()
    {
        for (int i = 0; i < waitForSecondsList.Count; i++)
        {
            if (waitForSecondsList[i] != null)
            {
                if (EditorApplication.timeSinceStartup - waitForSecondsList[i].InitTime > waitForSecondsList[i].Time)
                {
                    waitForSecondsList[i].isActive = false;
                    waitForSecondsList.RemoveAt(i);
                }
            }
            else
            {
                Debug.LogError("rem");
                waitForSecondsList.RemoveAt(i);
            }
        }
    }

    //=====================================================================================
    //関数 

    /// <summary>
    /// コルーチンを起動します 
    /// </summary>
    static public EditorCoroutine.Coroutine Start(IEnumerator iEnumerator)
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            EditorCoroutine.Coroutine c = new Coroutine();
            if (!asyncList.Keys.Contains(iEnumerator)) asyncList.Add(iEnumerator, c);
            iEnumerator.MoveNext();
            return c;
        }
        else
        {
            Debug.LogError("EditorCoroutine.Startはゲーム起動中に使うことはできません");
            return null;
        }
    }

    /// <summary>
    /// コルーチンを停止します 
    /// </summary>
    static public void Stop(IEnumerator iEnumerator)
    {
        if (Application.isEditor)
        {
            if (asyncList.Keys.Contains(iEnumerator))
            {
                asyncList.Remove(iEnumerator);
            }
        }
        else
        {
            Debug.LogError("EditorCoroutine.Startはゲーム中に使うことはできません");
        }
    }

    /// <summary>
    /// 使用不可
    /// WaitForSecondsのインスタンスを登録します 
    /// </summary>
    static public void AddWaitForSecondsList(EditorCoroutine.WaitForSeconds coroutine)
    {
        if (waitForSecondsList.Contains(coroutine) == false)
        {
            waitForSecondsList.Add(coroutine);
        }
    }


    //=====================================================================================
    //待機処理用クラス 

    public class Coroutine
    {
        //trueなら待機させる 
        public bool isActive;

        public Coroutine()
        {
            isActive = true;
        }
    }

    public sealed class WaitForSeconds : EditorCoroutine.Coroutine
    {
        private float time;
        private double initTime;

        public float Time
        {
            get { return time; }
        }
        public double InitTime
        {
            get { return initTime; }
        }

        public WaitForSeconds(float time) : base()
        {
            this.time = time;
            this.initTime = EditorApplication.timeSinceStartup;
            EditorCoroutine.AddWaitForSecondsList(this);
        }
    }
}
