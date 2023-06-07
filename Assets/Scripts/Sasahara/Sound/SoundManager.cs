using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	#region property
	// プロパティを入れる。
	

	public static SoundManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<SoundManager>();
			}
			return _instance;
		}
	}
	public bool IsFade => _isFadeOut;
	#endregion

	#region serialize
	// unity inpectorに表示したいものを記述。

	[SerializeField] SoundsScriptableObj scriptableObj = default;

	//BGMがフェードするのにかかる時間
	[SerializeField] const float bgmDefaltFadeTime = 0.5f;
	//BGMのデフォルトボリューム
	[SerializeField] const float bgmDefaltVolume = 0.1f;
	//SEのデフォルトボリューム
	[SerializeField] const float seDefaltVolume = 0.3f;

	public AudioSource _attachBGMSource;
　　[SerializeField] GameObject _sePrefab = null;
	#endregion

	#region private
	// プライベートなメンバー変数。
	private static SoundManager _instance;

	//BGMをフェードアウト中か
	bool _isFadeOut = false;


	//次流すBGM名、SE名
	string _nextBGMName;
    string _nextSEName;
	
	float _fadeTimeCount = 0.0f;
	/// <summary>BGMをフェードさせる時間</summary>
	float _bgmfadeTime = bgmDefaltFadeTime;
	float _bgmVolume = bgmDefaltVolume;
	float _bgmNextVolume = bgmDefaltVolume;

	AudioSource[] _seAudioSources;
	float _seVolume = seDefaltVolume;
	#endregion

	#region Constant
	// 定数をいれる。
	#endregion

	#region Event
	//  System.Action, System.Func などのデリゲートやコールバック関数をいれるところ。
	#endregion

	#region unity methods
	//  Start, UpdateなどのUnityのイベント関数。
	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	private void Start()
	{
		_attachBGMSource.volume = bgmDefaltVolume;
		_seAudioSources = _sePrefab.GetComponentsInChildren<AudioSource>();

	}

	private void Update()
	{
		if (!_isFadeOut)
		{
			return;
		}

		//徐々にボリュームを下げていき、ボリュームが0になったらボリュームを戻し次の曲を流す
		_fadeTimeCount += Time.deltaTime;
		_attachBGMSource.volume = Mathf.Lerp(_bgmVolume, 0.0f, _fadeTimeCount / _bgmfadeTime);
		if (_attachBGMSource.volume <= 0)
		{
			_attachBGMSource.Stop();
			_attachBGMSource.volume = _bgmNextVolume;
			_isFadeOut = false;

			if (!string.IsNullOrEmpty(_nextBGMName))
			{
				PlayBGM(_nextBGMName);
			}
		}
	}
	#endregion

	#region public method
	//　自身で作成したPublicな関数を入れる。

	//---------BGM---------
	public void PlayBGM(string bgmName, float fadeSpeedTime = bgmDefaltFadeTime, float volume = bgmDefaltVolume)
	{
		AudioClip ac = null;
		foreach (var item in scriptableObj.bgmList)
		{
            if (ac == item.Clip)
            {
				Debug.Log(item.Key + "という名前のBGMが重複しています");
			}
			if (bgmName == item.Key)
			{
				ac = item.Clip;
				break;
			}
		}
		if (ac is null)
		{
			Debug.Log(bgmName + "という名前のBGMがありません");
			return;
		}

		//現在BGMが流れていない時はそのまま流す。流れているBGMをフェードアウトさせてから次を流す。
		if (!_attachBGMSource.isPlaying)
		{
			_nextBGMName = "";
			_attachBGMSource.clip = ac;
			_attachBGMSource.Play();
			Debug.Log(bgmName);
			_attachBGMSource.volume = volume;
		}
		else if (_attachBGMSource.clip.name != bgmName)
		{
			_nextBGMName = bgmName;
			_bgmNextVolume = volume;
			FadeOutBGM(fadeSpeedTime);
		}
	}

	/// <summary>
	/// 現在流れている曲をフェードアウトさせる
	/// fadeSpeedに指定した割合でフェードアウトするスピードが変わる
	/// </summary>
	public void FadeOutBGM(float fadeSpeed)
	{
		if (_isFadeOut) return;
		if (!_attachBGMSource.isPlaying) return;

		_bgmVolume = _attachBGMSource.volume;
		_fadeTimeCount = 0.0f;
		_bgmfadeTime = fadeSpeed;
		_isFadeOut = true;
	}

	//-------SE--------
	/// <summary>
	/// 効果音用メソッド　複数同時にならすことができる
	/// </summary>
	/// <param name="seName">keyに設定した音源名</param>
	/// <param name="delay">遅らせて鳴らす時間</param>
	/// <param name="volume">音量</param>
	public void PlaySE(string seName, float delay = 0.0f, float volume = seDefaltVolume)
	{
		AudioClip ac = null;
		AudioSource se = null;
		foreach (var item in scriptableObj.seList)
		{
			if (item.Key == seName)
			{
				ac = item.Clip;
			}
		}
		if (ac is null)
		{
			Debug.Log(seName + "という名前のSEがありません");
			return;
		}
		Debug.Log($"PlaySE name = {seName}");


		foreach (var childComponent in _seAudioSources)
		{
			if (!childComponent.isPlaying)
			{
				se = childComponent;
				Debug.Log(se.gameObject.name);
				break;
			}
		}

		se.clip = ac;
		se.volume = volume;
		se.Play();
		#endregion

		#region private method
		// 自身で作成したPrivateな関数を入れる。
		#endregion
	}
}
