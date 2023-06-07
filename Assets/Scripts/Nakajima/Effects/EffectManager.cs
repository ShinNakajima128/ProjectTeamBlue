using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エフェクト再生機能を管理するManagerクラス
/// </summary>
public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    #region property
    #endregion

    #region serialize
    [Tooltip("エフェクトデータ")]
    [SerializeField]
    private Effect[] _effects = default;

    [Header("Debug")]
    [Tooltip("デバッグモード")]
    [SerializeField]
    private bool _debugMode = false;

    [Tooltip("テストしたいエフェクトの種類")]
    [SerializeField]
    private EffectType _debugEffect = default;
    #endregion

    #region private
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < _effects.Length; i++)
        {
            _effects[i].Setup(transform);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayEffect(EffectType.Explosion, Vector3.zero);
        }
    }
    #endregion

    #region public method
    /// <summary>
    /// エフェクトを再生する
    /// </summary>
    /// <param name="type">エフェクトの種類</param>
    /// <param name="pos">再生する場所</param>
    public static void PlayEffect(EffectType type, Vector3 pos)
    {
        Effect effect = Instance._effects.FirstOrDefault(x => x.Type == type);

        foreach (var e in effect.EffectCtrlList)
        {
            if (e.IsActive())
            {
                continue;
            }
            else
            {
                e.Play(pos);
                return;
            }
        }
        effect.AddToEffectList();
    }
    #endregion

    #region private method
    #endregion
}

/// <summary>
/// エフェクトのデータをまとめたクラス
/// </summary>
[Serializable]
public class Effect
{
    #region public
    /// <summary>エフェクト名</summary>
    public string EffectName;
    /// <summary>エフェクトの種類</summary>
    public EffectType Type;
    /// <summary>エフェクトのPrefab</summary>
    public GameObject EffectPrefab;
    /// <summary>最初に生成するエフェクトの数</summary>
    public int StartGenerateNum;
    /// <summary>エフェクトを機能を管理するコンポーネントList</summary>
    [HideInInspector]
    public List<EffectController> EffectCtrlList = new List<EffectController>();
    #endregion

    #region private
    /// <summary>エフェクトの親オブジェクト</summary>
    private Transform _parent;
    #endregion

    #region public method
    /// <summary>
    /// エフェクトの生成後、待機状態にする
    /// </summary>
    /// <param name="parent">親オブジェクト</param>
    public void Setup(Transform parent)
    {
        //各エフェクトの親オブジェクトを生成
        GameObject p = new GameObject($"{EffectName}List");
        _parent = p.transform;
        //EffectManagerの子オブジェクトにセットする
        p.transform.SetParent(parent);

        for (int i = 0; i < StartGenerateNum; i++)
        {
            var effectPrefab = GameObject.Instantiate(EffectPrefab, _parent).AddComponent<EffectController>();
            effectPrefab.gameObject.SetActive(false);
            EffectCtrlList.Add(effectPrefab);
        }
    }

    /// <summary>
    /// エフェクトを追加して再生する。エフェクトの数が足りていない場合に実行される
    /// </summary>
    public void AddToEffectList()
    {
        var effectPrefab = GameObject.Instantiate(EffectPrefab, _parent).AddComponent<EffectController>();
        EffectCtrlList.Add(effectPrefab);
    }
    #endregion
}

/// <summary>
/// エフェクトの種類
/// </summary>
public enum EffectType
{
    Explosion,
    Damage
}