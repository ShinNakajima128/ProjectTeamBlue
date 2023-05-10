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
    #endregion

    #region private
    /// <summary>現在プレイしているステージ</summary>
    private GameStates _currentGameState;
    #endregion

    #region Constant
    #endregion

    #region Event
    public event Action OnGameStart;
    public event Action OnGamePause;
    public event Action OnGameEnd;
    #endregion

    #region unity methods
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
    Title,
    StageSelectView,
    Stage1,
    Stage2,
    Stage3
}
