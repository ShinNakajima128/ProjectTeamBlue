using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ゲーム全体を管理するManagerクラス
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    #region property
    public GameStates CurrentGameStates => _currentGameState;
    #endregion

    #region serialize
    [Tooltip("現在プレイしているステージ")]
    [SerializeField]
    private GameStates _currentGameState = GameStates.Lobby_Start;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    public event Action OnGameStart;
    public event Action OnGamePause;
    public event Action OnGameEnd;
    #endregion

    #region unity methods
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }
    #endregion

    #region public method
    /// <summary>現在のゲームの状態を設定する</summary>
    /// <param name="state">設定する状態</param>
    public void SetCurrentGameStates(GameStates state)
    {
        _currentGameState = state;
    }
    #endregion

    #region private method
    #endregion
}

/// <summary>
/// ゲームの状態
/// </summary>
public enum GameStates
{
    Lobby_Start,
    Lobby_StageSelect,
    Stage1,
    Stage2,
    Stage3
}
