using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _isCanMove = true;
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (!_isCanMove)
        {
            return;
        }

        // 方向の入力を取得し、方向を求める
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        // 入力方向のベクトルを組み立てる
        Vector3 dir = Vector3.forward * v + Vector3.right * h;

        if (dir == Vector3.zero)
        {
            _rb.velocity = Vector3.zero;
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _turnSpeed);

            _rb.velocity = dir.normalized * _moveSpeed;
        }

        _anim.SetFloat("Speed", _rb.velocity.magnitude);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    #endregion
}
