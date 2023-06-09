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
    public Subject<Unit> ReflashDataSubject => _reflashDataSubject;
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
    private Subject<Unit> _reflashDataSubject = new Subject<Unit>();
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

        FadeManager.Fade(FadeType.In);
        SoundManager.Instance.PlayBGM(SoundTag.BGM_Lobby);

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GameManager.Instance.CurrentGameStates != GameStates.Lobby_StageSelect || 
                    　　_isSwitchingPaneled)
                    {
                        return;
                    }

                    ReturnLobbyStart();
                }
            })
            .AddTo(this);
    }
    #endregion

    #region public method
    /// <summary>
    /// ステージ選択画面に進む
    /// </summary>
    public void StartStageSelect()
    {
        GameManager.Instance.SetCurrentGameStates(GameStates.Lobby_StageSelect);
        CameraManager.Instance.ChangeActiveCamera(CameraType.Lobby_StageSelect);
        _startPanel.SetActive(false);
        StartCoroutine(ActivePanelCoroutine(PanelType.StageSelect, _activePanelWaitTime, true));
        _isSwitchingPaneled = true;
    }

    /// <summary>
    /// スタート画面に戻る
    /// </summary>
    public void ReturnLobbyStart()
    {
        GameManager.Instance.SetCurrentGameStates(GameStates.Lobby_Start);
        CameraManager.Instance.ChangeActiveCamera(CameraType.Lobby_Start);
        StartCoroutine(ActivePanelCoroutine(PanelType.GameStart, _activePanelWaitTime, true));
        _stageSelectPanel.SetActive(false);
        _isSwitchingPaneled = true;
    }

    /// <summary>
    /// データを初期化する
    /// </summary>
    public void ResetData()
    {
        DataManager.Instance.ResetData();
        _reflashDataSubject.OnNext(Unit.Default);
    }
    /// <summary>
    /// ステージSceneを読み込む
    /// </summary>
    public void LoadStageScene(int stageType)
    {
        GameStates stage = (GameStates)(stageType + 1);

        //ステージ選択画面で押されたボタンのステージをGameManagerの関数の引数に入れる
        //※現在は仮の処理なため、システムができ次第修正予定

        GameManager.Instance.SetCurrentGameStates(stage);
        FadeManager.Fade(FadeType.Out, () =>
        {
            SceneManager.LoadScene("InGame");
        });
    }

    /// <summary>
    /// 選択音を再生する
    /// </summary>
    public void PlayButtonSelectSE()
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

    public void PlayStageSubmitSE()
    {
        SoundManager.Instance.PlaySE(SoundTag.SE_StageSubmit);
    }
    #endregion

    #region private method
    #endregion

    #region coroutine method
    /// <summary>
    /// パネルのアクティブ状態を切り替えるコルーチン
    /// </summary>
    /// <param name="panel">切り替えるPanel</param>
    /// <param name="waitTime">待機時間</param>
    /// <param name="isActived">アクティブにするかしないか</param>
    /// <returns></returns>
    private IEnumerator ActivePanelCoroutine(PanelType panel, float waitTime, bool isActived)
    {
        yield return new WaitForSeconds(waitTime);

        switch (panel)
        {
            case PanelType.GameStart:
                _startPanel.SetActive(isActived);
                SoundManager.Instance.PlaySE(SoundTag.SE_CursorMove);           
                break;
            case PanelType.StageSelect:
                _stageSelectPanel.SetActive(isActived);
                SoundManager.Instance.PlaySE(SoundTag.SE_View);
                break;
            default:
                break;
        }
        _isSwitchingPaneled = false;
    }
    #endregion
}

/// <summary>
/// パネルの種類
/// </summary>
public enum PanelType
{
    GameStart,
    StageSelect
}
