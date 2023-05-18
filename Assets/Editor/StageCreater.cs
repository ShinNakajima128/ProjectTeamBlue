using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StageCreater : EditorWindow
{
    #region property
    public int StageSize => _stageSize;
    public float GridSize => _gridSize;
    #endregion

    #region serialize
    #endregion

    #region private
    private Object _corridorDirectry;
    private Object _roomDirectry;
    private int _stageSize = 10;
    private float _gridSize = 50.0f;
    private Object _addToDirectry;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    #endregion

    #region public method
    #endregion

    #region private method
    [MenuItem("Window/StageCreater")]
    private static void ShowMainWindow()
    {
        GetWindow(typeof(StageCreater));
    }

    private void OnGUI()
    {
        //通路Prefabが入っているフォルダをセットする枠の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Corrider Directry", GUILayout.Width(150));
        _corridorDirectry = EditorGUILayout.ObjectField(_corridorDirectry, typeof(Object), true);
        GUILayout.EndHorizontal();
        
        EditorGUILayout.Space();

        //部屋Prefabが入っているフォルダをセットする枠の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Room Directry", GUILayout.Width(150));
        _roomDirectry = EditorGUILayout.ObjectField(_roomDirectry, typeof(Object), true);
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //ステージ全体の大きさの値を設定する項目の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Stage Size", GUILayout.Width(150));
        _stageSize = EditorGUILayout.IntField(_stageSize);
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //ステージ作成画面のグリッド幅の設定をする項目の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid Size", GUILayout.Width(150));
        _gridSize = EditorGUILayout.FloatField(_gridSize);
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //作成したステージPrefabの保存先のフォルダをセットする枠の作成
        GUILayout.BeginHorizontal();
        GUILayout.Label("Add to Directry", GUILayout.Width(150));
        _addToDirectry = EditorGUILayout.ObjectField(_addToDirectry, typeof(Object), true);
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();


    }

    private void ShowImageCorridorParts()
    {
        if (_corridorDirectry != null)
        {

        }
    }
    #endregion
}

public class StageCreaterSubWindow : EditorWindow
{
    #region private 
    private int _stageSize = 0;
    private float _gridSize = 0f;
    private PartsBase[,] _stage;
    private Rect[,] _gridRect;
    private StageCreater _parentWindow;
    #endregion

    #region constant
    /// <summary>ウィンドウの横幅</summary>
    private const float WINDOW_WIDTH = 750.0f;
    /// <summary>ウィンドウの縦幅</summary>
    private const float WINDOW_HEIGHT = 750.0f;
    #endregion

    #region public method
    public static StageCreaterSubWindow WillAppear(StageCreater parent)
    {
        StageCreaterSubWindow window = (StageCreaterSubWindow)GetWindow(typeof(StageCreaterSubWindow), false);
        window.Show();
        window.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
        window.SetParent(parent);

        return window;
    }
    public void Setup()
    {
        _stageSize = _parentWindow.StageSize;
        _gridSize = _parentWindow.GridSize;

        _stage = new PartsBase[_stageSize, _stageSize];

        for (int i = 0; i < _stage.GetLength(0); i++)
        {
            for (int j = 0; j < _stage.GetLength(1); j++)
            {
                _stage[i, j] = default;
            }
        }
    }
    #endregion

    #region private method
    private void SetParent(StageCreater parent)
    {
        _parentWindow = parent;
    }
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
            for (int row = 0; row < width; row++)
            {
                Rect r = new Rect(new Vector2(x, y), new Vector2(w, h));
                resultRects[column, row] = r;
            }
        }
        return null;
    }
    #endregion
}