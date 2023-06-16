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

    [Tooltip("リザルト画面の背景")]
    [SerializeField]
    private Image _resultBackground = default;

    [Tooltip("クリア時のロゴImage")]
    [SerializeField]
    private Image _clearLogoImage = default;

    [Tooltip("ゲームオーバー時のロゴImage")]
    [SerializeField]
    private Image _gameoverLogoImage = default;

    [Tooltip("ゲーム終了後の選択画面のPanel")]
    [SerializeField]
    private GameObject _selectPanel = default;

    [Tooltip("選択画面の背景Image")]
    [SerializeField]
    private Image _selectPanelBackground = default;

    [Tooltip("各ステージのイメージSprite")]
    [SerializeField]
    private Sprite[] _stageSprites = default;
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
                             .Subscribe(_ => ResultViewing())
                             .AddTo(this);
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
        SoundManager.Instance.PlayBGM(SoundTag.BGM_Result);
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
        //リザルト画面の背景をステージによって差し替える
        switch (GameManager.Instance.CurrentGameStates)
        {
            case GameStates.Stage1:
                _resultBackground.sprite = _stageSprites[0];
                break;
            case GameStates.Stage2:
                _resultBackground.sprite = _stageSprites[1];
                break;
            case GameStates.Stage3:
                _resultBackground.sprite = _stageSprites[2];
                break;
            default:
                break;
        }

        //リザルト画面のパネルを表示
        _resultGroup.alpha = 1;

        if (!StageManager.Instance.IsGameover)
        {
            yield return new WaitForSeconds(0.5f);

            //メインターゲットのスコアを表示
            int mainScore = 0;
            SoundManager.Instance.PlaySE(SoundTag.SE_ScoreView);
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
            if (ScoreCalculation.Instance.SubTargetScore > 0)
            {
                int subScore = 0;
                SoundManager.Instance.PlaySE(SoundTag.SE_ScoreView);
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
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundTag.SE_CursorMove);
                yield return new WaitForSeconds(1.0f);
            }
            //残り時間のスコアを表示
            int remainingTimeScore = 0;
            SoundManager.Instance.PlaySE(SoundTag.SE_ScoreView);
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
            SoundManager.Instance.PlaySE(SoundTag.SE_ScoreView);
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

            yield return new WaitForSeconds(1.0f);

            SoundManager.Instance.PlaySE(SoundTag.SE_CompleteMainMission);
            _scoreRankText.text = StageRankCalculator.Instance.CurrentRank.ToString();
            
            yield return new WaitForSeconds(_scoreViewingTime);

            _clearLogoImage.enabled = true;
        }
        else
        {
            _selectPanelBackground.DOFade(1.0f, 0f);
            //ゲームオーバー用のTextを表示する処理を作成
        }
        yield return new WaitForSeconds(_scoreViewingTime);

        _selectPanelBackground.DOFade(1.0f, 1.0f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() => 
                              {
                                  _gameoverLogoImage.enabled = true;
                                  _selectPanel.SetActive(true);
                              });
    }
    #endregion
}
