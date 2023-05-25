using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージの通路のコンポーネント
/// </summary>
public class Corridor : PartsBase
{
    #region property
    public CorridorType CorridorType => _corridorType;
    #endregion

    #region serialize
    [Tooltip("通路の種類")]
    [SerializeField]
    private CorridorType _corridorType = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    #endregion

    #region public method
    #endregion

    #region private method
    #endregion
}

/// <summary>
/// 通路の種類
/// </summary>
public enum CorridorType
{
    /// <summary>直線1</summary>
    Straight_1,
    /// <summary>直線2</summary>
    Straight_2,
    /// <summary>直線3</summary>
    Straight_3,
    /// <summary>直線（行き止まり）</summary>
    Straight_End,
    /// <summary>直線（横幅大きめ）</summary>
    Straight_Large,
    /// <summary>L字通路</summary>
    Sharp_L,
    /// <summary>T字通路</summary>
    Sharp_T,
    /// <summary>十字通路</summary>
    Cross
}
