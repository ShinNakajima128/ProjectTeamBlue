using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
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

    #region serialize
    // unity inpectorに表示したいものを記述。
    #endregion

    #region private
    // プライベートなメンバー変数。
    //Enemy enemy;
    float hp = 0;
    float maxHP = 0;
    #endregion

    #region Constant
    // 定数をいれる。
    #endregion

    #region Event
    //  System.Action, System.Func などのデリゲートやコールバック関数をいれるところ。
    #endregion

    #region unity methods
    //  Start, UpdateなどのUnityのイベント関数。
    private void Awake()
    {

    }

    private void Start()
    {
        //enemy = GetComponentInParent<Enemy>();
        //hp = enemy.hp;
        //maxHP = enemy.maxHP;
    }

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
        hpFillImage.fillAmount = hp * 1 / maxHP;
    }
    #endregion

    #region public method
    //　自身で作成したPublicな関数を入れる。
    #endregion

    #region private method
    // 自身で作成したPrivateな関数を入れる。
    #endregion
    // Start is called before the first frame update
}
