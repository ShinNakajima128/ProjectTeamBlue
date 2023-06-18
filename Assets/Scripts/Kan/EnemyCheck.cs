using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyCheck : MonoBehaviour
{
    //当たり判定について、https://tech.pjin.jp/blog/2020/10/30/unity-oncollision
    #region property
    public GameObject HitPlayer { get { return hitPlayer; } }//あたり対象のゲームオブジェクト(プレイヤー)

    public bool IsStartCheck { get { return isStartCheck; } set { isStartCheck = value; } }//あたり処理が開始されたかどうか

    public float detectionRadius;//検出半径 
    public float detectionAngle;//検出角度
    public string playerTag;//プレイヤーのタグ


    #endregion

    #region private
    GameObject hitPlayer = null;//あたり対象のゲームオブジェクト(プレイヤー)
    bool isStartCheck = false;//あたり処理が開始されたかどうか

    Mesh mesh;
    [SerializeField]
    Material mat;

    #endregion

    #region Constant
    SphereCollider hitCollider;//球形(視野範囲)コライダーコンポーネント
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        mesh = new Mesh(); //メッシュを作成する
        GetComponent<MeshFilter>().mesh = mesh;//メッシュフィルターにメッシュを設定する
    }

    private void Start()
    {
        isStartCheck = true;//あたり処理を開始する

        hitCollider = GetComponent<SphereCollider>();//球形コライダーを取得する
        hitCollider.radius = detectionRadius;//検出半径を設定する


        setChecker(isStartCheck);//チェッカーの状態を設定する
    }

    private void Update()
    {
        float radius = detectionRadius;//<0 検出半径を変数に代入する
        float innerRadius = 0;//>=0 内側の半径を0に設定する（円環ではなく円にするため）
        int segments = 50;//分割数を50に設定する（円の滑らかさに影響する）
        float angleDegree = detectionAngle;//検出角度を変数に代入する
        Vector3 centerCircle = new Vector3(0, 0, 0);//円の中心点を原点に設定する（ローカル座標）
        DrawHalfCycle(radius, innerRadius, segments, angleDegree, centerCircle);//半円を描画するメソッドを呼び出す
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))//もし他のオブジェクトがプレイヤーのタグを持っていたら
        {
            //プレイヤーへの方向ベクトルを計算する（正規化されている）
            Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;

            //自分の前方とプレイヤーへの方向ベクトルのなす角度を計算する（度数法）
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= detectionAngle / 2)//もし角度が検出角度の半分以下だったら（つまり、検出範囲内だったら）
            {
                //あたり対象としてプレイヤーのゲームオブジェクトを保持する
                hitPlayer = other.gameObject;
            }
            else//そうでなければ（つまり、検出範囲外だったら）
            {
                hitPlayer = null;//あたり対象をnullにする
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))//もし他のオブジェクトがプレイヤーのタグを持っていたら
        {
            //プレイヤーへの方向ベクトルを計算する（正規化されている）
            Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;

            //自分の前方とプレイヤーへの方向ベクトルのなす角度を計算する（度数法）
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            //もし角度が検出角度の半分以下だったら（つまり、検出範囲内だったら）
            if (angleToPlayer <= detectionAngle / 2)
            {
                hitPlayer = null;//あたり対象をnullにする
            }
        }
    }

    #endregion

    #region private method
    //チェッカーの状態を設定するメソッド
    void setChecker(bool checkBool)
    {
        //球形コライダーの有効/無効を引数に応じて切り替える
        hitCollider.enabled = checkBool;
    }

    //半円を描画するメソッド
    void DrawHalfCycle(float radius, float innerRadius, int segments, float angleDegree, Vector3 centerCircle)
    {
        //メッシュレンダラーにマテリアルを設定する
        gameObject.GetComponent<MeshRenderer>().material = mat;

        //頂点配列を作成する（分割数×2+2個）
        Vector3[] vertices = new Vector3[segments * 2 + 2];
        vertices[0] = centerCircle;//最初の頂点は円の中心点とする
        float angleRad = Mathf.Deg2Rad * angleDegree;//角度をラジアンに変換する
        float angleCur = angleRad + Mathf.Deg2Rad * angleDegree / 2;//現在の角度を初期化する（半円の中心から始める）
        float angledelta = angleRad / segments;//角度の変化量を計算する（分割数で割る）

        for (int i = 0; i < vertices.Length; i += 2)//頂点配列を埋めるループ
        {
            float cosA = Mathf.Cos(angleCur);//現在の角度の余弦を計算する
            float sinA = Mathf.Sin(angleCur);//現在の角度の正弦を計算する

            //外側の頂点を計算する（x座標は半径×余弦、y座標は内側の半径、z座標は半径×正弦）
            vertices[i] = new Vector3(radius * cosA, innerRadius, radius * sinA);
            angleCur -= angledelta;//角度を減らす

        }

        int[] triangles = new int[segments * 6];//三角形配列を作成する（分割数×6個）
        for (int i = 0, vi = 0; i < triangles.Length; i += 6, vi += 2)//三角形配列を埋めるループ
        {
            //各三角形の頂点インデックス
            triangles[i] = vi;
            triangles[i + 1] = vi + 3;
            triangles[i + 2] = vi + 1;
            triangles[i + 3] = vi + 2;
            triangles[i + 4] = vi + 3;
            triangles[i + 5] = vi;
        }

        mesh.Clear();//メッシュをクリアする
        mesh.vertices = vertices;//メッシュに頂点配列を設定する
        mesh.triangles = triangles;//メッシュに三角形配列を設定する
    }

#if UNITY_EDITOR
    void OnDrawGizmos()//ギズモを描画するメソッド（エディターでのみ実行される）
    {
        if (hitCollider == null)//もし球形コライダーがnullだったら
        {
            //球形コライダーを取得する
            hitCollider = GetComponent<SphereCollider>();
        }
        hitCollider.radius = detectionRadius;//検出半径を設定する

        Vector3 forward = transform.forward;//前方ベクトルを取得する
        Vector3 center = transform.position;//中心点を取得する
        Vector3 normal = transform.up;//法線ベクトルを取得する（上方向）
        float angle = detectionAngle;//検出角度を取得する
        float radius = detectionRadius;//検出半径を取得する

        Handles.color = Color.red;//ハンドルの色を赤に設定する
        Handles.DrawSolidArc(center, normal, forward, angle / 2f, radius);//半円の弧を描画する（中心点、法線ベクトル、前方ベクトル、角度の半分、半径）
        Handles.DrawSolidArc(center, normal, forward, -angle / 2f, radius);//半円の弧を描画する（中心点、法線ベクトル、前方ベクトル、マイナス角度の半分、半径）
    }
#endif

    #endregion

}
