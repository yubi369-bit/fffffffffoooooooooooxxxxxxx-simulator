using UnityEngine;
using UnityEngine.UI;
public class AudioSettingsUI : MonoBehaviour
{
    [Header("슬라이더 연결")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    private const string BGM_KEY = "BGM_Volume";
    private const string SFX_KEY = "SFX_Volume";
    private void Start()
    {
        float savedBGM = PlayerPrefs.GetFloat(BGM_KEY, 0.5f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_KEY, 1.0f);
        bgmSlider.value = savedBGM;
        sfxSlider.value = savedSFX;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(savedBGM);
            AudioManager.Instance.SetSFXVolume(savedSFX);
        }
        bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }
    private void OnBGMChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(value);
        }
        PlayerPrefs.SetFloat(BGM_KEY, value);
    }
    private void OnSFXChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }
    private void OnDestroy()
    {
        bgmSlider.onValueChanged.RemoveListener(OnBGMChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
    }
}