using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

public class Enemy : MonoBehaviour, IDamagable
{
    #region property
    public ENEMY_TYPE enemyType;//種類
    public float speed = 5f;//速度
    public float hp = 10;//体力
    public float atk = 1;//攻撃
    [HideInInspector]//Inspectorに表示させない
    public int CurrentHP { get { return (int)hp; } }//現在の体力
    public float maxHP = 10;//最大の体力
    public float attackFarWithPlayer = 1.3f;//攻撃距離
    public float stopFarWithPlayer = 1.5f;//プレイヤーとの停止距離
    public float stopFarWithPoint = .3f;//巡回ポイントとの停止距離
    public float followTime = 1f;////プレイヤーを追跡する時間
    public float rotSpeed = 1f;//巡回回転速度
    public float rotWaitTime = 2f;//巡回回転待機時間
    public float angleThreshold = 1.0f;//最小回転角度
    public GameObject enemyWaiter;//召喚された敵（雑魚）
    public int SummonNum = 1;//召喚可能敵数
    public Animator _anim;//アニメーター

    public float summonRadius;//召喚半径
    public float summonCheckRadius;//召喚出来ない円範囲の半径


    public List<Tuple<bool, Vector3>> checkPointPositionList;//巡回ポイントのリスト

    public static Enemy Instance { get; private set; }
    #endregion

    #region serialize
    #endregion

    #region private
    [SerializeField]
    ENEMY_ACT enemyAct;////敵の行動状態
    NavMeshAgent enemyAgent;//ナビメッシュエージェント（経路探索用）
    Coroutine backToStartPointCoroutine;//開始地点に戻るコルーチン
    Coroutine lookAroundCorutine;//周囲を見渡すコルーチン

    int currentTargetIndex = 0;//回転パターン
    bool isOverRound = false;//一周をチェックしたかどうか
    Vector3[] targets = new Vector3[] { new Vector3(0, -90, 0), Vector3.zero, new Vector3(0, 90, 0), new Vector3(0, 180, 0), new Vector3(0, 270, 0) };//回転パターンの配列（角度）
    bool isLookingAround = false;//巡回中かどうか
    int SummonCnt = 0;//召喚された敵数
    GameObject taget;//ターゲット（現状はプレイヤー）

    Vector3 nextPos = Vector3.zero;//次に向かう位置
    #endregion

    #region Constant
    EnemyCheck enemyCheck;//敵チェッククラス（プレイヤーを見つける処理）
    #endregion

    #region Event
    enum ENEMY_ACT//敵の行動状態
    {
        IDOL,//待機
        WAIT_AND_SEARCH,//探索
        GO_TO_NEXT_POINT,//次のポイントへ移動
        GOING_TO_NEXT_POINT,//次のポイントへ移動中
        BACK_TO_STARTPOINT,//開始地点に戻る
        CHASE,//追跡
        CHASEING,//追跡中
        ATTACK,//攻撃
        ATTACKING,//攻撃中
        SUMMON,//召喚
        DEATH,//死亡
        DESTORY//破棄
    }

    public enum ENEMY_TYPE//敵の種類
    {
        WARDER,//監視カメラタイプ
        WANDERER,//巡回タイプ
        WAITER,//召喚された雑魚
    }

    #endregion

    #region unity methods
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StageManager.Instance.GameEndSubject//ゲーム終了時の処理を登録
                             .Subscribe(_ =>
                             {
                                 //ゲーム終了のメッセージを受け取ったら自身を非アクティブにする処理
                                 gameObject.SetActive(false);
                             })
                             .AddTo(this);

        enemyAct = ENEMY_ACT.IDOL;//初期状態は待機
        enemyAgent = GetComponent<NavMeshAgent>();//ナビメッシュエージェントを取得
        checkPointPositionList = new List<Tuple<bool, Vector3>>();//巡回ポイントのリストを初期化
        enemyAgent.updateRotation = false;//回転は制御するためにfalseにする
        enemyAgent.speed = speed;//速度を設定
        enemyAgent.velocity = Vector3.zero;//慣性処理禁止
        backToStartPointCoroutine = null; //開始地点に戻るコルーチンを初期化
        lookAroundCorutine = null;//周囲を見渡すコルーチンを初期化
        enemyCheck = GetEnemyCheck();//敵チェックを取得（プレイヤーを見つけるため）

        //敵の種類が監視カメラタイプならば、ここで終了（巡回しない）
        if (enemyType == ENEMY_TYPE.WARDER)
            return;


        SetPosList();////巡回ポイントのリストを設定するメソッドを呼ぶ

        ////敵の種類が召喚された雑魚ならば、ここで終了（巡回しない）
        if (enemyType == ENEMY_TYPE.WAITER) return;

        //次に向かう位置を取得するメソッドを呼ぶ
        nextPos = GetNextPositon();
    }

    private void Update()
    {
        if (hp <= 0 && enemyAct != ENEMY_ACT.DESTORY)
        {
            StopAllCoroutines();
            enemyAct = ENEMY_ACT.DEATH;
        }

        if (_anim != null)
            _anim.SetFloat("Speed", enemyAgent.velocity.magnitude);

        switch (enemyAct)//敵の行動状態に応じて処理を分岐
        {
            case ENEMY_ACT.IDOL://待機状態

                //敵の種類が監視カメラタイプならば、探索状態に遷移
                if (enemyType == ENEMY_TYPE.WARDER)
                {
                    enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                }

                //敵の種類が巡回タイプか召喚された雑魚ならば
                if (enemyType == ENEMY_TYPE.WANDERER || enemyType == ENEMY_TYPE.WAITER)
                {

                    if (checkPointPositionList.Count == 0)//巡回ポイントのリストが空ならば、自分の位置を追加
                        checkPointPositionList.Add(new Tuple<bool, Vector3>(false, transform.position));
                    else//最初の巡回ポイントに移動
                        transform.position = checkPointPositionList[0].Item2;

                    //敵の種類が召喚された雑魚ならば、追跡状態に遷移
                    if (enemyType == ENEMY_TYPE.WAITER)
                    {
                        enemyAct = ENEMY_ACT.CHASE;
                        break;
                    }

                    //敵の種類が巡回タイプならば探索状態に遷移
                    enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
                }
                break;

            case ENEMY_ACT.WAIT_AND_SEARCH://探索状態

                //周囲を見渡していなければ、周囲を見渡すコルーチンを開始
                if (!isLookingAround)
                {
                    lookAroundCorutine = StartCoroutine(LookAround());
                }

                //EnemyCheck.csからプレイヤーを取得（見つからなければnull）
                GameObject playerObject = enemyCheck.HitPlayer;

                if (playerObject == null)
                {
                    break;
                }

                taget = playerObject;//プレイヤーをターゲットに設定

                StopCoroutine(lookAroundCorutine);//周囲を見渡すコルーチンを停止
                isLookingAround = false;//周囲を見渡しているフラグをfalseにする

                //敵の種類に応じて次の行動状態に遷移
                switch (enemyType)
                {
                    case ENEMY_TYPE.WARDER://監視カメラタイプならば、召喚状態に遷移
                        enemyAct = ENEMY_ACT.SUMMON;
                        break;
                    case ENEMY_TYPE.WANDERER://巡回タイプならば、追跡状態に遷移
                    case ENEMY_TYPE.WAITER://召喚された雑魚ならば、追跡状態に遷移
                        enemyAct = ENEMY_ACT.CHASE;
                        break;
                    default:
                        Debug.LogError("ENEMYACT_ERROR:" + enemyAct);
                        break;
                }
                break;

            case ENEMY_ACT.GO_TO_NEXT_POINT://次のポイントへ移動状態
                GoToNextPosintion();//次のポイントへ移動する
                enemyAct = ENEMY_ACT.GOING_TO_NEXT_POINT;//次のポイントへ移動中状態に遷移
                break;

            case ENEMY_ACT.GOING_TO_NEXT_POINT://次のポイントへ移動中状態
                GoingToNextPosintion();//次のポイントへ移動中
                break;

            case ENEMY_ACT.BACK_TO_STARTPOINT://開始地点に戻る状態
                //プレイヤーが見つかっているフラグがtrueならば、falseにする
                if (StageManager.Instance.IsFounded.Value)
                {
                    StageManager.Instance.IsFounded.Value = false;
                }
                BackToStartPosition();//開始地点に戻る
                break;

            case ENEMY_ACT.CHASE://追跡状態
                //プレイヤーが見つかっているフラグがfalseならば、trueにする
                if (!StageManager.Instance.IsFounded.Value)
                {
                    StageManager.Instance.IsFounded.Value = true;
                }

                //一定時間後に開始地点に戻るコルーチンを開始
                backToStartPointCoroutine = StartCoroutine(ReturnToOtherAct(ENEMY_ACT.BACK_TO_STARTPOINT, followTime));
                enemyAct = ENEMY_ACT.CHASEING; //追跡中状態に遷移
                break;

            case ENEMY_ACT.CHASEING://追跡中状態
                DoHunter();//追跡する
                break;
            case ENEMY_ACT.ATTACK://攻撃状態
                StopCoroutine(backToStartPointCoroutine);//開始地点に戻るコルーチンを停止
                enemyAct = ENEMY_ACT.ATTACKING;//攻撃中状態に遷移
                break;

            case ENEMY_ACT.ATTACKING://攻撃中状態
                _anim.SetTrigger("IsAttack");//アニメーターに攻撃Triggerを送る
                DoAttack();//攻撃する
                break;

            case ENEMY_ACT.SUMMON://召喚状態
                DoSummon();//召喚する
                break;

            case ENEMY_ACT.DEATH://死亡
                Death();//死亡
                break;
            case ENEMY_ACT.DESTORY://破棄
                DoDestroy();//破棄する
                break;

            default:
                //それ以外はエラーとする
                Debug.LogError("ENEMYACT_ERROR:" + enemyAct);
                break;
        }
    }
    #endregion

    #region public method
    //敵攻撃アニメション完了時に呼ばれる
    public void OnEnemyDisableAttack()
    {
        //Debug.Log("敵攻撃アクション完了");
        PlayerController.Instance.Damage((int)atk);//プレイヤーにダメージを与える
        SoundManager.Instance.PlaySE(SoundTag.SE_Attack);//攻撃音を再生する
    }

    //敵攻撃アニメション開始時に呼ばれる
    public void OnEnemyEnableAttack()
    {
        //Debug.Log("敵攻撃アクション開始");
    }

    //ダメージを受けるメソッド（インターフェースで実装）
    public void Damage(int damageAmount)
    {
        hp -= damageAmount;//体力を減らす
                           //Debug.Log("ENEMY_HP:" + hp);
                           //if (hp <= 0)
                           //{
                           //体力が0以下ならば、死亡状態に遷移
                           //Debug.Log("ENEMY_DIE");
                           //}
    }
    #endregion

    #region private method
    void SetPosList()//巡回ポイントのリストを設定する
    {
        //敵の種類が召喚された雑魚ならば、自分の位置をリストに追加して終了
        if (enemyType == ENEMY_TYPE.WAITER)
        {
            checkPointPositionList.Add(new Tuple<bool, Vector3>(true, transform.position));
            return;
        }

        //巡回ポイントのオブジェクトを取得
        List<GameObject> objArr = GetComponent<EnemyCheckPoint>().GetChildsFormPointArr();

        bool isFirst = true;

        //オブジェクトの位置をリストに追加
        foreach (GameObject obj in objArr)
        {
            Vector3 pos = obj.transform.position;
            Tuple<bool, Vector3> t;
            if (isFirst == true)//最初の位置はtrueにする（開始地点）
            {
                t = new Tuple<bool, Vector3>(true, pos);
                isFirst = false;
            }
            else
                t = new Tuple<bool, Vector3>(false, pos);
            checkPointPositionList.Add(t);
        }
    }
    EnemyCheck GetEnemyCheck()//敵チェックを取得する（プレイヤーを見つけるため）
    {
        EnemyCheck ret = null;
        //子オブジェクトからEnemyCheckコンポーネントを探す
        foreach (EnemyCheck hit in GetComponentsInChildren<EnemyCheck>())
        {
            string name = hit.name;
            //名前がCheckBoxならば、それが敵チェックとする
            if (name == "CheckBox")
            {
                ret = hit;
                break;
            }
        }
        return ret;
    }

    void DoHunter()//追跡する
    {

        if (taget)//ターゲット(プレイヤー)が存在すれば
        {
            //ターゲットとの距離が攻撃距離以内ならば
            if (NearByTarget(taget.transform.position, attackFarWithPlayer))
            {
                enemyAgent.isStopped = true;//ナビメッシュエージェントを停止
                enemyAgent.velocity = Vector3.zero;//速度を0にする
                enemyAct = ENEMY_ACT.ATTACK;//攻撃状態に遷移
                return;
            }

            enemyAgent.isStopped = false;//ナビメッシュエージェントを再開
            enemyAgent.SetDestination(taget.transform.position); //ターゲットの位置に向かう
            transform.LookAt(taget.transform);//ターゲットの方向に向く

        }

    }

    void GoToNextPosintion()//次のポイントへ移動する
    {
        int cnt = 0;
        //巡回ポイントのリストから未到達のものを探す
        foreach (Tuple<bool, Vector3> item in checkPointPositionList)
        {
            if (item.Item1 == false)//未到達ならば
            {
                enemyAgent.isStopped = false;//ナビメッシュエージェントを再開
                enemyAgent.SetDestination(item.Item2);//その位置に向かう

                return;
            }
            cnt++;
        }

        //巡回ポイントのリストがすべて到達済みならば、リストを初期化する
        if (cnt >= checkPointPositionList.Count)
        {
            InitializeAllPoint();
            enemyAgent.isStopped = false;//ナビメッシュエージェントを再開
            enemyAgent.SetDestination(checkPointPositionList[0].Item2);//最初の座標へ戻る
            enemyAct = ENEMY_ACT.GO_TO_NEXT_POINT; //次のポイントへ移動状態に遷移
        }
    }

    void GoingToNextPosintion()//次のポイントへ移動中のメソッド
    {
        //敵チェックからプレイヤーを取得（見つからなければnull）
        GameObject playerObject = enemyCheck.HitPlayer;

        if (playerObject != null)//プレイヤーが見つかれば
        {
            taget = playerObject; //プレイヤーをターゲットに設定
            //巡回ポイントのリストから未到達のものを探す
            for (int cnt = 0; cnt < checkPointPositionList.Count; cnt++)
            {
                var tuple = checkPointPositionList[cnt];
                if (tuple.Item1 == false)//未到達ならば、到達済みに変更する（追跡するためにスキップする）
                {
                    checkPointPositionList[cnt] = new Tuple<bool, Vector3>(true, tuple.Item2);
                    break;
                }
            }

            enemyAct = ENEMY_ACT.CHASE;//追跡状態に遷移
            return;
        }

        //巡回ポイントのリストから未到達のものを探す
        for (int cnt = 0; cnt < checkPointPositionList.Count; cnt++)
        {
            //未到達ならば
            if (checkPointPositionList[cnt].Item1 == false)
            {
                //その位置との距離が停止距離以内ならば
                if (NearByTarget(checkPointPositionList[cnt].Item2, stopFarWithPoint))
                {
                    var tuple = checkPointPositionList[cnt];
                    checkPointPositionList[cnt] = new Tuple<bool, Vector3>(true, tuple.Item2);
                    nextPos = GetNextPositon();//次に向かう位置を取得する

                    enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;//探索状態に遷移
                }

                return;
            }
        }
    }

    Vector3 GetNextPositon()//次に向かう位置を取得する
    {
        Vector3 ret = Vector3.zero;
        int cnt = 0;
        //巡回ポイントのリストから未到達のものを探す
        foreach (Tuple<bool, Vector3> item in checkPointPositionList)
        {
            //未到達ならば、その位置を返す
            if (item.Item1 == false)
            {
                return item.Item2;
            }

            cnt++;
        }
        //巡回ポイントのリストがすべて到達済みならば、最初の位置を返す
        if (cnt >= checkPointPositionList.Count)
        {
            ret = checkPointPositionList[0].Item2;
            InitializeAllPoint();//巡回ポイントのリストを初期化する
        }

        return ret;//次に向かう位置を返す
    }

    void InitializeAllPoint()//巡回ポイントのリストを初期化するメソッド（すべて未到達にする）
    {
        for (int cnt = 0; cnt < checkPointPositionList.Count; cnt++)
        {
            var tuple = checkPointPositionList[cnt];
            checkPointPositionList[cnt] = new Tuple<bool, Vector3>(false, tuple.Item2);
        }
    }

    void BackToStartPosition()//開始地点に戻る
    {
        Vector3 startPosition = Vector3.zero;
        //巡回ポイントのリストから最初の位置（開始地点）を探す
        foreach (Tuple<bool, Vector3> item in checkPointPositionList)
        {
            if (item.Item1 == true)//最初の位置ならば、その位置を取得
                startPosition = item.Item2;
        }

        startPosition = new Vector3(startPosition.x, transform.position.y, startPosition.z);

        enemyAgent.isStopped = false;//ナビメッシュエージェントを再開
        enemyAgent.SetDestination(startPosition);//開始地点に向かう
        transform.LookAt(startPosition);//開始地点の方向に向く

        //開始地点との距離が停止距離以内ならば
        if (NearByTarget(startPosition, stopFarWithPoint))
        {
            enemyAgent.ResetPath();//ナビメッシュエージェントの経路をリセット
            StopCoroutine(backToStartPointCoroutine);//開始地点に戻るコルーチンを停止
            enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;//探索状態に遷移
        }
    }

    void DoAttack()//攻撃する
    {
        if (taget) //ターゲットが存在すれば
        {
            //ターゲットの方向に向く
            transform.LookAt(taget.transform);
            //ターゲットとの距離が攻撃距離以外ならば、追跡状態に遷移
            if (!NearByTarget(taget.transform.position, attackFarWithPlayer))
            {
                enemyAct = ENEMY_ACT.CHASE;
                return;
            }

        }

    }

    void DoSummon()//召喚する
    {
        //召喚された敵数が召喚可能敵数以上ならば、何もしない
        if (SummonCnt >= SummonNum)
        {
            //敵チェックからプレイヤーを取得（見つからなければnull）
            GameObject playerObject = enemyCheck.HitPlayer;

            //プレイヤーが見つかれば、プレイヤーの方向に向く
            if (playerObject != null)
            {
                Vector3 playerPos = new Vector3(playerObject.transform.position.x, transform.position.y, playerObject.transform.position.z);
                transform.LookAt(playerPos);
            }
            else //プレイヤーが見つからなければ、探索状態に遷移
                enemyAct = ENEMY_ACT.WAIT_AND_SEARCH;
            return;
        }

        if (enemyWaiter == null) return;//召喚する敵オブジェクトがなければ、何もしない


        Vector3 position = UnityEngine.Random.insideUnitCircle * summonRadius;//insideUnitCircle円内乱数で位置を決める

        position = new Vector3(position.x, 0, position.y) + transform.position;//自分の位置に足す

        NavMeshHit hit;

        if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas))//移動できるポジションを取得
        {
            //召喚出来ない円範囲に他の敵がいなければ
            if (!Physics.CheckSphere(hit.position, summonCheckRadius, gameObject.layer))//~gameObject.layer))
            {
                GameObject obj = Instantiate(enemyWaiter, hit.position, Quaternion.identity);//召喚する敵オブジェクトを生成
                obj.GetComponent<Enemy>().taget = taget;//ターゲットを設定
                SummonCnt++; //召喚された敵数を増やす
            }
            else
            {
                //召喚出来ない円範囲に他の敵がいれば、再度召喚するメソッドを呼ぶ
                DoSummon();
            }
        }
        else//移動できるポジションが取得できなければ、再度召喚するメソッドを呼ぶ
        {
            DoSummon();
        }
    }

    void Death()//死亡する
    {
        string deathName = "IsDead";//死亡アニメーションのパラメータ名
        _anim.SetBool(deathName, true);//アニメーターに死亡パラメータを送る
        enemyAct = ENEMY_ACT.DESTORY;//破棄状態に遷移

    }

    void DoDestroy()//破棄する
    {
        //アニメーターの現在のステート情報を取得
        AnimatorStateInfo stateInfo = _anim.GetCurrentAnimatorStateInfo(0);
        string deathName = "IsDead";//死亡アニメーションのパラメータ名
        bool isDead = _anim.GetBool(deathName);//アニメーターから死亡パラメータを取得

        //死亡パラメータがtrueで、アニメーションが終了していれば
        if (isDead && stateInfo.normalizedTime >= stateInfo.length)
        {
            Destroy(gameObject);//自分自身を破棄する
        }
    }

    IEnumerator LookAround()//周囲を見渡すコルーチン
    {
        isLookingAround = true;//周囲を見渡しているフラグをtrueにする

        while (true)//無限ループ（探索状態が変わるまで）
        {
            //現在の回転から目標の回転に向かって徐々に回転する
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targets[currentTargetIndex]), Time.deltaTime * rotSpeed);
            // 回転角度<1,次の向きへ
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(targets[currentTargetIndex])) < angleThreshold)
            {
                //回転角度が最小回転角度以下ならば、次の回転パターンに遷移する
                currentTargetIndex = (currentTargetIndex + 1) % targets.Length; //次の回転パターンのインデックスを取得（ループする）
                yield return new WaitForSeconds(rotWaitTime); //回転待ち時間だけ待つ
            }

            //敵の種類が監視カメラタイプならば、ここで終了（巡回しない）
            if (enemyType == ENEMY_TYPE.WARDER)
            {
                continue;
            }

            //回転パターンが最後に達していて、一周していなければ
            if (currentTargetIndex >= targets.Length - 1 && isOverRound == false)
            {
                //一周したフラグをtrueにする
                isOverRound = true;
            }



            if (isOverRound)//一周したならば
            {
                //次に向かう位置との方向ベクトルを取得
                Vector3 toOther = (nextPos - transform.position).normalized;

                //自分の前方ベクトルとの内積を取得
                float dotProduct = Vector3.Dot(transform.forward, toOther);
                //if (nextPos == Vector3.zero)
                //Debug.Log(checkPointPositionList.Count);

                float threshold = .95f;//内積がこの値以上ならば、ほぼ同じ方向とみなす

                if (dotProduct >= threshold)
                {
                    //内積が閾値以上ならば、次に向かう位置の方向に向いていると判断
                    transform.LookAt(nextPos);//次に向かう位置の方向に向く
                    currentTargetIndex = 0;//回転パターンのインデックスを0に戻す

                    isOverRound = false;//一周したフラグをfalseに戻す
                    isLookingAround = false;//周囲を見渡しているフラグをfalseに戻す

                    enemyAct = ENEMY_ACT.GO_TO_NEXT_POINT;//次のポイントへ移動状態に遷移
                    yield break;//コルーチンを終了する
                }
            }

            yield return null;//次のフレームまで待つ
        }
    }

    //一定時間後に他の行動状態に遷移するコルーチン
    IEnumerator ReturnToOtherAct(ENEMY_ACT act, float time)
    {
        yield return new WaitForSeconds(time);
        enemyAct = act;//指定した行動状態に遷移する
    }


    //ターゲットとの距離が指定した距離以内かどうかを判定する
    bool NearByTarget(Vector3 pos, float far)
    {
        bool ret = true;
        Vector3 tagetPos = new Vector3(pos.x, 0, pos.z);//高さを無視するためにy座標を0にする
        Vector3 thisPos = new Vector3(transform.position.x, 0, transform.position.z);//高さを無視するためにy座標を0にする

        //ターゲットとの距離を計算する
        float dis = Vector3.Distance(tagetPos, thisPos);

        if (dis >= far)//距離が指定した距離以上ならば、falseを返す
        {
            ret = false;
        }

        return ret;//判定結果を返す
    }

    #endregion
}
