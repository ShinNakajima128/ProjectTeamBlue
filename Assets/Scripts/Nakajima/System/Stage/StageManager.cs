using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>各ステージのオブジェクトの配置、処理等を行うManagerクラス</summary>
public class StageManager : MonoBehaviour
{
    #region property
    public static StageManager Instance { get; private set; }
    public Subject<bool> IsInGameSubject => _isInGameSubject;
    public Stage CurrentStage => _currentStage;
    #endregion

    #region serialize
    [Tooltip("各ステージのオブジェクト情報を持つクラスの配列")]
    [SerializeField]
    private StageModel[] _stageModels = default;

    [SerializeField]
    private int _countDownTime = 3; 
    #endregion

    #region private
    private Stage _currentStage;
    private bool _inGame = false;
    private Subject<bool> _isInGameSubject = new Subject<bool>();
    #endregion

    #region Constant
    private const int COUNT_TIME = 1;
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Start()
    {
        SetupStage();
        StartCoroutine(GameStartCoroutine());
    }
    #endregion

    #region public method
    #endregion

    #region private method
    private void SetupStage()
    {
        var currentStageType = GameManager.Instance.CurrentGameStates;

        _currentStage = DataManager.Instance.Data.Stages.FirstOrDefault(x => x.StageType == currentStageType);

        var stagePrefab = _stageModels.FirstOrDefault(x => x.StageType == currentStageType).StagePrefab;
        Instantiate(stagePrefab);
    }
    #endregion

    #region coroutine
    /// <summary>
    /// ゲーム開始前のCoroutine
    /// </summary>
    private IEnumerator GameStartCoroutine()
    {
        for (int i = _countDownTime; i > 0; i--)
        {
            Debug.Log(i.ToString());
            yield return new WaitForSeconds(COUNT_TIME);
        }
        Debug.Log("GameStart");
        _inGame = true;
    }
    #endregion
}

[Serializable]
public class StageModel
{
    public string StageName;
    public GameStates StageType;
    public GameObject StagePrefab;
}
