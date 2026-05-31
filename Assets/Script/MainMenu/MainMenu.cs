using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
    }
    public void UpdateSFXVolume()
    {
        float value = SFX_Slider.value;

        // Avoid Log10(0)
        value = Mathf.Max(value, 0.0001f);

        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
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