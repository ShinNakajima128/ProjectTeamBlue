using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// プレイヤーの移動処理を行うコンポーネント
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("移動速度")]
    [SerializeField]
    private float _moveSpeed = 5.0f;

    [Tooltip("旋回速度")]
    [SerializeField]
    private float _turnSpeed = 8.0f;
    #endregion

    #region private
    private Rigidbody _rb;
    private Animator _anim;
    private Vector3 _dir;
    private bool _isCanMove = false;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _anim);
    }

    private void Start()
    {
        //移動可不可の処理を登録
        PlayerController.Instance.IsOperable
                        .Subscribe(SwitchIsCanMove)
                        .AddTo(this);

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                OnRotate();
            });

        this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                OnMoving();
            })
            .AddTo(this);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    /// <summary>
    /// 移動処理
    /// </summary>
    private void OnMoving()
    {
        //操作不能の場合は処理を行わない
        if (!_isCanMove)
        {
            return;
        }

        // 方向の入力を取得し、方向を求める
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        // 入力方向のベクトルを組み立てる
        _dir = Vector3.forward * v + Vector3.right * h;

        if (_dir == Vector3.zero)
        {
            _rb.velocity = Vector3.zero;
        }
        else
        {
            _rb.velocity = _dir.normalized * _moveSpeed;
        }

        _anim.SetFloat("Speed", _rb.velocity.magnitude);
    }
    private void OnRotate()
    {
        if (_dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _turnSpeed);
        }
    }
    /// <summary>
    /// 操作できるかどうかを切り替える
    /// </summary>
    /// <param name="value"> ON/OFF </param>
    private void SwitchIsCanMove(bool value)
    {
        _isCanMove = value;
    }
    #endregion
}
