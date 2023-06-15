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

    Mesh mesh;
    [SerializeField]
    Material mat;

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
        mesh = new Mesh(); 
        GetComponent<MeshFilter>().mesh = mesh;
        
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
        float radius = detectionRadius;//<0
        float innerRadius = 0;//>=0
        int segments = 50;
        float angleDegree = detectionAngle;
        Vector3 centerCircle = new Vector3(0, 0, 0);
        DrawHalfCycle(radius, innerRadius, segments, angleDegree, centerCircle);
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

    #endregion

    #region private method
    void setChecker(bool checkBool)
    {
        hitCollider.enabled = checkBool;
    }

    void DrawHalfCycle(float radius, float innerRadius, int segments, float angleDegree, Vector3 centerCircle)
    {
        
        gameObject.GetComponent<MeshRenderer>().material = mat;
        



        Vector3[] vertices = new Vector3[segments * 2 + 2];
        vertices[0] = centerCircle;
        float angleRad = Mathf.Deg2Rad * angleDegree;
        float angleCur = angleRad+ Mathf.Deg2Rad* angleDegree/2;
        float angledelta = angleRad / segments;

        for (int i = 0; i < vertices.Length; i += 2)
        {
            float cosA = Mathf.Cos(angleCur);
            float sinA = Mathf.Sin(angleCur);

            vertices[i] = new Vector3(radius * cosA, innerRadius, radius * sinA);
            angleCur -= angledelta;

        }

        int[] triangles = new int[segments * 6];
        for (int i = 0, vi = 0; i < triangles.Length; i += 6, vi += 2)
        {
            triangles[i] = vi;
            triangles[i + 1] = vi + 3;
            triangles[i + 2] = vi + 1;
            triangles[i + 3] = vi + 2;
            triangles[i + 4] = vi + 3;
            triangles[i + 5] = vi;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (hitCollider == null)
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
