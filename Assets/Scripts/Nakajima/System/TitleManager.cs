using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトルの機能を管理するManagerクラス
/// </summary>
public class TitleManager : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("ボタンを押して読み込むSceneの名前(仮機能)")]
    [SerializeField]
    private string _loadSceneName = "";
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {

    }

    private void Start()
    {

    }
    #endregion

    #region public method
    /// <summary>
    /// ステージ選択画面を読み込む
    /// </summary>
    public void LoadStageSelectScene()
    {
        SceneManager.LoadScene(_loadSceneName);
    }
    #endregion

    #region private method
    #endregion
}
