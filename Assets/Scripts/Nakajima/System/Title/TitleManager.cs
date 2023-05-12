using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using Cinemachine;

/// <summary>
/// タイトルの機能を管理するManagerクラス
/// </summary>
public class TitleManager : MonoBehaviour
{
    #region property
    public static TitleManager Instance { get; private set; }
    #endregion

    #region serialize
    [Tooltip("ゲーム開始時に表示されているボタンObject")]
    [SerializeField]
    GameObject _startButton = default;

    [Tooltip("ボタンを押して読み込むSceneの名前(仮機能)")]
    [SerializeField]
    private string _loadSceneName = "";
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
    }

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GameManager.Instance.CurrentGameStates != GameStates.Lobby_StageSelect)
                    {
                        return;
                    }

                    GameManager.Instance.SetCurrentGameStates(GameStates.Lobby_Start);
                    CameraManager.Instance.ChangeActiveCamera(CameraType.Lobby_Start);
                    _startButton.SetActive(true);
                }
            });
    }
    #endregion

    #region public method
    public void PressStartButtonAction()
    {
        GameManager.Instance.SetCurrentGameStates(GameStates.Lobby_StageSelect);
        CameraManager.Instance.ChangeActiveCamera(CameraType.Lobby_StageSelect);
        _startButton.SetActive(false);
    }
    /// <summary>
    /// ステージ選択画面を読み込む
    /// </summary>
    public void LoadStageSelectScene()
    {
        //SceneManager.LoadScene(_loadSceneName);
    }
    #endregion

    #region private method
    #endregion
}
