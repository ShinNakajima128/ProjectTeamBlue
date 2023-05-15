using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        yield return null;

        Setup();
    }
    #endregion

    #region public method
    #endregion

    #region private method
    private void Setup()
    {
        var data = StageManager.Instance.Data;

        for (int i = 0; i < _stageinfos.Length; i++)
        {
            _stageinfos[i].IsClearStageText.text = data.Stages[i].IsClearedStage ? "クリア済" : "未クリア";
            _stageinfos[i].IsClearSubMissionsText.text = $"サブミッション　{data.Stages[i].IsClearedSubMissions.Count(x => x)} ／ {data.Stages[i].IsClearedSubMissions.Length}";
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
}