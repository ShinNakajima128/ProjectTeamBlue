using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageData
{
    #region property
    public Stage[] Stages => _stages;
    #endregion

    #region serialize
    [Tooltip("各ステージの情報")]
    [SerializeField]
    Stage[] _stages = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region public method
    /// <summary>
    /// データをセットする。保存データがある場合に使用
    /// </summary>
    /// <param name="stageDatas"></param>
    public void Setup(Stage[] stageDatas)
    {
        for (int i = 0; i < _stages.Length; i++)
        {
            _stages[i].SetupData(stageDatas[i]);
        }
    }

    /// <summary>
    /// データを初期状態にリセットする
    /// </summary>
    public void DataReset()
    {
        for (int i = 0; i < _stages.Length; i++)
        {
            _stages[i].Reset();
        }
    }
    #endregion

    #region private method
    #endregion
}
