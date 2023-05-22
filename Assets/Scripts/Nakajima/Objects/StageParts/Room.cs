using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージの広間となるパーツのコンポーネント
/// </summary>
public class Room : PartsBase
{
    #region property
    public RoomType RoomType => _roomType;
    #endregion

    #region serialize
    [Tooltip("部屋の種類")]
    [SerializeField]
    private RoomType _roomType = default;
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
/// 部屋の種類
/// </summary>
public enum RoomType
{
    /// <summary>スタート部屋</summary>
    Start,
    /// <summary>十字部屋</summary>
    Cross,
    /// <summary>メインターゲット部屋</summary>
    MainTarget,
    /// <summary>サブターゲット部屋</summary>
    SubTarget,
    /// <summary>脱出部屋</summary>
    Escape
}