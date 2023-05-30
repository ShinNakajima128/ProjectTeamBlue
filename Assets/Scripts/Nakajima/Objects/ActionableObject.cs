using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクション可能なオブジェクトの機能を持つコンポーネント
/// </summary>
public class ActionableObject : MonoBehaviour, IActionable
{
    #region property
    public TargetType Type => _targetType;
    /// <summary>既に完了しているかどうか</summary>
    public bool IsCompleted { get; private set; } = false;
    #endregion

    #region serialize
    [Tooltip("ターゲットの種類")]
    [SerializeField]
    private TargetType _targetType = default;
    #endregion

    #region private
    private PlayerAction _playerAction;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_playerAction == null)
            {
                _playerAction = other.GetComponent<PlayerAction>();
            }

            _playerAction.SetAction(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerAction.ResetAction();
            Debug.Log("登録されたアクションをリセット");
        }
    }
    #endregion

    #region public method
    public void OnAction()
    {
        print("アクション実行");
        IsCompleted = true;
    }
    #endregion

    #region private method
    #endregion
}

/// <summary>
/// ターゲットの種類
/// </summary>
public enum TargetType
{
    Main,
    Sub,
    EscapePoint
}
