using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyCheck : MonoBehaviour
{
    //https://tech.pjin.jp/blog/2020/10/30/unity-oncollision
    #region property
    public GameObject HitPlayer { get { return hitPlayer; } }//あたり対象あり
    //public List<GameObject> hitGameObjectList { get; set; }
    //public List<GameObject> stayGameObjectList { get; set; }

    public bool IsStartCheck { get { return isStartCheck; } set { isStartCheck = value; } }//あたり処理動作中

    public float detectionRadius;
    public float detectionAngle;
    //public LayerMask playerLayer;
    public string playerTag;
    

    #endregion

    #region serialize
    #endregion

    #region private
    GameObject hitPlayer = null;
    bool isStartCheck = false;
    CapsuleCollider capsuleCollider;


    //private GameObject hitGameObject
    #endregion

    #region Constant
    SphereCollider hitCollider;
    Transform playerTransform;
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        //hitGameObjectList = new List<GameObject>();
        //stayGameObjectList = new List<GameObject>();
    }

    private void Start()
    {
        //hitByPlayer = false;

        isStartCheck = true;

        hitCollider = GetComponent<SphereCollider>();
        hitCollider.radius = detectionRadius;

        playerTransform = GameObject.FindWithTag("Player").transform;


        setChecker(isStartCheck);
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag(playerTag))
        //{
        //    Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;

        //    float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        //    if(angleToPlayer <= detectionAngle /2)
        //    {
        //        hitByPlayer = true;
        //    }
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;

            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= detectionAngle / 2)
            {
                hitPlayer = other.gameObject;
            }else
            {
                hitPlayer = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //hitGameObjectList.Add(other.gameObject);
        //hitGameObjectList.Clear();
        //stayGameObjectList.Clear();

        if (other.CompareTag(playerTag))
        {
            Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;

            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= detectionAngle / 2)
            {
                hitPlayer = null;
            }
        }
    }

    #endregion

    #region public method
    //public bool HitingSomething()
    //{
    //    bool hitSomething = false;
    //    foreach(var item in hitGameObjectList)
    //    {
    //        if (item.tag != "Ground")
    //            hitSomething = true;
    //    }
    //    return hitSomething;
    //}

    //public GameObject GetHitGameObjectByTag(string tagStr)
    //{
    //    GameObject ret = null;

    //    foreach (var item in hitGameObjectList)
    //    {
    //        if (item.CompareTag(tagStr))
    //            ret = item;
    //    }

    //    return ret;
    //}

    //public bool StayingInSomething()
    //{
    //    bool ret = false;
    //    foreach (var item in stayGameObjectList)
    //    {
    //        if (!item.CompareTag("Ground"))
    //            ret = true;
    //    }
    //    return ret;
    //}

    //public GameObject GetStayInGameObjectByTag(string tagStr)
    //{
    //    GameObject ret = null;

    //    foreach (var item in stayGameObjectList)
    //    {
    //        if (item.CompareTag(tagStr))
    //            ret = item;
    //    }

    //    return ret;
    //}

    #endregion

    #region private method
    void setChecker(bool checkBool)
    {
        hitCollider.enabled = checkBool;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if(hitCollider == null)
        {
            hitCollider = GetComponent<SphereCollider>();
        }
        hitCollider.radius = detectionRadius;

        Vector3 forward = transform.forward;
        Vector3 center = transform.position;
        Vector3 normal = transform.up;
        float angle = detectionAngle;
        float radius = detectionRadius;

        Handles.color = Color.red;
        Handles.DrawSolidArc(center, normal, forward, angle / 2f, radius);
        Handles.DrawSolidArc(center, normal, forward, -angle / 2f, radius);
    }
#endif
#endregion

}
