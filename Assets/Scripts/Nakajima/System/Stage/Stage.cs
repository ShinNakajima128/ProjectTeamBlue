using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stage
{
    #region property
    public string StageName => _stageName;
    public GameStates StageType => _stageType;
    public int SubMissionNum => _subMissionNum;
    #endregion

    #region serialize
    [Tooltip("ステージの名前")]
    [SerializeField]
    private string _stageName = "ステージ";
    
    [Tooltip("ステージの種類")]
    [SerializeField]
    private GameStates _stageType;

    [Tooltip("サブミッションの数")]
    [SerializeField]
    private int _subMissionNum = 0;

    [Tooltip("ステージをクリアしているかの判定")]
    [SerializeField]
    private bool _isClearedStage = false;

    [Tooltip("クリアしたサブミッションの判定")]
    [SerializeField]
    private bool[] _isClearedSubMissions = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region public method
    public void SetupData(Stage stage)
    {
        _isClearedSubMissions = new bool[_subMissionNum];

        //新しくゲームを始めた場合
        if (stage == null) 
        {
            for (int i = 0; i < _isClearedSubMissions.Length; i++)
            {
                _isClearedSubMissions[i] = false;
            }
        }
        else
        {
            //読み込んだデータからクリア状況をセットする
            for (int i = 0; i < _isClearedSubMissions.Length; i++)
            {
                _isClearedSubMissions[i] = stage._isClearedSubMissions[i];
            }
        }
        
    }
    #endregion

    #region private method
    #endregion
}