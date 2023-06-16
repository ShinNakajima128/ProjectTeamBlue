using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

/// <summary>
/// ポーズ画面を表示する機能を持つコンポーネント
/// </summary>
public class PauseViewer : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("現在のステージを最初からやり直すボタン")]
    [SerializeField]
    private Button _restartButton = default;

    [Tooltip("ロビーに戻るボタン")]
    [SerializeField]
    private Button _lobbyButton = default;

    [Tooltip("ボタンをまとめている親オブジェクト")]
    [SerializeField]
    private GameObject _buttonParent = default;
    #endregion

    #region private
    /// <summary>ポーズ画面のグループ</summary>
    private CanvasGroup _pauseGroup = default;
    /// <summary>表示しているかどうか</summary>
    private bool _isViewed = false;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        TryGetComponent(out _pauseGroup);
        _pauseGroup.alpha = 0;
        _buttonParent.SetActive(false);

        //ポーズ画面の各ボタンに処理を登録
        _restartButton.OnClickAsObservable()
                      .Subscribe(_ => Restart())
                      .AddTo(this);
        　

        _lobbyButton.OnClickAsObservable()
                      .Subscribe(_ => ReturnLobby())
                      .AddTo(this);

        //UniRxを使用していない場合はこちらの書き方で処理を登録
        //_lobbyButton.onClick.AddListener(() => ReturnLobby());
    }

    private void Start()
    {
        //ポーズを開いた時の処理を登録
        StageManager.Instance.GamePauseSubject
                             .Subscribe(_ => PauseView())
                             .AddTo(this);
    }
    #endregion

    #region public method
    /// <summary>
    /// ステージをやり直す
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1;
        FadeManager.Fade(FadeType.Out, () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });   
    }

    /// <summary>
    /// ロビーに戻る
    /// </summary>
    public void ReturnLobby()
    {
        Time.timeScale = 1;
        FadeManager.Fade(FadeType.Out, () =>
         {
             SceneManager.LoadScene("Lobby");
         });
    }

    public void PlayEnterSE()
    {
        SoundManager.Instance.PlaySE(SoundTag.SE_CursorMove);
    }

    /// <summary>
    /// 決定音を再生
    /// </summary>
    public void PlaySubmitSE()
    {
        SoundManager.Instance.PlaySE(SoundTag.SE_Submit);
    }
    #endregion

    #region private method
    /// <summary>
    /// ポーズ画面を表示する
    /// </summary>
    private void PauseView()
    {
        //既に開いている場合は処理を行わない
        if (_isViewed)
        {
            return;
        }
        _isViewed = true;
        _pauseGroup.alpha = 1;
        _buttonParent.SetActive(true);
        StartCoroutine(PauseCoroutine());
        SoundManager.Instance.PlaySE(SoundTag.SE_CursorMove);
        Time.timeScale = 0;
    }
    #endregion

    #region coroutine
    /// <summary>
    /// ポーズを開いた時に実行されるコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator PauseCoroutine()
    {
        //同フレーム上でポーズ画面を開く処理を行わないように1フレーム待機
        yield return null;

        while (_isViewed)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //同フレーム上でポーズ画面を開く処理を行わないように1フレーム待機
                yield return null;

                _pauseGroup.alpha = 0;
                _buttonParent.SetActive(false);
                SoundManager.Instance.PlaySE(SoundTag.SE_CursorMove);
                Time.timeScale = 1;
                _isViewed = false;
                break;
            }
            yield return null;
        }
    }
    #endregion
}
