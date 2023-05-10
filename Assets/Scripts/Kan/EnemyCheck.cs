using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheck : MonoBehaviour
{
    //https://tech.pjin.jp/blog/2020/10/30/unity-oncollision
    #region property
    //public bool HitByPlayer { get { return hitByPlayer; } }//あたり対象あり
    public GameObject StayGameObject { get { return stayGameObject; } set { stayGameObject = value; } }//ずっと当たる対象を取得

    public GameObject HitGameObject { get { return hitGameObject; } set { hitGameObject = value; } }//当たる対象を取得
    
    public bool IsStartCheck { get { return isStartCheck; } set { isStartCheck = value; } }//あたり処理動作中


    #endregion

    #region serialize
    #endregion

    #region private
    //bool hitByPlayer = false;
    bool isStartCheck = false;
    GameObject hitGameObject = null;
    GameObject stayGameObject = null;

    //private GameObject hitGameObject
    #endregion

    #region Constant
    Collider hitCollider;
    Transform playerTransform;
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        
    }

    private void Start()
    {
        //hitByPlayer = false;

        isStartCheck = false;

        hitCollider = GetComponent<SphereCollider>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        
        setChecker(isStartCheck);
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //hitGameObject = null;
        //hitByPlayer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.tag == "Player")
            //hitByPlayer = true;

        hitGameObject = other.gameObject;
        //stayGameObject = other.gameObject;
    }

    private void OnTriggerStay(Collider other)
    {
        stayGameObject = other.gameObject;
        //Debug.Log(stayGameObject.name);
    }

    #endregion

    #region public method
    #endregion

    #region private method
    void setChecker(bool checkBool)
    {
        hitCollider.enabled = checkBool;
    }
    #endregion
}
