using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/StageData")]
[Serializable]
public class StageData : ScriptableObject
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
    public void Setup(Stage[] stageDatas)
    {
        for (int i = 0; i < _stages.Length; i++)
        {
            _stages[i].SetupData(stageDatas[i]);
        }
    }
    #endregion

    #region private method
    #endregion
}
