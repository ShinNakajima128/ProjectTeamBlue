using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;

public class LastBoomShowTrigger : MonoBehaviour
{
    public PlayableDirector director;

    public PlayerMove playerMove;

    protected bool alreadyTriggered;

    float playerSpeed = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (playerMove == null)
        {
            playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        }

        #region written by nakajima
        StageManager.Instance.EscapeSubject
                             .Subscribe(_ => PlayTimeline())
                             .AddTo(this);

        director.stopped += x =>
        {
            StageManager.Instance.OnGameEnd();
        };
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (alreadyTriggered)
    //        return;
    //    director.Play();
    //    alreadyTriggered = true;
    //    playerMove.enabled = false;
    //    playerSpeed = playerMove.GetComponent<Animator>().speed;
    //    playerMove.GetComponent<Animator>().speed = 0;
    //    Invoke("FinishInvoke", (float)director.duration);//タイムライン実行終了後FinishInvoke関数内処理を実行
    //}

    void FinishInvoke()
    {
        playerMove.enabled = true;
        playerMove.GetComponent<Animator>().speed = playerSpeed;
    }

    private void PlayTimeline()
    {
        if (alreadyTriggered)
            return;
        FadeManager.Fade(FadeType.Out, () =>
        {
            FadeManager.Fade(FadeType.In);
            director.Play();
            alreadyTriggered = true;
            playerMove.enabled = false;
            playerSpeed = playerMove.GetComponent<Animator>().speed;
            playerMove.GetComponent<Animator>().speed = 0;
            Invoke("FinishInvoke", (float)director.duration);//タイムライン実行終了後FinishInvoke関数内処理を実行
        });
    }
}
