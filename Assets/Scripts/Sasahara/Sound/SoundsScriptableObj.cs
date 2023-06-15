using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scriptable Object", menuName = "Create Scriptable Object")]
//SoundデータをBGMとSEに分けて格納
public class SoundsScriptableObj : ScriptableObject
{
    public List<BGM> bgmList = new List<BGM>();
    public List<SE> seList = new List<SE>();
}
