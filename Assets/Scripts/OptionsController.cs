using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("UI Elements")]
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
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false);
    }
}