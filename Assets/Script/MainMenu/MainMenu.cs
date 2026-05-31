using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    [SerializeField] private AudioClip mainMenuBGM;
    public List<GameObject> panels = new();
    public AudioMixer mixer;
    [SerializeField] private Slider BGM_Slider;
    [SerializeField] private Slider SFX_Slider;
    public void UpdateMusicVolume()
    {
        float value = BGM_Slider.value;

        // Avoid Log10(0)
        value = Mathf.Max(value, 0.0001f);

        mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, BGM_Slider.value);
        PlayerPrefs.Save();
    }
    public void UpdateSFXVolume()
    {
        float value = SFX_Slider.value;

        // Avoid Log10(0)
        value = Mathf.Max(value, 0.0001f);

        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, SFX_Slider.value);
        PlayerPrefs.Save();
    }

    public void ShowPanel(GameObject panelToShow)
    {
        if (panels != null)
        {
            foreach (var panel in panels)
                panel.SetActive(panel == panelToShow);
        }   
    }

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(mainMenuBGM);
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        BGM_Slider.value = bgmVolume;
        SFX_Slider.value = sfxVolume;

        UpdateMusicVolume();
        UpdateSFXVolume();
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }

    public void GoToScene(string Scenes)
    {
        SceneManager.LoadScene(Scenes);
    }

    public void GoToLevelMalin()
    {
        SceneManager.LoadScene("MapScene");
    }
}