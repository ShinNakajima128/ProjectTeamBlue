using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventForwarder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDisableAttackCollider()
    {
        //Debug.Log(transform.parent.name);
        transform.parent.BroadcastMessage("OnEnemyDisableAttack", SendMessageOptions.DontRequireReceiver);
    }

    public void OnEnableAttackCollider()
    {
        transform.parent.BroadcastMessage("OnEnemyEnableAttack", SendMessageOptions.DontRequireReceiver);
    }
}
