using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// カメラの演出機能を管理するManagerクラス
/// </summary>
public class CameraManager : MonoBehaviour
{
    #region property
    public static CameraManager Instance { get; private set; }
    #endregion

    #region serialize
    [Tooltip("演出カメラ")]
    [SerializeField]
    DirectionCamera[] _cameras = default;
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
        Instance = this;
    }

    private void Start()
    {

    }
    #endregion

    #region public method
    /// <summary>
    /// アクティブなカメラを変更する
    /// </summary>
    /// <param name="type">カメラの種類</param>
    public void ChangeActiveCamera(CameraType type)
    {
        foreach (var camera in _cameras)
        {
            camera.Camera.Priority = 10;
        }

        var selectCamera = _cameras.FirstOrDefault(c => c.CameraType == type);

        selectCamera.Camera.Priority = 15;
    }
    #endregion

    #region private method
    #endregion
}

/// <summary>
/// 演出カメラ
/// </summary>
[Serializable]
public class DirectionCamera
{
    public string CameraName;
    public CinemachineVirtualCamera Camera;
    public CameraType CameraType;
}

/// <summary>
/// カメラの種類
/// </summary>
public enum CameraType
{
    None,
    Lobby_Start,
    Lobby_StageSelect
}
