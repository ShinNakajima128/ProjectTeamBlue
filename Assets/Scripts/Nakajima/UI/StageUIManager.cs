using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// インゲーム中のステージのHUDを管理するManagerクラス
/// </summary>
public class StageUIManager : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    [Tooltip("HUDのパネル")]
    [SerializeField]
    private GameObject _hudPanel = default;

    [Tooltip("カウントダウンを表示するText")]
    [SerializeField]
    private Text _countDownText = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Start()
    {
        _hudPanel.SetActive(false);

        //カウントダウンの処理を登録
        StageManager.Instance.StartCountDownNum
                             .Subscribe(value =>
                             {
                                 _countDownText.text = value.ToString();

                                 if (value <= 0)
                                 {
                                     _countDownText.text = "START!!";
                                     StartCoroutine(InactiveCountDownText());
                                 }
                             });

        //インゲーム中のみ、HUDを表示する処理を登録
        StageManager.Instance.IsInGameSubject
                             .Subscribe(value => ChangePanelActive(value))
                             .AddTo(this);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    private void ChangePanelActive(bool isActive)
    {
        _hudPanel.SetActive(isActive);
    }
    #endregion

    #region coroutine method
    /// <summary>
    /// 時間差でカウントダウン用のTextを非アクティブにする
    /// </summary>
    /// <returns></returns>
    private IEnumerator InactiveCountDownText()
    {
        yield return new WaitForSeconds(1.0f);

        _countDownText.text = "";
        _countDownText.gameObject.SetActive(false);
    }
    #endregion
}
