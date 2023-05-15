﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using Cinemachine;

/// <summary>
/// タイトルの機能を管理するManagerクラス
/// </summary>
public class LobbyManager : MonoBehaviour
{
    #region property
    public static LobbyManager Instance { get; private set; }
    #endregion

    #region serialize
    [Tooltip("ゲーム開始時に表示されているPanel")]
    [SerializeField]
    private GameObject _startPanel = default;

    [Tooltip("ステージ選択画面のPanel")]
    [SerializeField]
    private GameObject _stageSelectPanel = default;

    [Tooltip("Panel切り替え時に次のPanelが表示するまでの時間")]
    [SerializeField]
    private float _activePanelWaitTime = 2.0f;
    #endregion

    #region private
    /// <summary>"Panelの切り替え中かどうか"</summary>
    private bool _isSwitchingPaneled = false;
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
        _stageSelectPanel.SetActive(false);

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GameManager.Instance.CurrentGameStates != GameStates.Lobby_StageSelect || _isSwitchingPaneled)
                    {
                        return;
                    }

                    GameManager.Instance.SetCurrentGameStates(GameStates.Lobby_Start);
                    CameraManager.Instance.ChangeActiveCamera(CameraType.Lobby_Start);
                    StartCoroutine(ActivePanelCoroutine(_startPanel, _activePanelWaitTime, true));
                    _stageSelectPanel.SetActive(false);
                    _isSwitchingPaneled = true;
                }
            });
    }
    #endregion

    #region public method
    public void PressStartButtonAction()
    {
        GameManager.Instance.SetCurrentGameStates(GameStates.Lobby_StageSelect);
        CameraManager.Instance.ChangeActiveCamera(CameraType.Lobby_StageSelect);
        _startPanel.SetActive(false);
        StartCoroutine(ActivePanelCoroutine(_stageSelectPanel, _activePanelWaitTime, true));
        _isSwitchingPaneled = true;
    }
    /// <summary>
    /// ステージ選択画面を読み込む
    /// </summary>
    public void LoadStageSelectScene(string stageName)
    {
        SceneManager.LoadScene(stageName);
    }
    #endregion

    #region private method
    #endregion

    #region coroutine method
    private IEnumerator ActivePanelCoroutine(GameObject panel, float waitTime, bool isActived)
    {
        yield return new WaitForSeconds(waitTime);

        panel.SetActive(isActived);
        _isSwitchingPaneled = false;
    }
    #endregion
}