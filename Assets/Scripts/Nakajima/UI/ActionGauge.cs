using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// アクションを行う先のゲージの機能を持つコンポーネント
/// </summary>
public class ActionGauge : MonoBehaviour
{
    #region property
    public static ActionGauge Instance { get; private set; }
    #endregion

    #region serialize
    [Tooltip("各アクション時に表示されるゲージObject")]
    [SerializeField]
    GameObject _actionGauge = default;

    [Tooltip("ゲージの増減を行うImage")]
    [SerializeField]
    private Image _fillGauge = default;
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
        _actionGauge.SetActive(false);
    }
    #endregion

    #region public method
    /// <summary>
    /// アクションを開始する
    /// </summary>
    /// <param name="time"></param>
    public static void StartAction(float time)
    {
        Instance._actionGauge.SetActive(true);

        Instance._fillGauge.fillAmount = 0f;

        Instance._fillGauge.DOFillAmount(1.0f, time)
                           .SetEase(Ease.Linear)
                           .OnComplete(() =>
                           {
                               Instance._actionGauge.SetActive(false);
                               print("アクション完了");
                           });
    }
    #endregion

    #region private method
    #endregion
}
