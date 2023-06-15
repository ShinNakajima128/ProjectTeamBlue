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
    [SerializeField] private Image _MasterSlider;
    [SerializeField] private Image _bgmSlider;
    [SerializeField] private Image _seSlider;
    #endregion

    #region private
    // プライベートなメンバー変数。
    private int _defaltValue = 5;
    private int _current_Master_Value = 0;
    private int _current_BGM_Value = 0;
    private int _current_SE_Value = 0;
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
        //各ValueにDefalt値を設定（半分）
        _current_Master_Value = _defaltValue;
        _current_BGM_Value = _defaltValue;
        _current_SE_Value = _defaltValue;    
    }

    private void Start()
    {
        _MasterSlider.fillAmount = _current_Master_Value / 10f;
        _bgmSlider.fillAmount = _current_BGM_Value / 10f;
        _seSlider.fillAmount = _current_SE_Value / 10f;
    }

    private void Update()
    {

    }
    #endregion

    #region public method
    //　自身で作成したPublicな関数を入れる。
    //=====Slider=====
    //Master
    public void AudioSliderMaster(int value)
    {
        _current_Master_Value += value;
        _current_Master_Value = Mathf.Clamp(_current_Master_Value, 0, 10);
        _MasterSlider.fillAmount = _current_Master_Value / 10f;
        SetAudioMixerMaster(_current_Master_Value);
    }

    //BGM
    public void AudioSliderBGM(int value)
    {
        _current_BGM_Value += value;
        _current_BGM_Value = Mathf.Clamp(_current_BGM_Value, 0, 10);
        _bgmSlider.fillAmount = _current_BGM_Value / 10f;
        SetAudioMixerBGM(_current_BGM_Value);
    }

    //SE
    public void AudioSliderSE(int value)
    {
        _current_SE_Value += value;
        _current_SE_Value = Mathf.Clamp(_current_SE_Value, 0, 10);
        _seSlider.fillAmount = _current_SE_Value / 10f;
        SetAudioMixerSE(_current_SE_Value);
    }

    //==========
    #endregion

    #region private method
    //自身で作成したprivateな関数を入れる。
    //=====AudioMixer=====
    //Master
    private void SetAudioMixerMaster(float value)
    {
        //10段階補正
        value /= 10;
        //-80~0に変換(デシベル)
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //AudioMixerに代入
        _audioMixer.SetFloat("Master", volume);
        Debug.Log($"master:{volume}");
    }

    //BGM
    private void SetAudioMixerBGM(float value)
    {
        //10段階補正
        value /= 10;
        //-80~0に変換(デシベル)
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //AudioMixerに代入
        _audioMixer.SetFloat("BGM", volume);
        Debug.Log($"BGM:{volume}");
    }

    //SE
    private void SetAudioMixerSE(float value)
    {
        //10段階補正
        value /= 10;
        //-80~0に変換(デシベル)
        var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
        //AudioMixerに代入
        _audioMixer.SetFloat("SE", volume);
        Debug.Log($"SE:{volume}");
    }
    //==========
    #endregion
}