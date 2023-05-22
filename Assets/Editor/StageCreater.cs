using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageCreater : EditorWindow
{
    #region property
    public int StageSize => _stageSize;
    public float GridSize => _gridSize;
    public string StageName => _stageName;
    public string SelectedPrefabPath => _selectedPrefabPath;
    public bool IsEraserMode => _isEraserMode;
    #endregion

    #region serialize
    #endregion

    #region private
    /// <summary>通路パーツをまとめたフォルダ</summary>
    private Object _corridorDirectry;
    /// <summary>部屋パーツをまとめたフォルダ</summary>
    private Object _roomDirectry;
    /// <summary>ステージ全体の大きさ</summary>
    private int _stageSize = 30;
    /// <summary>エディタ画面のグリッドの大きさ</summary>
    private float _gridSize = 20.0f;
    /// <summary>作成したオブジェクトの保存先フォルダ</summary>
    private Object _addToDirectry;
    /// <summary>ステージ名</summary>
    private string _stageName = "";
    /// <summary>ステージ作成画面のサブウィンドウ</summary>
    private StageCreaterSubWindow _subWindow;
    /// <summary>選択しているPrefabのパス</summary>
    private string _selectedPrefabPath = "";
    /// <summary>消しゴム状態かどうか</summary>
    private bool _isEraserMode = false;
    //===Objects===
    private List<GameObject> _corridorPrefabList = new List<GameObject>();
    private List<Texture> _corridorTextureList = new List<Texture>();
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void OnGUI()
    {
        //通路Prefabが入っているフォルダをセットする枠の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Corrider Directry", GUILayout.Width(150));
        _corridorDirectry = EditorGUILayout.ObjectField(_corridorDirectry, typeof(Object), true);
        GUILayout.EndHorizontal();

        //部屋Prefabが入っているフォルダをセットする枠の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Room Directry", GUILayout.Width(150));
        _roomDirectry = EditorGUILayout.ObjectField(_roomDirectry, typeof(Object), true);
        GUILayout.EndHorizontal();

        //ステージ全体の大きさの値を設定する項目の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Stage Size", GUILayout.Width(150));
        _stageSize = EditorGUILayout.IntField(_stageSize);
        GUILayout.EndHorizontal();

        //ステージ作成画面のグリッド幅の設定をする項目の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Size", GUILayout.Width(150));
        _gridSize = EditorGUILayout.FloatField(_gridSize);
        GUILayout.EndHorizontal();

        //ステージ名を入力する項目の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("StageName", GUILayout.Width(150));
        _stageName = EditorGUILayout.TextField(_stageName);
        GUILayout.EndHorizontal();

        ////テスト用のボタン
        //if (GUILayout.Button("テストボタン"))
        //{
        //    //ステージのパーツを入れる親オブジェクトを作成
        //    //var parentObj = new GameObject(_stageName);[

        //    if (_corridorDirectry != null)
        //    {
        //        string folderPath = AssetDatabase.GetAssetPath(_corridorDirectry);
        //        string[] fileaddlesses = Directory.GetFiles(folderPath);

        //        var excludedList = fileaddlesses.Where(x => !x.Contains(".meta"));

        //        _corridorPrefabList.Clear();
        //        var parentObj = new GameObject(_stageName);

        //        foreach (var addless in excludedList)
        //        {
        //            var corridorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(addless);
        //            _corridorPrefabList.Add(corridorPrefab);
        //            Instantiate(corridorPrefab, parentObj.transform);
        //        }
        //    }
        //}
        //ツールバー描画
        DrawToolBar();

        //通路パーツ一覧描画
        DrawImageCorridorParts();

        //部屋パーツ一覧描画
        DrawImageRoomParts();

        GUILayout.FlexibleSpace();

        //ステージ編集画面表示ボタン描画
        DrawStageWindowButton();
    }
    #endregion

    #region public method
    #endregion

    #region private method
    [MenuItem("Window/StageCreater")]
    private static void ShowMainWindow()
    {
        GetWindow(typeof(StageCreater));
    }

    private void DrawToolBar()
    {
        var e = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AssetStoreTools/Temps/Editor/Eraser.png");

        if (e != null)
        {
            EditorGUILayout.LabelField("ツール一覧");

            float x = 0f;
            float y = 0f;
            float width = 50.0f;
            float height = 50.0f;

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button(e, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height)))
            {
                _isEraserMode = true;
                _selectedPrefabPath = "";
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawImageCorridorParts()
    {
        if (_corridorDirectry != null)
        {
            EditorGUILayout.LabelField("通路パーツ一覧");

            float x = 0f;
            float y = 0f;
            float width = 50.0f;
            float height = 50.0f;
            float maxWidth = 500.0f;

            //通路フォルダのパスを取得
            string folderPath = AssetDatabase.GetAssetPath(_corridorDirectry);
            //フォルダ内のファイルのアドレスを全て取得
            string[] fileaddlesses = Directory.GetFiles(folderPath);
            //取得したリストからmetaファイルのアドレスを除く
            var excludedList = fileaddlesses.Where(f => !f.Contains(".meta")).ToArray();

            EditorGUILayout.BeginVertical();

            foreach (var path in excludedList)
            {
                if (x > maxWidth)
                {
                    x = 0f;
                    y += height;
                    EditorGUILayout.EndHorizontal();
                }

                if (x == 0f)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                var corridorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<Corridor>();
                Texture2D texture = corridorPrefab.PartsTextures[0];

                //ボタンの描画
                if (GUILayout.Button(texture, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height)))
                {
                    _isEraserMode = false; //消しゴムモードの場合は解除
                    _selectedPrefabPath = path;
                    corridorPrefab.CurrentDirType = DirectionType.North; //パーツ選択時に向きをリセット
                }
                x += width;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawImageRoomParts()
    {
        if (_roomDirectry != null)
        {
            EditorGUILayout.LabelField("部屋パーツ一覧");

            float x = 0f;
            float y = 0f;
            float width = 50.0f;
            float height = 50.0f;
            float maxWidth = 500.0f;

            //部屋フォルダのパスを取得
            string folderPath = AssetDatabase.GetAssetPath(_roomDirectry);
            //フォルダ内のファイルのアドレスを全て取得
            string[] fileaddlesses = Directory.GetFiles(folderPath);
            //取得したリストからmetaファイルのアドレスを除く
            var excludedList = fileaddlesses.Where(f => !f.Contains(".meta")).ToArray();

            EditorGUILayout.BeginVertical();

            foreach (var path in excludedList)
            {
                if (x > maxWidth)
                {
                    x = 0f;
                    y += height;
                    EditorGUILayout.EndHorizontal();
                }

                if (x == 0f)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                var roomPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<Room>();
                Texture2D texture = roomPrefab.PartsTextures[0];

                //ボタンの描画
                if (GUILayout.Button(texture, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height)))
                {
                    _isEraserMode = false; //消しゴムモードの場合は解除
                    _selectedPrefabPath = path;
                    roomPrefab.CurrentDirType = DirectionType.North; //パーツ選択時に向きをリセット
                }
                x += width;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawStageWindowButton()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("ステージエディタを開く"))
        {
            if (_subWindow == null)
            {
                //新しくサブウィンドウを作成する
                _subWindow = StageCreaterSubWindow.WillAppear(this);
            }
            else
            {
                _subWindow.Focus();
            }
        }
        EditorGUILayout.EndVertical();
    }
    #endregion
}

public class StageCreaterSubWindow : EditorWindow
{
    #region private 
    private int _stageSize = 0;
    private float _gridSize = 0f;
    private string[,] _stage;
    private StageCell[,] _stageCells;
    private Rect[,] _gridRect;
    private StageCreater _parentWindow;
    #endregion

    #region constant
    /// <summary>ウィンドウの横幅</summary>
    private const float WINDOW_WIDTH = 750.0f;
    /// <summary>ウィンドウの縦幅</summary>
    private const float WINDOW_HEIGHT = 750.0f;
    #endregion

    #region unity method
    private void OnGUI()
    {
        // グリッド線を描画する
        for (int column = 0; column < _stageSize; column++)
        {
            for (int row = 0; row < _stageSize; row++)
            {
                DrawGridLine(_gridRect[column, row]);
            }
        }

        // クリックされた位置を探して、その場所に画像データを入れる
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.type != EventType.ContextClick)
        {
            if (!_parentWindow.IsEraserMode)
            {
                Vector2 pos = Event.current.mousePosition;
                int row;

                for (row = 0; row < _stageSize; row++)
                {
                    Rect r = _gridRect[0, row];

                    if (r.x <= pos.x && pos.x <= r.x + r.width)
                    {
                        break;
                    }
                }

                for (int column = 0; column < _stageSize; column++)
                {
                    if (_gridRect[column, row].Contains(pos))
                    {
                        _stageCells[column, row].SetData(_stageCells, _parentWindow.SelectedPrefabPath);
                        Repaint();
                        break;
                    }
                }
            }
            else
            {
                //パーツ画像を消去する処理
                Vector2 pos = Event.current.mousePosition;
                int row;

                for (row = 0; row < _stageSize; row++)
                {
                    Rect r = _gridRect[0, row];

                    if (r.x <= pos.x && pos.x <= r.x + r.width)
                    {
                        break;
                    }
                }

                for (int column = 0; column < _stageSize; column++)
                {
                    if (_gridRect[column, row].Contains(pos))
                    {
                        //_stage[column, row] = "";
                        _stageCells[column, row].ResetData(_stageCells);
                        Repaint();
                        break;
                    }
                }
            }
        }
        else if (e.type == EventType.ContextClick)
        {


            if (_parentWindow.SelectedPrefabPath != "")
            {
                var parts = AssetDatabase.LoadAssetAtPath<GameObject>(_parentWindow.SelectedPrefabPath).GetComponent<PartsBase>();

                switch (parts.CurrentDirType)
                {
                    case DirectionType.North:
                        parts.CurrentDirType = DirectionType.East;
                        break;
                    case DirectionType.East:
                        parts.CurrentDirType = DirectionType.Sorth;
                        break;
                    case DirectionType.Sorth:
                        parts.CurrentDirType = DirectionType.West;
                        break;
                    case DirectionType.West:
                        parts.CurrentDirType = DirectionType.North;
                        break;
                    default:
                        break;
                }
            }
            Repaint();
        }

        for (int column = 0; column < _stageSize; column++)
        {
            for (int row = 0; row < _stageSize; row++)
            {
                //ステージデータにパスがセットされている部分に画像を描写する
                if (_stage[column, row] != null && _stage[column, row].Length > 0)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_stage[column, row]).GetComponent<PartsBase>();
                    Texture2D texture = prefab.CurrentDirTexture;

                    GUI.DrawTexture(_gridRect[column, row], texture);
                }

                if (_stageCells[column, row] != null &&
                    _stageCells[column, row].CurrentState != CellState.None &&
                    _stageCells[column, row].IsOriginSet)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_stageCells[column, row].PrefabPath).GetComponent<PartsBase>();

                    switch (prefab.PartsType)
                    {
                        case PartsType.None:
                            break;
                        case PartsType.Corridor:
                            Corridor c = prefab.GetComponent<Corridor>();
                            DrawCorridorTexture(column, row, c.CorridorType, _stageCells[column, row].CurrentDir, c.GetTextureByDirection(_stageCells[column, row].CurrentDir));
                            break;
                        case PartsType.Room:
                            Room r = prefab.GetComponent<Room>();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        Rect rect = new Rect(0, WINDOW_WIDTH - 50, 300, 50);
        GUILayout.BeginArea(rect);
        if (GUILayout.Button("ステージ生成", GUILayout.MinWidth(300), GUILayout.MinHeight(50)))
        {
            CheckCurrentStageData(_stageCells);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();

        if (_parentWindow.SelectedPrefabPath != "")
        {
            if (Event.current.mousePosition.x >= 0 &&
                Event.current.mousePosition.x <= _stageSize * _gridSize &&
                Event.current.mousePosition.y >= 0 &&
                Event.current.mousePosition.y <= _stageSize * _gridSize)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_parentWindow.SelectedPrefabPath).GetComponent<PartsBase>();
                Texture2D texture = prefab.CurrentDirTexture;
                Rect currentSelectParts = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 30, 30);
                GUI.DrawTexture(currentSelectParts, texture);
                Repaint();
            }
        }
    }
    #endregion

    #region public method
    public static StageCreaterSubWindow WillAppear(StageCreater parent)
    {
        StageCreaterSubWindow window = (StageCreaterSubWindow)GetWindow(typeof(StageCreaterSubWindow), false);
        window.Show();
        window.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
        window.SetParent(parent);
        window.Setup();
        return window;
    }
    public void Setup()
    {
        _stageSize = _parentWindow.StageSize;
        _gridSize = _parentWindow.GridSize;

        _stage = new string[_stageSize, _stageSize];
        _stageCells = new StageCell[_stageSize, _stageSize];

        for (int i = 0; i < _stage.GetLength(0); i++)
        {
            for (int j = 0; j < _stage.GetLength(1); j++)
            {
                _stage[i, j] = default;
            }
        }

        for (int i = 0; i < _stageCells.GetLength(0); i++)
        {
            for (int j = 0; j < _stageCells.GetLength(1); j++)
            {
                _stageCells[i, j] = new StageCell(i, j);
            }
        }

        _gridRect = CreateGrid(_stageSize);
    }
    #endregion

    #region private method
    private void SetParent(StageCreater parent)
    {
        _parentWindow = parent;
    }

    /// <summary>
    /// グリッドのデータを作成
    /// </summary>
    /// <param name="size">グリッドのサイズ</param>
    /// <returns>作成したデータ</returns>
    private Rect[,] CreateGrid(int size)
    {
        int width = size;
        int height = size;

        float x = 0f;
        float y = 0;
        float w = _gridSize;
        float h = _gridSize;

        Rect[,] resultRects = new Rect[width, height];

        for (int column = 0; column < height; column++)
        {
            x = 0f;
            for (int row = 0; row < width; row++)
            {
                Rect r = new Rect(new Vector2(x, y), new Vector2(w, h));
                resultRects[column, row] = r;
                x += w;
            }
            y += h;
        }
        return resultRects;
    }

    /// <summary>
    /// グリッド線を描画
    /// </summary>
    /// <param name="r"></param>
    private void DrawGridLine(Rect r)
    {
        Handles.color = new Color(1f, 1f, 1f, 0.5f);

        //上側のライン
        Handles.DrawLine(new Vector2(r.position.x, r.position.y),
                         new Vector2(r.position.x + r.size.x, r.position.y));
        //下側のライン
        Handles.DrawLine(new Vector2(r.position.x, r.position.y + r.size.y),
                         new Vector2(r.position.x + r.size.x, r.position.y + r.size.y));
        //左側のライン
        Handles.DrawLine(new Vector2(r.position.x, r.position.y),
                         new Vector2(r.position.x, r.position.y + r.size.y));
        //右側のライン
        Handles.DrawLine(new Vector2(r.position.x + r.size.x, r.position.y),
                         new Vector2(r.position.x + r.size.x, r.position.y + r.size.y));
    }

    private void DrawCorridorTexture(int column, int row, CorridorType corridorType, DirectionType dirType, Texture2D texture)
    {
        switch (corridorType)
        {
            case CorridorType.Straight_1:
                GUI.DrawTexture(_gridRect[column, row], texture);
                break;
            case CorridorType.Straight_2:
                GUI.DrawTexture(_gridRect[column, row], texture);
                break;
            case CorridorType.Straight_3:
                GUI.DrawTexture(_gridRect[column, row], texture);
                break;
            case CorridorType.Straight_End:
                GUI.DrawTexture(_gridRect[column, row], texture);
                break;
            case CorridorType.Straight_Large:
                switch (dirType)
                {
                    case DirectionType.North:
                        //大きい通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x,
                                                 _gridRect[column, row].position.y - _gridSize,
                                                 _gridSize,
                                                 _gridSize * 3), texture);
                        break;
                    case DirectionType.East:
                        //大きい通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x + _gridSize,
                                                 _gridRect[column, row].position.y,
                                                 _gridSize * 3,
                                                 _gridSize), texture);
                        break;
                    case DirectionType.Sorth:
                        //大きい通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x,
                                                 _gridRect[column, row].position.y - _gridSize,
                                                 _gridSize,
                                                 _gridSize * 3), texture);
                        break;
                    case DirectionType.West:
                        //大きい通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x + _gridSize,
                                                 _gridRect[column, row].position.y,
                                                 _gridSize * 3,
                                                 _gridSize), texture);
                        break;
                    default:
                        break;
                }
                break;
            case CorridorType.Sharp_L:
                //L字通路のテクスチャ用にRectサイズを変更して描画する
                GUI.DrawTexture(new Rect(_gridRect[column, row].position.x,
                                         _gridRect[column, row].position.y,
                                         _gridSize * 2,
                                         _gridSize * 2), texture);
                break;
            case CorridorType.Sharp_T:
                switch (dirType)
                {
                    case DirectionType.North:
                        //T字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x - _gridSize,
                                                 _gridRect[column, row].position.y,
                                                 _gridSize * 3,
                                                 _gridSize * 2), texture);
                        break;
                    case DirectionType.East:
                        //T字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x - _gridSize,
                                                 _gridRect[column, row].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 3), texture);
                        break;
                    case DirectionType.Sorth:
                        //T字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x - _gridSize,
                                                 _gridRect[column, row].position.y,
                                                 _gridSize * 3,
                                                 _gridSize * 2), texture);
                        break;
                    case DirectionType.West:
                        //T字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x - _gridSize,
                                                 _gridRect[column, row].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 3), texture);
                        break;
                    default:
                        break;
                }
                //T字通路のテクスチャ用にRectサイズを変更して描画する
                GUI.DrawTexture(new Rect(_gridRect[column, row].position.x - _gridSize,
                                         _gridRect[column, row].position.y,
                                         _gridSize * 3,
                                         _gridSize * 2), texture);
                break;
            case CorridorType.Cross:
                //十字通路のテクスチャ用にRectサイズを変更して描画する
                GUI.DrawTexture(new Rect(_gridRect[column, row].position.x - _gridSize,
                                         _gridRect[column, row].position.y - _gridSize,
                                         _gridSize * 3,
                                         _gridSize * 3), texture);
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 現在のステージデータを確認する
    /// </summary>
    /// <param name="stageCells">確認するデータ</param>
    private void CheckCurrentStageData(StageCell[,] stageCells)
    {
        string dataStr = "";

        for (int column = 0; column < stageCells.GetLength(0); column++)
        {
            for (int row = 0; row < stageCells.GetLength(1); row++)
            {
                if (stageCells[column, row].CurrentState != CellState.None)
                {
                    dataStr += "#";
                }
                else
                {
                    dataStr += ".";
                }
            }
            dataStr += "\n";
        }

        Debug.Log(dataStr);
    }
    #endregion
}

/// <summary>
/// グリッド上に配置するマス
/// </summary>
public class StageCell
{
    #region property
    public int Column => _column;
    public int Row => _row;
    public CellState CurrentState { get; set; } = CellState.None;
    public DirectionType CurrentDir { get; set; } = DirectionType.North;
    public string PrefabPath { get; set; } = "";
    public bool IsOriginSet => _isOriginSet;
    #endregion

    #region private
    private int _column;
    private int _row;
    /// <summary>パーツの原点がセットされているか</summary>
    private bool _isOriginSet;
    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="column">列</param>
    /// <param name="row">行</param>
    public StageCell(int column, int row)
    {
        _column = column;
        _row = row;
    }

    public void SetData(StageCell[,] cells, string path)
    {
        var t = AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<PartsBase>();

        switch (t.PartsType)
        {
            case PartsType.None:
                Debug.LogError("パーツの指定が間違っています。");
                break;
            case PartsType.Corridor:
                Corridor c = t.GetComponent<Corridor>();

                if (CheckGrid(cells, c.CorridorType))
                {
                    PrefabPath = path;
                    CurrentDir = c.CurrentDirType;
                }
                else
                {
                    Debug.LogError("パーツをセットできませんでした");
                }
                break;
            case PartsType.Room:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// マスのデータをリセットする
    /// </summary>
    /// <param name="cells">マス全体のデータ</param>
    public void ResetData(StageCell[,] cells)
    {
        CurrentState = CellState.None;
        PrefabPath = "";
        CurrentDir = DirectionType.North;
        _isOriginSet = false;

        switch (CurrentState)
        {
            case CellState.None:
                break;
            case CellState.Corridor_Straight:
                break;
            case CellState.Corridor_Straight_Large:
                cells[_column - 1, _row].CurrentState = CellState.None;
                cells[_column + 1, _row].CurrentState = CellState.None;
                break;
            case CellState.Corridor_Sharp_L:
                cells[_column + 1, _row].CurrentState = CellState.None;
                cells[_column, _row + 1].CurrentState = CellState.None;
                cells[_column + 1, _row + 1].CurrentState = CellState.None;
                break;
            case CellState.Corridor_Sharp_T:
                cells[_column - 1, _row].CurrentState = CellState.None;
                cells[_column + 1, _row].CurrentState = CellState.None;
                cells[_column - 1, _row + 1].CurrentState = CellState.None;
                cells[_column, _row + 1].CurrentState = CellState.None;
                cells[_column + 1, _row + 1].CurrentState = CellState.None;
                break;
            case CellState.Corridor_Cross:
                cells[_column - 1, _row - 1].CurrentState = CellState.None;
                cells[_column, _row - 1].CurrentState = CellState.None;
                cells[_column + 1, _row - 1].CurrentState = CellState.None;
                cells[_column - 1, _row].CurrentState = CellState.None;
                cells[_column + 1, _row].CurrentState = CellState.None;
                cells[_column - 1, _row + 1].CurrentState = CellState.None;
                cells[_column, _row + 1].CurrentState = CellState.None;
                cells[_column + 1, _row + 1].CurrentState = CellState.None;
                break;
            case CellState.Room_Start:
                break;
            case CellState.Room_MainMissionTarget:
                break;
            case CellState.Room_subMissionTarget:
                break;
            case CellState.Room_Escape:
                break;
            case CellState.Room_Cross:
                break;
            default:
                break;
        }
    }

    private bool CheckGrid(StageCell[,] cells, CorridorType type)
    {
        //既にセットされている場合は処理を行わない
        if (CurrentState != CellState.None)
        {
            return false;
        }

        switch (type)
        {
            case CorridorType.Straight_1:
                CurrentState = CellState.Corridor_Straight;
                _isOriginSet = true;
                break;
            case CorridorType.Straight_2:
                CurrentState = CellState.Corridor_Straight;
                _isOriginSet = true;
                break;
            case CorridorType.Straight_3:
                CurrentState = CellState.Corridor_Straight;
                _isOriginSet = true;
                break;
            case CorridorType.Straight_End:
                CurrentState = CellState.Corridor_Straight;
                _isOriginSet = true;
                break;
            case CorridorType.Straight_Large:
                //パーツがグリッドの範囲外に出る場合は処理を行わない
                if (_column - 1 < 0 ||
                    _column + 1 > cells.GetLength(0) - 1)
                {
                    return false;
                }

                CurrentState = CellState.Corridor_Straight_Large;
                _isOriginSet = true;

                //上下1マスを同じステータスに変更
                cells[_column - 1, _row].CurrentState = CellState.Corridor_Straight_Large;
                cells[_column + 1, _row].CurrentState = CellState.Corridor_Straight_Large;
                break;
            case CorridorType.Sharp_L:
                //パーツがグリッドの範囲外に出る場合は処理を行わない
                if (_column + 1 > cells.GetLength(0) - 1 ||
                    _row + 1 > cells.GetLength(1) - 1)
                {
                    return false;
                }

                CurrentState = CellState.Corridor_Sharp_L;
                _isOriginSet = true;

                //原点以外のマスを同じステータスに変更
                cells[_column + 1, _row].CurrentState = CellState.Corridor_Sharp_L;
                cells[_column, _row + 1].CurrentState = CellState.Corridor_Sharp_L;
                cells[_column + 1, _row + 1].CurrentState = CellState.Corridor_Sharp_L;
                break;
            case CorridorType.Sharp_T:
                //パーツがグリッドの範囲外に出る場合は処理を行わない
                if (_column + 1 > cells.GetLength(0) - 1 ||
                    _row - 1 < 0 ||
                    _row + 1 > cells.GetLength(1) - 1)
                {
                    return false;
                }

                CurrentState = CellState.Corridor_Sharp_T;
                _isOriginSet = true;

                //原点以外のマスを同じステータスに変更
                cells[_column - 1, _row].CurrentState = CellState.Corridor_Sharp_T;
                cells[_column + 1, _row].CurrentState = CellState.Corridor_Sharp_T;
                cells[_column - 1, _row + 1].CurrentState = CellState.Corridor_Sharp_T;
                cells[_column, _row + 1].CurrentState = CellState.Corridor_Sharp_T;
                cells[_column + 1, _row + 1].CurrentState = CellState.Corridor_Sharp_T;
                break;
            case CorridorType.Cross:
                //パーツがグリッドの範囲外に出る場合は処理を行わない
                if (_column - 1 < 0 ||
                    _column + 1 > cells.GetLength(0) - 1 ||
                    _row - 1 < 0 ||
                    _row + 1 > cells.GetLength(1) - 1)
                {
                    return false;
                }

                CurrentState = CellState.Corridor_Cross;
                _isOriginSet = true;

                //原点以外のマスを同じステータスに変更
                cells[_column - 1, _row - 1].CurrentState = CellState.Corridor_Cross;
                cells[_column, _row - 1].CurrentState = CellState.Corridor_Cross;
                cells[_column + 1, _row - 1].CurrentState = CellState.Corridor_Cross;
                cells[_column - 1, _row].CurrentState = CellState.Corridor_Cross;
                cells[_column + 1, _row].CurrentState = CellState.Corridor_Cross;
                cells[_column - 1, _row + 1].CurrentState = CellState.Corridor_Cross;
                cells[_column, _row + 1].CurrentState = CellState.Corridor_Cross;
                cells[_column + 1, _row + 1].CurrentState = CellState.Corridor_Cross;
                break;
            default:
                break;
        }
        return true;
    }
}

/// <summary>
/// マスの状態
/// </summary>
public enum CellState
{
    None,
    Corridor_Straight,
    Corridor_Straight_Large,
    Corridor_Sharp_L,
    Corridor_Sharp_T,
    Corridor_Cross,
    Room_Start,
    Room_MainMissionTarget,
    Room_subMissionTarget,
    Room_Escape,
    Room_Cross
}