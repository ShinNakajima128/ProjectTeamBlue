using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;
using UnityEngine.SceneManagement;

/// <summary>
/// リザルトを表示する機能を持つコンポーネント
/// </summary>
public class ResultViewer : MonoBehaviour
{
    #region property
    public static ResultViewer Instance { get; private set; }
    #endregion

    #region serialize
    [Header("Variables")]
    [Tooltip("各スコアの数値を表示完了するまでの時間")]
    [SerializeField]
    private float _scoreViewingTime = 2.0f;

    [Header("Objects")]
    [Tooltip("メインターゲットのスコアを表示するText")]
    [SerializeField]
    private Text _mainTargetScoreText = default;

    [Tooltip("サブターゲットのスコアを表示するText")]
    [SerializeField]
    private Text _subTargetScoreText = default;

    [Tooltip("残り時間のスコアを表示するText")]
    [SerializeField]
    private Text _remainingTimeScoreText = default;

    [Tooltip("合計のスコアを表示するText")]
    [SerializeField]
    private Text _totalScoreText = default;

    [Tooltip("ランクを表示するText")]
    [SerializeField]
    private Text _scoreRankText = default;
    #endregion

    #region private
    private CanvasGroup _resultGroup = default;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Start()
    {
        _resultGroup = GetComponent<CanvasGroup>();
        _resultGroup.alpha = 0;
        StageManager.Instance.GameEndSubject
                             .Subscribe(_ => ResultViewing());
    }
    #endregion

    #region public method
    #endregion

    #region private method
    /// <summary>
    /// リザルトを表示する
    /// </summary>
    private void ResultViewing()
    {
        StartCoroutine(ViewingCoroutine());
    }
    #endregion

    #region coroutine
    /// <summary>
    /// スコアを表示するコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ViewingCoroutine()
    {
        //リザルト画面のパネルを表示
        _resultGroup.alpha = 1;

        yield return new WaitForSeconds(1.5f);

        //メインターゲットのスコアを表示
        int mainScore = 0;
        yield return DOTween.To(() =>
                            mainScore,
                            (n) => mainScore = n,
                            ScoreCalculation.Instance.MainTargetScore,
                            _scoreViewingTime)
                            .OnUpdate(() =>
                            {
                                _mainTargetScoreText.text = mainScore.ToString();
                            })
                            .WaitForCompletion();

        //サブターゲットのスコアを表示
        int subScore = 0;
        yield return DOTween.To(() =>
                            subScore,
                            (n) => subScore = n,
                            ScoreCalculation.Instance.SubTargetScore,
                            _scoreViewingTime)
                            .OnUpdate(() =>
                            {
                                _subTargetScoreText.text = subScore.ToString();
                            })
                            .WaitForCompletion();

        //残り時間のスコアを表示
        int remainingTimeScore = 0;
        yield return DOTween.To(() =>
                            remainingTimeScore,
                            (n) => remainingTimeScore = n,
                            ScoreCalculation.Instance.RemainingTimeScore,
                            _scoreViewingTime)
                            .OnUpdate(() =>
                            {
                                _remainingTimeScoreText.text = remainingTimeScore.ToString();
                            })
                            .WaitForCompletion();

        //合計のスコアを表示
        int totalScore = 0;
        yield return DOTween.To(() =>
                            totalScore,
                            (n) => totalScore = n,
                            ScoreCalculation.Instance.ResultScore,
                            _scoreViewingTime)
                            .OnUpdate(() =>
                            {
                                _totalScoreText.text = totalScore.ToString();
                            })
                            .WaitForCompletion();

        yield return new WaitForSeconds(_scoreViewingTime);

        _scoreRankText.text = StageRankCalculator.Instance.CurrentRank.ToString();

        yield return new WaitUntil(() => Input.anyKeyDown);

        //Scene遷移機能をどのように作るか不明なため、仮のロード処理を行っている
        SceneManager.LoadScene("Lobby");
    }
    #endregion
}
