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
        SoundManager.Instance.PlayBGM(SoundTag.BGMStage1);
    }
    public void BGMLobby()
    {
        SoundManager.Instance.PlayBGM(SoundTag.BGMLobby);
    }
    public void BGMFound()
    {
        SoundManager.Instance.PlayBGM(SoundTag.BGMFound);
    }
}
