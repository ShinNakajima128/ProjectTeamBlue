using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// ステージ選択画面のデータを表示するコンポーネント
/// </summary>
public class StageSelect : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("ステージ内容を画面に表示するオブジェクトを持つクラスの配列")]
    [SerializeField]
    private StageInfo[] _stageinfos = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private IEnumerator Start()
    {
        //ステージデータ更新時の処理を登録
        LobbyManager.Instance.ReflashDataSubject
                    .Subscribe(_ => Reflash())
                    .AddTo(this);

        //各機能の登録処理を待機するため、1フレーム飛ばす
        yield return null;

        Reflash();
    }
    #endregion

    #region public method
    #endregion

    #region private method
    /// <summary>
    /// ステージ画面のデータを更新する
    /// </summary>
    private void Reflash()
    {
        var data = DataManager.Instance.Data;

        for (int i = 0; i < _stageinfos.Length; i++)
        {
            _stageinfos[i].IsClearStageText.text = data.Stages[i].IsClearedStage ? "Complete" : "Incomplete";
            _stageinfos[i].IsClearSubMissionsText.text = $"{data.Stages[i].IsClearedSubMissions.Count(x => x)} / {data.Stages[i].IsClearedSubMissions.Length}";
            _stageinfos[i].HighScoreText.text = data.Stages[i].HighScore.ToString();
        }
    }
    #endregion
}

[Serializable]
public class StageInfo
{
    public string StageName;
    public Text IsClearStageText;
    public Text IsClearSubMissionsText;
    public Text HighScoreText;
}