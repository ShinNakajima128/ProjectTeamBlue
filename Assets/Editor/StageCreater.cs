using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// ステージをエディタ上で生成する自作ウィンドウEditor拡張機能
/// </summary>
public class StageCreater : EditorWindow
{
    #region property
    public int StageSize => _stageSize;
    public float GridSize => _gridSize;
    public string StageName => _stageName;
    public string SelectedPrefabPath => _selectedPrefabPath;
    public bool IsEraserMode => _isEraserMode;
    public bool IsCreatePrefab => _isCreatePrefab;
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
    /// <summary>ステージ名</summary>
    private string _stageName = "";
    /// <summary>ステージ作成画面のサブウィンドウ</summary>
    private StageCreaterSubWindow _subWindow;
    /// <summary>選択しているPrefabのパス</summary>
    private string _selectedPrefabPath = "";
    /// <summary>消しゴム状態かどうか</summary>
    private bool _isEraserMode = false;
    /// <summary>prefabを作成するかどうか</summary>
    private bool _isCreatePrefab = false;
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

        GUILayout.BeginHorizontal();
        _isCreatePrefab = EditorGUILayout.Toggle("IsCreate Prefab", _isCreatePrefab);
        GUILayout.EndHorizontal();

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
    private StageCell[][] _stageCells;
    private Rect[,] _gridRect;
    private StageCreater _parentWindow;
    private DirectionType _currentSelectDirType = DirectionType.North;
    private Object _stageDataFileDirectry;
    #endregion

    #region constant
    /// <summary>ウィンドウの横幅</summary>
    private const float WINDOW_WIDTH = 600.0f;
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
        
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            //消しゴムモードではない場合
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
                        _stageCells[column][row].SetData(_stageCells, _parentWindow.SelectedPrefabPath, _currentSelectDirType);
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
                        _stageCells[column][row].ResetData(_stageCells);
                        Repaint();
                        break;
                    }
                }
            }
        }
        //右クリックでパーツを回転させる処理
        else if (e.type == EventType.ContextClick && e.button == 1)
        {
            if (_parentWindow.SelectedPrefabPath != "")
            {
                switch (_currentSelectDirType)
                {
                    case DirectionType.North:
                        _currentSelectDirType = DirectionType.East;
                        break;
                    case DirectionType.East:
                        _currentSelectDirType = DirectionType.Sorth;
                        break;
                    case DirectionType.Sorth:
                        _currentSelectDirType = DirectionType.West;
                        break;
                    case DirectionType.West:
                        _currentSelectDirType = DirectionType.North;
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
                if (_stage[column, row] != null && _stageCells[column][row].PrefabPath.Length > 0)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_stage[column, row]).GetComponent<PartsBase>();
                    Texture2D texture = prefab.CurrentDirTexture;

                    GUI.DrawTexture(_gridRect[column, row], texture);
                }

                if (_stageCells[column][row] != null &&
                    _stageCells[column][row].IsOriginSet)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_stageCells[column][row].PrefabPath).GetComponent<PartsBase>();

                    switch (prefab.PartsType)
                    {
                        case PartsType.None:
                            break;
                        case PartsType.Corridor:
                            Corridor c = prefab.GetComponent<Corridor>();
                            DrawCorridorTexture(column, row, c.CorridorType, _stageCells[column][row].CurrentDir, c.GetTextureByDirection(_stageCells[column][row].CurrentDir));
                            break;
                        case PartsType.Room:
                            Room r = prefab.GetComponent<Room>();
                            DrawRoomTexture(column, row, r.GetTextureByDirection(_stageCells[column][row].CurrentDir));
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        GUILayout.Space(600);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Stage Data File", GUILayout.Width(150));
        _stageDataFileDirectry = EditorGUILayout.ObjectField(_stageDataFileDirectry, typeof(Object), true);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("ステージデータ読込み", GUILayout.MinWidth(200), GUILayout.MinHeight(30)))
        {
            //ファイルがセットされている場合
            if (_stageDataFileDirectry != null)
            {
                LoadStageDataFile();
            }
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("【操作方法】", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("・パーツ選択後、配置したいマスを「左クリック」でパーツを配置");
        EditorGUILayout.LabelField("・「右クリック」で回転");
        EditorGUILayout.LabelField("・配置完了後、「ステージ生成」で「Hierarchy」に生成されます");
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        Rect rect = new Rect(WINDOW_WIDTH - 200, WINDOW_HEIGHT - 50, 200, 50);
        GUILayout.BeginArea(rect);
        if (GUILayout.Button("ステージ生成", GUILayout.MinWidth(200), GUILayout.MinHeight(50)))
        {
            GenerateStageObject(_stageCells);
            //CheckCurrentStageData(_stageCells);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();
        GUILayout.EndVertical();

        if (_parentWindow.SelectedPrefabPath != "")
        {
            if (Event.current.mousePosition.x >= 0 &&
                Event.current.mousePosition.x <= _stageSize * _gridSize &&
                Event.current.mousePosition.y >= 0 &&
                Event.current.mousePosition.y <= _stageSize * _gridSize)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_parentWindow.SelectedPrefabPath).GetComponent<PartsBase>();
                
                Texture2D texture = prefab.GetTextureByDirection(_currentSelectDirType);
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
        _stageCells = new StageCell[_stageSize][];

        for (int i = 0; i < _stageSize; i++)
        {
            _stageCells[i] = new StageCell[_stageSize];
        }

        for (int i = 0; i < _stage.GetLength(0); i++)
        {
            for (int j = 0; j < _stage.GetLength(1); j++)
            {
                _stage[i, j] = default;
            }
        }

        for (int i = 0; i < _stageSize; i++)
        {
            for (int j = 0; j < _stageSize; j++)
            {
                _stageCells[i][j] = new StageCell();
                _stageCells[i][j].Column = i;
                _stageCells[i][j].Row = j;
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
                switch (dirType)
                {
                    case DirectionType.North:
                        //L字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x,
                                                 _gridRect[column, row].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 2), texture);
                        break;
                    case DirectionType.East:
                        //L字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column, row - 1].position.x,
                                                 _gridRect[column, row - 1].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 2), texture);
                        break;
                    case DirectionType.Sorth:
                        //L字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column - 1, row - 1].position.x,
                                                 _gridRect[column - 1, row - 1].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 2), texture);
                        break;
                    case DirectionType.West:
                        //L字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column - 1, row].position.x,
                                                 _gridRect[column - 1, row].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 2), texture);
                        break;
                    default:
                        break;
                }
                
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
                        GUI.DrawTexture(new Rect(_gridRect[column - 1, row].position.x - _gridSize,
                                                 _gridRect[column - 1, row].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 3), texture);
                        break;
                    case DirectionType.Sorth:
                        //T字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column - 1, row].position.x - _gridSize,
                                                 _gridRect[column - 1, row].position.y,
                                                 _gridSize * 3,
                                                 _gridSize * 2), texture);
                        break;
                    case DirectionType.West:
                        //T字通路のテクスチャ用にRectサイズを変更して描画する
                        GUI.DrawTexture(new Rect(_gridRect[column - 1, row + 1].position.x - _gridSize,
                                                 _gridRect[column - 1, row + 1].position.y,
                                                 _gridSize * 2,
                                                 _gridSize * 3), texture);
                        break;
                    default:
                        break;
                }
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
    /// 部屋のテクスチャを描画する
    /// </summary>
    /// <param name="column">列</param>
    /// <param name="row">行</param>
    /// <param name="texture">テクスチャ</param>
    private void DrawRoomTexture(int column, int row, Texture2D texture)
    {
        //部屋パーツのテクスチャ用にRectサイズを変更して描画する
        GUI.DrawTexture(new Rect(_gridRect[column, row].position.x - _gridSize,
                                 _gridRect[column, row].position.y - _gridSize,
                                 _gridSize * 3,
                                 _gridSize * 3), texture);
    }

    /// <summary>
    /// ステージをヒエラルキー上に生成する
    /// </summary>
    /// <param name="stageCells">ステージデータ</param>
    private void GenerateStageObject(StageCell[][] stageCells)
    {
        //親オブジェクトとなる空のオブジェクトを作成
        var parentObj = new GameObject(_parentWindow.StageName);
        int generatePoint_X = 0;
        int generatePoint_Z = 0;

        for (int column = 0; column < stageCells.Length; column++)
        {
            for (int row = 0; row < stageCells[0].Length; row++)
            {
                //パーツの原点がセットされていない場合はマスを飛ばす
                if (!stageCells[column][row].IsOriginSet)
                {
                    generatePoint_X += 4;
                    continue;
                }

                var parts = AssetDatabase.LoadAssetAtPath<PartsBase>(stageCells[column][row].PrefabPath);
                
                if (parts.PartsType == PartsType.Room)
                {
                    Room r = parts.GetComponent<Room>();
                    var partsObj = Instantiate(r, new Vector3(generatePoint_X, 0, generatePoint_Z), Quaternion.identity);
                    partsObj.RotateByDirection(stageCells[column][row].CurrentDir);

                    partsObj.transform.SetParent(parentObj.transform);
                }
                else
                {
                    var partsObj = Instantiate(parts, new Vector3(generatePoint_X, 0, generatePoint_Z), Quaternion.identity);
                    partsObj.RotateByDirection(stageCells[column][row].CurrentDir);

                    partsObj.transform.SetParent(parentObj.transform);
                }

                
                generatePoint_X += 4;
            }
            generatePoint_X = 0;
            generatePoint_Z -= 4;
        }

        //生成と同時にPrefab化する設定がTrueの場合
        if (_parentWindow.IsCreatePrefab)
        {
            // Prefabとして保存するためのパスを指定
            string saveFolderPath = $"Assets/Prefabs/Nakajima/Stages/StagePrefabs/{parentObj.name}.prefab";

            var prefab = PrefabUtility.SaveAsPrefabAsset(parentObj, saveFolderPath);
            Debug.Log($"{parentObj.name}をPrefab化しました");

            DestroyImmediate(parentObj);
            Instantiate(prefab);
        }

        //ステージデータの作成
        StageCellData data = new StageCellData();

        data.Cells = _stageCells;
        //CheckCurrentStageData(data.Cells);

        LocalData.SaveMultidimArray($"SaveData/StageData/{_parentWindow.StageName}.json", data);
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

    private void LoadStageDataFile()
    {
        string path = AssetDatabase.GetAssetPath(_stageDataFileDirectry);
        StageCellData data = LocalData.LoadMultidimArray<StageCellData>(path);

        if (data == null)
        {
            Debug.LogError("データがありません");
            return;
        }
        
        _stageCells = data.Cells;
        Repaint();
    }
    #endregion
}

/// <summary>
/// グリッド上に配置するマス
/// </summary>
[System.Serializable]
public class StageCell
{
    #region property
    public int Column = 0;
    public int Row = 0;
    public CellState CurrentState = CellState.None;
    public DirectionType CurrentDir = DirectionType.North;
    public string PrefabPath = "";
    /// <summary>パーツの原点がセットされているか</summary>
    public bool IsOriginSet = false;
    #endregion

    #region private
    #endregion

    #region public method
    public void SetData(StageCell[][] cells, string path, DirectionType dirType)
    {
        var t = AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<PartsBase>();

        switch (t.PartsType)
        {
            case PartsType.None:
                Debug.LogError("パーツの指定が間違っています。");
                break;
            case PartsType.Corridor:
                Corridor c = t.GetComponent<Corridor>();

                if (CheckGridForCorridor(cells, c.CorridorType, dirType))
                {
                    PrefabPath = path;
                    CurrentDir = dirType;
                }
                else
                {
                    Debug.LogError("パーツをセットできませんでした");
                }
                break;
            case PartsType.Room:
                Room r = t.GetComponent<Room>();

                if (CheckGridForRoom(cells, r.RoomType))
                {
                    PrefabPath = path;
                    CurrentDir = dirType;
                }
                else
                {
                    Debug.LogError("パーツをセットできませんでした");
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// マスのデータをリセットする
    /// </summary>
    /// <param name="cells">マス全体のデータ</param>
    public void ResetData(StageCell[][] cells)
    {
        //マスの状態に応じて
        switch (CurrentState)
        {
            case CellState.None:
                break;
            case CellState.Corridor_Straight:
                break;
            case CellState.Corridor_Straight_Large:
                switch (CurrentDir)
                {
                    case DirectionType.North:
                        cells[Column - 1][Row].CurrentState = CellState.None;
                        cells[Column + 1][Row].CurrentState = CellState.None;
                        break;
                    case DirectionType.East:
                        cells[Column][Row - 1].CurrentState = CellState.None;
                        cells[Column][Row + 1].CurrentState = CellState.None;
                        break;
                    case DirectionType.Sorth:
                        cells[Column - 1][Row].CurrentState = CellState.None;
                        cells[Column + 1][Row].CurrentState = CellState.None;
                        break;
                    case DirectionType.West:
                        cells[Column][Row - 1].CurrentState = CellState.None;
                        cells[Column][Row + 1].CurrentState = CellState.None;
                        break;
                    default:
                        break;
                }
                break;
            case CellState.Corridor_Sharp_L:
                switch (CurrentDir)
                {
                    case DirectionType.North:
                        cells[Column + 1][Row].CurrentState = CellState.None;
                        cells[Column][Row + 1].CurrentState = CellState.None;
                        cells[Column + 1][Row + 1].CurrentState = CellState.None;
                        break;
                    case DirectionType.East:
                        cells[Column - 1][Row].CurrentState = CellState.None;
                        cells[Column][Row + 1].CurrentState = CellState.None;
                        cells[Column - 1][Row + 1].CurrentState = CellState.None;
                        break;
                    case DirectionType.Sorth:
                        cells[Column - 1][Row].CurrentState = CellState.None;
                        cells[Column][Row - 1].CurrentState = CellState.None;
                        cells[Column - 1][Row - 1].CurrentState = CellState.None;
                        break;
                    case DirectionType.West:
                        cells[Column + 1][Row].CurrentState = CellState.None;
                        cells[Column + 1][Row - 1].CurrentState = CellState.None;
                        cells[Column][Row - 1].CurrentState = CellState.None;
                        break;
                    default:
                        break;
                }
                break;
            case CellState.Corridor_Sharp_T:
                switch (CurrentDir)
                {
                    case DirectionType.North:
                        cells[Column - 1][Row].CurrentState = CellState.None;
                        cells[Column + 1][Row].CurrentState = CellState.None;
                        cells[Column - 1][Row + 1].CurrentState = CellState.None;
                        cells[Column][Row + 1].CurrentState = CellState.None;
                        cells[Column + 1][Row + 1].CurrentState = CellState.None;
                        break;
                    case DirectionType.East:
                        cells[Column - 1][Row - 1].CurrentState = CellState.None;
                        cells[Column][Row - 1].CurrentState = CellState.None;
                        cells[Column - 1][Row].CurrentState = CellState.None;
                        cells[Column - 1][Row + 1].CurrentState = CellState.None;
                        cells[Column][Row + 1].CurrentState = CellState.None;
                        break;
                    case DirectionType.Sorth:
                        cells[Column - 1][Row].CurrentState = CellState.None;
                        cells[Column + 1][Row].CurrentState = CellState.None;
                        cells[Column - 1][Row - 1].CurrentState = CellState.None;
                        cells[Column][Row - 1].CurrentState = CellState.None;
                        cells[Column + 1][Row - 1].CurrentState = CellState.None;
                        break;
                    case DirectionType.West:
                        cells[Column][Row - 1].CurrentState = CellState.None;
                        cells[Column + 1][Row - 1].CurrentState = CellState.None;
                        cells[Column + 1][Row].CurrentState = CellState.None;
                        cells[Column][Row + 1].CurrentState = CellState.None;
                        cells[Column + 1][Row + 1].CurrentState = CellState.None;
                        break;
                    default:
                        break;
                }      
                break;
            case CellState.Corridor_Cross:
                cells[Column - 1][Row - 1].CurrentState = CellState.None;
                cells[Column][Row - 1].CurrentState = CellState.None;
                cells[Column + 1][Row - 1].CurrentState = CellState.None;
                cells[Column - 1][Row].CurrentState = CellState.None;
                cells[Column + 1][Row].CurrentState = CellState.None;
                cells[Column - 1][Row + 1].CurrentState = CellState.None;
                cells[Column][Row + 1].CurrentState = CellState.None;
                cells[Column + 1][Row + 1].CurrentState = CellState.None;
                break;
            case CellState.Room_Start:
                cells[Column - 1][Row - 1].CurrentState = CellState.None;
                cells[Column][Row - 1].CurrentState = CellState.None;
                cells[Column + 1][Row - 1].CurrentState = CellState.None;
                cells[Column - 1][Row].CurrentState = CellState.None;
                cells[Column + 1][Row].CurrentState = CellState.None;
                cells[Column - 1][Row + 1].CurrentState = CellState.None;
                cells[Column][Row + 1].CurrentState = CellState.None;
                cells[Column + 1][Row + 1].CurrentState = CellState.None;
                break;
            case CellState.Room_MainMissionTarget:
                cells[Column - 1][Row - 1].CurrentState = CellState.None;
                cells[Column][Row - 1].CurrentState = CellState.None;
                cells[Column + 1][Row - 1].CurrentState = CellState.None;
                cells[Column - 1][Row].CurrentState = CellState.None;
                cells[Column + 1][Row].CurrentState = CellState.None;
                cells[Column - 1][Row + 1].CurrentState = CellState.None;
                cells[Column][Row + 1].CurrentState = CellState.None;
                cells[Column + 1][Row + 1].CurrentState = CellState.None;
                break;
            case CellState.Room_subMissionTarget:
                cells[Column - 1][Row - 1].CurrentState = CellState.None;
                cells[Column][Row - 1].CurrentState = CellState.None;
                cells[Column + 1][Row - 1].CurrentState = CellState.None;
                cells[Column - 1][Row].CurrentState = CellState.None;
                cells[Column + 1][Row].CurrentState = CellState.None;
                cells[Column - 1][Row + 1].CurrentState = CellState.None;
                cells[Column][Row + 1].CurrentState = CellState.None;
                cells[Column + 1][Row + 1].CurrentState = CellState.None;
                break;
            case CellState.Room_Escape:
                cells[Column - 1][Row - 1].CurrentState = CellState.None;
                cells[Column][Row - 1].CurrentState = CellState.None;
                cells[Column + 1][Row - 1].CurrentState = CellState.None;
                cells[Column - 1][Row].CurrentState = CellState.None;
                cells[Column + 1][Row].CurrentState = CellState.None;
                cells[Column - 1][Row + 1].CurrentState = CellState.None;
                cells[Column][Row + 1].CurrentState = CellState.None;
                cells[Column + 1][Row + 1].CurrentState = CellState.None;
                break;
            case CellState.Room_Cross:
                cells[Column - 1][Row - 1].CurrentState = CellState.None;
                cells[Column][Row - 1].CurrentState = CellState.None;
                cells[Column + 1][Row - 1].CurrentState = CellState.None;
                cells[Column - 1][Row].CurrentState = CellState.None;
                cells[Column + 1][Row].CurrentState = CellState.None;
                cells[Column - 1][Row + 1].CurrentState = CellState.None;
                cells[Column][Row + 1].CurrentState = CellState.None;
                cells[Column + 1][Row + 1].CurrentState = CellState.None;
                break;
            default:
                break;
        }

        //選択したマスのデータを初期化
        CurrentState = CellState.None;
        PrefabPath = "";
        CurrentDir = DirectionType.North;
        IsOriginSet = false;
    }
    #endregion

    #region private method
    /// <summary>
    /// グリッドの範囲内か確認する（通路）
    /// </summary>
    /// <param name="cells">ステージデータ</param>
    /// <param name="type">通路の種類</param>
    /// <param name="dirType">向き</param>
    /// <returns></returns>
    private bool CheckGridForCorridor(StageCell[][] cells, CorridorType type, DirectionType dirType)
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
                break;
            case CorridorType.Straight_2:
                CurrentState = CellState.Corridor_Straight;
                break;
            case CorridorType.Straight_3:
                CurrentState = CellState.Corridor_Straight;
                break;
            case CorridorType.Straight_End:
                CurrentState = CellState.Corridor_Straight;
                break;
            case CorridorType.Straight_Large:
                if (dirType == DirectionType.North || dirType == DirectionType.Sorth)
                {
                    //パーツがグリッドの範囲外に出る場合は処理を行わない
                    if (Column - 1 < 0 ||
                        Column + 1 > cells.GetLength(0) - 1)
                    {
                        return false;
                    }

                    //上下1マスを同じステータスに変更
                    cells[Column - 1][Row].CurrentState = CellState.Corridor_Straight_Large;
                    cells[Column + 1][Row].CurrentState = CellState.Corridor_Straight_Large;
                }
                else
                {
                    //パーツがグリッドの範囲外に出る場合は処理を行わない
                    if (Row - 1 < 0 ||
                        Row + 1 > cells.GetLength(1) - 1)
                    {
                        return false;
                    }

                    //上下1マスを同じステータスに変更
                    cells[Column][Row - 1].CurrentState = CellState.Corridor_Straight_Large;
                    cells[Column][Row + 1].CurrentState = CellState.Corridor_Straight_Large;
                }

                CurrentState = CellState.Corridor_Straight_Large;
                break;
            case CorridorType.Sharp_L:
                switch (dirType)
                {
                    case DirectionType.North:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column + 1 > cells[0].Length - 1 ||
                            Row + 1 > cells[0].Length - 1)
                        {
                            return false;
                        }

                        //パーツの範囲内のマスを同じステータスに変更
                        cells[Column + 1][Row].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column][Row + 1].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column + 1][Row + 1].CurrentState = CellState.Corridor_Sharp_L;
                        break;
                    case DirectionType.East:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column - 1 < 0 ||
                            Row + 1 > cells[0].Length - 1)
                        {
                            return false;
                        }

                        //パーツの範囲内のマスを同じステータスに変更
                        cells[Column - 1][Row].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column][Row + 1].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column - 1][Row + 1].CurrentState = CellState.Corridor_Sharp_L;
                        break;
                    case DirectionType.Sorth:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column - 1 < 0 ||
                            Row - 1 < 0)
                        {
                            return false;
                        }

                        //パーツの範囲内のマスを同じステータスに変更
                        cells[Column - 1][Row].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column][Row - 1].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column - 1][Row - 1].CurrentState = CellState.Corridor_Sharp_L;
                        break;
                    case DirectionType.West:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column + 1 > cells.GetLength(0) - 1 ||
                            Row - 1 < 0)
                        {
                            return false;
                        }

                        //パーツの範囲内のマスを同じステータスに変更
                        cells[Column + 1][Row].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column][Row - 1].CurrentState = CellState.Corridor_Sharp_L;
                        cells[Column + 1][Row - 1].CurrentState = CellState.Corridor_Sharp_L;
                        break;
                    default:
                        break;
                }
                CurrentState = CellState.Corridor_Sharp_L;
                break;
            case CorridorType.Sharp_T:

                switch (dirType)
                {
                    case DirectionType.North:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column - 1 < 0 ||
                            Column + 1 > cells[0].Length - 1 ||
                            Row + 1 > cells[0].Length - 1)
                        {
                            return false;
                        }

                        //原点以外のマスを同じステータスに変更
                        cells[Column - 1][Row].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column + 1][Row].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column - 1][Row + 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column][Row + 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column + 1][Row + 1].CurrentState = CellState.Corridor_Sharp_T;
                        break;
                    case DirectionType.East:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column - 1 < 0 ||
                            Row - 1 < 0 ||
                            Row + 1 > cells[0].Length - 1)
                        {
                            return false;
                        }

                        //原点以外のマスを同じステータスに変更
                        cells[Column - 1][Row].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column - 1][Row - 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column - 1][Row + 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column][Row - 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column][Row + 1].CurrentState = CellState.Corridor_Sharp_T;

                        break;
                    case DirectionType.Sorth:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column - 1 < 0 ||
                            Column + 1 > cells[0].Length - 1 ||
                            Row - 1 < 0)
                        {
                            return false;
                        }

                        //原点以外のマスを同じステータスに変更
                        cells[Column - 1][Row].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column + 1][Row].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column - 1][Row - 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column][Row - 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column + 1][Row - 1].CurrentState = CellState.Corridor_Sharp_T;

                        break;
                    case DirectionType.West:
                        //パーツがグリッドの範囲外に出る場合は処理を行わない
                        if (Column < 0 ||
                            Column + 1 > cells[0].Length - 1 ||
                            Row - 1 < 0)
                        {
                            return false;
                        }

                        //原点以外のマスを同じステータスに変更
                        cells[Column - 1][Row].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column + 1][Row].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column - 1][Row - 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column][Row - 1].CurrentState = CellState.Corridor_Sharp_T;
                        cells[Column + 1][Row - 1].CurrentState = CellState.Corridor_Sharp_T;
                        break;
                    default:
                        break;
                }
                
                CurrentState = CellState.Corridor_Sharp_T;
                break;
            case CorridorType.Cross:
                //パーツがグリッドの範囲外に出る場合は処理を行わない
                if (Column - 1 < 0 ||
                    Column + 1 > cells[0].Length - 1 ||
                    Row - 1 < 0 ||
                    Row + 1 > cells[0].Length - 1)
                {
                    return false;
                }

                CurrentState = CellState.Corridor_Cross;
                IsOriginSet = true;

                //原点以外のマスを同じステータスに変更
                cells[Column - 1][Row - 1].CurrentState = CellState.Corridor_Cross;
                cells[Column][Row - 1].CurrentState = CellState.Corridor_Cross;
                cells[Column + 1][Row - 1].CurrentState = CellState.Corridor_Cross;
                cells[Column - 1][Row].CurrentState = CellState.Corridor_Cross;
                cells[Column + 1][Row].CurrentState = CellState.Corridor_Cross;
                cells[Column - 1][Row + 1].CurrentState = CellState.Corridor_Cross;
                cells[Column][Row + 1].CurrentState = CellState.Corridor_Cross;
                cells[Column + 1][Row + 1].CurrentState = CellState.Corridor_Cross;
                break;
            default:
                break;
        }
        IsOriginSet = true;
        return true;
    }

    private bool CheckGridForRoom(StageCell[][] cells, RoomType type)
    {
        //既にセットされている場合は処理を行わない
        if (CurrentState != CellState.None)
        {
            return false;
        }

        //パーツがグリッドの範囲外に出る場合は処理を行わない
        if (Column - 1 < 0 ||
            Column + 1 > cells[0].Length - 1 ||
            Row - 1 < 0 ||
            Row + 1 > cells[0].Length - 1)
        {
            return false;
        }

        IsOriginSet = true;

        switch (type)
        {
            case RoomType.Start:
                CurrentState = CellState.Room_Start;
                break;
            case RoomType.Cross:
                CurrentState = CellState.Room_Cross;
                break;
            case RoomType.MainTarget:
                CurrentState = CellState.Room_MainMissionTarget;
                break;
            case RoomType.SubTarget:
                CurrentState = CellState.Room_subMissionTarget;
                break;
            case RoomType.Escape:
                CurrentState = CellState.Room_Escape;
                break;
            default:
                break;
        }

        //原点以外のマスを同じステータスに変更
        cells[Column - 1][Row - 1].CurrentState = CurrentState;
        cells[Column][Row - 1].CurrentState = CurrentState;
        cells[Column + 1][Row - 1].CurrentState = CurrentState;
        cells[Column - 1][Row].CurrentState = CurrentState;
        cells[Column + 1][Row].CurrentState = CurrentState;
        cells[Column - 1][Row + 1].CurrentState = CurrentState;
        cells[Column][Row + 1].CurrentState = CurrentState;
        cells[Column + 1][Row + 1].CurrentState = CurrentState;
        return true;
    }
    #endregion
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

/// <summary>
/// ステージのマスデータ
/// </summary>
[System.Serializable]
public class StageCellData
{
    public StageCell[][] Cells;
}