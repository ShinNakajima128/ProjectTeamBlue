using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ステージの機能、データを管理するManagerクラス
/// </summary>
public class StageManager : SingletonMonoBehaviour<StageManager>
{
    #region property
    /// <summary>ステージのデータ</summary>
    public StageData Data => _data;

    /// <summary>ステージの合計</summary>
    public int AllStageNum => _data.Stages.Length;
    #endregion

    #region serialize
    [Tooltip("各ステージのデータ")]
    [SerializeField]
    StageData _data = default;

    [Header("Debug")]
    [Tooltip("デバッグ機能のON/OFF切り替え")]
    [SerializeField]
    private bool _debugMode = false;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private  void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (!_debugMode)
        {
            LoadData();
        }
    }
    #endregion

    #region public method
    public void LoadData()
    {
        string json = "";

        print(Application.persistentDataPath);

        try
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, "SaveData.txt"));
        }
        catch
        {
            json = CreateNewData();
            print(json);
        }
        print(json);
        var data = JsonUtility.FromJson<StageData>(json);

        _data.Setup(data.Stages);
    }

    public string CreateNewData()
    {
        var json = JsonUtility.ToJson(_data);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "SaveData.txt"), json);
        print($"新しいセーブデータを作成します{json}");
        return json;
    }
    #endregion

    #region private method
    #endregion
}
