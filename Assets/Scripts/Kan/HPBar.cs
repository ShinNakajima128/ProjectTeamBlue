using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour//HPバーを作成、アイコンに変更するためにHPバー関連処理を廃止
{
    #region property
    // プロパティを入れる。
    public Image hpFillImage;
    public float HP
    {
        get { return hp; }
        set { hp = value; }
    }
    public float MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }
    #endregion

    #region private
    // プライベートなメンバー変数。
    float hp = 0;
    float maxHP = 0;
    #endregion

    #region Event
    //  System.Action, System.Func などのデリゲートやコールバック関数をいれるところ。
    #endregion

    #region unity methods
    //  Start, UpdateなどのUnityのイベント関数。
    
    private void LateUpdate()//全ての処理が完了後実行
    {
        //hpFillImage.fillAmount = Mathf.Clamp(hp * 1 / maxHP, 0, 1);

        transform.forward = Camera.main.transform.forward;//向きはずっとカメラに
    }

    #endregion
}
