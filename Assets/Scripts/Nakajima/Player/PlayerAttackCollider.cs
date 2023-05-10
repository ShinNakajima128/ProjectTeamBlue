using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃判定を行うコンポーネント
/// </summary>
public class PlayerAttackCollider : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [SerializeField]
    private string _enemyTag = "Enemy";
    #endregion

    #region private
    private PlayerAttack _playerAttack;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        _playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //敵に攻撃がヒットした場合
        if (other.CompareTag(_enemyTag))
        {
            Debug.Log("敵がダメージを受けた");

            if (TryGetComponent(out IDamagable target))
            {
                target.Damage(_playerAttack.AttackPower);
            }
        }
    }
    #endregion

    #region public method
    #endregion

    #region private method
    #endregion
}
