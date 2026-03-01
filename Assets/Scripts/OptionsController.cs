using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;
    public Toggle fullscreenToggle;

    void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float savedSound = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
        bool isFull = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        if (musicSlider != null) musicSlider.value = savedMusic;
        if (soundSlider != null) soundSlider.value = savedSound;

        Screen.fullScreen = isFull;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFull;

        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (soundSlider != null) soundSlider.onValueChanged.AddListener(SetSoundVolume);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        ApplyInitialVolumes(savedMusic, savedSound);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseOptions();
        }
    }

    void ApplyInitialVolumes(float music, float sound)
    {
        SetMusicVolume(music);
        SetSoundVolume(sound);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        GameObject musicObj = GameObject.Find("SoundtrackSource");
        if (musicObj != null)
        {
            AudioSource audioSrc = musicObj.GetComponent<AudioSource>();
            if (audioSrc != null) audioSrc.volume = volume;
        }
    }

    public void SetSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundVolume", volume);
        GameObject sfxObj = GameObject.Find("SFXSource");
        if (sfxObj != null)
        {
            AudioSource audioSrc = sfxObj.GetComponent<AudioSource>();
            if (audioSrc != null) audioSrc.volume = volume;
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.sfxButtonClicked != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClicked);
        }

        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void CloseOptions()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.sfxButtonClicked != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.sfxButtonClicked);
        }

        gameObject.SetActive(false);
    }
}