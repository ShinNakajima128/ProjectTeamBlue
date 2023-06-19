using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵のアニメーションイベントを転送
public class EnemyAnimationEventForwarder : MonoBehaviour
{
    // 攻撃コライダーを無効にする場合
    public void OnDisableAttackCollider()
    {
        // 親オブジェクト(敵)にOnEnemyDisableAttackメッセージを送信する
        transform.parent.BroadcastMessage("OnEnemyDisableAttack", SendMessageOptions.DontRequireReceiver);
    }

    // 攻撃コライダーを有効にする場合
    public void OnEnableAttackCollider()
    {
        // 親オブジェクト(敵)にOnEnemyEnableAttackメッセージを送信する
        transform.parent.BroadcastMessage("OnEnemyEnableAttack", SendMessageOptions.DontRequireReceiver);
    }
}
