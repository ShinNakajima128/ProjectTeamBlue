using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// ミニマップ用のカメラを制御するコンポーネント
/// </summary>
public class MiniMapCamera : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        transform.SetParent(playerTrans);
    }

    private void Start()
    {
        this.LateUpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.eulerAngles = new Vector3(90, 0, 0);
            });
    }
    #endregion

    #region public method
    #endregion

    #region private method
    #endregion
}
