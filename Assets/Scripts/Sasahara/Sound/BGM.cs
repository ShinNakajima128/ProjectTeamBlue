using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
//BGMのデータクラス
public class BGM
{
    #region property
    // プロパティを入れる。
    public string Key => _key;
    public AudioClip Clip => _clip;
    public float Volume => _volume;
    #endregion

    #region serialize
    // unity inpectorに表示したいものを記述。
    [SerializeField]
    private string _key;
    [SerializeField]
    private AudioClip _clip;
    [SerializeField, Range(0f, 1f)]
    private float _volume = 1;
    #endregion

    #region private
    // プライベートなメンバー変数。
    #endregion

    #region Constant
    // 定数をいれる。
    #endregion

    #region Event
    //  System.Action, System.Func などのデリゲートやコールバック関数をいれるところ。
    #endregion

    #region unity methods
    //  Start, UpdateなどのUnityのイベント関数。
    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {

    }
    #endregion

    #region public method
    //　自身で作成したPublicな関数を入れる。
    #endregion

    #region private method
    // 自身で作成したPrivateな関数を入れる。
    #endregion
}
