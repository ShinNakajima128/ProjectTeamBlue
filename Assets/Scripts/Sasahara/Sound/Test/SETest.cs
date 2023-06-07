using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SETest : MonoBehaviour
{
    public void SE(string seName)
    {
        SoundManager.Instance.PlaySE(seName);
    }
}
