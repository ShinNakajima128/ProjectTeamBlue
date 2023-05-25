using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコアを算出するコンポーネント
/// </summary>
public class ScoreCalculation : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("メインターゲットに掛ける係数")]
    [SerializeField]
    private int _mainTargetCoefficient = 3000;

    [Tooltip("サブターゲットに掛ける係数")]
    [SerializeField]
    private int _subTargetCoefficient = 500;

    [Tooltip("残り時間に掛ける係数")]
    [SerializeField]
    private int _remainingTimeCoefficient = 10;
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
    /// スコアを算出する
    /// </summary>
    /// <param name="subTargetCompleteNum">達成したサブミッション数</param>
    /// <param name="remainingTime">残り時間</param>
    /// <returns></returns>
    public int CalcurlationScore(int subTargetCompleteNum, int remainingTime)
    {
        int result = 0;

        result += _mainTargetCoefficient;
        result += subTargetCompleteNum * _subTargetCoefficient;
        result += remainingTime * _remainingTimeCoefficient;

        return result;
    }
    #endregion

    #region private method
    #endregion
}
