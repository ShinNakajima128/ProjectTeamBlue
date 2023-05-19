using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージを構成する各パーツの基底クラス。オブジェクトへのアタッチは不可
/// </summary>
public abstract class PartsBase : MonoBehaviour
{
    #region property
    public string PartsName => _partsName;
    public PartsType PartsType => _partsType;
    public Texture2D PartsTexture => _partsTexture;
    #endregion

    #region serialize
    [Tooltip("パーツの名前")]
    [SerializeField]
    protected string _partsName = "";

    [Tooltip("パーツの種類")]
    [SerializeField]
    protected PartsType _partsType = default;

    [Tooltip("パーツの画像")]
    [SerializeField]
    protected Texture2D _partsTexture = default;
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