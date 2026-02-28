using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public Slider musicSlider;
    public Slider soundSlider;
    public Toggle fullscreenToggle;

    void Start()
    {
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (soundSlider != null) soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.75f);
        if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;

        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (soundSlider != null) soundSlider.onValueChanged.AddListener(SetSoundVolume);

        ApplyInitialVolumes();
    }

    void ApplyInitialVolumes()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.75f));
        SetSoundVolume(PlayerPrefs.GetFloat("SoundVolume", 0.75f));
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