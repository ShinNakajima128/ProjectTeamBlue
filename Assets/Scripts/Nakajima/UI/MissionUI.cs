using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// 
/// </summary>
public class MissionUI : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("メインターゲット")]
    [SerializeField]
    private Text _mainTargetNumText = default;

    [Tooltip("サブターゲット")]
    [SerializeField]
    private Text _subTargetNumText = default;
    #endregion

    #region private
    private int _currentStageSubMissionNum = 0;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {

    }

    private void Start()
    {
        StageManager.Instance.MainTargetCompleteSubject
                             .Subscribe(_ => CompleteMainMission())
                             .AddTo(this);

        //
        StageManager.Instance.CompleteSubMissionNum
                             .Subscribe(value => CompleteSubMission(value))
                             .AddTo(this);

        _mainTargetNumText.text = "0 / 1";
        CompleteSubMission(0);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    private void CompleteMainMission()
    {
        _mainTargetNumText.text = "1 / 1";
    }

    private void CompleteSubMission(int value)
    {
        _subTargetNumText.text = $"{value} / {_currentStageSubMissionNum}";
    }
    #endregion
}
