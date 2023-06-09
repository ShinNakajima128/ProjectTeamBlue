﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

/// <summary>
/// 敵に発見された時の機能を管理するManagerクラス
/// </summary>
public class AleamManager : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Header("Variables")]
    [Tooltip("点滅する間隔")]
    [SerializeField]
    private float _flashingTime = 4.0f;

    [Tooltip("点滅時の光源のの強さ")]
    [SerializeField]
    private float _intensityValue = 1.5f;

    [Tooltip("点滅のアニメーション処理の種類")]
    [SerializeField]
    private Ease _alermEaseType = default;

    [Header("Objects")]
    [Tooltip("警報が発令された時に赤く点滅するLight")]
    [SerializeField]
    private Light _alermLight = default;
    #endregion

    #region private
    private bool _isOnAlerm = false;
    Tweener _currentTween;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Start()
    {
        StageManager.Instance.IsFounded
                             .Skip(1)
                             .Subscribe(value => SetAlerm(value))
                             .AddTo(this);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    /// <summary>
    /// 警報をON/OFFにする
    /// </summary>
    private void SetAlerm(bool isFounded)
    {
        if (isFounded)
        {
            OnAlerm();
            Debug.Log("Found");
        }
        else
        {
            OffAlerm();
            Debug.Log("Lost");
        }
    }
    /// <summary>
    /// 警報を発令
    /// </summary>
    private void OnAlerm()
    {
        if (_isOnAlerm)
        {
            return;
        }

        SoundManager.Instance.PlayBGM(SoundTag.BGM_Found);
        _isOnAlerm = true;

        _currentTween = _alermLight.DOIntensity(_intensityValue, _flashingTime)
                        .SetEase(_alermEaseType)
                        .SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// アラームを停止する
    /// </summary>
    private void OffAlerm()
    {
        SoundManager.Instance.PlayBGM(SoundTag.BGM_Stage1);
        _isOnAlerm = false;

        if (_currentTween != null)
        {
            _currentTween.Kill();
            _currentTween = null;
        }

        _alermLight.intensity = 0;
    }
    #endregion
}
