using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// プレイヤーの攻撃機能を持つコンポーネント
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    #region property
    public int AttackPower => _attackPower;
    #endregion

    #region serialize
    [Tooltip("プレイヤーの攻撃力")]
    [SerializeField]
    private int _attackPower = 1;

    [Tooltip("プレイヤーの攻撃判定")]
    [SerializeField]
    private Collider _attackCollider = default;

    [Tooltip("攻撃後の操作可能になるまでの時間")]
    [SerializeField]
    private float _moveInterval = 2.5f;
    #endregion

    #region private
    private Animator _anim;
    private bool _isCanAttack = false;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        TryGetComponent(out _anim);
        _attackCollider.enabled = false;
    }

    private void Start()
    {
        //攻撃可能かどうかの処理を登録
        PlayerController.Instance.IsOperable
                        .Subscribe(SwitchIsCanAttack)
                        .AddTo(this);

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                OnAttacking();
            })
            .AddTo(this);
    }
    #endregion

    #region public method
    /// <summary>
    /// 攻撃判定を有効にする
    /// </summary>
    public void OnEnableAttackCollider()
    {
        _attackCollider.enabled = true;
    }

    /// <summary>
    /// 攻撃判定を無効にする
    /// </summary>
    public void OnDisableAttackCollider()
    {
        _attackCollider.enabled = false;
    }
    #endregion

    #region private method
    private void OnAttacking()
    {
        //攻撃ができない状態の場合は処理を行わない
        if (!_isCanAttack)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("攻撃");
            _anim.SetTrigger("IsAttack");
            PlayerController.Instance.MoveInterval(_moveInterval);
        }
    }
    private void SwitchIsCanAttack(bool value)
    {
        _isCanAttack = value;
    }
    #endregion
}
