using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// エフェクトの再生時、終了時の機能を持つコンポーネント
/// </summary>
public class EffectController : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    #endregion

    #region private
    private ParticleSystem[] _particles;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        _particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void Start()
    {
        //エフェクトが再生しているか確認する処理の登録
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                foreach (var particle in _particles)
                {
                    if (particle.isPlaying)
                    {
                        return;
                    }
                }

                gameObject.SetActive(false);
            })
            .AddTo(this);
    }
    #endregion

    #region public method
    /// <summary>
    /// エフェクトを再生する
    /// </summary>
    /// <param name="pos">再生する座標</param>
    public void Play(Vector3 pos)
    {
        gameObject.SetActive(true);
        transform.localPosition = pos;
        foreach (var particle in _particles)
        {
            particle.Play();
        }
    }
    /// <summary>
    /// エフェクトの再生を止める
    /// </summary>
    public void Return()
    {
        foreach (var particle in _particles)
        {
            particle.Stop();
        }
    }

    /// <summary>
    /// 使用中か確認する
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
    #endregion

    #region private method
    #endregion
}
