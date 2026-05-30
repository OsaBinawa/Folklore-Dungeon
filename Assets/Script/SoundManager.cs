using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    [SerializeField] private AudioSource BGM_source, SFX_source;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f); 

        if (bgmSlider != null)
            bgmSlider.value = bgmVolume;

        if (sfxSlider != null)
            sfxSlider.value = sfxVolume;
    }

    public void PlayBGM(AudioClip clip)
    {
        BGM_source.clip = clip;
        BGM_source.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFX_source.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        BGM_source.volume = volume;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SFX_source.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}