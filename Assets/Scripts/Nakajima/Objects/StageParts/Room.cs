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

    #region protected method
    public override void RotateByDirection(DirectionType type)
    {
        Debug.Log("派生メソッド");

        switch (_roomType)
        {
            case RoomType.Start:
                switch (type)
                {
                    case DirectionType.North:
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;
                    case DirectionType.East:
                        transform.Rotate(new Vector3(0, 270, 0));
                        break;
                    case DirectionType.Sorth:
                        transform.Rotate(new Vector3(0, 0, 0));
                        break;
                    case DirectionType.West:
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;
                    default:
                        break;
                }
                break;
            case RoomType.Cross:
                switch (type)
                {
                    case DirectionType.North:
                        transform.Rotate(new Vector3(0, 0, 0));
                        break;
                    case DirectionType.East:
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;
                    case DirectionType.Sorth:
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;
                    case DirectionType.West:
                        transform.Rotate(new Vector3(0, 270, 0));
                        break;
                    default:
                        break;
                }
                break;
            case RoomType.MainTarget:
                switch (type)
                {
                    case DirectionType.North:
                        transform.Rotate(new Vector3(0, 270, 0));
                        break;
                    case DirectionType.East:
                        transform.Rotate(new Vector3(0, 0, 0));
                        break;
                    case DirectionType.Sorth:
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;
                    case DirectionType.West:
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;
                    default:
                        break;
                }
                break;
            case RoomType.SubTarget:
                switch (type)
                {
                    case DirectionType.North:
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;
                    case DirectionType.East:
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;
                    case DirectionType.Sorth:
                        transform.Rotate(new Vector3(0, 270, 0));
                        break;
                    case DirectionType.West:
                        transform.Rotate(new Vector3(0, 0, 0));
                        break;
                    default:
                        break;
                }
                break;
            case RoomType.Escape:
                switch (type)
                {
                    case DirectionType.North:
                        transform.Rotate(new Vector3(0, 0, 0));
                        break;
                    case DirectionType.East:
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;
                    case DirectionType.Sorth:
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;
                    case DirectionType.West:
                        transform.Rotate(new Vector3(0, 270, 0));
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
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