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
    [Tooltip("メインターゲットの破壊工作が完了するまでの時間")]
    [SerializeField]
    private float _mainTargetActionTime = 4.0f;

    [Tooltip("サブミッションが完了するまでの時間")]
    [SerializeField]
    private float _subTargetActionTime = 2.0f;
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
            //既にミッションを完了している場合は処理を行わない
            if (_currentActionable.IsCompleted)
            {
                return;
            }

            _isInAction = true;
            float currentActionTime = 0;

            switch (_currentActionable.Type)
            {
                case TargetType.Main:
                    currentActionTime = _mainTargetActionTime;
                    
                    //仮でこの場所に処理を記入しているが別の場所に今後書き直す予定
                    StageManager.Instance.IsCompletedMainMission = true;
                    break;
                case TargetType.Sub:
                    currentActionTime = _subTargetActionTime;

                    //仮でこの場所に処理を記入しているが別の場所に今後書き直す予定
                    StageManager.Instance.CompleteSubMissionNum++;
                    break;
                case TargetType.EscapePoint:
                    
                    if (!StageManager.Instance.IsCompletedMainMission)
                    {
                        Debug.Log("メインミッションを達成していません");
                        _isInAction = false;
                        return;
                    }
                 
                    StageManager.Instance.OnGameEnd(); 
                    return;
            }

            PlayerController.Instance.MoveInterval(currentActionTime);
            StartCoroutine(OnActionCoroutine(currentActionTime));
            ActionGauge.StartAction(currentActionTime);
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
