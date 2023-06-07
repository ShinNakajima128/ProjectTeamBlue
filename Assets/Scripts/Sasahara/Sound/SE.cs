using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SE
{
    [SerializeField] string _key;
    [SerializeField] AudioClip _clip;
    [SerializeField] float _volume;

    public string Key => _key;
    public AudioClip Clip => _clip;
    public float Volume => _volume;
}
