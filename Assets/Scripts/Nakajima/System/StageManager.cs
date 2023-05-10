using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ステージの機能、データを管理するManagerクラス
/// </summary>
public class StageManager : SingletonMonoBehaviour<StageManager>
{
    #region property
    /// <summary>ステージのデータ</summary>
    public StageData Data => _data;

    /// <summary>ステージの合計</summary>
    public int AllStageNum => _data.Stages.Length;
    #endregion

    #region serialize
    [Tooltip("各ステージのデータ")]
    [SerializeField]
    StageData _data = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
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
    #endregion

    #region private method
    #endregion
}
