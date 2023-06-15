using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

/// <summary>各ステージのオブジェクトの配置、処理等を行うManagerクラス</summary>
public class StageManager : MonoBehaviour
{
    #region property
    public static StageManager Instance { get; private set; }
    public Stage CurrentStage => _currentStage;
    public ReactiveProperty<float> CurrentLimitTime => _currentLimitTime;
    public bool IsCompletedMainMission { get; set; } = false;
    public ReactiveProperty<bool> IsFounded => _isFounded;
    public ReactiveProperty<int> CompleteSubMissionNum => _currentSubMissionCompleteNum;
    public ReactiveProperty<int> StartCountDownNum => _startCountDownNum;
    public bool IsGameover { get; set; } = false;
    public Subject<Vector3> SetStartPositionSubject => _setPlayerSubject;
    public Subject<bool> IsInGameSubject => _isInGameSubject;
    public Subject<Unit> GameStartSubject => _gameStartSubject;
    public Subject<Unit> GamePauseSubject => _gamePauseSubject;
    public Subject<Unit> MainTargetCompleteSubject => _mainTargetCompleteSubject;
    public Subject<Unit> SubMissionCompleteSubject => _subTargetCompleteSubject;
    public Subject<Unit> EnemySpottedSubject => _enemySpottedSubject;
    public Subject<Unit> EnemyLostPlayerSubject => _enemyLostPlayerSubject;
    public Subject<Unit> EscapeSubject => _escapeSubject;
    public Subject<Unit> GameRestartSubject => _gameRestartSubject;
    public Subject<Unit> GameoverSubject => _gameoverSubject;
    public Subject<Unit> GameEndSubject => _gameEndSubject;
    #endregion

    #region serialize
    [Tooltip("制限時間")]
    [SerializeField]
    private float _limitTime = 300f;

    [Tooltip("カウントダウン時間")]
    [SerializeField]
    private int _countDownTime = 3;

    [Tooltip("各ステージのオブジェクト情報を持つクラスの配列")]
    [SerializeField]
    private StageModel[] _stageModels = default;
    #endregion

    #region private
    private Stage _currentStage;
    private ScoreCalculation _scoreCalc;
    private PlayerController _playerCtrl;
    private bool _inGame = false;
    private ReactiveProperty<bool> _isFounded = new ReactiveProperty<bool>();
    private ReactiveProperty<int> _currentSubMissionCompleteNum = new ReactiveProperty<int>();
    private ReactiveProperty<int> _startCountDownNum = new ReactiveProperty<int>();
    private int _currrentScore = 0;
    private ReactiveProperty<float> _currentLimitTime = new ReactiveProperty<float>();
    private Subject<Vector3> _setPlayerSubject = new Subject<Vector3>();
    private Subject<bool> _isInGameSubject = new Subject<bool>();
    private Subject<Unit> _gameStartSubject = new Subject<Unit>();
    private Subject<Unit> _gamePauseSubject = new Subject<Unit>();
    private Subject<Unit> _mainTargetCompleteSubject = new Subject<Unit>();
    private Subject<Unit> _subTargetCompleteSubject = new Subject<Unit>();
    private Subject<Unit> _enemySpottedSubject = new Subject<Unit>();
    private Subject<Unit> _enemyLostPlayerSubject = new Subject<Unit>();
    private Subject<Unit> _escapeSubject = new Subject<Unit>();
    private Subject<Unit> _gameRestartSubject = new Subject<Unit>();
    private Subject<Unit> _gameoverSubject = new Subject<Unit>();
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
        TryGetComponent(out _scoreCalc);
        _playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _startCountDownNum.Value = _countDownTime;
        _isFounded.Value = false;
    }
    private IEnumerator Start()
    {
        //BGMを再生する
        SoundManager.Instance.PlayBGM(SoundTag.BGM_Stage1);

        //画面フェード
        FadeManager.Fade(FadeType.In);

        //ゲーム開始時に、生成されたステージのスタート位置にプレイヤーを移動する処理を登録
        SetStartPositionSubject
        .Subscribe(_playerCtrl.SetStartPosition)
        .AddTo(this);

        //インゲーム中の操作可不可を切り替える処理を登録
        IsInGameSubject
        .Subscribe(_playerCtrl.ChangeIsOperatable)
        .AddTo(this);


        //Subjectの登録処理のため、1フレーム待機
        yield return null;

        SetupStage();
        StartCoroutine(GameStartCoroutine());

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (_inGame)
                {
                    _currentLimitTime.Value -= Time.deltaTime;

                    //制限時間が終了したらゲーム終了
                    if (_currentLimitTime.Value <= 0)
                    {
                        _inGame = false;
                        OnGameEnd();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnGamePause();
                    }
                }                
            })
            .AddTo(this);
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

    public void OnEscapeEvent()
    {
        _inGame = false;
        _isInGameSubject.OnNext(false);
        _escapeSubject.OnNext(Unit.Default);
    }
    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void OnGameEnd()
    {
        _inGame = false;

        FadeManager.Fade(FadeType.Out, () =>
        {
            FadeManager.Fade(FadeType.In);
            _gameEndSubject.OnNext(Unit.Default);
            StartCoroutine(GameEndCoroutine());
        });
    }
    
    /// <summary>
    /// メインターゲット達成時に実行する
    /// </summary>
    public void OnMainTargetComplete()
    {
        IsCompletedMainMission = true;
        SoundManager.Instance.PlaySE(SoundTag.SE_CompleteMainMission);
        _mainTargetCompleteSubject.OnNext(Unit.Default);
    }

    /// <summary>
    /// サブターゲット達成時に実行する
    /// </summary>
    public void OnSubTargetComplete()
    {
        _currentSubMissionCompleteNum.Value++;
        SoundManager.Instance.PlaySE(SoundTag.SE_CompleteSubMission);
        _subTargetCompleteSubject.OnNext(Unit.Default);
        Debug.Log("サブターゲット達成");
    }

    /// <summary>
    /// 逃げ切り（ステージクリア）時に実行する
    /// </summary>
    public void OnEscape()
    {
        if (IsCompletedMainMission)
        {
            _escapeSubject.OnNext(Unit.Default);
        }
    }
    #endregion

    #region private method
    private void SetupStage()
    {
        var currentStageType = GameManager.Instance.CurrentGameStates;

        _currentStage = DataManager.Instance.Data.Stages.FirstOrDefault(x => x.StageType == currentStageType);

        _currentLimitTime.Value = _limitTime;

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
        for (int i = _startCountDownNum.Value; i > 0; i--)
        {
            yield return new WaitForSeconds(COUNT_TIME);
            _startCountDownNum.Value--;
        }

        yield return new WaitForSeconds(COUNT_TIME);

        OnGameStart();
    }

    /// <summary>
    /// ゲーム終了時に実行するコルーチン
    /// </summary>
    private IEnumerator GameEndCoroutine()
    {
        Debug.Log("ゲーム終了");

        //ゲームオーバー状態ではない場合
        if (!IsGameover)
        {
            //テスト処理
            _currrentScore = _scoreCalc.CalcurlationScore(_currentSubMissionCompleteNum.Value, (int)_currentLimitTime.Value); ;

            _currentStage.SetClearData(_currentSubMissionCompleteNum.Value, _currrentScore);
            //データを保存
            DataManager.Instance.SaveData();
        }
        else
        {
            //yield return new WaitForSeconds(2.0f);

            //Scene遷移機能をどのように作るか不明なため、仮のロード処理を行っている
            //SceneManager.LoadScene("Lobby");
        }
        yield return null;
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
