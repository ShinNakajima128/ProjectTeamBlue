using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Map creater 
/// </summary>
public class MapCreater : EditorWindow
{

	// 画像ディレクトリ
	private Object imgDirectory;
	// 出力先ディレクトリ(nullだとAssets下に出力します)
	private Object outputDirectory;
	// マップエディタのマスの数
	private int mapSize = 10;
	// グリッドの大きさ、小さいほど細かくなる
	private float gridSize = 50.0f;
	// 出力ファイル名
	private string outputFileName;
	// 選択した画像パス
	private string selectedImagePath;
	// サブウィンドウ
	private MapCreaterSubWindow subWindow;

	[UnityEditor.MenuItem("Window/MapCreater")]
	static void ShowTestMainWindow()
	{
		EditorWindow.GetWindow(typeof(MapCreater));
	}

	void OnGUI()
	{
		// GUI
		GUILayout.BeginHorizontal();
		GUILayout.Label("Image Directory : ", GUILayout.Width(110));
		imgDirectory = EditorGUILayout.ObjectField(imgDirectory, typeof(UnityEngine.Object), true);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("map size : ", GUILayout.Width(110));
		mapSize = EditorGUILayout.IntField(mapSize);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("grid size : ", GUILayout.Width(110));
		gridSize = EditorGUILayout.FloatField(gridSize);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Save Directory : ", GUILayout.Width(110));
		outputDirectory = EditorGUILayout.ObjectField(outputDirectory, typeof(UnityEngine.Object), true);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Save filename : ", GUILayout.Width(110));
		outputFileName = (string)EditorGUILayout.TextField(outputFileName);
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		DrawImageParts();

		DrawSelectedImage();

		DrawMapWindowButton();
	}

	// 画像一覧をボタン選択出来る形にして出力
	private void DrawImageParts()
	{
		if (imgDirectory != null)
		{
			float x = 0.0f;
			float y = 00.0f;
			float w = 50.0f;
			float h = 50.0f;
			float maxW = 300.0f;

			string path = AssetDatabase.GetAssetPath(imgDirectory);
			string[] names = Directory.GetFiles(path, "*.png");
			EditorGUILayout.BeginVertical();
			foreach (string d in names)
			{
				if (x > maxW)
				{
					x = 0.0f;
					y += h;
					EditorGUILayout.EndHorizontal();
				}
				if (x == 0.0f)
				{
					EditorGUILayout.BeginHorizontal();
				}
				GUILayout.FlexibleSpace();
				Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(d, typeof(Texture2D));
				if (GUILayout.Button(tex, GUILayout.MaxWidth(w), GUILayout.MaxHeight(h), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
				{
					selectedImagePath = d;
				}
				GUILayout.FlexibleSpace();
				x += w;
			}
			EditorGUILayout.EndVertical();
		}
	}

	// 選択した画像データを表示
	private void DrawSelectedImage()
	{
		if (selectedImagePath != null)
		{
			Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(selectedImagePath, typeof(Texture2D));
			EditorGUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.Label("select : " + selectedImagePath);
			GUILayout.Box(tex);
			EditorGUILayout.EndVertical();

		}
	}

	// マップウィンドウを開くボタンを生成
	private void DrawMapWindowButton()
	{
		EditorGUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("open map editor"))
		{
			if (subWindow == null)
			{
				subWindow = MapCreaterSubWindow.WillAppear(this);
			}
			else
			{
				subWindow.Focus();
			}
		}
		EditorGUILayout.EndVertical();
	}

	public string SelectedImagePath
	{
		get { return selectedImagePath; }
	}

	public int MapSize
	{
		get { return mapSize; }
	}

	public float GridSize
	{
		get { return gridSize; }
	}

	// 出力先パスを生成
	public string OutputFilePath()
	{
		string resultPath = "";
		if (outputDirectory != null)
		{
			resultPath = AssetDatabase.GetAssetPath(outputDirectory);
		}
		else
		{
			resultPath = Application.dataPath;
		}

		return resultPath + "/" + outputFileName + ".txt";
	}
}

/// <summary>
/// Map creater sub window.
/// </summary>
public class MapCreaterSubWindow : EditorWindow
{
	// マップウィンドウのサイズ
	const float WINDOW_W = 750.0f;
	const float WINDOW_H = 750.0f;
	// マップのグリッド数
	private int mapSize = 0;
	// グリッドサイズ。親から値をもらう
	private float gridSize = 0.0f;
	// マップデータ
	private string[,] map;
	// グリッドの四角
	private Rect[,] gridRect;
	// 親ウィンドウの参照を持つ
	private MapCreater parent;

	// サブウィンドウを開く
	public static MapCreaterSubWindow WillAppear(MapCreater _parent)
	{
		MapCreaterSubWindow window = (MapCreaterSubWindow)EditorWindow.GetWindow(typeof(MapCreaterSubWindow), false);
		window.Show();
		window.minSize = new Vector2(WINDOW_W, WINDOW_H);
		window.SetParent(_parent);
		window.init();
		return window;
	}

	private void SetParent(MapCreater _parent)
	{
		parent = _parent;
	}

	// サブウィンドウの初期化
	public void init()
	{
		mapSize = parent.MapSize;
		gridSize = parent.GridSize;

		// マップデータを初期化
		map = new string[mapSize, mapSize];
		for (int i = 0; i < mapSize; i++)
		{
			for (int j = 0; j < mapSize; j++)
			{
				map[i, j] = "";
			}
		}
		// グリッドデータを生成
		gridRect = CreateGrid(mapSize);
	}



	void OnGUI()
	{
		// グリッド線を描画する
		for (int yy = 0; yy < mapSize; yy++)
		{
			for (int xx = 0; xx < mapSize; xx++)
			{
				DrawGridLine(gridRect[yy, xx]);
			}
		}

		// クリックされた位置を探して、その場所に画像データを入れる
		Event e = Event.current;
		if (e.type == EventType.MouseDown)
		{
			Vector2 pos = Event.current.mousePosition;
			int xx;
			// x位置を先に計算して、計算回数を減らす
			for (xx = 0; xx < mapSize; xx++)
			{
				Rect r = gridRect[0, xx];
				if (r.x <= pos.x && pos.x <= r.x + r.width)
				{
					break;
				}
			}

			// 後はy位置だけ探す
			for (int yy = 0; yy < mapSize; yy++)
			{
				if (gridRect[yy, xx].Contains(pos))
				{
					// 消しゴムの時はデータを消す
					if (parent.SelectedImagePath.IndexOf("000") > -1)
					{
						map[yy, xx] = "";
					}
					else
					{
						map[yy, xx] = parent.SelectedImagePath;
					}
					Repaint();
					break;
				}
			}
		}

		// 選択した画像を描画する
		for (int yy = 0; yy < mapSize; yy++)
		{
			for (int xx = 0; xx < mapSize; xx++)
			{
				if (map[yy, xx] != null && map[yy, xx].Length > 0)
				{
					Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(map[yy, xx], typeof(Texture2D));
					GUI.DrawTexture(gridRect[yy, xx], tex);
				}
			}
		}

		// 出力ボタン
		Rect rect = new Rect(0, WINDOW_H - 50, 300, 50);
		GUILayout.BeginArea(rect);
		if (GUILayout.Button("output file", GUILayout.MinWidth(300), GUILayout.MinHeight(50)))
		{
			OutputFile();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();

	}

	// グリッドデータを生成
	private Rect[,] CreateGrid(int div)
	{
		int sizeW = div;
		int sizeH = div;

		float x = 0.0f;
		float y = 0.0f;
		float w = gridSize;
		float h = gridSize;

		Rect[,] resultRects = new Rect[sizeH, sizeW];

		for (int yy = 0; yy < sizeH; yy++)
		{
			x = 0.0f;
			for (int xx = 0; xx < sizeW; xx++)
			{
				Rect r = new Rect(new Vector2(x, y), new Vector2(w, h));
				resultRects[yy, xx] = r;
				x += w;
			}
			y += h;
		}

		return resultRects;
	}

	// グリッド線を描画
	private void DrawGridLine(Rect r)
	{
		// grid
		Handles.color = new Color(1f, 1f, 1f, 0.5f);

		// upper line
		Handles.DrawLine(
			new Vector2(r.position.x, r.position.y),
			new Vector2(r.position.x + r.size.x, r.position.y));

		// bottom line
		Handles.DrawLine(
			new Vector2(r.position.x, r.position.y + r.size.y),
			new Vector2(r.position.x + r.size.x, r.position.y + r.size.y));

		// left line
		Handles.DrawLine(
			new Vector2(r.position.x, r.position.y),
			new Vector2(r.position.x, r.position.y + r.size.y));

		// right line
		Handles.DrawLine(
			new Vector2(r.position.x + r.size.x, r.position.y),
			new Vector2(r.position.x + r.size.x, r.position.y + r.size.y));
	}

	// ファイルで出力
	private void OutputFile()
	{
		string path = parent.OutputFilePath();

		FileInfo fileInfo = new FileInfo(path);
		StreamWriter sw = fileInfo.AppendText();
		sw.WriteLine(GetMapStrFormat());
		sw.Flush();
		sw.Close();

		// 完了ポップアップ
		EditorUtility.DisplayDialog("MapCreater", "output file success\n" + path, "ok");
	}

	// 出力するマップデータ整形
	private string GetMapStrFormat()
	{
		string result = "";
		for (int i = 0; i < mapSize; i++)
		{
			for (int j = 0; j < mapSize; j++)
			{
				result += OutputDataFormat(map[i, j]);
				if (j < mapSize - 1)
				{
					result += ",";
				}
			}
			result += "\n";
		}
		return result;
	}

	private string OutputDataFormat(string data)
	{
		if (data != null && data.Length > 0)
		{
			string[] tmps = data.Split('/');
			string fileName = tmps[tmps.Length - 1];
			return fileName.Split('.')[0];
		}
		else
		{
			return "";
		}
	}
}