using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/StageData")]
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
    #endregion

    #region private method
    #endregion
}
