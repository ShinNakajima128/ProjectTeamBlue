using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerAction : MonoBehaviour
{
    #region property
    /// <summary>アクション対象のオブジェクトに近づいたかどうか</summary>
    public bool IsApproached { get; private set; }
    #endregion

    #region serialize
    [Tooltip("アクションが完了するまでの時間")]
    [SerializeField]
    private float _actionTime = 3.0f;
    #endregion

    #region private
    /// <summary>アクション中かどうか</summary>
    private bool _isInAction = false;
    /// <summary>現在行えるアクション</summary>
    IActionable _currentActionable = default;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                OnAction();
            })
            .AddTo(this);
    }
    #endregion

    #region public method
    /// <summary>
    /// アクションをセットする
    /// </summary>
    /// <param name="action">セットするアクション</param>
    public void SetAction(IActionable action)
    {
        _currentActionable = action;
        IsApproached = true;
        print("アクションをセット");
    }

    /// <summary>
    /// アクションをリセットする
    /// </summary>
    public void ResetAction()
    {
        _currentActionable = null;
        IsApproached = false;
    }
    #endregion

    #region private method
    /// <summary>
    /// アクション実行判定を行う
    /// </summary>
    private void OnAction()
    {
        if (!IsApproached || _isInAction)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _isInAction = true;
            PlayerController.Instance.MoveInterval(_actionTime);
            StartCoroutine(OnActionCoroutine(_actionTime));
            ActionGauge.StartAction(_actionTime);
        }
    }
    #endregion

    #region coroutine method
    private IEnumerator OnActionCoroutine(float actionTime)
    {
        yield return new WaitForSeconds(actionTime);

        _currentActionable.OnAction();
        _isInAction = false;
    }
    #endregion
}
