using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージのスコア毎のランクを算出するクラス
/// </summary>
public class StageRankCalculator :MonoBehaviour
{
    #region property
    public static StageRankCalculator Instance { get; private set; }
    public ScoreRank CurrentRank { get; private set; }
    #endregion

    #region serialize
    [Tooltip("各ステージのランク算出の基準データ")]
    [SerializeField]
    private RankedScore[] _rankedScores = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity method
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region public method
    /// <summary>
    /// 入力されたスコアによってランクを算出する
    /// </summary>
    /// <param name="stageType">ステージの種類</param>
    /// <param name="score">スコア</param>
    /// <returns></returns>
    public static ScoreRank CalculateScoreRank(GameStates stageType, int score)
    {
        ScoreRank rank = default;

        var rs = Instance._rankedScores.FirstOrDefault(x => x.StageType == stageType);

        if (score >= rs.Time_Rank_S)
        {
            rank = ScoreRank.S;
        }
        else if (score >= rs.Time_Rank_A)
        {
            rank = ScoreRank.A;
        }
        else if (score <= rs.Time_Rank_B)
        {
            rank = ScoreRank.B;
        }
        else
        {
            rank = ScoreRank.C;
        }
        Instance.CurrentRank = rank;
        return rank;
    }
    #endregion

    #region private method
    #endregion
}

/// <summary>
/// ステージの各ランクの基準データのクラス
/// </summary>
[Serializable]
public class RankedScore
{
    public string StageName;
    public GameStates StageType;
    public int Time_Rank_S;
    public int Time_Rank_A;
    public int Time_Rank_B;
    public int Time_Rank_C;
}
/// <summary>
/// スコアに応じたランク
/// </summary>
public enum ScoreRank
{
    None,
    S,
    A,
    B,
    C
}
