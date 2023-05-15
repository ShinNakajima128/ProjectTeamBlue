﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stage
{
    #region property
    public string StageName => _stageName;
    public GameStates StageType => _stageType;
    public bool IsClearedStage => _isClearedStage;

    /// <summary>"サブミッションの数"</summary>
    public int SubMissionNum => _isClearedSubMissions.Length;
    public bool[] IsClearedSubMissions => _isClearedSubMissions;
    #endregion

    #region serialize
    [Tooltip("ステージの名前")]
    [SerializeField]
    private string _stageName = "ステージ";
    
    [Tooltip("ステージの種類")]
    [SerializeField]
    private GameStates _stageType;

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
        _isClearedSubMissions = new bool[stage.SubMissionNum];

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

    /// <summary>
    /// クリア状況をリセットする
    /// </summary>
    public void Reset()
    {
        _isClearedStage = false;

        for (int i = 0; i < _isClearedSubMissions.Length; i++)
        {
            _isClearedSubMissions[i] = false;
        }
    }
    #endregion

    #region private method
    #endregion
}