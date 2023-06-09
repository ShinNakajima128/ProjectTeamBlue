using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スコアを算出するコンポーネント
/// </summary>
public class ScoreCalculation : MonoBehaviour
{
    #region property
    public static ScoreCalculation Instance { get; private set; }
    public int MainTargetScore => _mainTargetScore;
    public int SubTargetScore => _subTargetScore;
    public int RemainingTimeScore => _remainingTimeScore;
    public int ResultScore => _resultScore;
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
    private int _mainTargetScore = 0;
    private int _subTargetScore = 0;
    private int _remainingTimeScore = 0;
    private int _resultScore = 0;
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
        _mainTargetScore = _mainTargetCoefficient;
        _subTargetScore = subTargetCompleteNum * _subTargetCoefficient;
        _remainingTimeScore = remainingTime * _remainingTimeCoefficient;

        _resultScore = _mainTargetScore + _subTargetScore + _remainingTimeScore;
        return _resultScore;
    }
    #endregion

    #region private method
    #endregion
}
