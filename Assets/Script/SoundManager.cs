using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource BGM_source, SFX_source;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
}