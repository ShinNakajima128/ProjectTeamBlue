using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 画面のフェード機能を管理するManagerクラス
/// </summary>
public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    #region property
    #endregion

    #region serialize
    [Header("Variables")]
    [Tooltip("フェードにかける時間")]
    [SerializeField]
    private float _fadeTime = 1.0f;

    [Header("Objects")]
    [Tooltip("フェードするImage")]
    [SerializeField]
    private Image _fadeImage = default;
    #endregion

    #region private
    /// <summary>フェード中かどうか</summary>
    private bool _isFading = false;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region public method
    /// <summary>
    /// 画面を徐々に暗転させる
    /// </summary>
    public static void Fade(FadeType type, Action callback = null)
    {
        if (Instance._isFading)
        {
            return;
        }

        Instance._isFading = true;
        float fadeTarget = default;

        switch (type)
        {
            case FadeType.In:
                fadeTarget = 0f;
                if (Instance._fadeImage.color.a < 1.0f)
                {
                    Instance._fadeImage.DOFade(1.0f, 0f);
                }
                break;
            case FadeType.Out:
                fadeTarget = 1.0f;
                if (Instance._fadeImage.color.a > 0f)
                {
                    Instance._fadeImage.DOFade(0f, 0f);
                }
                break;
            default:
                break;
        }

        Instance._fadeImage.DOFade(fadeTarget, Instance._fadeTime)
                           .SetEase(Ease.Linear)
                           .OnComplete(() =>
                           {
                               //完了後に引数に渡されている処理を実行
                               callback?.Invoke();
                               Instance._isFading = false;
                           });
    }
    #endregion

    #region private method
    #endregion
}

public enum FadeType
{
    In,
    Out
}
