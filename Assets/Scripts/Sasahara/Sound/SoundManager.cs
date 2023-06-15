using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
	#region property
	// プロパティを入れる。

	public bool IsFade => _isFadeOut;
	#endregion

	#region serialize
	// unity inpectorに表示したいものを記述。

	//Soundデータのスクリプタブルオブジェクト
	[SerializeField]
	SoundsScriptableObj scriptableObj = default;

	//BGMがフェードするのにかかる時間
	[SerializeField]
	const float bgmDefaltFadeTime = 0.5f;
	//BGMのデフォルトボリューム
	[SerializeField]
	const float bgmDefaltVolume = 0.1f;
	//SEのデフォルトボリューム
	[SerializeField]
	const float seDefaltVolume = 0.3f;

	[SerializeField]
	private AudioSource _attachBGMSource;
	[SerializeField,Range(0, 20)]
	private int _seAudioPrefabCount = 10;
　　[SerializeField]
	private GameObject _seAudioPrefab = null;
　　[SerializeField]
	private GameObject _seAudioObj = null;
　　[SerializeField]
	private AudioMixerGroup _seAudioGroup = null;
	[SerializeField]
	private GameObject _SoundWindow = null;
	#endregion

	#region private
	// プライベートなメンバー変数。

	//BGMをフェードアウト中か
	private bool _isFadeOut = false;


	//次流すBGM名、SE名
	private string _nextBGMName;
	private string _nextSEName;

	private float _fadeTimeCount = 0.0f;
	/// <summary>BGMをフェードさせる時間</summary>
	private float _bgmfadeTime = bgmDefaltFadeTime;
	private float _bgmVolume = bgmDefaltVolume;
	private float _bgmNextVolume = bgmDefaltVolume;

	private AudioSource[] _seAudioSources;
	private float _seVolume = seDefaltVolume;
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
		_SoundWindow.SetActive(false);

		_attachBGMSource.volume = bgmDefaltVolume;

		//SE用のAudioSouceの用意
		for (int i = 0; i < _seAudioPrefabCount; i++)
		{
			GameObject obj = Instantiate(_seAudioObj);
			obj.transform.parent = _seAudioPrefab.transform;
			AudioSource source = obj.GetComponent<AudioSource>();
			source.playOnAwake = false;
			source.loop = false;
			source.outputAudioMixerGroup = _seAudioGroup;
		}
		_seAudioSources = _seAudioPrefab.GetComponentsInChildren<AudioSource>();

		//シングルトン化
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

	private void Start()
	{
		

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
	/// <summary>BGM用メソッド　フェードを設定できる</summary>
	/// <param name="bgmName">Keyに設定した音源名</param>
	/// <param name="fadeSpeedTime">フェードにかける時間</param>
	/// <param name="volume">音量</param>
	public void PlayBGM(string bgmName, float fadeSpeedTime = bgmDefaltFadeTime, float volume = bgmDefaltVolume)
	{
		BGM bgm = null;
		foreach (var item in scriptableObj.bgmList)
		{
			if (bgmName == item.Key)
			{
				bgm = item;
				break;
			}
		}
		if (bgm is null)
		{
			Debug.Log(bgmName + "という名前のBGMがありません");
			return;
		}

		//現在BGMが流れていない時はそのまま流す。流れているBGMをフェードアウトさせてから次を流す。
		if (!_attachBGMSource.isPlaying)
		{
			_nextBGMName = "";
			_attachBGMSource.clip = bgm.Clip;
			_attachBGMSource.loop = true;
			_attachBGMSource.Play();
			Debug.Log(bgmName);
			_attachBGMSource.volume = volume * bgm.Volume;
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
	/// fadeSpeedに指定した数値でフェードアウトするスピードが変わる
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
		SE currentSE = null;
		AudioSource se = null;
		foreach (var item in scriptableObj.seList)
		{
			if (item.Key == seName)
			{
				currentSE = item;
				break;
			}
		}
		if (currentSE is null)
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
		se.clip = currentSE.Clip;
		se.volume = volume * currentSE.Volume;
		se.Play();
	}

	//UIのウインドウのオン
	public void SoundWindowOn()
    {
		_SoundWindow.SetActive(true);
    }
	//UIのウインドウのオフ
	public void SoundWindowOff()
    {
		_SoundWindow.SetActive(false);
	}
	#endregion

	#region private method
	// 自身で作成したPrivateな関数を入れる。
	#endregion
}
