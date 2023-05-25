using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

/// <summary>各ステージのオブジェクトの配置、処理等を行うManagerクラス</summary>
public class StageManager : MonoBehaviour
{
    #region property
    public static StageManager Instance { get; private set; }
    public Stage CurrentStage => _currentStage;
    public bool IsCompletedMainMission { get; set; } = false;
    public int CompleteSubMissionNum { get; set; } = 0;
    public bool IsGameover { get; set; } = false;
    public Subject<Vector3> SetStartPositionSubject => _setPlayerSubject;
    public Subject<bool> IsInGameSubject => _isInGameSubject;
    public Subject<Unit> GameStartSubject => _gameStartSubject;
    public Subject<Unit> GamePauseSubject => _gamePauseSubject;
    public Subject<Unit> GameRestartSubject => _gameRestartSubject;
    public Subject<Unit> GameEndSubject => _gameEndSubject;
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
    private int _currentSubMissionCompleteNum = 0;
    private int _currrentScore = 0;
    private Subject<Vector3> _setPlayerSubject = new Subject<Vector3>();
    private Subject<bool> _isInGameSubject = new Subject<bool>();
    private Subject<Unit> _gameStartSubject = new Subject<Unit>();
    private Subject<Unit> _gamePauseSubject = new Subject<Unit>();
    private Subject<Unit> _mainTargetCompleteSubject = new Subject<Unit>();
    private Subject<Unit> _subTargetCompleteSubject = new Subject<Unit>();
    private Subject<Unit> _gameRestartSubject = new Subject<Unit>();
    private Subject<Unit> _gameEndSubject = new Subject<Unit>();
    #endregion

    #region Constant
    private const int COUNT_TIME = 1;
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        Instance = this;
    }
    private IEnumerator Start()
    {
        //Subjectの登録処理のため、1フレーム待機
        yield return null;

        SetupStage();
        StartCoroutine(GameStartCoroutine());
    }
    #endregion

    /// <summary>
    /// ゲームを開始する
    /// </summary>
    #region public method
    public void OnGameStart()
    {
        _inGame = true;
        _isInGameSubject.OnNext(true);
        _gameStartSubject.OnNext(Unit.Default);
    }

    /// <summary>
    /// ゲームを中断する
    /// </summary>
    public void OnGamePause()
    {
        _gamePauseSubject.OnNext(Unit.Default);
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void OnGameEnd()
    {
        _inGame = false;
        _isInGameSubject.OnNext(false);
        _gameEndSubject.OnNext(Unit.Default);
        StartCoroutine(GameEndCoroutine());
    }
    #endregion

    #region private method
    private void SetupStage()
    {
        var currentStageType = GameManager.Instance.CurrentGameStates;

        _currentStage = DataManager.Instance.Data.Stages.FirstOrDefault(x => x.StageType == currentStageType);

        var stagePrefab = _stageModels.FirstOrDefault(x => x.StageType == currentStageType).StagePrefab;
        Instantiate(stagePrefab);

        //子オブジェクトにあるRoomコンポーネントを全て取得
        Room[] rooms = stagePrefab.GetComponentsInChildren<Room>();

        //スタート位置の座標を取得
        Vector3 startPos = rooms.FirstOrDefault(x => x.RoomType == RoomType.Start).transform.position;
        _setPlayerSubject.OnNext(startPos);
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

        yield return new WaitForSeconds(COUNT_TIME);

        OnGameStart();
    }

    /// <summary>
    /// ゲーム終了時に実行するコルーチン
    /// </summary>
    private IEnumerator GameEndCoroutine()
    {
        Debug.Log("ゲーム終了");
        yield return new WaitForSeconds(2.0f);

        //ゲームオーバー状態ではない場合
        if (!IsGameover)
        {
            //テスト処理
            _currrentScore = 3000;
            _currentSubMissionCompleteNum = 1;

            _currentStage.SetClearData(_currentSubMissionCompleteNum, _currrentScore);
        }
        //データを保存
        DataManager.Instance.SaveData();

        //Scene遷移機能をどのように作るか不明なため、仮のロード処理を行っている
        SceneManager.LoadScene("Lobby");
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
