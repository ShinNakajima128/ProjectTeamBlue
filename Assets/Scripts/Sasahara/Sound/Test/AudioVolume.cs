using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour
{
    #region property
    // プロパティを入れる。
    #endregion

    #region serialize
    // unity inpectorに表示したいものを記述。
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _MasterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _seSlider;
    #endregion

    #region private
    // プライベートなメンバー変数。

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
        //スライダーを動かした時の処理を登録
        _MasterSlider.onValueChanged.AddListener(SetAudioMixerMaster);
        _bgmSlider.onValueChanged.AddListener(SetAudioMixerBGM);
        _seSlider.onValueChanged.AddListener(SetAudioMixerSE);
    }

    private void Start()
    {
        _MasterSlider.value = _MasterSlider.maxValue / 2;
        _bgmSlider.value = _bgmSlider.maxValue / 2;
        _seSlider.value = _seSlider.maxValue / 2;
    }

    private void Update()
    {

    }
    #endregion

    #region public method
    //　自身で作成したPublicな関数を入れる。

    //Master
    public void SetAudioMixerMaster(float value)
    {
        //5段階補正
        value /= 10;
        //-80~0に変換
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //audioMixerに代入
        _audioMixer.SetFloat("Master", volume);
        Debug.Log($"Master:{volume}");
    }

    //BGM
    public void SetAudioMixerBGM(float value)
    {
        //5段階補正
        value /= 10;
        //-80~0に変換
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //audioMixerに代入
        _audioMixer.SetFloat("BGM", volume);
        Debug.Log($"BGM:{volume}");
    }

    //SE
    public void SetAudioMixerSE(float value)
    {
        //5段階補正
        value /= 10;
        //-80~0に変換
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //audioMixerに代入
        _audioMixer.SetFloat("SE", volume);
        Debug.Log($"SE:{volume}");
    }
    #endregion

    #region private method
    // 自身で作成したPrivateな関数を入れる。
    #endregion
}
