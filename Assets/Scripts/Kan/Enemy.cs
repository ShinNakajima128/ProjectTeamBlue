using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    #region property
    public ENEMY_TYPE enemyType;
    public float speed = 5f;
    public float hp = 10;
    public float maxHP = 10;
    public float attackFarWithPlayer = 1.3f;
    public float stopFarWithPlayer = 1.5f;
    public float stopFarWithPoint = .3f;
    public float followTime = 1f;
    public float attcckCoolTime = 1f;
    public float rotSpeed = 1f;
    public float rotWaitTime = 2f;
    public float patrolTurningSpeed = 1f;//巡回回転速度
    public float angleThreshold = 1.0f;//最小回転角度

    public List<Tuple<bool,Vector3>> checkPointPositionList;
    public HPBar hpBar;
    #endregion

    #region serialize
    #endregion

    #region private
    [SerializeField]
    ENEMY_ACT enemyAct;
    ENEMY_ACT oldEnemyAct;
    NavMeshAgent enemyAgent;
    Coroutine backToStartPointCoroutine;
    Coroutine backToWaitAndSearchCoroutine;
    Coroutine backToGoNextPointCorutine;
    Coroutine lookAroundCorutine;

    int currentTargetIndex = 0;//回転パターン
    Vector3[] targets = new Vector3[] { new Vector3(0, -90, 0),Vector3.zero, new Vector3(0, 90, 0), new Vector3(0, 180, 0), new Vector3(0, 270, 0) };//回転パターン
    bool isLookingAround = false;//巡回中
    bool isFindPlayer = false;
    float totalAngle = 0.0f;//全回転角度
    GameObject taget;
    Quaternion initialRotation;

    Vector3 nextPos = Vector3.zero;
    #endregion

    #region Constant
    EnemyCheck enemyCheck;
    #endregion

    #region Event
    enum ENEMY_ACT
    {
        IDOL,
        WAIT_AND_SEARCH,
        GO_TO_NEXT_POINT,
        GOING_TO_NEXT_POINT,
        BACK_TO_STARTPOINT,
        //BACKED_STARTPOINT,
        CHASE,
        CHASEING,
        ATTACK,
        ATTACKING,
        SUMMON,
        DEATH
    }

    public enum ENEMY_TYPE
    {
        WARDER,
        WANDERER,
        WAITER,
        CAPTAIN
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
        enemyAgent.velocity = Vector3.zero;//慣性処理禁止
        initialRotation = transform.rotation;
        lookAroundCorutine = null;
        //enemyAgent.isStopped = false;
        enemyCheck = GetEnemyCheck();

        if (enemyType == ENEMY_TYPE.WARDER)
            return;

        
        SetPosList();
        nextPos = GetNextPositon();
    }

    private void Update()
    {

        switch(enemyAct)
        {
            case ENEMY_ACT.IDOL:
                if(enemyType == ENEMY_TYPE.WARDER)
                {
                    enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                }

                if (enemyType == ENEMY_TYPE.WANDERER)
                {
                    if (checkPointPositionList.Count == 0)
                        checkPointPositionList.Add(new Tuple<bool, Vector3>(false, transform.position));
                    else
                        transform.position = checkPointPositionList[0].Item2;




                    //backToGoNextPointCorutine = StartCoroutine(ReturnToOtherAct(ENEMY_ACT.GO_TO_NEXT_POINT, waitTime));

                    enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                }
                
                break;
            case ENEMY_ACT.WAIT_AND_SEARCH:

                if (enemyType == ENEMY_TYPE.WARDER)
                {
                    if (!isLookingAround)
                    {
                        lookAroundCorutine = StartCoroutine(LookAround());
                    }

                        GameObject playerObject = enemyCheck.HitPlayer;
                        if (playerObject == null)
                        {
                            //enemyAct = ENEMY_ACT.GO_TO_NEXT_POINT;
                            break;
                        }

                        taget = playerObject;
                        StopCoroutine(lookAroundCorutine);
                        isLookingAround = false;
                        enemyAct = ENEMY_ACT.SUMMON;
                    
                }

                if (enemyType == ENEMY_TYPE.WANDERER)
                {
                    //if (backToWaitAndSearchCoroutine != null)
                    //StopCoroutine(backToWaitAndSearchCoroutine);

                    if (!isLookingAround)
                    {
                        lookAroundCorutine = StartCoroutine(LookAround());
                    }

                    GameObject playerObject = enemyCheck.HitPlayer;
                    if (playerObject == null)
                    {
                        //enemyAct = ENEMY_ACT.GO_TO_NEXT_POINT;
                        break;
                    }

                    //isFindPlayer = true;

                    //if (NearByTarget(playerObject.transform.position, stopFarWithPlayer))
                    //{
                    //    //StopCoroutine(backToWaitAndSearchCoroutine);

                    //    if (oldEnemyAct == ENEMY_ACT.ATTACKING)
                    //    {
                    //        //enemyAct = ENEMY_ACT.ATTACK;
                    //Debug.Log("TEST");
                    //        break;
                    //    }

                    //break;
                    //}
                    taget = playerObject;
                    StopCoroutine(lookAroundCorutine);
                    isLookingAround = false;
                    enemyAct = ENEMY_ACT.CHASE;
                }
                break;
            case ENEMY_ACT.GO_TO_NEXT_POINT:
                //StopCoroutine(backToGoNextPointCorutine);
                isFindPlayer = false;
                GoToNextPosintion();
                enemyAct = ENEMY_ACT.GOING_TO_NEXT_POINT;
                break;
            case ENEMY_ACT.GOING_TO_NEXT_POINT:
                GoingToNextPosintion();
                break;
            case ENEMY_ACT.BACK_TO_STARTPOINT:
                BackToStartPosition();
                break;
            case ENEMY_ACT.CHASE:
                backToStartPointCoroutine = StartCoroutine(ReturnToOtherAct(ENEMY_ACT.BACK_TO_STARTPOINT, followTime));
                enemyAct = ENEMY_ACT.CHASEING;
                break;
            case ENEMY_ACT.CHASEING:
                DoHunter();
                break;
            case ENEMY_ACT.ATTACK:
                StopCoroutine(backToStartPointCoroutine);
                //backToWaitAndSearchCoroutine = StartCoroutine(ReturnToOtherAct(ENEMY_ACT.WAIT_AND_SEARCH, attcckCoolTime));
                //checkPointPositionList[checkPointPositionList.Count - 1] = new Tuple<bool, Vector3>(true, transform.position);//敵の最初座標を更新
                enemyAct = ENEMY_ACT.ATTACKING;
                break;
            case ENEMY_ACT.ATTACKING:
                //if(enemyCheck.StayingInSomething() == true || enemyCheck.GetStayInGameObjectByTag("Player"))
                //enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                DoAttack();
                break;
            case ENEMY_ACT.SUMMON:
                Debug.Log("ENEMY_ACT.SUMMON");
                //TODO:type3敵を作成し、召喚を実装
                break;
            case ENEMY_ACT.DEATH:
                break;
            default:
                Debug.LogError("ENEMYACT_ERROR:" + enemyAct);
                break;
        }

        UpdateHPBar(maxHP, hp);

        oldEnemyAct = enemyAct;
    }
    #endregion

    #region public method
    #endregion

    #region private method
    void SetPosList()
    {
        List<GameObject> objArr = GetComponent<EnemyCheckPoint>().GetChildsFormPointArr();

        bool isFirst = true;

        foreach (GameObject obj in objArr)
        {
            Vector3 pos = obj.transform.position;
            Tuple<bool, Vector3> t;
            if (isFirst == true)
            {
                t = new Tuple<bool, Vector3>(true, pos);
                isFirst = false;
            }
            else
                t = new Tuple<bool, Vector3>(false, pos);
            checkPointPositionList.Add(t);
        }
    }
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
        //GameObject playerObject = enemyCheck.HitPlayer;

        //if (playerObject)
        //{
        //    if(NearByTarget(playerObject.transform.position, stopFarWithPlayer))
        //    {
        //        Debug.Log("DoHunter");
        //        //StopCoroutine(backToStartpointCoroutine);
        //        enemyAct = ENEMY_ACT.ATTACK;
        //        enemyAgent.velocity = Vector3.zero;//慣性処理禁止;
        //        //enemyCheck.IsStartCheck=false;
        //        enemyAgent.isStopped = true;
                
        //        return;
        //    }
        //}
        ////else
        ////{
        ////    enemyAct = ENEMY_ACT.BACK_TO_STARTPOINT;
        ////    enemyAgent.isStopped = true;
        ////    return;
        ////}

        if(taget)
        {
            if (NearByTarget(taget.transform.position, attackFarWithPlayer))
            {
                enemyAgent.isStopped = true;
                enemyAgent.velocity = Vector3.zero;
                enemyAct = ENEMY_ACT.ATTACK;
                return;
            }

            enemyAgent.isStopped = false;
            enemyAgent.SetDestination(taget.transform.position);
            
        }
        
    }

    void GoToNextPosintion()
    {
        int cnt = 0;
        foreach(Tuple<bool, Vector3> item in checkPointPositionList)
        {
            if(item.Item1 == false)
            {
                enemyAgent.isStopped = false;
                enemyAgent.SetDestination(item.Item2);
                
                //var tuple = checkPointPositionList[cnt];
                //checkPointPositionList[cnt] = new Tuple<bool, Vector3>(true, tuple.Item2);
                return;
            }
            cnt++;
        }

        if (cnt >= checkPointPositionList.Count)
        {
            InitializeAllPoint();
            enemyAgent.isStopped = false;
            enemyAgent.SetDestination(checkPointPositionList[0].Item2);//最初の座標へ戻る
            enemyAct = ENEMY_ACT.GO_TO_NEXT_POINT;
        }



        //cnt = 0;
        //foreach (Tuple<bool, Vector3> item in checkPointPositionList)
        //{
        //    if (cnt == 0)
        //        enemyAgent.SetDestination(item.Item2);
        //    var tuple = checkPointPositionList[cnt];
        //    checkPointPositionList[cnt] = new Tuple<bool, Vector3>(false, tuple.Item2);
        //    cnt++;
        //}
    }

    void GoingToNextPosintion()
    {
        GameObject playerObject = enemyCheck.HitPlayer;

        if (playerObject != null)
        {
            taget = playerObject;
            for(int cnt = 0;cnt < checkPointPositionList.Count;cnt++)
            {
                var tuple = checkPointPositionList[cnt];
                if (tuple.Item1 == false)
                {
                    checkPointPositionList[cnt] = new Tuple<bool, Vector3>(true, tuple.Item2);
                    break;
                }
            }

            enemyAct = ENEMY_ACT.CHASE;
            return;
        }


        for (int cnt = 0; cnt < checkPointPositionList.Count; cnt++)
        {

            if (checkPointPositionList[cnt].Item1 == false)
            {
                if (NearByTarget(checkPointPositionList[cnt].Item2, stopFarWithPoint))
                {
                    //if(cnt >= checkPointPositionList.Count)
                    //{
                    //InitializeAllPoint();
                    //}
                    var tuple = checkPointPositionList[cnt];
                    checkPointPositionList[cnt] = new Tuple<bool, Vector3>(true, tuple.Item2);
                    //nextPos = tuple.Item2;
                    nextPos = GetNextPositon();

                    enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                }

                return;
            }
        }
    }

    Vector3 GetNextPositon()
    {
        Vector3 ret = Vector3.zero;
        int cnt = 0;
        foreach (Tuple<bool, Vector3> item in checkPointPositionList)
        {

            if (item.Item1 == false)
            {
                return item.Item2;
            }

            cnt++;
        }

        if(cnt >= checkPointPositionList.Count)
        {
            ret = checkPointPositionList[0].Item2;
            InitializeAllPoint();
        }

        return ret;
    }

    void InitializeAllPoint()
    {
        for (int cnt = 0; cnt < checkPointPositionList.Count; cnt++)
        {
            var tuple = checkPointPositionList[cnt];
            //if (cnt == 0)
                //checkPointPositionList[cnt] = new Tuple<bool, Vector3>(true, tuple.Item2);
            //else
                checkPointPositionList[cnt] = new Tuple<bool, Vector3>(false, tuple.Item2);
        }
    }

    void BackToStartPosition()
    {
        Vector3 startPosition =  Vector3.zero;
        foreach(Tuple<bool, Vector3> item in checkPointPositionList)
        {
            if (item.Item1 == true)
                startPosition = item.Item2;
        }

        startPosition = new Vector3(startPosition.x, transform.position.y, startPosition.z);

        enemyAgent.isStopped = false;
        enemyAgent.SetDestination(startPosition);
        transform.LookAt(startPosition);

        if (NearByTarget(startPosition, stopFarWithPoint))
        {
            //enemyCheck.HitingSomething() = null;
            //enemyCheck.hitGameObjectList.Clear();
            enemyAgent.ResetPath();
            StopCoroutine(backToStartPointCoroutine);
            enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
        }
    }

    Tuple<int,Vector3> GetStartPosition()
    {
        Tuple<int, Vector3> retPosItem = null;
        //int cnt = 0;
        //foreach (Tuple<bool, Vector3> posItem in  checkPointPositionList)
        //{
        //    if (posItem.Item1 == false)
        //    {
        //        retPosItem.
        //        //retPosItem.Item2 = posItem  ;
        //        break;
        //    }
        //    cnt++;
        //}



        return retPosItem;
    }

    void UpdateHPBar(float maxHP, float hp)
    {
        hpBar.MaxHP = maxHP;
        hpBar.HP = hp;
    }

    void DoAttack()
    {
        if (taget)
        {
            if (!NearByTarget(taget.transform.position, attackFarWithPlayer))
            {
                //enemyAgent.isStopped = true;
                //enemyAgent.velocity = Vector3.zero;
                enemyAct = ENEMY_ACT.CHASE;
                return;
            }

        }

    }
    bool isOverRound = false;
    IEnumerator LookAround()
    {
        isLookingAround = true;

        while (true)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targets[currentTargetIndex]), Time.deltaTime * rotSpeed);
            // 回転角度<1,次の向きへ
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(targets[currentTargetIndex])) < angleThreshold)
            {
                currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
                yield return new WaitForSeconds(rotWaitTime);
            }

            if(enemyType == ENEMY_TYPE.WARDER)
            {
                continue;
            }

            if(currentTargetIndex >= targets.Length-1 && isOverRound == false)
            {
                isOverRound = true;
            }

            
            
            if (isOverRound)
            {
                Vector3 toOther = (nextPos - transform.position).normalized;

                float dotProduct = Vector3.Dot(transform.forward, toOther);
                if (nextPos == Vector3.zero)
                    Debug.Log(checkPointPositionList.Count);
                //Debug.Log(transform.position + "⇒"+ nextPos);

                float threshold = .95f;

                //Quaternion.Slerp(transform.rotation,)
                if (dotProduct >= threshold)
                {
                    transform.LookAt(nextPos);
                    currentTargetIndex = 0;
                    
                    isOverRound = false;
                    isLookingAround = false;
                    
                    enemyAct = ENEMY_ACT.GO_TO_NEXT_POINT;
                    yield break;
                }
            }

            yield return null;
        }
    }

    IEnumerator ReturnToOtherAct(ENEMY_ACT act,float time)
    {
        yield return new WaitForSeconds(time);
        enemyAct = act;
    }

    bool NearByTarget(Vector3 pos,float far)
    {
        bool ret = true;
        Vector3 tagetPos = new Vector3(pos.x, 0, pos.z);
        Vector3 thisPos = new Vector3(transform.position.x, 0, transform.position.z);
        float dis = Vector3.Distance(tagetPos, thisPos);

        if(dis >= far)
        {
            ret = false;
        }

        return ret;
    }
    #endregion
}
