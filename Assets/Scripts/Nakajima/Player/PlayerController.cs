using System.Collections;
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
    #endregion

    #region private
    private int _currentHP;
    private Subject<bool> _isOperable = new Subject<bool>();
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
        StageManager.Instance.IsInGameSubject.Subscribe(ChangeIsOperatable);
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
            Debug.Log("Gameover");
        }
    }
    #endregion

    #region private method
    /// <summary>
    /// プレイヤー操作のON/OFFを切り替える
    /// </summary>
    /// <param name="isOperatable">ON/OFF</param>
    private void ChangeIsOperatable(bool isOperatable)
    {
        _isOperable.OnNext(isOperatable);
    }
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

public enum PlayerState
{
    Idle,
    Damage,
    Dead
}
