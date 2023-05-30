using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// 制限時間を表示するコンポーネント
/// </summary>
public class LimitTimeView : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("制限時間を表示するText")]
    [SerializeField]
    private Text _limitTimeText = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Start()
    {
        //制限時間の表示を更新する処理を登録
        StageManager.Instance.CurrentLimitTime
                    .Subscribe(value => ReflashLimitTime(value))
                    .AddTo(this);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    /// <summary>
    /// 制限時間の表示を更新する
    /// </summary>
    /// <param name="currentTime">現在の制限時間</param>
    private void ReflashLimitTime(float currentTime)
    {
        _limitTimeText.text = $"{((int)(currentTime / 60)).ToString("00")} : {((int)(currentTime % 60)).ToString("00")}";
    }
    #endregion
}
