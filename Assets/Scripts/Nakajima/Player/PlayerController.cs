﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// プレイヤーの機能全般をまとめたコンポーネント
/// </summary>
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour, IDamagable
{
    #region property
    public static PlayerController Instance { get; private set; }
    public int CurrentHP => _currentHP;
    public Subject<bool> IsOperable => _isOperable;
    #endregion

    #region serialize
    [SerializeField]
    private int _maxHP = 5;

    [SerializeField]
    private bool _debugMode = false;
    #endregion

    #region private
    private int _currentHP;
    private Subject<bool> _isOperable = new Subject<bool>();
    private bool _isdead = false;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        Instance = this;
        _currentHP = _maxHP;
    }

    private void Start()
    {
        if (_debugMode)
        {
            _isOperable.OnNext(true);
        }
    }
    #endregion

    #region public method
    /// <summary>
    /// 操作できるようになるまでのインターバルを作る
    /// </summary>
    /// <param name="interval"> 操作不可時間 </param>
    public void MoveInterval(float interval)
    {
        _isOperable.OnNext(false);

        StartCoroutine(MoveIntervalCoroutine(interval));
    }
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damageValue"> ダメージ量 </param>
    public void Damage(int damageValue)
    {
        _currentHP -= damageValue;

        if (_currentHP <= 0)
        {
            if (!_isdead)
            {
                Debug.Log("Gameover");
                StageManager.Instance.IsGameover = true;
                StageManager.Instance.OnGameEnd();
                _isdead = true;
            }
        }
    }
    /// <summary>
    /// プレイヤー操作のON/OFFを切り替える
    /// </summary>
    /// <param name="isOperatable">ON/OFF</param>
    public void ChangeIsOperatable(bool isOperatable)
    {
        _isOperable.OnNext(isOperatable);
    }

    public void SetStartPosition(Vector3 startPos)
    {
        transform.position = startPos;
    }
    #endregion

    #region private method
    #endregion

    #region coroutine method
    /// <summary>
    /// 移動可能になるまでの処理を行うコルーチン
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    private IEnumerator MoveIntervalCoroutine(float interval)
    {
        print("インターバル開始");
        yield return new WaitForSeconds(interval);

        _isOperable.OnNext(true);
        print("インターバル終了");
    }
    #endregion
}

/// <summary>
/// プレイヤーの状態
/// </summary>
public enum PlayerState
{
    Idle,
    Damage,
    Dead
}
