using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region property
    public float speed = 5f;
    public float hp = 10;
    public float maxHP = 10;
    public float stopFarWithPlayer = 2.3f;
    public float stopFarWithStartPoint = .3f;
    public float followTime = 1f;
    public float followCoolTime = 1f;
    public List<Tuple<bool,Vector3>> checkPointPositionList;
    public HPBar hpBar;
    #endregion

    #region serialize
    #endregion

    #region private
    [SerializeField]
    ENEMY_ACT enemyAct;
    NavMeshAgent enemyAgent;
    Coroutine backToStartpointCoroutine;
    #endregion

    #region Constant
    EnemyCheck enemyCheck;
    #endregion

    #region Event
    enum ENEMY_ACT
    {
        IDOL,
        WAIT_AND_SEARCH,
        BACK_TO_STARTPOINT,
        //BACKED_STARTPOINT,
        CHASE,
        CHASEING,
        ATTACK,
        ATTACKING,
        DEATH
    }

    //public struct checkPointInfo
    //{
    //    bool isLastPoint;
    //    Vector3 Vec3Pos;
    //}
    #endregion

    #region unity methods
    private void Awake()
    {

    }

    private void Start()
    {
        enemyAct = ENEMY_ACT.IDOL;
        enemyAgent = GetComponent<NavMeshAgent>();
        checkPointPositionList = new List<Tuple<bool,Vector3>>();
        enemyAgent.updateRotation = false;
        enemyAgent.speed = speed;
        enemyCheck = GetEnemyCheck();
        
    }

    private void Update()
    {

        switch(enemyAct)
        {
            case ENEMY_ACT.IDOL:
                enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                //TODO:接地後最初の座標を記録
                if (checkPointPositionList.Count == 0)
                    checkPointPositionList.Add(new Tuple<bool,Vector3>(true, transform.position));
                break;
            case ENEMY_ACT.WAIT_AND_SEARCH:
                if (enemyCheck.HitGameObject != null && enemyCheck.HitGameObject.tag == "Player")
                {
                    enemyAct = ENEMY_ACT.CHASE;
                }
                break;
            case ENEMY_ACT.BACK_TO_STARTPOINT:
                BackToStartPosition();
                break;
            //case ENEMY_ACT.BACKED_STARTPOINT:

                //break;
            case ENEMY_ACT.CHASE:
                backToStartpointCoroutine = StartCoroutine(ReturnToOtherAct(ENEMY_ACT.BACK_TO_STARTPOINT, followTime));
                enemyAct = ENEMY_ACT.CHASEING;
                break;
            case ENEMY_ACT.CHASEING:
                DoHunter();
                break;
            case ENEMY_ACT.ATTACK:
                //StartCoroutine(ReturnToOtherAct(ENEMY_ACT.ATTACKING, followCoolTime));
                //checkPointPositionList[checkPointPositionList.Count - 1] = new Tuple<bool, Vector3>(true, transform.position);//敵の最初座標を更新
                enemyAct = ENEMY_ACT.ATTACKING;
                break;
            case ENEMY_ACT.ATTACKING:
                if(enemyCheck.StayGameObject == null || enemyCheck.StayGameObject.tag != "Player")
                    enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                //enemyCheck.StayGameObject = null;
                break;
            case ENEMY_ACT.DEATH:
                break;
            default:
                Debug.LogError("ENEMYACT_ERROR:" + enemyAct);
                break;
        }

        UpdateHPBar(10, 5);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    EnemyCheck GetEnemyCheck()
    {
        EnemyCheck ret = null;
        foreach (EnemyCheck hit in GetComponentsInChildren<EnemyCheck>())
        {
            string name = hit.name;
            if (name == "CheckBox")
            {
                ret = hit;
                break;
            }
        }
        return ret;
    }

    void DoHunter()
    {
        if(enemyCheck.HitGameObject.tag == "Player")
        {
            if(NearByTarget(enemyCheck.HitGameObject.transform.position, stopFarWithPlayer))
            {
                StopCoroutine(ReturnToOtherAct(ENEMY_ACT.BACK_TO_STARTPOINT, followTime));
                enemyAct = ENEMY_ACT.ATTACK;
                enemyCheck.IsStartCheck=false;
                enemyAgent.isStopped = true;
                enemyAgent.velocity = Vector3.zero;//慣性処理禁止
                StopCoroutine(backToStartpointCoroutine);
                return;
            }
        }

        if(enemyCheck.HitGameObject == null)
        {
            enemyAct = ENEMY_ACT.BACK_TO_STARTPOINT;
            enemyAgent.isStopped = true;
            enemyAgent.velocity = Vector3.zero;//慣性処理禁止
            return;
        }

        if(enemyCheck.HitGameObject.tag == "Player")
        {
            enemyAgent.isStopped = false;
            enemyAgent.SetDestination(enemyCheck.HitGameObject.transform.position);
            
        }
        
    }

    void BackToStartPosition()
    {
        Tuple<bool,Vector3> startPositionInfo =  checkPointPositionList[checkPointPositionList.Count - 1];
        enemyAgent.SetDestination(startPositionInfo.Item2);

        if (NearByTarget(startPositionInfo.Item2,stopFarWithStartPoint))
        {
            enemyCheck.HitGameObject = null;
            enemyAgent.ResetPath();
            enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
        }
    }

    void UpdateHPBar(float maxHP, float hp)
    {
        hpBar.MaxHP = maxHP;
        hpBar.HP = hp;
    }

    void DoAttack()
    {
        
        
    }

    IEnumerator ReturnToOtherAct(ENEMY_ACT act,float time)
    {
        yield return new WaitForSeconds(time);
        enemyAct = act;
    }

    bool NearByTarget(Vector3 pos,float far)
    {
        bool ret = true;
        
        float dis = Vector3.Distance(pos, transform.position);

        if(dis >= far)
        {
            ret = false;
        }

        return ret;
    }
    #endregion
}
