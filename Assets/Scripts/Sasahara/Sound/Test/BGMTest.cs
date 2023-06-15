using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMTest : MonoBehaviour
{
    public void BGM(string bgmName)
    {
        SoundManager.Instance.PlayBGM(bgmName);
    }
    public void BGMStage1()
    {
        SoundManager.Instance.PlayBGM(SoundTag.BGM_Stage1);
    }
    public void BGMLobby()
    {
        SoundManager.Instance.PlayBGM(SoundTag.BGM_Lobby);
    }
    public void BGMFound()
    {
        SoundManager.Instance.PlayBGM(SoundTag.BGM_Found);
    }
}
