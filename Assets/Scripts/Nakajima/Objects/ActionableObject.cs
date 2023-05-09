using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクション可能なオブジェクトの機能を持つコンポーネント
/// </summary>
public class ActionableObject : MonoBehaviour, IActionable
{
    #region property
    #endregion

    #region serialize
    #endregion

    #region private
    PlayerAction _playerAction; 
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
        }
    }
    #endregion

    #region public method
    public void OnAction()
    {
        print("アクション実行");
    }
    #endregion

    #region private method
    #endregion
}
