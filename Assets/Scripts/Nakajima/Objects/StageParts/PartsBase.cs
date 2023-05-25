using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// ステージを構成する各パーツの基底クラス。オブジェクトへのアタッチは不可
/// </summary>
public abstract class PartsBase : MonoBehaviour
{
    #region property
    public string PartsName => _partsName;
    public PartsType PartsType => _partsType;
    public Texture2D[] PartsTextures => _partsTextures;
    public Texture2D CurrentDirTexture => GetTextureByDirection(CurrentDirType);
    public DirectionType CurrentDirType 
    {
        get => _currentDirType;
        set
        {
            _currentDirType = value;
            RotateByDirection(_currentDirType);
        }
    }
    #endregion

    #region serialize
    [Tooltip("パーツの名前")]
    [SerializeField]
    protected string _partsName = "";

    [Tooltip("パーツの種類")]
    [SerializeField]
    protected PartsType _partsType = default;

    [Tooltip("パーツの向き")]
    [SerializeField]
    private DirectionType _currentDirType = DirectionType.North;

    [Tooltip("パーツの画像")]
    [SerializeField]
    protected Texture2D[] _partsTextures = default;
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
    /// <summary>
    /// 方向毎のテクスチャを取得する
    /// </summary>
    /// <param name="type">現在のパーツの向き</param>
    public Texture2D GetTextureByDirection(DirectionType type)
    {
        Texture2D current = null;

        switch (type)
        {
            case DirectionType.North:
                current = _partsTextures[0];
                break;
            case DirectionType.East:
                current = _partsTextures[1];
                break;
            case DirectionType.Sorth:
                current = _partsTextures[2];
                break;
            case DirectionType.West:
                current = _partsTextures[3];
                break;
            default:
                break;
        }

        return current;
    }
    #endregion

    #region private method
    /// <summary>
    /// 向きによってオブジェクトを回転させる
    /// </summary>
    /// <param name="type"></param>
    public virtual void RotateByDirection(DirectionType type)
    {
        Debug.Log($"向き:{type}");
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
    }
    #endregion
}

/// <summary>
/// パーツの種類
/// </summary>
public enum PartsType
{
    None,
    /// <summary>通路</summary>
    Corridor,
    /// <summary>部屋</summary>
    Room
}

public enum DirectionType
{
    North,
    East,
    Sorth,
    West
}