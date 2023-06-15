using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SE
{
    [SerializeField] string _key;
    [SerializeField] AudioClip _clip;
    [SerializeField, Range(0f, 1f)] float _volume = 1;

    public string Key => _key;
    public AudioClip Clip => _clip;
    public float Volume => _volume;
}
